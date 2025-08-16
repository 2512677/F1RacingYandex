using UnityEngine;
using System.Collections;
using System;

namespace RGSK
{
    public class OrbitCamera : MonoBehaviour
    {
        public Transform target;
        public float distance = 5.0f;
        public float distanceMin = .5f;
        public float distanceMax = 15f;

        [Space(10)]
        public float xSpeed = 120.0f;
        public float ySpeed = 120.0f;
        public float yMinLimit = -20f;
        public float yMaxLimit = 80f;

        float x = 0.0f, velX;
        float y = 0.0f, velY;
        float inputX, inputY;

        [Header("Orbit Setting")]
        public float orbitSpeed = 1;
        public KeyCode orbitButton = KeyCode.Mouse1;
        private bool inOrbit;
        private bool isMobilePlatform;
        private Touch touch;

        void Start()
        {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;

            isMobilePlatform = Application.isMobilePlatform;
        }


        void Update()
        {
            inOrbit = !isMobilePlatform ? Input.GetKey(orbitButton) : Input.touchCount > 0;

            if (!isMobilePlatform)
            {
                inputX = Input.GetAxis("LookX") * xSpeed * 0.02f;
                inputY = Input.GetAxis("LookY") * ySpeed * 0.02f;
            }
            else
            {
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    touch = Input.GetTouch(0);
                    inputX = touch.deltaPosition.x * xSpeed * 0.02f;
                    inputY = touch.deltaPosition.y * ySpeed * 0.02f;
                }
            }
        }


        void LateUpdate()
        {
            if (target)
            {
                if(inOrbit)
                {
                    velX += inputX * xSpeed * distance * 0.02f;
                    velY -= inputY * ySpeed * 0.02f;
                }

                y = Mathf.Lerp(y, velY, orbitSpeed * Time.deltaTime);
                x = Mathf.Lerp(x, velX, orbitSpeed * Time.deltaTime);

                y = ClampAngle(y, yMinLimit, yMaxLimit);

                Quaternion rotation = Quaternion.Euler(y, x, 0);

                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * negDistance + target.position;

                transform.rotation = rotation;
                transform.position = position;
            }
        }


        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }


        public void SetTarget(Transform t)
        {
            target = t;
        }
    }
}
