using UnityEngine;
using System.Collections;

namespace RGSK
{
    [RequireComponent(typeof(LensFlare))]
    public class FlareBrightness : MonoBehaviour
    {
        public float maxBrightness = 0.3f;
        public float maxDistance = 50; //расстояние, на котором яркость будет равна 0
        private CameraManager cameraManager;
        private LensFlare lensFlare;

        void Start()
        {
            lensFlare = GetComponent<LensFlare>();
            cameraManager = FindObjectOfType<CameraManager>();
        }


        void Update() 
        {
            //Без менеджера камер мы не сможем получить активную камеру, поэтому возвращаем
            if (cameraManager == null)
                return;

            float distanceFromCamera = (cameraManager.GetActiveCamera().transform.position - transform.position).magnitude;
            float ratio = Mathf.InverseLerp(0, maxDistance, distanceFromCamera);
            lensFlare.brightness = Mathf.Lerp(maxBrightness, 0, ratio);
        }
    }
}