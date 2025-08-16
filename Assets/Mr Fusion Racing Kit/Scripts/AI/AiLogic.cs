using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RGSK
{
    public class AiLogic : MonoBehaviour
    {
        [Header("Behaviour")]
        [Range(0, 1)] public float throttleSensitivity = 0.8f;
        [Range(0, 1)] public float brakeSensitivity = 0.25f;
        [Range(0, 1)] public float steerSensitivity = 0.5f;
        [Range(0.8f, 1)] public float speedModifier = 1;
        public bool revOnRaceCountdown;

        // Навигация
        private TrackLayout racingLine;
        public Transform racingLineTarget { get; private set; }
        public TrackSpline.RoutePoint racingLinePoint { get; private set; }
        private float racingLineDistance;
        private float currentSpeed;
        private float targetSpeed;
        private int racingLineNodeIndex;
        private float targetDistanceAhead;

        // Обгон и обход препятствий
        public float cautionDistance = 10f;           // дистанция, при которой начнётся медленное объезжание
        private float travelOffset;                  // текущее боковое смещение, задающее направление обхода
        private float newTravelOffset;               // временное значение смещения при выборе стороны обхода
        private bool isAvoiding;                     // флаг, что мы сейчас выполняем объезд препятствия
        private Collider closestThreat;              // ближайшее препятствие спереди
        private bool slowDownThreat;                 // флаг, что нужно сбросить скорость из-за слишком близкого препятствия
        private float overrideSpeed = -1f;           // значение скорости, на которое нужно сбавить ход (если < 0, игнорируется)
        private Vector3 obstacleHitPosition;         // точка столкновения из BoxCast’а
        private Vector3 lateralObstaclePosition;     // позиция препятствия относительно центра трассы

        // Поля для собственного обхода через BoxCast
        private List<Collider> childColliders = new List<Collider>();

        // Восстановление
        private float recoverTimer;
        private float reverseTimer;
        private bool reversing;
        public float respawnWait = 10f;

        [Header("Сенсоры")]
        public bool visualizeSensors = true;
        [Space(10)]
        // Передний датчик (BoxCast)
        public float frontSensorDistance = 20f;
        // Вместо фронтального BoxCollider используем BoxCast с размерами, зависящими от размеров коллайдера
        [Space(5)]
        // Левый и правый датчики через BoxCast
        public float sideSensorWidthMultiplier = 0.5f;

        [Header("Рейкастеры для обнаружения препятствий")]
        public float raycastOriginOffset = 1f;
        public LayerMask obstacleLayers = ~0; // По умолчанию все слои

        // Ссылки
        private IAiInput aiInput;
        private Rigidbody rigid;
        private RacerStatistics racerStatistics;

        // Входные значения
        public float throttleInput { get; private set; }
        public float steerInput { get; private set; }
        public float brakeInput { get; private set; }
        public float handbrakeInput { get; private set; }

        // --- Переменные для сохранения «любимого» смещения ---
        private float preferredOffset;

        void Awake()
        {
            racingLine = FindObjectOfType<TrackLayout>();
            if (racingLine != null)
            {
                racingLineTarget = new GameObject("RacingLineTarget:" + gameObject.name).transform;
                GameObject trackers = GameObject.Find("RGSK_Trackers");
                if (trackers != null) racingLineTarget.parent = trackers.transform;
            }
        }

        void Start()
        {
            aiInput = GetComponent<IAiInput>();
            rigid = GetComponent<Rigidbody>();
            racerStatistics = GetComponent<RacerStatistics>();

            GetComponentsInChildren<Collider>(childColliders);

            // Инициализируем preferredOffset в зависимости от ширины трассы
            if (racingLine != null)
            {
                float maxLeft = racingLine.GetLeftWidth(racingLineNodeIndex);
                float maxRight = racingLine.GetRightWidth(racingLineNodeIndex);
                preferredOffset = Random.Range(-maxLeft, maxRight);
            }
        }

        void FixedUpdate()
        {
            UpdateTargetPosition();
            CalculateSpeedValues();

            // Выполняем логику обхода препятствий перед навигацией
            PerformAvoidanceLogic();

            // Навигация и управление
            Navigate();
            Recover();
        }

        void PerformAvoidanceLogic()
        {
            // Сбрасываем флаги
            isAvoiding = false;
            slowDownThreat = false;
            overrideSpeed = -1f;

            // Получаем размеры «бокса» автомобиля для BoxCast
            Vector3 dims;
            Collider mainCol = GetComponent<Collider>();
            if (mainCol != null) dims = mainCol.bounds.size;
            else dims = new Vector3(1f, 1f, 2f);

            // --- BoxCast вперед ---
            if (Physics.BoxCast(
                        transform.position,
                        dims / 2.5f,
                        transform.forward,
                        out RaycastHit forwardHit,
                        transform.rotation,
                        frontSensorDistance,
                        obstacleLayers,
                        QueryTriggerInteraction.Ignore))
            {
                if (!childColliders.Contains(forwardHit.collider))
                {
                    obstacleHitPosition = forwardHit.point;
                    Rigidbody otherRigid = forwardHit.collider.GetComponentInParent<Rigidbody>();
                    float otherSpeed = otherRigid != null ? otherRigid.linearVelocity.magnitude * 3.6f : 0f;
                    float distance = Vector3.Distance(transform.position, forwardHit.collider.transform.position);
                    float relativeVelocity = Mathf.Abs(currentSpeed - otherSpeed);
                    if (relativeVelocity < 0.01f) relativeVelocity = 0.01f;
                    float impactTime = distance / relativeVelocity;

                    // Если мы быстрее препятствия и время до столкновения меньше порога
                    if (currentSpeed > otherSpeed && impactTime < 1f)
                    {
                        isAvoiding = true;

                        // Определяем сторону обхода
                        Vector3 localDelta = transform.InverseTransformPoint(forwardHit.collider.transform.position);
                        float avoidDirection = -Mathf.Sign(Mathf.Atan2(localDelta.x, localDelta.z));
                        newTravelOffset = CalculateLateralOffset(avoidDirection, dims);

                        // Проверяем границы трассы
                        float leftBound = racingLine.GetLeftWidth(racingLineNodeIndex);
                        float rightBound = racingLine.GetRightWidth(racingLineNodeIndex);
                        if (newTravelOffset < -leftBound || newTravelOffset > rightBound)
                        {
                            // Меняем направление, если уходим за границы
                            avoidDirection *= -1f;
                            newTravelOffset = CalculateLateralOffset(avoidDirection, dims);
                        }

                        travelOffset = newTravelOffset;
                    }

                    // Если слишком близко — сброcаем скорость
                    if (distance < cautionDistance)
                    {
                        slowDownThreat = true;
                        overrideSpeed = Mathf.Clamp(
                            otherSpeed,
                            10f,
                            otherSpeed
                        );
                    }
                }
            }

            // --- BoxCast влево ---
            if (!isAvoiding) // боковые проверки только если впереди нет срочной необходимости объезжать спереди
            {
                if (Physics.BoxCast(
                            transform.position,
                            dims / 2.5f,
                            -transform.right,
                            out RaycastHit leftHit,
                            transform.rotation,
                            dims.x * sideSensorWidthMultiplier,
                            obstacleLayers,
                            QueryTriggerInteraction.Ignore))
                {
                    if (!childColliders.Contains(leftHit.collider))
                    {
                        isAvoiding = true;
                        obstacleHitPosition = leftHit.point;
                        lateralObstaclePosition = GetLateralPositionFromCenter(obstacleHitPosition);

                        float leftBound = racingLine.GetLeftWidth(racingLineNodeIndex);
                        if (lateralObstaclePosition.x < -leftBound)
                        {
                            lateralObstaclePosition.x = -leftBound + dims.x;
                        }

                        travelOffset = lateralObstaclePosition.x + dims.x;
                    }
                }
            }

            // --- BoxCast вправо ---
            if (!isAvoiding)
            {
                if (Physics.BoxCast(
                            transform.position,
                            dims / 2.5f,
                            transform.right,
                            out RaycastHit rightHit,
                            transform.rotation,
                            dims.x * sideSensorWidthMultiplier,
                            obstacleLayers,
                            QueryTriggerInteraction.Ignore))
                {
                    if (!childColliders.Contains(rightHit.collider))
                    {
                        isAvoiding = true;
                        obstacleHitPosition = rightHit.point;
                        lateralObstaclePosition = GetLateralPositionFromCenter(obstacleHitPosition);

                        float rightBound = racingLine.GetRightWidth(racingLineNodeIndex);
                        if (lateralObstaclePosition.x > rightBound)
                        {
                            lateralObstaclePosition.x = rightBound - dims.x;
                        }

                        travelOffset = lateralObstaclePosition.x - dims.x;
                    }
                }
            }

            // Заменяем targetSpeed, если нужно снизить скорость
            if (slowDownThreat && overrideSpeed >= 0f)
            {
                targetSpeed = overrideSpeed;
            }
        }

        float CalculateLateralOffset(float dir, Vector3 dims)
        {
            // Получаем текущую боковую позицию препятствия относительно центра трассы
            float obstacleLateral = GetLateralPositionFromCenter(obstacleHitPosition).x;
            // Если dir > 0, обходим справа, иначе слева
            return dir > 0
                ? obstacleLateral + dims.x * 2f
                : obstacleLateral - dims.x * 2f;
        }

        Vector3 GetLateralPositionFromCenter(Vector3 worldPos)
        {
            // Переводим мировую точку в локальные координаты racingLineTarget
            if (racingLineTarget == null)
                return Vector3.zero;
            return racingLineTarget.InverseTransformPoint(worldPos);
        }

        void Navigate()
        {
            // Определяем точку, к которой будем стремиться по гоночной траектории
            Vector3 desiredPosition = racingLineTarget.position;
            // Если мы выполняем объезд препятствия, desiredPosition сдвигаем на travelOffset
            desiredPosition += racingLineTarget.right * travelOffset;

            // Рассчитываем угол руления к desiredPosition
            Vector3 localOffset = transform.InverseTransformPoint(desiredPosition);
            float steerAngle = Mathf.Atan2(localOffset.x, localOffset.z) * Mathf.Rad2Deg;
            float baseSteerValue = Mathf.Clamp(
                steerAngle * (steerSensitivity / 10f),
                -1f,
                1f
            ) * Mathf.Sign(currentSpeed);

            // Если сейчас выполняется объезд, используем baseSteerValue без дополнительной корректировки
            // Иначе — добавляем коррекцию рейкастами
            float rayInput = 0f;
            if (!isAvoiding)
            {
                rayInput = FixedRaycasts();
            }
            float finalSteer = isAvoiding
                ? baseSteerValue
                : Mathf.Clamp(baseSteerValue + rayInput, -1f, 1f);

            // Плавное возвращение к preferredOffset, когда нет препятствий спереди
            if (!isAvoiding)
            {
                traveOffsetResetTimer += Time.deltaTime;
                if (traveOffsetResetTimer >= 2f)
                {
                    travelOffset = Mathf.MoveTowards(
                        travelOffset,
                        preferredOffset,
                        Time.deltaTime * 2f
                    );
                    float leftBound = racingLine.GetLeftWidth(racingLineNodeIndex);
                    float rightBound = racingLine.GetRightWidth(racingLineNodeIndex);
                    travelOffset = Mathf.Clamp(travelOffset, -leftBound, rightBound);
                }
            }
            else
            {
                traveOffsetResetTimer = 0f;
            }

            // Управление скоростью
            float sensitivity = currentSpeed > targetSpeed
                ? brakeSensitivity / 10f
                : throttleSensitivity / 10f;
            float throttleBrakeRatio = Mathf.Clamp(
                (targetSpeed - currentSpeed) * sensitivity,
                -1f,
                1f
            );

            // Если reversing — меняем управление
            if (reversing)
            {
                steerInput = -baseSteerValue;
                throttleInput = 0f;
                brakeInput = 1f;
            }
            else
            {
                steerInput = finalSteer;
                throttleInput = Mathf.Clamp(throttleBrakeRatio, 0f, 1f);
                brakeInput = -Mathf.Clamp(throttleBrakeRatio, -1f, 0f);
            }
            handbrakeInput = 0f;

            // Передаём значения в IAiInput
            if (aiInput != null)
            {
                if (RaceManager.instance != null && RaceManager.instance.raceStarted)
                {
                    aiInput.SetInputValues(throttleInput, brakeInput, steerInput, handbrakeInput);
                }
                else
                {
                    float revInput = 0f;
                    if (revOnRaceCountdown && RaceManager.instance != null && RaceManager.instance.isCountdownStarted)
                    {
                        revInput = Mathf.Repeat(Time.time * 2f, 1f);
                    }
                    aiInput.SetInputValues(revInput, 0f, 0f, 1f);
                }
            }
        }

        // Таймер для плавного возврата к preferredOffset
        private float traveOffsetResetTimer;

        // Метод FixedRaycasts выполняет серию лучей для обнаружения препятствий при навигации
        float FixedRaycasts()
        {
            Vector3 pivotPos = transform.position + transform.forward * raycastOriginOffset;
            float wideRayLength = 20f;
            float tightRayLength = 20f;
            float sideRayLength = 3f;
            float rayInput = 0f;
            RaycastHit hit;
            float newinputSteer1 = 0f, newinputSteer2 = 0f, newinputSteer3 = 0f, newinputSteer4 = 0f, newinputSteer5 = 0f, newinputSteer6 = 0f;

            // Широкий рейкаст влево (25°)
            Vector3 dir = Quaternion.AngleAxis(25, transform.up) * transform.forward;
            if (Physics.Raycast(pivotPos, dir, out hit, wideRayLength, obstacleLayers) && !hit.collider.isTrigger)
            {
                Debug.DrawRay(pivotPos, dir * wideRayLength, Color.red);
                newinputSteer1 = Mathf.Lerp(-0.5f, 0f, hit.distance / wideRayLength);
            }
            else
            {
                Debug.DrawRay(pivotPos, dir * wideRayLength, Color.white);
            }

            // Широкий рейкаст вправо (-25°)
            dir = Quaternion.AngleAxis(-25, transform.up) * transform.forward;
            if (Physics.Raycast(pivotPos, dir, out hit, wideRayLength, obstacleLayers) && !hit.collider.isTrigger)
            {
                Debug.DrawRay(pivotPos, dir * wideRayLength, Color.red);
                newinputSteer4 = Mathf.Lerp(0.5f, 0f, hit.distance / wideRayLength);
            }
            else
            {
                Debug.DrawRay(pivotPos, dir * wideRayLength, Color.white);
            }

            // Узкий рейкаст влево (7°)
            dir = Quaternion.AngleAxis(7, transform.up) * transform.forward;
            if (Physics.Raycast(pivotPos, dir, out hit, tightRayLength, obstacleLayers) && !hit.collider.isTrigger)
            {
                Debug.DrawRay(pivotPos, dir * tightRayLength, Color.red);
                newinputSteer3 = Mathf.Lerp(-1f, 0f, hit.distance / tightRayLength);
            }
            else
            {
                Debug.DrawRay(pivotPos, dir * tightRayLength, Color.white);
            }

            // Узкий рейкаст вправо (-7°)
            dir = Quaternion.AngleAxis(-7, transform.up) * transform.forward;
            if (Physics.Raycast(pivotPos, dir, out hit, tightRayLength, obstacleLayers) && !hit.collider.isTrigger)
            {
                Debug.DrawRay(pivotPos, dir * tightRayLength, Color.red);
                newinputSteer2 = Mathf.Lerp(1f, 0f, hit.distance / tightRayLength);
            }
            else
            {
                Debug.DrawRay(pivotPos, dir * tightRayLength, Color.white);
            }

            // Боковой рейкаст влево (90°)
            dir = Quaternion.AngleAxis(90, transform.up) * transform.forward;
            if (Physics.Raycast(pivotPos, dir, out hit, sideRayLength, obstacleLayers) && !hit.collider.isTrigger)
            {
                Debug.DrawRay(pivotPos, dir * sideRayLength, Color.red);
                newinputSteer5 = Mathf.Lerp(-1f, 0f, hit.distance / sideRayLength);
            }
            else
            {
                Debug.DrawRay(pivotPos, dir * sideRayLength, Color.white);
            }

            // Боковой рейкаст вправо (-90°)
            dir = Quaternion.AngleAxis(-90, transform.up) * transform.forward;
            if (Physics.Raycast(pivotPos, dir, out hit, sideRayLength, obstacleLayers) && !hit.collider.isTrigger)
            {
                Debug.DrawRay(pivotPos, dir * sideRayLength, Color.red);
                newinputSteer6 = Mathf.Lerp(1f, 0f, hit.distance / sideRayLength);
            }
            else
            {
                Debug.DrawRay(pivotPos, dir * sideRayLength, Color.white);
            }

            rayInput = newinputSteer1 + newinputSteer2 + newinputSteer3 + newinputSteer4 + newinputSteer5 + newinputSteer6;
            return rayInput;
        }

        void CalculateSpeedValues()
        {
            if (racingLine != null)
            {
                racingLineNodeIndex = racingLine.GetNodeIndexAtDistance(racingLineDistance);
                if (!slowDownThreat)
                {
                    targetSpeed = racingLine.GetSpeedAtNode(racingLineNodeIndex);
                    targetSpeed *= speedModifier;
                }
            }

            if (rigid != null)
            {
                currentSpeed = rigid.linearVelocity.magnitude * 3.6f;
            }

            if (RaceManager.instance != null && RaceManager.instance.isRollingStart)
            {
                targetSpeed = RaceManager.instance.rollingStartSpeed;
            }

            if (racerStatistics != null)
            {
                if (racerStatistics.finished || racerStatistics.disqualified)
                {
                    if (RaceManager.instance != null)
                    {
                        targetSpeed *= RaceManager.instance.postRaceSpeedMultiplier;
                    }
                }
            }
        }

        void UpdateTargetPosition()
        {
            if (racingLine == null)
                return;

            racingLineTarget.position = racingLine.GetRoutePoint(racingLineDistance + targetDistanceAhead).position;
            racingLineTarget.rotation = Quaternion.LookRotation(
                racingLine.GetRoutePoint(racingLineDistance + targetDistanceAhead).direction
            );
            racingLinePoint = racingLine.GetRoutePoint(racingLineDistance);
            Vector3 progressDelta = racingLinePoint.position - transform.position;

            if (Vector3.Dot(progressDelta, racingLinePoint.direction) < 0f)
            {
                racingLineDistance += progressDelta.magnitude * 0.5f;
            }
            else if (Vector3.Dot(progressDelta, racingLinePoint.direction) > 2f)
            {
                racingLineDistance -= progressDelta.magnitude * 0.5f;
            }

            if (racingLineDistance >= racingLine.distances[racingLine.distances.Length - 1])
            {
                racingLineDistance = 0f;
            }

            targetDistanceAhead = Mathf.Clamp(
                (currentSpeed * 0.2f),
                racingLine.minTargetDistance,
                racingLine.maxTargetDistance
            );
        }

        void Recover()
        {
            if (!racerStatistics.started)
                return;

            if (currentSpeed <= 2f)
            {
                recoverTimer += Time.deltaTime;
            }
            else
            {
                recoverTimer = 0f;
                reverseTimer = 0f;
                reversing = false;
            }

            if (recoverTimer >= 2f)
            {
                reversing = true;
                reverseTimer += Time.deltaTime;
                if (reverseTimer >= Random.Range(1f, 1.5f))
                {
                    reverseTimer = 0f;
                    reversing = false;
                }
            }

            if (recoverTimer > respawnWait)
            {
                RaceManager.instance.RespawnVehicle(transform);
            }
        }

        void OnCollisionEnter(Collision col)
        {
            var collisionLocalDelta = transform.InverseTransformPoint(col.transform.position);
            float angle = Mathf.Atan2(collisionLocalDelta.x, collisionLocalDelta.z);
            travelOffset = 1.5f * -Mathf.Sign(angle);
            float leftBound = racingLine.GetLeftWidth(racingLineNodeIndex);
            float rightBound = racingLine.GetRightWidth(racingLineNodeIndex);
            travelOffset = Mathf.Clamp(travelOffset, -leftBound, rightBound);
        }

        void OnDrawGizmos()
        {
            if (visualizeSensors)
            {
                Gizmos.matrix = transform.localToWorldMatrix;

                // Строим бокс-каст визуально, если нужен
                Vector3 dims;
                Collider mainCol = GetComponent<Collider>();
                if (mainCol != null) dims = mainCol.bounds.size;
                else dims = new Vector3(1f, 1f, 2f);

                Vector3 center = transform.position + transform.forward * (frontSensorDistance / 2f);
                Vector3 size = new Vector3(dims.x / 2.5f, dims.y / 2.5f, frontSensorDistance);
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(center - transform.position, size);

                // Боковые бокскасты
                float sideWidth = dims.x * sideSensorWidthMultiplier * 2f;
                Vector3 leftCenter = transform.position - transform.right * (dims.x * 0.5f);
                Vector3 leftSize = new Vector3(dims.x / 2.5f, dims.y / 2.5f, dims.x * sideSensorWidthMultiplier);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(leftCenter - transform.position, leftSize);

                Vector3 rightCenter = transform.position + transform.right * (dims.x * 0.5f);
                Vector3 rightSize = new Vector3(dims.x / 2.5f, dims.y / 2.5f, dims.x * sideSensorWidthMultiplier);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(rightCenter - transform.position, rightSize);
            }
        }

        public void SetDifficulty(AiDifficulty difficulty)
        {
            throttleSensitivity = difficulty.throttleSensitivity;
            brakeSensitivity = difficulty.brakeSensitivity;
            steerSensitivity = difficulty.steerSensitivity;
            speedModifier = difficulty.speedModifier;
        }
    }

}

