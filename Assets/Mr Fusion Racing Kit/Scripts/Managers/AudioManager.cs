using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace RGSK
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;
        public AudioMixer gameAudioMixer { get { return GlobalSettings.Instance.gameAudioMixer; } }
        private AudioSource soundAudio;
        private Queue<AudioClip> audioClipQueue;
        private bool playingFromQueue;
        public bool dontDestroyOnLoad;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            if (dontDestroyOnLoad)
            {
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }

            audioClipQueue = new Queue<AudioClip>();
        }


        void Start()
        {
            soundAudio = Helper.CreateAudioSource(gameObject, null, "",0, 1, false, false);
        }


        public void PlayClip(AudioClip clip)
        {
            if (soundAudio == null)
                return;

            soundAudio.PlayOneShot(clip);
        }


        public void PlayClipQueue(AudioClip clip)
        {
            if (soundAudio == null)
                return;

            audioClipQueue.Enqueue(clip);

            if (!soundAudio.isPlaying)
                PlayNextClipInQueue();
        }


        void PlayNextClipInQueue()
        {
            if(audioClipQueue.Count > 0)
            {
                AudioClip clip = audioClipQueue.Dequeue();
                soundAudio.clip = clip;
                soundAudio.Play();
                Invoke("PlayNextClipInQueue", clip.length);
            }
            else
            {
                CancelInvoke("PlayNextClipInQueue");
            }
        }


        public void PlayClipWithDelay(AudioClip clip, float delay)
        {
            if (soundAudio == null)
                return;

            StartCoroutine(DoPlayClipWithDelay(clip,delay));
        }


        IEnumerator DoPlayClipWithDelay(AudioClip clip, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlayClip(clip);
        }


        public void PlayRandomClip(AudioClip[] clips)
        {
            if (soundAudio == null)
                return;

            int random = Random.Range(0, clips.Length);
            soundAudio.PlayOneShot(clips[random]);
        }


        public void PlayClipAtPoint(AudioClip clip, Vector3 position)
        {
            AudioSource.PlayClipAtPoint(clip, position);
        }


        public void PlayRandomClipAtPoint(AudioClip[] clip, Vector3 position)
        {
            int random = Random.Range(0, clip.Length);
            AudioSource.PlayClipAtPoint(clip[random], position);
        }


        public void ToggleAudioGroupMute(string group, bool mute)
        {
            if (gameAudioMixer == null)
                return;

			if (mute) 
			{
				//Set the audio group volume to 0
				gameAudioMixer.SetFloat (group, Helper.LinearToDecibel (0));
			} 
			else 
			{
				//Set the auido group volume to game data volume. A value of 1 is used if no game data found
				gameAudioMixer.SetFloat (group, Helper.LinearToDecibel (GetAudioGroupVolumeFromGameData(group)));
			}
        }


		float GetAudioGroupVolumeFromGameData(string group)
		{
			float vol = 1;

			if (PlayerData.instance != null) 
			{
				switch (group) 
				{
				case "Master_Volume":
					vol = PlayerPrefs.GetFloat("MasterVolume", 1);
					break;

				case "SFX_Volume":
					vol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
                        break;


				case "Music_Volume":
					vol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
                        break;
				}
			}

			return vol;
		}
    }
}