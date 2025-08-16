using UnityEngine;
using System.Collections;

namespace RGSK
{
    [RequireComponent(typeof(Rigidbody))]
    public class Nitro : MonoBehaviour
    {
        private Rigidbody rigid;
        [HideInInspector]
        public float throttle;
        [HideInInspector]
        public bool nitroEngaged;
        private bool runningRoutine;

        [Header("Settings")]
        [Range(0, 10)]
        public float force = 2;
        [Range(0, 1)]
        public float capacity = 1;
        [Range(0, 1)]
        public float regenerationRate = 0.1f;
        [Range(0, 1)]
        public float depletionRate = 0.25f;

        [Header("Audio")]
        public AudioClip nitroClip;
        private AudioSource nitroAudio;
        [Range(0.1f, 1)]
        public float maxVolume = 1.0f;
        [Range(0.1f, 2)]
        public float minPitch = 0.5f;
        [Range(0.1f, 2)]
        public float maxPitch = 1.5f;

        [Header("Particles")]
        public ParticleSystem[] nitroEffects;
        private bool particleEmit;

        void Start()
        {
            //Set the capacity to 1
            capacity = 1;

            //Get the rigidbody component
            rigid = GetComponent<Rigidbody>();

            //Make sure that no nitro particles are active on start
            if (nitroEffects.Length > 0)
            {
                EndNitro();
            }

            //Create the nitro audio source
            if (nitroClip != null)
            {
                nitroAudio = Helper.CreateAudioSource(gameObject, nitroClip, "SFX",1, 0, true, false);
                nitroAudio.Play();
            }
        }



        void FixedUpdate()
        {
            if (capacity > 0 && throttle > 0 && nitroEngaged)
            {
                //deplete the nitro
                capacity = Mathf.MoveTowards(capacity, 0, depletionRate * Time.deltaTime);

                //Lerp audiosource values
                if (nitroAudio)
                {
                    nitroAudio.volume = Mathf.Lerp(nitroAudio.volume, maxVolume, Time.deltaTime * 2f);
                    nitroAudio.pitch = Mathf.Lerp(nitroAudio.pitch, maxPitch, Time.deltaTime * 1f);
                }

                //Emit the particle effects
                if (!particleEmit)
                    StartNitro();

                //Add a force to the rigidbody
                rigid.AddForce(transform.forward * force, ForceMode.Acceleration);
            }
            else
            {
                //Regenerate the nitro
                if (!nitroEngaged)
                    capacity = Mathf.MoveTowards(capacity, 1, regenerationRate * Time.deltaTime);

                //Lerp audiosource values back to normal
                if (nitroAudio)
                {
                    nitroAudio.volume = Mathf.Lerp(nitroAudio.volume, 0, Time.deltaTime * 3.5f);
                    nitroAudio.pitch = Mathf.Lerp(nitroAudio.pitch, minPitch, Time.deltaTime * 1f);
                }

                //Disable the particle effects
                if (particleEmit)
                {
                    EndNitro();
                }
            }
        }


        void StartNitro()
        {
            particleEmit = true;

            if (nitroEffects.Length > 0)
            {
                foreach (ParticleSystem p in nitroEffects)
                {
                    p.Play();
                }
            }
        }


        void EndNitro()
        {
            particleEmit = false;

            if (nitroEffects.Length > 0)
            {
                foreach (ParticleSystem p in nitroEffects)
                {
                    p.Stop();
                }
            }
        }
    }
}
