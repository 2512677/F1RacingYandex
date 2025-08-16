using UnityEngine;
using System.Collections;

namespace RGSK
{
    [RequireComponent(typeof(WheelCollider))]
	public class VehicleWheel : MonoBehaviour 
	{
        private RCC_CarControllerV3 vehicle;
		private WheelCollider wheelCollider;
        private Transform wheelTransform;
		private WheelHit hit;
		private PhysicsMaterial physicMat;
		private PhysicsMaterial cachePhysicMat;
        private Texture2D terrainTex;
        private Texture2D cacheTerrainTex;
        private Terrain terrain;
        private TerrainData terrainData;
        private SplatPrototype[] splatPrototypes;
        private Collider terrainCollider;
        private TrackSurface trackSurface;
		private Skidmarks skidmarks;
		private AudioSource wheelSlipAudiosource;
		private int lastIndex = -1;
		private ParticleSystem surfaceParticle;
		private bool emitParticles;
        private bool emitSkidmarks;
        private WheelFrictionCurve forwardFrictionCurve;
        private WheelFrictionCurve sidewaysFrictionCurve;
        private float minSlip = 0.3f;
        private Vector2 surfaceGripValues = new Vector2(1,1);
        public bool isOffTrack { get; private set; }
        public bool wheelImpact { get; private set; }
        private float lastWheelPosition;
        private float lastBumpNormal;

        public float wheelImpactThreshold = 0.01f;

        [Header("Wheel Spin Effect")]
		public float speedThreshold = 50;
		public GameObject normalModel;
		public GameObject speedModel;


        void Awake()
        {
            wheelCollider = GetComponent<WheelCollider>();

            wheelCollider.mass = GetComponentInParent<RGSKMotorbike>() ? wheelCollider.attachedRigidbody.mass * 0.9f  : 
                                wheelCollider.attachedRigidbody.mass / 15;

            wheelCollider.forceAppPointDistance = 0.2f;
        }


        void Start () 
		{           
            vehicle = GetComponentInParent<RCC_CarControllerV3>();

            if(vehicle != null)
            {
                wheelTransform = vehicle.GetWheelTransform(wheelCollider);
            }

            trackSurface = FindObjectOfType(typeof(TrackSurface)) as TrackSurface;

            wheelSlipAudiosource = Helper.CreateAudioSource(gameObject, null, "SFX", 1, 0, true, false);

            InstaniateParticleSystems();

            GetTerrainInfo();
		}


		void InstaniateParticleSystems()
		{
            if (trackSurface == null)
                return;

			for(int i = 0; i < trackSurface.surfaces.Length; i++)
			{
				if(trackSurface.surfaces[i].particleSystem != null)
				{
					ParticleSystem p = (ParticleSystem)Instantiate (trackSurface.surfaces[i].particleSystem,
                                        transform.position, Quaternion.identity);

                    p.transform.SetParent (transform);
				}
			}
		}


		void FixedUpdate () 
		{

            //Emit based on the wheel slip amount
            emitParticles = wheelCollider.isGrounded && Mathf.Abs(hit.sidewaysSlip) > minSlip 
                        || wheelCollider.isGrounded && Mathf.Abs(hit.forwardSlip) > (minSlip * 2);

            emitSkidmarks = wheelCollider.isGrounded && Mathf.Abs(hit.sidewaysSlip) > (minSlip * 1.5f)
                        || wheelCollider.isGrounded && Mathf.Abs(hit.forwardSlip) > (minSlip * 2.5f);

            if (emitParticles)
            {
                //Emit the particle system
                if (surfaceParticle != null)
                {
                    surfaceParticle.Emit(1);
                }
            }

            if (emitSkidmarks)
            {
                //Draw skidmarks
                if (skidmarks != null)
                {
                    Vector3 point = hit.point + (wheelCollider.attachedRigidbody.linearVelocity * Time.deltaTime);
                    float intensity = Mathf.Abs((hit.sidewaysSlip / 2 + hit.forwardSlip / 2));
                    lastIndex = skidmarks.AddSkidMark(point, hit.normal, intensity, lastIndex);
                }
            }
            else
            {
                //reset skidmark index
                lastIndex = -1;
            }

            //handle audio volume 
            wheelSlipAudiosource.volume = emitParticles ? Mathf.Abs(hit.sidewaysSlip) + Mathf.Abs(hit.forwardSlip) + 0.5f
                                            : Mathf.Lerp(wheelSlipAudiosource.volume, 0f, Time.deltaTime * 10f);

            //handle audio pitch
            wheelSlipAudiosource.pitch = 1 + Mathf.Abs(hit.sidewaysSlip/2);
            wheelSlipAudiosource.pitch = Mathf.Clamp(wheelSlipAudiosource.pitch, 1, 1.2f);

            CheckWheelSurface();
            WheelSpinEffect();

            //Check for wheel impact
            if (wheelTransform != null)
            {
                wheelImpact = wheelCollider.isGrounded && Mathf.Abs(lastWheelPosition - wheelTransform.localPosition.y) >= wheelImpactThreshold;
                lastWheelPosition = wheelTransform.localPosition.y;
            }
        }


        void CheckWheelSurface()
        {
            //Check which surface this wheel is currently on
            if (wheelCollider.isGrounded)
            {
                wheelCollider.GetGroundHit(out hit);

                if (hit.collider.sharedMaterial != null)
                {
                    physicMat = hit.collider.sharedMaterial;
                    terrainTex = null;
                }

                if (hit.collider == terrainCollider)
                {
                    terrainTex = splatPrototypes[GetTerrainTexture(hit.point)].texture;
                    physicMat = null;
                }
            }

            //Store the caches to avoid calling UpdateWheelSurface() every fixed update frame
            if (cachePhysicMat != physicMat)
            {
                cachePhysicMat = physicMat;
                UpdateWheelProperties();
            }

            if (cacheTerrainTex != terrainTex)
            {
                cacheTerrainTex = terrainTex;
                UpdateWheelProperties();
            }
        }


