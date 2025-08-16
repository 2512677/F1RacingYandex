using UnityEngine;
using System.Collections;

namespace RGSK
{

    [RequireComponent(typeof(Camera))]
    public class MinimapCamera : MonoBehaviour
    {
        public Transform target;
        public bool followPosition = true;
        public bool followRotation = true;

        // Параметры динамического зума для ортографической камеры
        public bool dynamicZoom = true;
        public float minSize = 100f;
        public float maxSize = 200f;
        public float speedForMax = 100f; // скорость, при которой достигается maxSize

        private float height = 100;
        private Camera cam;

        private void Start()
        {
            cam = GetComponent<Camera>();

            // Предупреждение, если камера не ортографическая
            if (!cam.orthographic)
            {
                Debug.LogWarning("MinimapCamera работает только с ортографической камерой. Установите Camera.orthographic = true");
            }

            height = cam.farClipPlane / 2;
            transform.position = new Vector3(transform.position.x, height, transform.position.z);

            if (dynamicZoom)
                cam.orthographicSize = minSize;
        }

        void LateUpdate()
        {
            if (!target)
                return;

            if (followPosition)
                transform.position = new Vector3(target.position.x, height, target.position.z);

            if (followRotation)
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, target.eulerAngles.y, transform.eulerAngles.z);

            if (dynamicZoom)
            {
                Rigidbody rb = target.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    float currentSpeed = rb.linearVelocity.magnitude;
                    float t = Mathf.Clamp01(currentSpeed / speedForMax);
                    cam.orthographicSize = Mathf.Lerp(minSize, maxSize, t);
                }
            }
        }

        public void SetTarget(Transform t)
        {
            target = t;
        }
    }
}
