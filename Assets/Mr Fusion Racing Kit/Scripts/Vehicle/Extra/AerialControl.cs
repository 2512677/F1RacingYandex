using UnityEngine;
using System.Collections;

namespace RGSK
{
    /// <summary>
    /// AerialControl.cs allows the vehicle to flip itself over when upside down and control it's roll, pitch and yaw while airborne
    /// </summary>
	[RequireComponent(typeof(Rigidbody))]
	public class AerialControl : MonoBehaviour 
	{
        private RCC_CarControllerV3 vehicle;
        private Rigidbody rigid;
		private Vector3 groundPos;

		[Header("Grounded")]
		public float flipRollForce = 20000;

		[Header("Airborne")]
		public float rollForce = 10000;
		public float pitchForce = 10000;
		public float yawForce = 10000;

		[Header("Checks")]
		public bool flipped;
		public bool airborne;
		private bool isColliding;

		//Input
		private float pitchInput, yawInput, rollInput;

		void Start()
		{
			rigid = GetComponent<Rigidbody> ();
			vehicle = GetComponent<RCC_CarControllerV3>();
		}
			
		void FixedUpdate()
		{
			groundPos = transform.position + -transform.up;	

			flipped = transform.localEulerAngles.z > 80 && transform.localEulerAngles.z < 280;
			airborne = !Physics.Linecast (transform.position, groundPos) && Mathf.Abs (rigid.linearVelocity.y) > 10;

			if (vehicle != null)
            {
				rollInput = vehicle.steerInput;
				pitchInput = vehicle.throttleInput;
				yawInput = vehicle.handbrakeInput;
            }

			//Flipped State
			if(flipped)
			{
				if(rigid.linearVelocity.magnitude <= 2.0f && !airborne)
					rigid.AddRelativeTorque (-Vector3.forward * flipRollForce * rollInput);
			}

			//Airborne State
			if(airborne)
			{
				rigid.AddRelativeTorque (Vector3.right * pitchForce * pitchInput);

				if(yawInput > 0)
				{
					rigid.AddRelativeTorque (Vector3.up * yawForce * rollInput);
				}
				else
				{
					rigid.AddRelativeTorque (-Vector3.forward * rollForce * rollInput);
				}
			}
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere (groundPos, 0.1f);
		}
	}
}
