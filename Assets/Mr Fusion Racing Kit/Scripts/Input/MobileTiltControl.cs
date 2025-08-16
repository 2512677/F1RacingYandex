using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class MobileTiltControl : MonoBehaviour
    {
        public MobileTouchButton steerLeft;
        public MobileTouchButton steerRight;
        public float tiltSensitivity = 1f;


        void Update()
        {
            if (steerLeft == null || steerRight == null)
                return;

            float inputValue = Input.acceleration.x * tiltSensitivity;
            steerLeft.InputValue = -Mathf.Clamp01(-inputValue);
            steerRight.InputValue = Mathf.Clamp01(inputValue);
        }
    }
}