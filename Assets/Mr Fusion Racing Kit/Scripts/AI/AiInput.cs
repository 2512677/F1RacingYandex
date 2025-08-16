using UnityEngine;
using System.Collections;
using System;

namespace RGSK
{
    public class AiInput : MonoBehaviour, IAiInput
    {
		private RCC_CarControllerV3 vehicle;
        private Nitro nitro;

        void Start()
        {
			vehicle = GetComponent<RCC_CarControllerV3>();
            nitro = GetComponent<Nitro>();
        }


        public void SetInputValues(float throttle, float brake, float steer, float handbrake)
        {
            if(vehicle != null)
            {
                vehicle.GetInput(throttle, brake, steer, handbrake);
            }

            if(nitro != null)
            {
                nitro.throttle = throttle;
            }
        }
    }
}