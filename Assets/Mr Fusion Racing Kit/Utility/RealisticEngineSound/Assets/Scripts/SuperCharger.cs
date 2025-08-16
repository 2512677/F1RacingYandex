//______________________________________________//
//___________Realistic Engine Sounds____________//
//______________________________________________//
//_______Copyright © 2025 Skril Studio__________//
//______________________________________________//
//__________ http://skrilstudio.com/ ___________//
//______________________________________________//
//________ http://fb.com/yugelmobile/ __________//
//______________________________________________//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SkrilStudio
{
    public class SuperCharger : MonoBehaviour
    {
        RealisticEngineSound res;
        // master volume setting
        [Range(0.1f, 1.0f)]
        public float masterVolume = 1f;
        [Range(0.1f, 2.0f)]
        public float pitchMultiplier = 1f;
        // audio mixer
        public AudioMixerGroup audioMixer;
        private AudioMixerGroup _audioMixer;
        // audio clips
        public AudioClip chargerOnLoopClip;
        public AudioClip chargerOffLoopClip;
        public AnimationCurve chargerOnVolCurve;
        public AnimationCurve chargerOffVolCurve;
        public AnimationCurve chargerPitchCurve;
        // curve settings
        private AudioSource chargerOnLoop;
        private AudioSource chargerOffLoop;
        private float clipsValue;
        private float[] dynamicFloats = new float[8];
        // distance values
        private float[] audioFloats = new float[4];
        public float minDistance = 1; // when this value is changed during runtime, prefab needs to be disabled and re-enabled to make the changes take effect
        public float maxDistance = 50; // when this value is changed during runtime, prefab needs to be disabled and re-enabled to make the changes take effect
        void Start()
        {
            res = gameObject.transform.parent.GetComponent<RealisticEngineSound>();
            if (audioMixer != null) // user is using a seperate audio mixer for this prefab
            {
                _audioMixer = audioMixer;
            }
            else
            {
                if (res.audioMixer != null) // use engine sound's audio mixer for this prefab
                {
                    _audioMixer = res.audioMixer;
                    audioMixer = _audioMixer;
                }
            }
        }
        void Update()
        {
            clipsValue = DynamicSoundController.BasicClipsValue(res.engineCurrentRPM, res.maxRPMLimit); // calculate % percentage of rpm
            DynamicSoundController.AudioValueControll(audioFloats, minDistance, maxDistance, res.dopplerLevel, res.spatialBlend);
            DynamicSoundController.DynamicSoundValues(dynamicFloats, clipsValue, masterVolume, res.engineLoad, pitchMultiplier, res.optimisationLevel, 1, 1, 1);
            if (res.offLoadType == RealisticEngineSound.OffLoadType.Prerecorded)
            {
                chargerOnLoop = DynamicSoundController.DynamicSound(gameObject, chargerOnLoop, chargerOnLoop, chargerOnLoopClip, res.audioRolloffMode, res.audioVelocityUpdateMode, audioFloats, audioMixer, chargerOnVolCurve, chargerPitchCurve, res.onLoadVolumeByRPM, dynamicFloats, res.optimiseAudioSources, res.isAudible, true, false);
                chargerOffLoop = DynamicSoundController.DynamicSound(gameObject, chargerOffLoop, chargerOffLoop, chargerOffLoopClip, res.audioRolloffMode, res.audioVelocityUpdateMode, audioFloats, audioMixer, chargerOffVolCurve, chargerPitchCurve, res.offLoadVolumeByRPM, dynamicFloats, res.optimiseAudioSources, res.isAudible, false, false);
            }
            else
            {
                chargerOnLoop = DynamicSoundController.DynamicSoundUnloaded(gameObject, chargerOnLoop, chargerOnLoop, chargerOnLoopClip, res.audioRolloffMode, res.audioVelocityUpdateMode, audioFloats, audioMixer, chargerOnVolCurve, chargerPitchCurve, res.offLoadVolumeByRPM, res.onLoadVolumeByRPM, dynamicFloats, res.optimiseAudioSources, res.isAudible, false, true);
            }
        }
#if UNITY_EDITOR
        private void LateUpdate()
        {
            // velocity update mode got changed on runtime, remake all audio sources
            if (chargerOnLoop != null && chargerOnLoop.velocityUpdateMode != res.audioVelocityUpdateMode)
                DestroyAll();
        }
#endif
        private void OnEnable() // if prefab got new audiomixer on runtime, it will use that after prefab got re-enabled
        {
            Start();
        }
        private void OnDisable() // destroy audio sources if disabled
        {
            DestroyAll();
        }
        private void DestroyAll()
        {
            if (chargerOnLoop != null)
                Destroy(chargerOnLoop);
            if (chargerOffLoop != null)
                Destroy(chargerOffLoop);
        }
    }
}
