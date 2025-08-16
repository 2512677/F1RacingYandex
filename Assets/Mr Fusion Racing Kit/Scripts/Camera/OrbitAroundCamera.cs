using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class OrbitAroundCamera : MonoBehaviour
    {

        public Transform target;
        public float distance = 5;
        public float height = 2;
        public float rotateSpeed = 5.0f;
        private float x;
        [Space(10)]
        public bool autoFindTarget;
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
            if (target == null)
                return;

            x += Time.unscaledDeltaTime * rotateSpeed;

            Quaternion rotation = Quaternion.Euler(0, x, 0);
            Vector3 position = rotation * (new Vector3(0.0f, height, -distance)) + target.position;

            transform.rotation = rotation;
            transform.position = position;

            transform.LookAt(target);
        }
    }
}
