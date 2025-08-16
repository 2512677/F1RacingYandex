using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class SwayCamera : MonoBehaviour
    {
        public Transform target;
        public float distance = 5;
        public float height = 1.5f;
        public float lookAngle = 0;
        public float swaySpeed = 0.2f;
        public float swayAmount = 5f;
        [Space(5)]
        public bool autoFindTarget = true;
        public string targetTag = "Player";


        void Update()
        {
            if (autoFindTarget && target == null)
            {
                if (GameObject.FindGameObjectWithTag(targetTag))
                    target = GameObject.FindGameObjectWithTag(targetTag).transform;
            }
        }


        void LateUpdate()
        {
            if (target == null) return;

            float bx = (Mathf.PerlinNoise(0, Time.time * swaySpeed) - 0.5f);
            float by = (Mathf.PerlinNoise(0, (Time.time * swaySpeed) + 100)) - 0.5f;
            
            bx *= swayAmount;
            by *= swayAmount;

            float tx = (Mathf.PerlinNoise(0, Time.time * swaySpeed) - 0.5f);
            float ty = ((Mathf.PerlinNoise(0, (Time.time * swaySpeed) + 100)) - 0.5f);

            float wantedRotation = target.eulerAngles.y + lookAngle;
            Quaternion swayRotation = Quaternion.Euler(bx + tx, by + ty, 0);
            Vector3 wantedPosition = target.position;
            wantedPosition.y = height;
            
            transform.position = wantedPosition;
            transform.position += (swayRotation * Quaternion.Euler(0, wantedRotation, 0)) * Vector3.forward * distance;
            transform.LookAt(target);       
        }
    }
}
