using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class Customizer : MonoBehaviour
    {
        public string uniqueID;

        [Header("Visual")]
        public Material material;

        [Header("Performance")]
        public int performanceLevel;
        public PerformanceUpgradeLevel[] performanceUpdgradeLevels;


        void Awake()
        {
            LoadColor();
            LoadPerformanceUpgrade();
        }


        public void LoadColor()
        {
            if (material == null)
                return;

            //Do not load colors if no keys are found for this unique ID
            if (!PlayerPrefs.HasKey(uniqueID + "_colorR"))
                return;

            float r = PlayerPrefs.GetFloat(uniqueID + "_colorR", 1);
            float g = PlayerPrefs.GetFloat(uniqueID + "_colorG", 0);
            float b = PlayerPrefs.GetFloat(uniqueID + "_colorB", 0);

            Color newColor = new Color(r, g, b, 1);
            material.SetColor("_Color", newColor);
        }


        public void SaveColor(Color color)
        {
            if (material == null)
                return;

            PlayerPrefs.SetFloat(uniqueID + "_colorR", color.r);
            PlayerPrefs.SetFloat(uniqueID + "_colorG", color.g);
            PlayerPrefs.SetFloat(uniqueID + "_colorB", color.b);
        }


        public void LoadPerformanceUpgrade()
        {
            RCC_CarControllerV3 vehicle = GetComponent<RCC_CarControllerV3>();
            Rigidbody rigid = GetComponent<Rigidbody>();
            if (vehicle == null)
                return;

            for (int i = 0; i < performanceUpdgradeLevels.Length; i++)
            {
                if(performanceUpdgradeLevels[i].performanceLevel == performanceLevel)
                {
                    //Top Speed / Accel
                    vehicle.engineTorque = performanceUpdgradeLevels[performanceLevel].engineTorque;
                    vehicle.finalDriveRatio = performanceUpdgradeLevels[performanceLevel].finalDriveRatio;
                    vehicle.shiftTime = performanceUpdgradeLevels[performanceLevel].shiftTime;

                    if (rigid != null)
                    {
                        rigid.mass = performanceUpdgradeLevels[performanceLevel].mass;
                    }

                    //Brake
                    vehicle.brakeTorque = performanceUpdgradeLevels[performanceLevel].brakeTorque;

                    //Handling
                    vehicle.minSteerAngle = performanceUpdgradeLevels[performanceLevel].minSteerAngle;
                    vehicle.maxSteerAngle = performanceUpdgradeLevels[performanceLevel].maxSteerAngle;        
                }
            }
        }


        [System.Serializable]
        public class PerformanceUpgradeLevel
        {
            public int performanceLevel;

            [Space(10)]

            //Top Speed / Accel
            public float engineTorque;
            public float finalDriveRatio = 4f;
            public float shiftTime;
            public float mass;

            //Brake
            public float brakeTorque;

            //Handling
            public float minSteerAngle;
            public float maxSteerAngle;
        }
    }
}
