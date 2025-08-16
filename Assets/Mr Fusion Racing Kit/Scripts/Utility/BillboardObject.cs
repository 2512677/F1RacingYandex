using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class BillboardObject : MonoBehaviour
    {
        private CameraManager cameraManager; //Store a reference to the camera manager to fetch the active camera
        private Transform camTransform;

        void Start()
        {
            cameraManager = FindObjectOfType<CameraManager>();  
        }



        void LateUpdate()
        {
            GetCameraTransform();

            if (camTransform != null)
            {
                if (camTransform.InverseTransformPoint(transform.position).z >= 0)
                {
                    Vector3 v = camTransform.position - transform.position;
                    Quaternion rot = Quaternion.LookRotation(-v);
                    transform.rotation = rot;
                }
            }
        }



        void GetCameraTransform()
        {
            if (cameraManager != null)
            {
                camTransform = cameraManager.GetActiveCamera().transform;
            }
        }
    }
}