		void UpdateWheelProperties()
		{
			if (trackSurface == null)
				return;

            //Clear the values incase the new surface is null
            surfaceParticle = null;
            skidmarks = null;
            wheelSlipAudiosource.clip = null;

            //Get the surface using the wheel physic material / terrain texture cache
            Surface surface = trackSurface.GetSurfaceData(cachePhysicMat, cacheTerrainTex);
            if(surface != null)
            {
                //Wheel collider values
                surfaceGripValues = new Vector2(surface.sidewaysGrip, surface.forwardGrip);
                OverrideForwardFriction(surfaceGripValues.y);
                OverrideSidewaysFriction(surfaceGripValues.x);
                wheelCollider.wheelDampingRate = surface.damping;

                minSlip = surface.minWheelSlip;
                isOffTrack = surface.offTrack;

                if(vehicle != null)
                {
                    vehicle.UpdateWheelsOffTrack();
                }

                //Particle system
                if (surface.particleSystem != null)
                {
                    Transform childParticleSystem = transform.Find(surface.particleSystem.name + "(Clone)");
                    if (childParticleSystem != null)
                    {
                        ParticleSystem p = childParticleSystem.GetComponent<ParticleSystem>();
                        surfaceParticle = p;
                    }
                }

                //Skidmarks
                if (surface.surfaceSkidmarks != null)
                {
                    lastIndex = -1;
                    skidmarks = surface.surfaceSkidmarks;
                }

                //Sound
                if (surface.soundClip != null)
                {
                    wheelSlipAudiosource.clip = surface.soundClip;
                    wheelSlipAudiosource.Play();
                }
            }
		}


        void WheelSpinEffect()
        {
            if (speedModel == null || normalModel == null)
                return;

            if (WheelSpeed() > speedThreshold)
            {
                //Activate the speed model
                if (normalModel.activeSelf == true)
                    normalModel.SetActive(false);

                if (speedModel.activeSelf == false)
                    speedModel.SetActive(true);            
            }
            else
            {
                //Activate the normal model
                if (normalModel.activeSelf == false)
                    normalModel.SetActive(true);

                if (speedModel.activeSelf == true)
                    speedModel.SetActive(false);
            }
        }


        public void UpdateForwardFrictionCurve(WheelFrictionCurve curve)
        {
            wheelCollider.forwardFriction = curve;
        }


        public void UpdateSidewaysFrictionCurve(WheelFrictionCurve curve)
        {
            wheelCollider.sidewaysFriction = curve;
        }


        public void OverrideForwardExtremumAndAsymptope(float extr, float asym)
        {
            forwardFrictionCurve = wheelCollider.forwardFriction;
            forwardFrictionCurve.extremumValue = extr;
            forwardFrictionCurve.asymptoteValue = asym;
            wheelCollider.forwardFriction = forwardFrictionCurve;
        }


        public void OverrideSidewaysExtremumSlip(float slip)
        {
            sidewaysFrictionCurve = wheelCollider.sidewaysFriction;
			sidewaysFrictionCurve.extremumSlip = slip;
            wheelCollider.sidewaysFriction = sidewaysFrictionCurve;
        }


        public void OverrideForwardFriction(float value)
        {
            forwardFrictionCurve = wheelCollider.forwardFriction;
            forwardFrictionCurve.stiffness = value;
            wheelCollider.forwardFriction = forwardFrictionCurve;
        }


        public void OverrideSidewaysFriction(float value)
        {
            sidewaysFrictionCurve = wheelCollider.sidewaysFriction;
            sidewaysFrictionCurve.stiffness = value;
            wheelCollider.sidewaysFriction = sidewaysFrictionCurve;
        }


        public void RevertFrictionValuesToMatchSurface()
        {
            if (forwardFrictionCurve.stiffness == surfaceGripValues.y && sidewaysFrictionCurve.stiffness == surfaceGripValues.x)
                return;

            OverrideForwardFriction(surfaceGripValues.y);
            OverrideSidewaysFriction(surfaceGripValues.x);
        }


        public float WheelSpeed()
        {
            return Mathf.Abs(((wheelCollider.rpm * wheelCollider.radius) / 2.9f));
        }


        public Vector2 GetWheelSlip()
        {
            return new Vector2(hit.sidewaysSlip, hit.forwardSlip);
        }


        void GetTerrainInfo()
        {
            if (Terrain.activeTerrain)
            {
                terrain = Terrain.activeTerrain;
                terrainData = terrain.terrainData;
                splatPrototypes = terrain.terrainData.splatPrototypes;
                terrainCollider = terrain.GetComponent<Collider>();
            }
        }


        private float[] GetTextureMix(Vector3 worldPos)
        {

            terrain = Terrain.activeTerrain;
            terrainData = terrain.terrainData;
            Vector3 terrainPos = terrain.transform.position;

            int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
            int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

            float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

            float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];
            for (int n = 0; n < cellMix.Length; ++n)
            {
                cellMix[n] = splatmapData[0, 0, n];
            }

            return cellMix;
        }


        private int GetTerrainTexture(Vector3 worldPos)
        {

            float[] mix = GetTextureMix(worldPos);
            float maxMix = 0;
            int maxIndex = 0;

            for (int n = 0; n < mix.Length; ++n)
            {

                if (mix[n] > maxMix)
                {
                    maxIndex = n;
                    maxMix = mix[n];
                }
            }

            return maxIndex;
        } 
    }
}
