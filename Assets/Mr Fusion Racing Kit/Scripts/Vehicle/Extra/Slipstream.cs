using UnityEngine;
using System.Collections;

namespace RGSK
{
    [RequireComponent(typeof(Rigidbody))]
    public class Slipstream : MonoBehaviour
    {
        [Header("Параметры сенсора")]
        [Tooltip("Длина сенсора")]
        public float sensorLength = 20; // Длина сенсора

        [Tooltip("Ширина сенсора")]
        public float sensorWidth = 2f; // Ширина сенсора

        [Tooltip("Вертикальное смещение сенсора")]
        public float yOffset = 0.5f; // Смещение по оси Y

        [Header("Настройки слепстрима")]
        [Tooltip("Настройки для эффекта слепстрима")]
        public SlipstreamSettings slipstreamOptions; // Настройки слепстрима

        private Sensor sensor;
        private Rigidbody rigid;
        private RCC_CarControllerV3 vehicle;
        private bool isSlipstreaming;

        void Start()
        {
            rigid = GetComponent<Rigidbody>();
            vehicle = GetComponent<RCC_CarControllerV3>();

            if (RaceManager.instance != null)
            {
                slipstreamOptions = RaceManager.instance.slipstreamSettings;
            }

            Vector3 center = new Vector3(0, yOffset, (sensorLength / 2) + 0.5f);
            Vector3 size = new Vector3(sensorWidth, 1, sensorLength);
            BoxCollider slipstreamTrigger = new GameObject("slipstream sensor").AddComponent<BoxCollider>();
            slipstreamTrigger.transform.SetParent(transform);
            slipstreamTrigger.transform.localRotation = Quaternion.Euler(Vector3.zero);
            slipstreamTrigger.transform.localPosition = Vector3.zero;
            slipstreamTrigger.center = center;
            slipstreamTrigger.size = size;
            slipstreamTrigger.isTrigger = true;
            slipstreamTrigger.gameObject.layer = LayerMask.NameToLayer("AISensor");
            sensor = slipstreamTrigger.gameObject.AddComponent<Sensor>();
            sensor.AddLayer(LayerMask.NameToLayer("Vehicle"));
        }

        void FixedUpdate()
        {
            if (!slipstreamOptions.enableSlipstream)
                return;

            float speed = rigid.linearVelocity.magnitude * 3.6f;

            isSlipstreaming = sensor.collidersInRange.Count > 0 && (speed >= slipstreamOptions.minSlipstreamSpeed);
            if (isSlipstreaming && vehicle != null)
            {
                rigid.AddForce(transform.forward * slipstreamOptions.slipstreamStrength * vehicle.throttleInput, ForceMode.Acceleration);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            Vector3 center = new Vector3(0, yOffset, (sensorLength / 2) + 0.5f);
            Vector3 size = new Vector3(sensorWidth, 1, sensorLength);
            Gizmos.DrawWireCube(center, size * 0.98f);
        }
    }

    [System.Serializable]
    public class SlipstreamSettings
    {
        [Header("Общие настройки")]
        [Tooltip("Включение/выключение эффекта слепстрима")]
        public bool enableSlipstream = true; // Включение/выключение эффекта

        [Header("Сила слепстрима")]
        [Tooltip("Сила ускорения при слепстриме")]
        [Range(1, 10)] public float slipstreamStrength = 1.1f; // Сила ускорения

        [Header("Минимальная скорость")]
        [Tooltip("Минимальная скорость для активации слепстрима (в км/ч)")]
        public float minSlipstreamSpeed = 100; // Минимальная скорость
    }
}
