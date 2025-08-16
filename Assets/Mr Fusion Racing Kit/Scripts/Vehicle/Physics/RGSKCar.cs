using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RGSK
{
	public class RGSKCar : RCC_CarControllerV3
	{
		public float antiRollAmount = 5000;


		public override void FixedUpdate()
		{
			base.FixedUpdate ();
            AntirollBars ();
        }


		void AntirollBars()
		{
			//Front, assuming [0] and [1] are the front wheels
			WheelHit hit1;
			float travel01 = 1.0f;
			float travel02 = 1.0f;

			bool grounded01 = vehicleWheels[0].wheelCollider.GetGroundHit(out hit1);
			if (grounded01)
				travel01 = (-vehicleWheels[0].wheelCollider.transform.InverseTransformPoint(hit1.point).y - vehicleWheels[0].wheelCollider.radius)
					/ vehicleWheels[0].wheelCollider.suspensionDistance;

			bool grounded02 = vehicleWheels[1].wheelCollider.GetGroundHit(out hit1);
			if (grounded02)
				travel02 = (-vehicleWheels[1].wheelCollider.transform.InverseTransformPoint(hit1.point).y - vehicleWheels[1].wheelCollider.radius)
					/ vehicleWheels[1].wheelCollider.suspensionDistance;

			float antiRollForce1 = (travel01 - travel02) * antiRollAmount;

			if (grounded01)
				rigid.AddForceAtPosition(vehicleWheels[0].wheelCollider.transform.up * -antiRollForce1, vehicleWheels[0].wheelCollider.transform.position);

			if (grounded02)
				rigid.AddForceAtPosition(vehicleWheels[1].wheelCollider.transform.up * antiRollForce1, vehicleWheels[1].wheelCollider.transform.position);

			//Rear, assuming [2] and [3] are the rear wheels
			WheelHit hit2;
			float travel03 = 1.0f;
			float travel04 = 1.0f;

			bool grounded03 = vehicleWheels[2].wheelCollider.GetGroundHit(out hit2);
			if (grounded03)
				travel03 = (-vehicleWheels[2].wheelCollider.transform.InverseTransformPoint(hit2.point).y - vehicleWheels[2].wheelCollider.radius)
					/ vehicleWheels[2].wheelCollider.suspensionDistance;

			bool grounded04 = vehicleWheels[3].wheelCollider.GetGroundHit(out hit2);
			if (grounded04)
				travel04 = (-vehicleWheels[3].wheelCollider.transform.InverseTransformPoint(hit2.point).y - vehicleWheels[3].wheelCollider.radius)
					/ vehicleWheels[3].wheelCollider.suspensionDistance;

			float antiRollForce2 = (travel03 - travel04) * antiRollAmount;

			if (grounded03)
				rigid.AddForceAtPosition(vehicleWheels[2].wheelCollider.transform.up * -antiRollForce2, vehicleWheels[2].wheelCollider.transform.position);

			if (grounded04)
				rigid.AddForceAtPosition(vehicleWheels[3].wheelCollider.transform.up * antiRollForce2, vehicleWheels[3].wheelCollider.transform.position);
		}


        public override void WheelAlignment()
        {
            for (int i = 0; i < vehicleWheels.Count; i++)
            {
                Quaternion rot;
                Vector3 pos;

                vehicleWheels[i].wheelCollider.GetWorldPose(out pos, out rot);

                //Set rotation & position of the wheels
                vehicleWheels[i].wheelTransform.rotation = rot;
                vehicleWheels[i].wheelTransform.position = pos;
            }
        }
    }
}