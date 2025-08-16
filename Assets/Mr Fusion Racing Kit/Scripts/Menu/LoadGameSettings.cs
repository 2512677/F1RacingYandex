using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace RGSK
{
    public class LoadGameSettings : MonoBehaviour
    {
        void Awake()
        {
            LoadAudioSettings();
            LoadVideoSettings();
        }


        public void LoadAudioSettings()
        {
            //Load audio volumes
            float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1);
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);

            AudioMixer mixer = GlobalSettings.Instance.gameAudioMixer;
            if (mixer != null)
            {
                //Set mixer group volumes
                mixer.SetFloat("Master_Volume", Helper.LinearToDecibel(masterVolume));
                mixer.SetFloat("SFX_Volume", Helper.LinearToDecibel(sfxVolume));
                mixer.SetFloat("Music_Volume", Helper.LinearToDecibel(musicVolume));
            }

            //Save the default values
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        }


        public void LoadVideoSettings()
        {
            //Load quality level
            int qualityLevel = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
            QualitySettings.SetQualityLevel(qualityLevel, true);

            //Load resolution / fullscreen
            int resX = PlayerPrefs.GetInt("ResolutionX", Screen.currentResolution.width);
            int resY = PlayerPrefs.GetInt("ResolutionY", Screen.currentResolution.height);
            int fullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 0 : 1);
            Screen.SetResolution(resX, resY, fullscreen == 0);

            //Save the default values
            PlayerPrefs.SetInt("QualityLevel", qualityLevel);
            PlayerPrefs.SetInt("ResolutionX", resX);
            PlayerPrefs.SetInt("ResolutionY", resY);
            PlayerPrefs.SetInt("Fullscreen", fullscreen);
        }
    }
}