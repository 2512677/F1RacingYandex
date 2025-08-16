using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RGSK
{
	public class RGSKMotorbike : RCC_CarControllerV3
	{
		//Chassis
		public Transform chassis;
		public float maxLeanAngle = 40;
		public float maxLeanSpeed = 50;
        public float minLeanHeight;
        public float leanDamping = 2.5f;
		private float lean;
		private float targetAngle;

        //Rider
		public BikeRider bikeRider;
		public float minImpactForce = 20;
        public float resetTime = 3;
        public float resetHeightOffset = 0.1f;

        //
        public float angularDrag = 2.0f;
        public float maxReverseSpeed = 10;
        public Transform fender;
        private RaycastHit hit;


        public override void FixedUpdate()
        {
            base.FixedUpdate();
            LeanChassis();

            if (currentGear <= 1 && throttleInput > 0)
            {
                //Limit the reversing speed
                rigid.linearVelocity = Vector3.ClampMagnitude(rigid.linearVelocity, maxReverseSpeed / 3.6f);
            }

            //Rotate fender with handle bars
            if (fender != null)
            {
                fender.localRotation = steeringObject.localRotation;
            }
        }


        void LeanChassis()
		{
            //Calculate the lean direction
            lean = Mathf.LerpAngle(lean, steerInput * -maxLeanAngle, Time.deltaTime * 5);

            //Calculate the target angle
            targetAngle = Mathf.LerpAngle(targetAngle, lean * Mathf.Clamp(currentSpeedKPH / maxLeanSpeed, 0, 1), Time.deltaTime * leanDamping);

            if (rigid.constraints == RigidbodyConstraints.FreezeRotationZ && chassis != null)
			{
                //Lean the chassis towards the target angle
				chassis.transform.localRotation = Quaternion.Euler(0, 0, transform.localEulerAngles.z + targetAngle);

                //Keep the parent transform upright
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
                
                //Adjust the height of the chassis as the bike leans
                Vector3 localPosition = chassis.localPosition;
                float ratio = Mathf.InverseLerp(0, maxLeanAngle, Mathf.Abs(targetAngle));
                localPosition.y = Mathf.Lerp(0, minLeanHeight, ratio);
                chassis.localPosition = localPosition;
			}

            //Adjust the angular drag depending on whether the bike rider is alive			
            rigid.angularDamping = bikeRider != null ? bikeRider.isAlive ? angularDrag : 0.0f : angularDrag;

            //Freeze the Z rotation depending on whether the bike rider is alive
            rigid.constraints = bikeRider != null ? bikeRider.isAlive ? RigidbodyConstraints.FreezeRotationZ 
                                                                        : RigidbodyConstraints.None 
				                                                        : RigidbodyConstraints.FreezeRotationZ;
		}


		public override void WheelAlignment()
		{
			for (int i = 0; i < vehicleWheels.Count; i++)
			{
                Vector3 localPosition = vehicleWheels[i].wheelTransform.localPosition;
                WheelHit hit;

                if (vehicleWheels[i].wheelCollider.GetGroundHit(out hit))
                {
                    localPosition.y -= Vector3.Dot(vehicleWheels[i].wheelTransform.position - hit.point, transform.up) - vehicleWheels[i].wheelCollider.radius;
                }
                else
                {
                    localPosition.y = vehicleWheels[i].wheelCollider.suspensionDistance * -1;
                }

                //Position the wheels
                vehicleWheels[i].wheelTransform.localPosition = localPosition;

                //Rotate the wheels with the wheel collider
                vehicleWheels[i].wheelTransform.Rotate(vehicleWheels[i].wheelCollider.rpm / 60 * 360 * Time.deltaTime, 0, 0);
                
                if (vehicleWheels[i].steer)
				{
                    //Rotate the wheel based on the wheel collider steer angle
					vehicleWheels[i].wheelTransform.localEulerAngles = new Vector3(vehicleWheels[i].wheelTransform.localEulerAngles.x,
						vehicleWheels[i].wheelCollider.steerAngle - vehicleWheels[i].wheelTransform.localEulerAngles.z,
						vehicleWheels[i].wheelTransform.localEulerAngles.z); 
				}
			}
		}
				

		public override void OnCollisionEnter(Collision col)
		{
			base.OnCollisionEnter (col);

			float impact = col.relativeVelocity.magnitude;

			if (bikeRider != null && impact >= minImpactForce)
			{
				bikeRider.EnableRagdoll();
				ResetValues();
                ignoreInput = true;
				Invoke("ResetBikeRider", resetTime);
			}
		}


		void ResetBikeRider()
		{
			if (RaceManager.instance != null && RaceManager.instance.raceState == RaceState.Replay)
				return;

            bikeRider.DisableRagdoll();       
            ignoreInput = false;
            transform.position = new Vector3(transform.position.x, transform.position.y + resetHeightOffset, transform.position.z);

            if (RaceManager.instance != null)
			{
				RaceManager.instance.RespawnVehicle(transform);
			}
		}
	}
}