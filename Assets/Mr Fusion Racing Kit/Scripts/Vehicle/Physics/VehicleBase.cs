using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RGSK
{
	public abstract class RCC_CarControllerV3 : MonoBehaviour
	{
		protected Rigidbody rigid;

        //Wheel Settings
		public List<WheelInfo> vehicleWheels = new List<WheelInfo>();
        public int frictionCurveID;
        public int frictionCurveID2;

        //Engine Settings
        public float maxEngineRPM = 8000;
		public float minEngineRPM = 1000;
		public float engineTorque = 250;
		public float engineTorqueRPM = 6000;
		public float engineRPM;
		public float currentSpeedKPH, currentSpeedMPH;
		public bool autoGenerateTorqueCurve = true;
		public Vector2[] torqueCurveValues =  //x = RPM, y = Torque
		{
			new Vector2  (0,0),
			new Vector2  (0,0),
			new Vector2  (0,0),
			new Vector2  (0,0),
			new Vector2  (0,0),
			new Vector2  (0,0),
			new Vector2  (0,0),
			new Vector2  (0,0),
			new Vector2  (0,0)
		};
		public AnimationCurve engineTorqueCurve;
		public AnimationCurve enginePowerCurve;
        public bool engineOn = true;

        //Transmission Settings
		public float[] gearRatios = { -3.5f, 0, 3.6f, 2.1f, 1.5f, 1.2f, 1f, 0.75f }; //R,N...
		public float finalDriveRatio = 4f;
        public TransmissionType transmissionType;
        public float shiftUpRPM = 7500;
        public float shiftDownRPM;
        public float minShiftDownRPM = 4000;
        public float maxShiftDownRPM = 5000;
        public int currentGear = 2;       
		public float shiftTime = 0.25f;
		public float min1stGearSpeed = 35;
		private bool shiftingGear;
		private int targetGear;
		private float lastShift;
		private bool revLimiterActive;
		private float poweredWheels;
		private bool isAutoReversing;

        //Brake Settings
        public float brakeTorque = 2000;
        public float handbrakeTorque = 5000;
        public float handbrakeFriction = 0.5f;
        public float decelerationRate = 2000;

        //Steering Settings
        public float minSteerAngle = 10;
        public float maxSteerAngle = 30;
        public float currentSteerAngle;
        public SteeringAxis steeringObjectAxis;
        public Transform steeringObject;
        public float maxSteeringObjectAngle = 90;
        private Quaternion targetSteeringRotation;

        //Suspension Settings
		public float frontSuspensionDistance = 0.3f;
		public float frontSuspensionSpring = 35000;
		public float frontSuspensionDamper = 4500;
		public float rearSuspensionDistance = 0.3f;
		public float rearSuspensionSpring = 35000;
		public float rearSuspensionDamper = 4500;
		public bool applySuspensionSettings = true;

		//Stability Settings
		public Vector3 centerOfMassOffset = new Vector3(0, 0, 0);
		public Transform centerOfMassPosition;
		public float downForce = 2;
        [Range(0, 1)]public float revShakeAmount = 0.25f;

       
		//Audio Settings
		public AudioClip engineAudio;
		public AudioClip gearshiftAudio;
		public AudioClip backfireAudio;
		public AudioClip headlightToggleAudio;
		public AudioClip windAudio;
        public AudioClip wheelImpactAudio;
        private AudioSource engineAudioSource;
		private AudioSource gearShiftAudioSource;
		private AudioSource backfireAudioSource;
		private AudioSource crashAudioSource;
		private AudioSource scrapeAudioSource;
		private AudioSource headlightAudioSource;
		private AudioSource windAudioSource;
        private AudioSource wheelImpactAudioSource;
        public Transform engineAudioPosition;
		public Transform backfireAudioPosition;
        public AnimationCurve enginePitchCurve;
        private float lastWheelImpact;

		//Dashboard Settings
		public TextMesh speedTextMesh;
		public TextMesh gearTextMesh;
		public TextMesh rpmTextMesh;
		public TextMesh fuelTextMesh;

        public Transform tachometerNeedle;
		public float minTachoAngle, maxTachoAngle;
		public float tachoOffsetValue = 1;

        public Transform speedometerNeedle;
		public float minSpeedoAngle, maxSpeedoAngle;
		public float speedoOffsetValue = 1;

        public Transform fuelNeedle;
		public float minFuelAngle, maxFuelAngle;
		public float fuelOffsetValue = 1;

        //Driving Assist Settings
        public bool TCS = true;
        [Range(0.01f, 0.5f)]
        public float tcsSlip = 0.25f;
        public bool isTcsActive { get; private set; }

        public bool ABS = true;
        [Range(0.01f, 0.5f)]
        public float absSlip = 0.25f;
        public bool isAbsActive { get; private set; }

        [Range(0, 1)]
        public float steerHelper = 0.25f;
        private float currentRotation;

        //Backfire Settings
        public bool enableBackfire;
        public ParticleSystem[] backfireEffects;
        private bool canBackfire = true;
        private float lastBackfire;

        //Light Settings
        public GameObject brakeLights;
        public GameObject reverseLights;
        public GameObject headLightGameObjects;
        public GameObject tailLightGameObjects;
        private bool headlightsOn;

        //Fuel Settings
        public bool fuelConsumption;
        public float fuel = 30;
        public float maxFuel = 60;
        public float fuelConsumptionRate = 0.01f; //minutes = (fuel / fuelConsumption) / 60
        
        //Other
		public bool showTelemetry;
		private ParticleSystem collisionParticle;
		private TrackSurface trackSurface;

		//Input
		public float throttleInput { get; set; }
		public float steerInput { get; set; }
		public float brakeInput { get; set; }
		public float handbrakeInput { get; set; }
        public bool engineRunning { get; internal set; }

        public bool ignoreInput;


        void Start()
		{
			//Get the rigidbody component
			rigid = GetComponent<Rigidbody>();

			//Perform initial setups
			poweredWheels = GetPoweredWheels();
			SetupCenterOfMass();
			SetupEngineTorqueCurve();
			SetupEngineAudioCurve();
			CreateVehicleAudioAndEffects();
			SetupWheelFrictionCurves();
			SetupSuspension();
			Headlights(false);
            engineOn = true;
        }


		void SetupCenterOfMass()
		{
			//Configure the center of mass to the transform if assigned, else use the Vector3 field
			if (centerOfMassPosition != null)
			{
				rigid.centerOfMass = transform.InverseTransformPoint(centerOfMassPosition.transform.position);
			}
			else
			{
				rigid.centerOfMass = centerOfMassOffset;
			}
		}


		public void SetupEngineTorqueCurve()
		{         
			engineTorqueCurve = new AnimationCurve();
			enginePowerCurve = new AnimationCurve();

			if (autoGenerateTorqueCurve)
			{
				//Round the max engine RPM to the nearest 1000
				maxEngineRPM = Helper.RoundToNearest1000(maxEngineRPM);

				//Automatically calculate torque curve values
				torqueCurveValues = new Vector2[((int)maxEngineRPM / 1000) + 1];
				for (int i = 0; i < torqueCurveValues.Length; i++)
				{
					if (i > 0)
					{
						//RPMS (x1000)
						torqueCurveValues[i].x = i * 1000;

						//Torque(Nm)
						if (torqueCurveValues[i].x < maxEngineRPM)
							torqueCurveValues[i].y = engineTorque * (-Sqr(torqueCurveValues[i].x / engineTorqueRPM - 1) + 1);
					}
				}
			}

			//Setup the torque and power animation curves
			for (int i = 0; i < torqueCurveValues.Length; i++)
			{
				engineTorqueCurve.AddKey(torqueCurveValues[i].x, torqueCurveValues[i].y);
				enginePowerCurve.AddKey(torqueCurveValues[i].x, torqueCurveValues[i].y * torqueCurveValues[i].x / 5252);
			}          

			//Make the torque and power curves linear
			Helper.SetCurveLinear(engineTorqueCurve);
			Helper.SetCurveLinear(enginePowerCurve);
		}


		void SetupEngineAudioCurve()
		{
			enginePitchCurve = new AnimationCurve();

			Keyframe[] keyframes = new Keyframe[2];
			keyframes[0] = new Keyframe(minEngineRPM, 0.8f);
			keyframes[1] = new Keyframe(maxEngineRPM, 2.5f);
			keyframes[0].inTangent = 0.0047f * Mathf.Deg2Rad;
			keyframes[0].outTangent = 0.0047f * Mathf.Deg2Rad;
			keyframes[1].inTangent = 0.0242f * Mathf.Deg2Rad;
			keyframes[1].outTangent = 0.0242f * Mathf.Deg2Rad;
			enginePitchCurve = new AnimationCurve(keyframes);
		}


		void CreateVehicleAudioAndEffects()
		{
			//Create the engine audio source
			if (engineAudio != null)
			{
				engineAudioSource = Helper.CreateAudioSource(gameObject, engineAudio, "SFX", 1, 1, true, true);
				engineAudioSource.dopplerLevel = 0;

				if (engineAudioPosition != null)
					engineAudioSource.transform.localPosition = engineAudioPosition.localPosition;
			}

			//Create the gear shift audio source
			if (gearshiftAudio != null)
			{
				gearShiftAudioSource = Helper.CreateAudioSource(gameObject, gearshiftAudio, "SFX", 1, 0.25f, false, false);
			}

			//Create the backfire audio source
			if (backfireAudio != null)
			{
				backfireAudioSource = Helper.CreateAudioSource(gameObject, backfireAudio, "SFX", 1, 0.25f, false, false);

				if (backfireAudioPosition != null)
					backfireAudioSource.transform.localPosition = backfireAudioPosition.localPosition;
			}

			//Create the crash audio source
			crashAudioSource = Helper.CreateAudioSource(gameObject, null, "SFX", 1, 1, false, false);

			//Create the crash scrape audio source
			scrapeAudioSource = Helper.CreateAudioSource(gameObject, null, "SFX", 1, 1, true, false);

			//Create headlight toggle audio source
			if (headlightToggleAudio != null)
			{
				headlightAudioSource = Helper.CreateAudioSource(gameObject, headlightToggleAudio, "SFX", 1, 1, false, false);
			}

			//Create wind audio source
			if (windAudio != null)
			{
				windAudioSource = Helper.CreateAudioSource(gameObject, windAudio, "SFX", 1, 0, true, true);
			}

            //Create wind audio source
            if (wheelImpactAudio != null)
            {
                wheelImpactAudioSource = Helper.CreateAudioSource(gameObject, wheelImpactAudio, "SFX", 1, 0.75f, false, false);
            }

            //Create collision effect particle systems
            trackSurface = FindObjectOfType<TrackSurface>();
			if(trackSurface != null)
			{
				for (int i = 0; i < trackSurface.collisionEffects.Length; i++)
				{
					//Instantiate collision particles
					if(trackSurface.collisionEffects[i].collisionParticle != null)
					{
						Instantiate(trackSurface.collisionEffects[i].collisionParticle, transform.position, Quaternion.identity, transform);
					}
				}
			}
		}


		void SetupWheelFrictionCurves()
		{
            //Assign the wheel component from the wheels
            for (int i = 0; i < vehicleWheels.Count; i++)
            {
                vehicleWheels[i].wheelComponent = vehicleWheels[i].wheelCollider.GetComponent<VehicleWheel>();
            }

            WheelFrictionCurveData data = WheelFrictionCurveData.Instance;
			if (data == null)
			{
				Debug.LogWarning("WheelFrictionCurveData was not found. Please create one and place it in the 'Resources/RGSK/ScriptableObjects' folder.");    
				return;
			}

			WheelFrictionCurve forwardCurve = new WheelFrictionCurve();
			WheelFrictionCurve sidewaysCurve = new WheelFrictionCurve();

			//Set a default stiffness of 1
			forwardCurve.stiffness = 1;
			sidewaysCurve.stiffness = 1;

			WheelFrictionCurveData.FrictionCurves curve = data.GetFrictionCurve(frictionCurveID);
			if(curve != null)
			{
				forwardCurve.extremumSlip = curve.forwardExtremumSlip;
				forwardCurve.extremumValue = curve.forwardExtremumValue;
				forwardCurve.asymptoteSlip = curve.forwardAsymptoteSlip;
				forwardCurve.asymptoteValue = curve.forwardAsymptoteValue;

                sidewaysCurve.extremumSlip = curve.sidewaysExtremumSlip;
				sidewaysCurve.extremumValue = curve.sidewaysExtremumValue;
				sidewaysCurve.asymptoteSlip = curve.sidewaysAsymptoteSlip;
				sidewaysCurve.asymptoteValue = curve.sidewaysAsymptoteValue;
			}
			else
			{
				Debug.LogWarning("The friction curve ID: " + frictionCurveID + " was not found. Please enter a vaild ID.");
				return; 
			}

			//Loop through all the wheels and assign the friction curve values
			for (int i = 0; i < vehicleWheels.Count; i++)
			{
                if (vehicleWheels[i].wheelComponent != null)
				{
					vehicleWheels[i].wheelComponent.UpdateForwardFrictionCurve(forwardCurve);
					vehicleWheels[i].wheelComponent.UpdateSidewaysFrictionCurve(sidewaysCurve);
				}                
			}
		}


		void SetupSuspension()
		{
			if (!applySuspensionSettings)
				return;
			
			for (int i = 0; i < vehicleWheels.Count; i++)
			{
				JointSpring fSpring = vehicleWheels[i].wheelCollider.suspensionSpring;
				fSpring.spring = frontSuspensionSpring;
				fSpring.damper = frontSuspensionDamper;

				JointSpring rSpring = vehicleWheels[i].wheelCollider.suspensionSpring;
				rSpring.spring = rearSuspensionSpring;
				rSpring.damper = rearSuspensionDamper;

				//Apply front suspension
				if (vehicleWheels [i].wheelAxle == WheelAxle.Front) 
				{
					vehicleWheels [i].wheelCollider.suspensionDistance = frontSuspensionDistance;
					vehicleWheels [i].wheelCollider.suspensionSpring = fSpring;
				}

				//Apply rear suspension
				if (vehicleWheels [i].wheelAxle == WheelAxle.Rear) 
				{
					vehicleWheels [i].wheelCollider.suspensionDistance = rearSuspensionDistance;
					vehicleWheels [i].wheelCollider.suspensionSpring = rSpring;
				}
			}
		}


		public virtual void FixedUpdate()
		{
			Drivetrain();
			WheelAlignment();
            VehicleSounds();
            SteerHelper();
			Backfire();
			RigidbodyForces();
			RotateSteeringMechanism();
			RearLights();
			Dashboard();
            FuelConsuption();
		}


        public void GetInput(float throttle, float brake, float steer, float handbrake)
		{
            if (ignoreInput)
                return;

			throttleInput = throttle;
			brakeInput = brake;
			steerInput = steer;
			handbrakeInput = handbrake;

			//handle automatic reverse logic
			if (transmissionType == TransmissionType.Automatic)
			{
				if (brakeInput > 0 && currentSpeedKPH <= 2)
				{
					isAutoReversing = true;
					ShiftToGear(0);
				}
				if (throttleInput > 0 && currentSpeedKPH <= 2 && currentGear == 0)
				{
					isAutoReversing = false;
					ShiftToGear(2);
				}
				if(currentGear == 1 && currentSpeedKPH <= 2 && isAutoReversing)
				{
					isAutoReversing = false;
				}

				if (isAutoReversing)
				{
					float temp = throttleInput;
					throttleInput = brakeInput;
					brakeInput = temp;
				}
			}
		}


		void Drivetrain()
		{
			//Calculate engine RPM     
			revLimiterActive = engineRPM > maxEngineRPM;
			if (engineOn)
			{
                engineRPM = !revLimiterActive ?
                    Mathf.Lerp(engineRPM, minEngineRPM + (GetWheelRPM() * ((currentGear != 1 && handbrakeInput < 1) ? 
						Mathf.Abs(gearRatios[currentGear]) 
						: gearRatios[2]) * finalDriveRatio), Time.fixedDeltaTime * 5)
					    : engineRPM -= 300;
			}
			else
			{
				engineRPM = Mathf.Lerp(engineRPM, 0, Time.fixedDeltaTime * 5);
			}

            //Clamp engine rpm
            engineRPM = Mathf.Abs(engineRPM);
            engineRPM = Mathf.Clamp(engineRPM, 0, maxEngineRPM * 1.05f);
			
			//Calculate steering angle
			float steeringFactor = currentSpeedKPH / 200;
            currentSteerAngle = Mathf.Lerp(maxSteerAngle, minSteerAngle, steeringFactor);

			//Calculate the total torque for each of the powered wheels
			float motorTorque = !shiftingGear ? (engineTorqueCurve.Evaluate(engineRPM) * gearRatios[currentGear] * finalDriveRatio) / poweredWheels : 0;

			foreach (WheelInfo wheel in vehicleWheels)
			{
				WheelHit hit;
				wheel.wheelCollider.GetGroundHit(out hit);

				//Apply motor torqe
				if (wheel.drive)
				{
					if(TCS)
						isTcsActive = throttleInput > 0 && Mathf.Abs(hit.forwardSlip) >= tcsSlip;

					wheel.wheelCollider.motorTorque = !isTcsActive ? motorTorque * throttleInput : (motorTorque * throttleInput) / 2;
                }

				//Apply brake torque
				if (wheel.brake)
				{
					if (handbrakeInput < 1)
					{
						if(ABS)
							isAbsActive = brakeInput > 0 && Mathf.Abs(hit.forwardSlip) >= absSlip;

						wheel.wheelCollider.brakeTorque = !isAbsActive ? (brakeTorque * brakeInput) : 0;
					}
				}

				//Apply steer
				if (wheel.steer)
				{
					wheel.wheelCollider.steerAngle = currentSteerAngle * steerInput;
				}

				//Apply handbrake
				if (wheel.handbrake)
				{
                    if(brakeInput < 1)
					    wheel.wheelCollider.brakeTorque = (handbrakeTorque * handbrakeInput);

					if (handbrakeInput > 0)
					{
						if (wheel.wheelComponent != null)
						{
							wheel.wheelComponent.OverrideSidewaysFriction(handbrakeFriction);
						}
					}
					else
					{
						if (wheel.wheelComponent != null)
						{
							wheel.wheelComponent.RevertFrictionValuesToMatchSurface();
						}
					}
				}
			}

			//Calculate vehicle speeds
			currentSpeedKPH = (int)Helper.MpsToKph(rigid.linearVelocity.magnitude);
			currentSpeedMPH = (int)Helper.MpsToMph(rigid.linearVelocity.magnitude);

            //Handle automatic transmission logic
            if (transmissionType == TransmissionType.Automatic)
            {
                AutomaticTransmission();
            }
		}


		void AutomaticTransmission()
		{
            //Shift up
			if (engineRPM >= shiftUpRPM 
                && currentGear > 0 
                && throttleInput > 0.5f
                && handbrakeInput < 1 
                && !isAutoReversing)
			{
                //Avoid shifting too early into 2nd gear
                if (currentGear == 2 && currentSpeedKPH < min1stGearSpeed)
                    return;

                //Leave a 1 second gap between shifts
				if (Time.time > lastShift)
				{
					lastShift = Time.time + 1;
					ShiftUp();
				}
			}

            //Shift down
			if (engineRPM <= shiftDownRPM && currentGear > 2)
			{
				ShiftDown();
			}

            //Calculate an appropriate shift down RPM for the current gear
            shiftDownRPM = CalculateShiftDownRPM();
		}


		public void ShiftUp()
		{
			if (shiftingGear)
				return;

			if (currentGear < gearRatios.Length - 1)
			{
				targetGear = currentGear + 1;
				StartCoroutine(ShiftToTargetGear());
			}
		}


		public void ShiftDown()
		{
			if (shiftingGear)
				return;

			if (currentGear > 0)
			{
				targetGear = currentGear - 1;
				StartCoroutine(ShiftToTargetGear());
			}
		}


		public void ShiftToGear(int gear)
		{
			currentGear = gear;
		}


		IEnumerator ShiftToTargetGear()
		{
			shiftingGear = true;

			if (gearShiftAudioSource != null)
			{
				gearShiftAudioSource.Play();
			}

			yield return new WaitForSeconds(shiftTime);

			currentGear = targetGear;

			shiftingGear = false;
		}


        void VehicleSounds()
        {
            //Adjust engine sound based on RPM
            if (engineAudioSource != null)
            {
                float factor = engineRPM / maxEngineRPM;
                engineAudioSource.volume = (engineOn ? 0.25f : 0) + factor;
                engineAudioSource.pitch = enginePitchCurve.Evaluate(engineRPM);
            }

            //Adjust wind volume based on speed
            if (windAudioSource != null)
            {
                float ratio = Mathf.InverseLerp(100, 300, currentSpeedKPH);
                windAudioSource.volume = ratio;
                windAudioSource.pitch = Random.Range(0.9f, 1);
            }

            //Wheel collision sound
            if (wheelImpactAudioSource != null)
            {
                if(Time.time > lastWheelImpact)
                {
                    for (int i = 0; i < vehicleWheels.Count; i++)
                    {
                        if (vehicleWheels[i].wheelComponent != null)
                        {
                            if (vehicleWheels[i].wheelComponent.wheelImpact)
                            {
                                lastWheelImpact = Time.time + 0.25f;
                                wheelImpactAudioSource.transform.position = vehicleWheels[i].wheelTransform.position;
                                wheelImpactAudioSource.Play();
                            }
                        }
                    }
                }
            }
        }


        void FuelConsuption()
        {
            if (!fuelConsumption)
                return;

            if (engineOn && currentSpeedKPH > 1 && currentGear != 1)
                fuel -= fuelConsumptionRate * throttleInput * Time.deltaTime;

            fuel = Mathf.Clamp(fuel, 0, maxFuel);
            if (fuel <= 0 && engineOn)
                engineOn = false;
        }


        void RigidbodyForces()
        {
            //Shake the chassis when idle reving
            if (rigid.linearVelocity.magnitude < 1)
            {
                float shakeForce = ((engineRPM / 2) * revShakeAmount * throttleInput) * Random.Range(-1.0f, 1.0f);
                rigid.AddRelativeTorque(Vector3.forward * shakeForce);
            }

            //Apply downforce
            rigid.AddForce(-transform.up * downForce * rigid.linearVelocity.magnitude);

            //Decelerate the vehicle
            if (throttleInput <= 0 && brakeInput <= 0 && engineOn)
            {
                if (Direction() > 0.1f)
                {
                    //Add a backward force to decelerate the vehicle
                    rigid.AddForce(-transform.forward * decelerationRate);
                }
                else if (Direction() < -0.1f)
                {
                    //Add a forward force to decelerate the vehicle
                    rigid.AddForce(transform.forward * decelerationRate);
                }
            }
        }


        void Backfire()
		{
			if (!engineOn)
				return;

			if (enableBackfire)
			{
				//check if the vehicle can backfire
				if (engineRPM < maxEngineRPM * 0.9f && !canBackfire)
				{
					canBackfire = true;
				}

				//Upshift backfire
				if (engineRPM >= shiftUpRPM && canBackfire)
				{
					if (throttleInput > 0 && currentGear > 0 && currentGear < gearRatios.Length - 1)
					{
						ActivateBackfire();
					}
				}

				//Downshift backfire
				if (engineRPM <= maxEngineRPM * 0.5f && canBackfire)
				{
					if (brakeInput > 0 && currentGear > 2 && currentSpeedKPH > 10f)
					{
						ActivateBackfire();
					}
				}
			}
		}


		void ActivateBackfire()
		{
			if (Time.time > lastBackfire)
			{
				lastBackfire = Time.time + Random.Range(0.5f, 2f);

				canBackfire = false;

				//Particle effect
				if (backfireEffects.Length > 0)
				{
					foreach (ParticleSystem p in backfireEffects)
					{
						if (!p.isPlaying)
							p.Emit(1);
					}
				}

				//Play sound
				if (backfireAudioSource != null)
				{
					if (!backfireAudioSource.isPlaying)
					{
						backfireAudioSource.pitch = Random.Range(0.9f, 1.1f);
						backfireAudioSource.Play();
					}
				}
			}
		}


        public virtual void WheelAlignment(){}


		void SteerHelper()
		{
			for (int i = 0; i < vehicleWheels.Count; i++)
			{
				WheelHit wheelhit;
				vehicleWheels[i].wheelCollider.GetGroundHit(out wheelhit);
				if (wheelhit.normal == Vector3.zero)
					return;
			}

			if (Mathf.Abs(currentRotation - transform.eulerAngles.y) < 10f)
			{
				float turnadjust = (transform.eulerAngles.y - currentRotation) * steerHelper;
				Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
				rigid.linearVelocity = velRotation * rigid.linearVelocity;
			}

			currentRotation = transform.eulerAngles.y;
		}


		void RotateSteeringMechanism()
		{
            if (steeringObject != null)
            {
                float angle = steerInput * maxSteeringObjectAngle;
                Quaternion currentRotation = steeringObject.localRotation;

                switch (steeringObjectAxis)
                {
                    case SteeringAxis.Z:
                        targetSteeringRotation = Quaternion.Euler(steeringObject.localEulerAngles.x,
                            steeringObject.localEulerAngles.y, -angle);
                        break;

                    case SteeringAxis.Y:
                        targetSteeringRotation = Quaternion.Euler(steeringObject.localEulerAngles.x, angle,
                    steeringObject.localEulerAngles.z);
                        break;
                }


                steeringObject.localRotation = Quaternion.Lerp(currentRotation, targetSteeringRotation, 5.0f * Time.fixedDeltaTime);
			}
		}


		void RearLights()
		{
			//Brake lights
			if (brakeLights != null) 
			{
				brakeLights.SetActive (brakeInput > 0);
			}

			//Reverse lights
			if (reverseLights != null) 
			{
				reverseLights.SetActive ((currentGear == 0 && throttleInput > 0));
			}
		}


		void Dashboard()
		{
			if(speedTextMesh != null)
			{
				speedTextMesh.text = currentSpeedKPH.ToString();
			}

			if (gearTextMesh != null)
			{
				gearTextMesh.text = currentGear == 0 ? "R" : (currentGear == 1) ? "N" : (currentGear - 1).ToString();
			}

			if (rpmTextMesh != null)
			{
				rpmTextMesh.text = "" + (int)engineRPM;
			}

			if (fuelTextMesh != null)
			{
				fuelTextMesh.text = fuel.ToString("F1");
			}

			if (tachometerNeedle)
			{
				float ratio = Mathf.InverseLerp(0, maxEngineRPM, engineRPM);
				float tachoAngle = Mathf.Lerp(minTachoAngle, maxTachoAngle, ratio) * tachoOffsetValue;
				tachometerNeedle.localRotation = Quaternion.Euler(0, 0, -tachoAngle);
			}

			if (speedometerNeedle)
			{
				float speedoAngle = Mathf.Clamp(currentSpeedKPH, minSpeedoAngle, maxSpeedoAngle) * speedoOffsetValue;
				speedometerNeedle.localRotation = Quaternion.Euler(0, 0, -speedoAngle);
			}

			if (fuelNeedle)
			{
				float ratio = Mathf.InverseLerp(0, maxFuel, fuel);
				float angle = Mathf.Lerp(minFuelAngle, maxFuelAngle, ratio) * fuelOffsetValue;
				fuelNeedle.localRotation = Quaternion.Euler(0, 0, -angle);
			}
		}


		public void ToggleHeadlights()
		{
			headlightsOn = !headlightsOn;
			Headlights(headlightsOn);
		}


		void Headlights(bool on)
		{
			if(headlightToggleAudio != null)
			{
				headlightAudioSource.PlayOneShot(headlightToggleAudio,1);
			}

			if (headLightGameObjects != null)
			{
				headLightGameObjects.SetActive(on);
			}

			if (tailLightGameObjects != null)
			{
				tailLightGameObjects.SetActive(on);
			}
		}


		public void ToggleEngine()
		{
			engineOn = !engineOn;
		}


		public virtual void OnCollisionEnter(Collision col)
		{
			float impact = col.relativeVelocity.magnitude;

			if(trackSurface != null)
			{
				crashAudioSource.clip = null;
				scrapeAudioSource.clip = null;
				collisionParticle = null;

				CollisionEffects collisionEffect = trackSurface.GetCollisionEffectData(col.collider.sharedMaterial);
				if (collisionEffect != null)
				{
					//Assign audio
					if (collisionEffect.collisionSound != null)
					{
						crashAudioSource.clip = collisionEffect.collisionSound;
					}

					if (collisionEffect.collisionScrapeSound != null)
					{
						scrapeAudioSource.clip = collisionEffect.collisionScrapeSound;
					}

					//Assign particle effect
					if (collisionEffect.collisionParticle != null)
					{
						collisionParticle = transform.Find(collisionEffect.collisionParticle.name + "(Clone)").GetComponent<ParticleSystem>();
					}
				}
			}

			if (crashAudioSource != null)
			{
				float volume = Mathf.Clamp01(impact * 0.1f);
				crashAudioSource.transform.localPosition = transform.InverseTransformPoint(col.contacts[0].point);
				crashAudioSource.volume = volume;
				crashAudioSource.Play();
			}

			if(collisionParticle != null)
			{
				if (impact >= 5)
				{
					collisionParticle.transform.position = col.contacts[0].point;
					collisionParticle.Emit(1);
				}
			}
		}


		void OnCollisionStay(Collision col)
		{
			float impact = col.relativeVelocity.magnitude;

			if (scrapeAudioSource != null)
			{
				float volume = Mathf.Clamp01(impact * 0.1f);
				Vector3 local = transform.InverseTransformPoint(col.contacts[0].point);
				scrapeAudioSource.transform.localPosition = new Vector2(local.x, local.y);

				scrapeAudioSource.volume = volume;

				if (!scrapeAudioSource.isPlaying)
				{
					scrapeAudioSource.Play();
				}
			}

			if (collisionParticle != null)
			{
				if (impact >= 5)
				{
					collisionParticle.transform.position = col.contacts[0].point;
					collisionParticle.Emit(1);
				}
			}
		}


		void OnCollisionExit(Collision collision)
		{
			if (scrapeAudioSource != null)
			{
				scrapeAudioSource.Stop();
			}
		}


		public void UpdateWheelsOffTrack()
		{
			int count = 0;
			for (int i = 0; i < vehicleWheels.Count; i++)
			{
				if (vehicleWheels[i].wheelComponent.isOffTrack)
					count++;
			}

			RacerStatistics racerStatistics = GetComponent<RacerStatistics>();
			if (racerStatistics != null)
			{
				racerStatistics.SetWheelsOffTrack(count);
			}
		}


		protected void ResetValues()
		{
			throttleInput = 0;
			brakeInput = 0;
			steerInput = 0;
			handbrakeInput = 0;
			currentGear = 2;
			isAutoReversing = false;
		}


		float Direction()
		{
			return transform.InverseTransformDirection(rigid.linearVelocity).z;
		}


		//void SetHorsePower(float hp)
		//{
		//    maxEngineHorsepower = hp;
		//    maxEngineTorque = maxEngineHorsepower * 5252 / maxEngineTorqueRPM;
		//}


		//void SetEngineTorque(float nM)
		//{
		//    maxEngineTorque = nM;
		//    maxEngineHorsepower = maxEngineTorque * maxEngineTorqueRPM / 5252;
		//}


		float GetWheelRPM()
		{
			float rpm = 0;
			foreach (WheelInfo wheel in vehicleWheels)
			{
				if (wheel.drive)
				{
					if (currentGear == 1 || handbrakeInput > 0)
					{
						rpm += wheel.wheelCollider.rpm + (750.0f * throttleInput);
					}
					else
					{
						rpm += wheel.wheelCollider.rpm;
					}
				}
			}

			if (poweredWheels > 0)
			{
				return Mathf.Abs(rpm /= poweredWheels);
			}

			return 0;
		}


		int GetPoweredWheels()
		{
			int count = 0;
			foreach (WheelInfo wheel in vehicleWheels)
			{
				if (wheel.drive)
					count++;
			}

			return count;
		}


        float CalculateShiftDownRPM()
        {
            float ratio = Mathf.InverseLerp(2, gearRatios.Length - 1, currentGear);
            return Mathf.Lerp(minShiftDownRPM, maxShiftDownRPM, ratio);
        }


		float Sqr(float f)
		{
			return f * f;
		}


		public float MaximunTorque()
		{
			return Helper.GetPeakPointInCurve(engineTorqueCurve.keys).value;
		}


		public float MaximunTorqueRPM()
		{
			return Helper.GetPeakPointInCurve(engineTorqueCurve.keys).time;
		}


		public float MaximumHorsepower()
		{
			return Helper.GetPeakPointInCurve(enginePowerCurve.keys).value;
		}


		public float MaximumHorsepowerRPM()
		{
			return Helper.GetPeakPointInCurve(enginePowerCurve.keys).time;
		}


		public float CalculateCurrentHorspower()
		{
			return enginePowerCurve.Evaluate(engineRPM);
		}


		public float CalculateCurrentTorque()
		{
			return engineTorqueCurve.Evaluate(engineRPM);
		}


		public float PowerToWeightRatio()
		{
			return (MaximumHorsepower() / rigid.mass) * 1000;
		}


        public Transform GetWheelTransform(WheelCollider wc)
        {
            for (int i = 0; i < vehicleWheels.Count; i++)
            {
                if(vehicleWheels[i].wheelCollider == wc)
                {
                    return vehicleWheels[i].wheelTransform;
                }
            }

            return null;
        }


		#if UNITY_EDITOR
		void OnGUI()
		{
			if (showTelemetry)
			{
				GUI.skin.label.fontSize = 16;

				GUILayout.BeginVertical("TextArea", GUILayout.Width(300));
				GUILayout.Label("Vehicle: " + gameObject.name);

				GUILayout.Label("-------------------------------------------------------");
				GUILayout.Label("Max Torque (Nm): " + (int)MaximunTorque() + " @ " + MaximunTorqueRPM() + " Rpm");
				GUILayout.Label("Max Horsepower (Hp): " + (int)MaximumHorsepower() + " @ " + MaximumHorsepowerRPM() + " Rpm");
				GUILayout.Label("PtW Ratio: " + (int)PowerToWeightRatio() + " Hp Per Tonne");

				GUILayout.Label("-------------------------------------------------------");
				GUILayout.Label("Throttle: " + throttleInput);
				GUILayout.Label("Brake: " + brakeInput);
				GUILayout.Label("Steer: " + steerInput);
				GUILayout.Label("Handbrake: " + handbrakeInput);

				GUILayout.Label("-------------------------------------------------------");
				GUILayout.Label("Engine On: " + engineOn);
				GUILayout.Label("Transmission: " + ((transmissionType == TransmissionType.Automatic) ? "Automatic" : "Manual"));
				GUILayout.Label("Engine Rpm: " + (int)engineRPM);
				GUILayout.Label("Torque: " + (int)CalculateCurrentTorque());
				GUILayout.Label("Horsepower: " + (int)CalculateCurrentHorspower());
				GUILayout.Label("Speed (kph / mph): " + currentSpeedKPH + " / " + currentSpeedMPH);
				GUILayout.Label("Gear: " + (currentGear - 1));
				GUILayout.Label("Fuel: " + fuel.ToString("F1"));

				GUILayout.Label("-------------------------------------------------------");
				for (int i = 0; i < vehicleWheels.Count; i++)
				{
					GUILayout.Label("Wheel[" + i + "] - Motor Torque: " + (int)vehicleWheels[i].wheelCollider.motorTorque);
					GUILayout.Label("Wheel[" + i + "] - Brake Torque: " + (int)vehicleWheels[i].wheelCollider.brakeTorque);
					GUILayout.Space(5);
				}
				GUILayout.EndVertical();
			}
		}
		#endif

		[System.Serializable]
		public class WheelInfo
		{
            public WheelAxle wheelAxle;
			[HideInInspector]public VehicleWheel wheelComponent;
			public WheelCollider wheelCollider;
			public Transform wheelTransform;
			public bool drive, steer, brake, handbrake;

			public WheelInfo(WheelCollider _wheelCollider, Transform _wheelTransform, bool _drive, bool _steer, bool _brake, bool _handbrake)
			{
				wheelCollider = _wheelCollider;
				wheelTransform = _wheelTransform;
				drive = _drive;
				steer = _steer;
				brake = _brake;
				handbrake = _handbrake;
			}
		}   
    }

    public enum TransmissionType
    {
        Automatic,
        Manual
    }

    public enum WheelAxle
    {
        Front,
        Rear
    }

    public enum SteeringAxis
    {
        Z,
        Y
    }
}