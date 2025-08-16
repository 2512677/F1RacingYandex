using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class RaceCamera : MonoBehaviour
    {
        public CameraMode cameraMode;
        private IInputManager inputManager;

        public Transform target;
        private Camera cam;
        private Rigidbody targetRigidbody;
        private Transform mountTarget;

        public global::RCC_CarControllerV4 carController; // Ссылка на RCC_CarControllerV4
                                                          // public Interior_Switch interiorSwitch;    // Ссылка на скрипт Interior_Switch

        private float targetSpeed;
        private bool lookLeft;
        private bool lookRight;
        private bool lookBack;

        //Chase Cam
        public ChaseCameraSettings chaseCameraSettings;
        private Vector3 wantedPosition;
        private float wantedRotation;
        private float wantedHeight;
        private float currentHeight;
        private float currentRotation;
        private bool smoothRotation;

        //CockpitCam
        public CockpitCameraSettings cockpitCameraSettings;
        private float x;
        private float y;
        private float orbitX;
        private float orbitY;
        private float autoResetTimer;

        //FixedCam
        public FixedCameraSettings fixedCameraSettings;

        // Новые настройки для определения бота в режиме lookBack
        [Header("Bot Detection Settings")]
        [Header("Максимальный угол (в градусах), на который камера может повернуться в сторону бота.")]
        public float maxSideAngle = 50f; // можно регулировать через ползунок в инспекторе

        [Header("Максимальная дистанция, на которой камера пытается обнаружить бота.")]
        public float detectionRange = 20f;

        void Start()
        {
            cam = GetComponent<Camera>();
            inputManager = InputManager.instance;

            // Динамически находим carController и interiorSwitch
            AssignTargetComponents();
        }


        void Update()
        {
            // Обновляем shakeAmount на основе скорости автомобиля
            if (carController != null)
            {
                float speedFactor = Mathf.Clamp01(carController.speed / carController.maxspeed);
                chaseCameraSettings.shakeAmount = Mathf.Lerp(0.1f, 0.5f, speedFactor); // Настраиваем диапазон
            }

            //Handle input
            if (inputManager != null)
            {
                lookLeft = inputManager.GetAxis(0, InputAction.LookLeft) > 0;
                lookRight = inputManager.GetAxis(0, InputAction.LookRight) > 0;
                lookBack = inputManager.GetButton(0, InputAction.LookBack);
            }
        }


        void LateUpdate()
        {
            switch (cameraMode)
            {
                case CameraMode.Chase:
                    //   SetActiveObjects(false);
                    ChaseCameraMode();
                    break;

                case CameraMode.Cockpit:
                    //  SetActiveObjects(true);
                    CockpitCameraMode();
                    break;

                case CameraMode.Fixed:
                    // SetActiveObjects(false);
                    FixedCameraMode();
                    break;
            }
        }

        //  void SetActiveObjects(bool isInterior)
        // {
        //   if (interiorSwitch != null)
        //   {
        //      interiorSwitch.IsInterior.SetActive(isInterior);
        //      interiorSwitch.IsBody.SetActive(!isInterior);
        // }
        // }


        void ChaseCameraMode()
        {
            if (target == null) return;

            wantedHeight = target.position.y + chaseCameraSettings.height;
            currentHeight = transform.position.y;
            wantedRotation = target.eulerAngles.y;
            currentRotation = transform.eulerAngles.y;

            // Обработка ввода для lookLeft
            if (lookLeft)
            {
                smoothRotation = false;
                wantedRotation = target.eulerAngles.y - 90;
            }

            // Обработка ввода для lookRight
            if (lookRight)
            {
                smoothRotation = false;
                wantedRotation = target.eulerAngles.y + 90;
            }

            // Модифицированная обработка lookBack с поиском бота по компоненту AiLogic
            if (lookBack || (lookLeft && lookRight))
            {
                // Базовое направление: смотрим назад (180 градусов от направления цели)
                float baseRotation = target.eulerAngles.y + 180;

                // Если нажата чистая кнопка lookBack (без комбинаций с lookLeft или lookRight)
                if (lookBack && !(lookLeft || lookRight))
                {
                    // Отключаем плавное переключение для мгновенного перехода
                    smoothRotation = false;
                    // Ищем ближайшего бота по компоненту AiLogic
                    GameObject nearestBot = FindNearestBotByComponent(target.position);
                    if (nearestBot != null)
                    {
                        // Вычисляем направление к боту
                        Vector3 directionToBot = (nearestBot.transform.position - target.position).normalized;
                        // Заднее направление автомобиля
                        Vector3 backDirection = -target.forward;
                        // Вычисляем угол между задним направлением и направлением на бота
                        float angleOffset = Vector3.SignedAngle(backDirection, directionToBot, Vector3.up);
                        // Ограничиваем угол смещения значением maxSideAngle
                        angleOffset = Mathf.Clamp(angleOffset, -maxSideAngle, maxSideAngle);
                        // Итоговый желаемый угол
                        wantedRotation = baseRotation + angleOffset;
                    }
                    else
                    {
                        // Если бот не найден, смотрим ровно назад
                        wantedRotation = baseRotation;
                    }
                }
                else
                {
                    // Если нажаты комбинированные кнопки (например, lookLeft и lookRight) или не чисто lookBack,
                    // оставляем стандартное поведение: смотрим назад
                    smoothRotation = false;
                    wantedRotation = baseRotation;
                }
            }

            if (!lookLeft && !lookRight && !lookBack)
            {
                if (!smoothRotation)
                    Invoke("ResetSmoothRotation", 0.01f);
            }

            currentRotation = (smoothRotation) ?
                Mathf.LerpAngle(currentRotation, wantedRotation, Time.deltaTime * chaseCameraSettings.rotationDamp) : wantedRotation;

            currentHeight = (smoothRotation) ?
                Mathf.Lerp(currentHeight, wantedHeight, chaseCameraSettings.heightDamp * Time.deltaTime) : wantedHeight;

            wantedPosition = target.position;
            wantedPosition.y = currentHeight;

            transform.position = wantedPosition;
            transform.position -= Quaternion.Euler(0, currentRotation, 0) * Vector3.forward * chaseCameraSettings.distance;
            transform.LookAt(target.position);

            //Set the camera FOV based off the velocity of the target
            if (chaseCameraSettings.velocityBasedFOV)
            {
                cam.fieldOfView = Mathf.Lerp(chaseCameraSettings.minFOV, chaseCameraSettings.maxFOV, targetSpeed / 200);
            }
            else
            {
                cam.fieldOfView = chaseCameraSettings.minFOV;
            }

            //Shake the camera 
            if (chaseCameraSettings.shake)
            {
                float shakeForce = Time.timeScale * Mathf.InverseLerp(chaseCameraSettings.minShakeSpeed, (chaseCameraSettings.minShakeSpeed * 2), targetSpeed);

                shakeForce *= chaseCameraSettings.shakeAmount / 50;

                transform.position += Random.insideUnitSphere * shakeForce;
            }
        }


        void CockpitCameraMode()
        {
            if (mountTarget == null)
                return;

            orbitX = Input.GetAxis("LookX");
            orbitY = Input.GetAxis("LookY");

            x += (orbitX * cockpitCameraSettings.xSpeed * Time.deltaTime);
            y -= (orbitY * cockpitCameraSettings.ySpeed * Time.deltaTime);

            if (orbitX == 0 && orbitY == 0)
            {
                autoResetTimer += Time.deltaTime;

                if (autoResetTimer >= cockpitCameraSettings.autoResetTimeout && cockpitCameraSettings.autoResetRotation)
                {
                    x = Mathf.Lerp(x, 0, Time.deltaTime * 2.5f);
                    y = Mathf.Lerp(y, 0, Time.deltaTime * 2.5f);
                }
            }
            else
            {
                autoResetTimer = 0.0f;
            }

            y = ClampAngle(y, cockpitCameraSettings.yMinLimit, cockpitCameraSettings.yMaxLimit);
            x = ClampAngle(x, cockpitCameraSettings.xMinLimit, cockpitCameraSettings.xMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            transform.position = mountTarget.position;
            transform.rotation = mountTarget.rotation * rotation;

            //Set the camera FOV based off the velocity of the target
            if (cockpitCameraSettings.velocityBasedFOV)
            {
                cam.fieldOfView = Mathf.Lerp(cockpitCameraSettings.minFOV, cockpitCameraSettings.maxFOV, targetSpeed / 200);
            }
            else
            {
                cam.fieldOfView = cockpitCameraSettings.minFOV;
            }

            //Shake the camera 
            if (cockpitCameraSettings.shake)
            {
                float shakeForce = Time.timeScale * Mathf.InverseLerp(cockpitCameraSettings.minShakeSpeed, (cockpitCameraSettings.minShakeSpeed * 2), targetSpeed);

                shakeForce *= cockpitCameraSettings.shakeAmount / 50;

                transform.position += Random.insideUnitSphere * shakeForce;
            }
        }


        void FixedCameraMode()
        {
            if (mountTarget == null)
                return;

            cam.fieldOfView = fixedCameraSettings.FOV;

            float wantedRotation = 0;
            transform.position = mountTarget.position;
            transform.rotation = mountTarget.rotation;

            if (lookLeft)
                wantedRotation = -90;
            else if (lookRight)
                wantedRotation = 90;
            else
                wantedRotation = 0;

            if (lookLeft || lookRight)
            {
                transform.rotation = transform.rotation * Quaternion.Euler(0, wantedRotation, 0);
            }

            //Shake the camera 
            if (fixedCameraSettings.shake)
            {
                float shakeForce = Time.timeScale * Mathf.InverseLerp(fixedCameraSettings.minShakeSpeed, (fixedCameraSettings.minShakeSpeed * 2), targetSpeed);

                shakeForce *= fixedCameraSettings.shakeAmount / 50;

                transform.position += Random.insideUnitSphere * shakeForce;
            }
        }


        void FixedUpdate()
        {
            //Get the speed of the target
            if (targetRigidbody != null)
            {
                targetSpeed = targetRigidbody.linearVelocity.magnitude * 3.6f;
            }
        }


        void ResetSmoothRotation()
        {
            smoothRotation = true;
        }


        public void SwitchCameraMode(CameraMode mode)
        {
            //Switch camera mode
            cameraMode = mode;

            //Set smooth rotation to false to avoid lerping rotation when switching back to Chase mode
            smoothRotation = false;

            //Reset orbit values
            x = 0;
            y = 0;
        }


        public void SetTarget(Transform _target)
        {
            target = _target;
            targetRigidbody = target.GetComponent<Rigidbody>();

            // Переопределяем carController и interiorSwitch для нового объекта
            AssignTargetComponents();
        }



        public void SetMountTarget(Transform _target)
        {
            mountTarget = _target;
        }

        private void AssignTargetComponents()
        {
            if (target != null)
            {
                carController = target.GetComponent<global::RCC_CarControllerV4>();
                if (carController == null)
                {
                    Debug.LogError("RCC_CarControllerV3 не найден на объекте " + target.name);
                }

            }
        }



        float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
            {
                angle += 360;
            }
            if (angle > 360)
            {
                angle -= 360;
            }
            return Mathf.Clamp(angle, min, max);
        }

        // Новый метод: поиск ближайшего бота по компоненту AiLogic
        GameObject FindNearestBotByComponent(Vector3 position)
        {
            // Получаем все объекты с компонентом AiLogic
            AiLogic[] bots = FindObjectsOfType<AiLogic>();
            GameObject nearestBot = null;
            float minDistance = detectionRange; // ищем ботов в пределах detectionRange

            foreach (AiLogic bot in bots)
            {
                float dist = Vector3.Distance(position, bot.transform.position);
                if (dist < minDistance)
                {
                    // Проверяем, находится ли бот в зоне сзади (в пределах maxSideAngle)
                    Vector3 directionToBot = (bot.transform.position - position).normalized;
                    float angle = Vector3.Angle(-target.forward, directionToBot);
                    if (angle < maxSideAngle)
                    {
                        minDistance = dist;
                        nearestBot = bot.gameObject;
                    }
                }
            }
            return nearestBot;
        }
    }


    [System.Serializable]
    public class ChaseCameraSettings
    {
        public float distance = 5;
        public float height = 2;
        public float heightDamp = 5;
        public float rotationDamp = 2;

        [Space(10)]
        public float minFOV = 60;
        public float maxFOV = 80;
        public bool velocityBasedFOV;

        [Space(10)]
        public bool shake;
        public float minShakeSpeed = 100;
        [Range(0f, 1f)]
        public float shakeAmount = 0.35f;

        [Space(10)]
        public bool showRearMirrorUI;
    }


    [System.Serializable]
    public class CockpitCameraSettings
    {
        public float xSpeed = 100.0f;
        public float ySpeed = 100.0f;
        public float yMinLimit = -20.0f;
        public float yMaxLimit = 30.0f;
        public float xMinLimit = -60.0f;
        public float xMaxLimit = 60.0f;
        public bool autoResetRotation = true;
        public float autoResetTimeout = 1;

        [Space(10)]
        public float minFOV = 60;
        public float maxFOV = 80;
        public bool velocityBasedFOV;

        [Space(10)]
        public bool shake;
        public float minShakeSpeed = 100;
        [Range(0f, 1f)]
        public float shakeAmount = 0.35f;
    }


    [System.Serializable]
    public class FixedCameraSettings
    {
        public float FOV = 60;

        [Space(10)]
        public bool shake;
        public float minShakeSpeed = 100;
        [Range(0f, 1f)]
        public float shakeAmount = 0.35f;
    }
}
