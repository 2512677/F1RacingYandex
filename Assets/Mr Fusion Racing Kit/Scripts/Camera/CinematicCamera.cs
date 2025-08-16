using UnityEngine;
using System.Collections.Generic;

namespace RGSK
{
    [RequireComponent(typeof(Camera))]
    public class CinematicCamera : MonoBehaviour
    {
        public Transform target;
        private Camera thisCamera;
        private TrackCamera[] trackCameras;
        private float newFOV;
        public float minFOV = 20;
        public float maxFOV = 60;
        private float distanceToNextCamera;


        void Start()
        {
            thisCamera = GetComponent<Camera>();
            trackCameras = FindObjectsOfType<TrackCamera>();
        }


        void LateUpdate()
        {
            if (!thisCamera.enabled || target == null)
                return;

            if (trackCameras.Length == 0)
                return;

            //Set the cameras position to the closest camera postion in the cameraPositions list
            transform.position = GetClosestCameraPosition();

            //Look at the target
            transform.LookAt(target.position);

            //Calculate the FOV using the distance from the target
            distanceToNextCamera = Vector3.Distance(target.position, transform.position);
            newFOV = Mathf.Lerp(maxFOV, minFOV, 20 / distanceToNextCamera);
            thisCamera.fieldOfView = Mathf.Lerp(thisCamera.fieldOfView, newFOV, Time.unscaledDeltaTime * 5);
        }


        Vector3 GetClosestCameraPosition()
        {
            //Return the closest cinematic camera positon and use it as our next "go to" position
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 closestPosition = new Vector3();

            foreach (TrackCamera camera in trackCameras)
            {
                float distanceToTarget = (camera.transform.position - target.position).sqrMagnitude;

                if (distanceToTarget < closestDistanceSqr)
                {
                    closestPosition = camera.transform.position;
                    closestDistanceSqr = distanceToTarget;
                }
            }

            return closestPosition;
        }


        public void SetTarget(Transform t)
        {
            target = t;
        }
    }
}