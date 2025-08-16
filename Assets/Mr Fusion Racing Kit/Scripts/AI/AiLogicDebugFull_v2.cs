using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class AiLogicDebugFull_v2 : MonoBehaviour
    {
        [Header("Behaviour")]
        [Range(0, 1)] public float throttleSensitivity = 0.8f;
        [Range(0, 1)] public float brakeSensitivity = 0.25f;
        [Range(0, 1)] public float steerSensitivity = 0.5f;
        [Range(0.8f, 1)] public float speedModifier = 1;
        public bool revOnRaceCountdown;

        // ���������
        private TrackLayout racingLine;
        public Transform racingLineTarget { get; private set; }
        public TrackSpline.RoutePoint racingLinePoint { get; private set; }
        private float racingLineDistance;
        private float currentSpeed;
        private float targetSpeed;
        private int racingLineNodeIndex;
        private float targetDistanceAhead;

        // �����
        public float cautionDistance = 10;
        public float travelOffset; // ������ public ��� �������� �������
        private float newTravelOffset;
        private Collider closestThreat;
        private bool slowDownThreat;
        private float traveOffsetResetTimer;

        // ��������������
        private float recoverTimer;
        private float reverseTimer;
        private bool reversing;
        public float respawnWait = 10;

        [Header("�������")]
        public bool visualizeSensors = true;
        [Space(10)]
        public float frontSensorDistance = 100;
        public float frontSensorWidth = 2;
        public float frontSensorHeight = 0.5f;
        public Sensor frontSensor { get; private set; }
        [Space(5)]
        public float leftSensorDistance = 5;
        public float leftSensorWidth = 2;
        public float leftSensorHeight = 0.5f;
        public float leftSensorOffset = 1;
        public Sensor leftSensor { get; private set; }
        [Space(5)]
        public float rightSensorDistance = 5;
        public float rightSensorWidth = 2;
        public float rightSensorHeight = 0.5f;
        public float rightSensorOffset = 1;
        public Sensor rightSensor { get; private set; }

        [Header("���������� ��� ����������� �����������")]
        public float raycastOriginOffset = 1f;
        public LayerMask obstacleLayers = ~0; // ��������� ��� ���� �� ���������

        // ������
        private IAiInput aiInput;
        private Rigidbody rigid;
        private RacerStatistics racerStatistics;

        // ������� ��������
        public float throttleInput { get; private set; }
        public float steerInput { get; private set; }
        public float brakeInput { get; private set; }
        public float handbrakeInput { get; private set; }

        // --- ����� ���������� ��� "��������" �������� ---
        private float preferredOffset;

        void Awake()
        {
            racingLine = FindObjectOfType<TrackLayout>();
            if (racingLine != null)
            {
                // ������� ���� ��� ���������� �����������
                racingLineTarget = new GameObject("RacingLineTarget:" + gameObject.name).transform;
                GameObject trackers = GameObject.Find("RGSK_Trackers");
                if (trackers != null)
                    racingLineTarget.parent = trackers.transform;
            }
        }

        void Start()
        {
            aiInput = GetComponent<IAiInput>();
            rigid = GetComponent<Rigidbody>();
            racerStatistics = GetComponent<RacerStatistics>();

            SetupSensors();

            // --- ������������� preferredOffset � travelOffset ---
            preferredOffset = 5f;
            travelOffset = preferredOffset;

            Debug.Log("Start: preferredOffset = " + preferredOffset + ", travelOffset = " + travelOffset);
        }

        void SetupSensors()
        {
            Vector3 center;
            Vector3 size;

            // �������� ������
            center = new Vector3(0, frontSensorHeight, (frontSensorDistance / 2) + 0.5f);
            size = new Vector3(frontSensorWidth, 1, frontSensorDistance);
            BoxCollider front = new GameObject("FrontSensor").AddComponent<BoxCollider>();
            front.gameObject.layer = LayerMask.NameToLayer("AISensor");
            front.transform.SetParent(transform);
            front.transform.localRotation = Quaternion.identity;
            front.transform.localPosition = Vector3.zero;
            front.center = center;
            front.size = size;
            front.isTrigger = true;
            frontSensor = front.gameObject.AddComponent<Sensor>();
            frontSensor.AddLayer(LayerMask.NameToLayer("Vehicle"));

            // ����� ������
            center = new Vector3(-leftSensorOffset, leftSensorHeight, 0);
            size = new Vector3(leftSensorWidth, 1, leftSensorDistance);
            BoxCollider left = new GameObject("LeftSensor").AddComponent<BoxCollider>();
            left.gameObject.layer = LayerMask.NameToLayer("AISensor");
            left.transform.SetParent(transform);
            left.transform.localRotation = Quaternion.identity;
            left.transform.localPosition = Vector3.zero;
            left.center = center;
            left.size = size;
            left.isTrigger = true;
            leftSensor = left.gameObject.AddComponent<Sensor>();
            leftSensor.AddLayer(LayerMask.NameToLayer("Vehicle"));

            // ������ ������
            center = new Vector3(rightSensorOffset, rightSensorHeight, 0);
            size = new Vector3(rightSensorWidth, 1, rightSensorDistance);
            BoxCollider right = new GameObject("RightSensor").AddComponent<BoxCollider>();
            right.gameObject.layer = LayerMask.NameToLayer("AISensor");
            right.transform.SetParent(transform);
            right.transform.localRotation = Quaternion.identity;
            right.transform.localPosition = Vector3.zero;
            right.center = center;
            right.size = size;
            right.isTrigger = true;
            rightSensor = right.gameObject.AddComponent<Sensor>();
            rightSensor.AddLayer(LayerMask.NameToLayer("Vehicle"));
        }

        void FixedUpdate()
        {
            UpdateTargetPosition();
            CalculateSpeedValues();
            Navigate();
            CheckFrontThreats();
            Recover();

            Debug.Log("FixedUpdate: travelOffset = " + travelOffset);
        }

        void Navigate()
        {
            // ��������� ������� �������� ����, �������� travelOffset
            Vector3 offset = racingLineTarget.position;
            offset += racingLineTarget.right * travelOffset;
            Vector3 local = transform.InverseTransformPoint(offset);
            float steerAngle = Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
            float steerValue = Mathf.Clamp(steerAngle * (steerSensitivity / 10f), -1f, 1f) * Mathf.Sign(currentSpeed);

            Debug.Log("Navigate (before): travelOffset = " + travelOffset);

            // ������ �������� �������� travelOffset � preferredOffset
            if (travelOffset != 0)
            {
                traveOffsetResetTimer += Time.deltaTime;
                if (traveOffsetResetTimer >= 5)
                {
                    travelOffset = Mathf.MoveTowards(travelOffset, preferredOffset, Time.deltaTime * 2);
                    Debug.Log("Navigate: ������������� travelOffset � preferredOffset (" + preferredOffset + "), ����� travelOffset = " + travelOffset);
                }
            }
            else
            {
                traveOffsetResetTimer = 0;
            }

            // ��������� steerInput, throttleInput, brakeInput
            steerInput = !reversing ? Mathf.Clamp(steerValue, -1f, 1f) : -steerValue;
            float sensitivity = (currentSpeed > targetSpeed) ? brakeSensitivity / 10f : throttleSensitivity / 10f;
            float throttleBrakeRatio = Mathf.Clamp((targetSpeed - currentSpeed) * sensitivity, -1f, 1f);
            throttleInput = !reversing ? Mathf.Clamp(throttleBrakeRatio, 0, 1) : 0;
            brakeInput = !reversing ? -Mathf.Clamp(throttleBrakeRatio, -1, 0) : 1;

            Debug.Log("Navigate (after): travelOffset = " + travelOffset);
            Debug.Log("Navigate: steerInput = " + steerInput + ", throttleInput = " + throttleInput + ", brakeInput = " + brakeInput);

            // �������� �������� � ���������� �� (���� RaceManager �������, ���������� ��������������� ����)
            if (aiInput != null)
            {
                if (RaceManager.instance != null)
                {
                    if (RaceManager.instance.raceStarted)
                    {
                        Debug.Log("aiInput.SetInputValues: (" + throttleInput + ", " + brakeInput + ", " + steerInput + ", 0)");
                        aiInput.SetInputValues(throttleInput, brakeInput, steerInput, 0);
                    }
                    else
                    {
                        float revInput = 0;
                        if (revOnRaceCountdown && RaceManager.instance.isCountdownStarted)
                        {
                            revInput = Mathf.Repeat(Time.time * 2f, 1);
                        }
                        Debug.Log("aiInput.SetInputValues (Countdown): (" + revInput + ", 0, 0, 1)");
                        aiInput.SetInputValues(revInput, 0, 0, 1);
                    }
                }
            }
        }

        void CheckFrontThreats()
        {
            Debug.Log("CheckFrontThreats: ������, travelOffset = " + travelOffset);

            // ���� ����� ���, ������������� ������� �������� ������ preferredOffset
            if (!IsThreatFront())
            {
                newTravelOffset = preferredOffset;
                travelOffset = Mathf.Lerp(travelOffset, newTravelOffset, Time.deltaTime * 2);
                Debug.Log("CheckFrontThreats: ������ ���, ������������� travelOffset � preferredOffset (" + preferredOffset + "), travelOffset = " + travelOffset);
                return;
            }

            // ���� ������ ����������, ��������� ����� ��������
            closestThreat = GetClosestThreat(frontSensor.collidersInRange.ToArray());
            if (closestThreat == null)
                return;

            float distanceToThreat = Vector3.Distance(closestThreat.transform.position, transform.position);
            Bounds bounds = new Bounds(closestThreat.bounds.center, closestThreat.bounds.size);
            float threatWidth = bounds.size.x;
            float threatSpeed = 0;
            Rigidbody threatRigid = closestThreat.GetComponentInParent<Rigidbody>();
            if (threatRigid != null)
                threatSpeed = threatRigid.linearVelocity.magnitude * 3.6f;

            if (currentSpeed > threatSpeed * 1.05f)
            {
                Vector3 threatLocalDelta = transform.InverseTransformPoint(closestThreat.transform.position);
                float threatAngle = Mathf.Atan2(threatLocalDelta.x, threatLocalDelta.z);
                float bestDirection = -Mathf.Sign(threatAngle);
                float threatTrackPosition = racingLineTarget.InverseTransformPoint(closestThreat.transform.position).x;

                if (bestDirection == 1)
                    newTravelOffset = threatTrackPosition + threatWidth;
                else if (bestDirection == -1)
                    newTravelOffset = threatTrackPosition - threatWidth;

                Debug.Log("CheckFrontThreats: ���������� ������, bestDirection = " + bestDirection + ", newTravelOffset = " + newTravelOffset);
            }

            slowDownThreat = distanceToThreat < cautionDistance;
            if (slowDownThreat)
                targetSpeed = threatSpeed;

            travelOffset = Mathf.Lerp(travelOffset, newTravelOffset, Time.deltaTime * 2);
            travelOffset = Mathf.Clamp(travelOffset, -racingLine.GetLeftWidth(racingLineNodeIndex), racingLine.GetRightWidth(racingLineNodeIndex));
            Debug.Log("CheckFrontThreats: ����� Lerp � Clamp, travelOffset = " + travelOffset);
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
                currentSpeed = rigid.linearVelocity.magnitude * 3.6f;

            if (RaceManager.instance != null && RaceManager.instance.isRollingStart)
                targetSpeed = RaceManager.instance.rollingStartSpeed;

            if (racerStatistics != null)
            {
                if (racerStatistics.finished || racerStatistics.disqualified)
                {
                    if (RaceManager.instance != null)
                        targetSpeed *= RaceManager.instance.postRaceSpeedMultiplier;
                }
            }
        }

        void UpdateTargetPosition()
        {
            if (racingLine == null)
                return;

            racingLineTarget.position = racingLine.GetRoutePoint(racingLineDistance + targetDistanceAhead).position;
            racingLineTarget.rotation = Quaternion.LookRotation(racingLine.GetRoutePoint(racingLineDistance + targetDistanceAhead).direction);
            racingLinePoint = racingLine.GetRoutePoint(racingLineDistance);
            Vector3 progressDelta = racingLinePoint.position - transform.position;

            if (Vector3.Dot(progressDelta, racingLinePoint.direction) < 0)
                racingLineDistance += progressDelta.magnitude * 0.5f;
            else if (Vector3.Dot(progressDelta, racingLinePoint.direction) > 2)
                racingLineDistance -= progressDelta.magnitude * 0.5f;

            if (racingLineDistance >= racingLine.distances[racingLine.distances.Length - 1])
                racingLineDistance = 0;

            targetDistanceAhead = Mathf.Clamp((currentSpeed * 0.2f), racingLine.minTargetDistance, racingLine.maxTargetDistance);
        }

        void Recover()
        {
            if (!racerStatistics.started)
                return;

            if (currentSpeed <= 2)
                recoverTimer += Time.deltaTime;
            else
            {
                recoverTimer = 0;
                reverseTimer = 0;
                reversing = false;
            }

            if (recoverTimer >= 2)
            {
                reversing = true;
                reverseTimer += Time.deltaTime;
                if (reverseTimer >= Random.Range(1f, 1.5f))
                {
                    reverseTimer = 0;
                    reversing = false;
                }
            }

            if (recoverTimer > respawnWait)
            {
                Debug.Log("Recover: Respawn ������, ������ ����� �������������������");
                RaceManager.instance.RespawnVehicle(transform);
            }
        }

        void OnCollisionEnter(Collision col)
        {
            Vector3 collisionLocalDelta = transform.InverseTransformPoint(col.transform.position);
            float angle = Mathf.Atan2(collisionLocalDelta.x, collisionLocalDelta.z);
            travelOffset = 1.5f * -Mathf.Sign(angle);
            travelOffset = Mathf.Clamp(travelOffset, -racingLine.GetLeftWidth(racingLineNodeIndex), racingLine.GetRightWidth(racingLineNodeIndex));
            Debug.Log("OnCollisionEnter: travelOffset ���������� � " + travelOffset);
        }

        public bool IsThreatFront()
        {
            return frontSensor.collidersInRange.Count > 0;
        }

        public Collider GetClosestThreat(Collider[] colliders)
        {
            float closestDistanceSqr = Mathf.Infinity;
            Collider closest = colliders[0];
            if (colliders.Length == 1)
                return colliders[0];

            foreach (Collider c in colliders)
            {
                float distanceToTarget = (c.transform.position - transform.position).sqrMagnitude;
                if (distanceToTarget < closestDistanceSqr)
                {
                    closest = c;
                    closestDistanceSqr = distanceToTarget;
                }
            }
            return closest;
        }

        public void SetDifficulty(AiDifficulty difficulty)
        {
            throttleSensitivity = difficulty.throttleSensitivity;
            brakeSensitivity = difficulty.brakeSensitivity;
            steerSensitivity = difficulty.steerSensitivity;
            speedModifier = difficulty.speedModifier;
        }

        void OnDrawGizmos()
        {
            if (visualizeSensors)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Vector3 center = new Vector3(0, frontSensorHeight, (frontSensorDistance / 2) + 0.5f);
                Vector3 size = new Vector3(frontSensorWidth, 1, frontSensorDistance);
                Gizmos.DrawWireCube(center, size);
                Vector3 left_center = new Vector3(-leftSensorOffset, leftSensorHeight, 0);
                Vector3 left_size = new Vector3(leftSensorWidth, 1, leftSensorDistance);
                Gizmos.DrawWireCube(left_center, left_size);
                Vector3 right_center = new Vector3(rightSensorOffset, rightSensorHeight, 0);
                Vector3 right_size = new Vector3(rightSensorWidth, 1, rightSensorDistance);
                Gizmos.DrawWireCube(right_center, right_size);
            }
        }
    }
}
