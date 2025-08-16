using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace RGSK
{
    public class VideoSettings : MonoBehaviour
    {
        public Dropdown qualityLevelDropdown;
        public Dropdown resolutionDropdown;
        public Toggle fullscreenToggle;
        public bool applyExpensiveGraphicChanges = true;

        private Resolution[] resolutions;


        void Start()
        {
            GetScreenResolutions();
            AddListeners();
            UpdateUIToMatchSettings();  
        }


        void AddListeners()
        {
            if (qualityLevelDropdown != null)
            {
                qualityLevelDropdown.onValueChanged.AddListener(delegate
                {
                    SetGraphicsQulality(qualityLevelDropdown.value);
                });
            }

            if (resolutionDropdown != null)
            {
                resolutionDropdown.onValueChanged.AddListener(delegate
                {
                    SetScreenResolution(resolutions[resolutionDropdown.value]);
                });
            }

            if (fullscreenToggle != null)
            {
                fullscreenToggle.onValueChanged.AddListener(delegate
                {
                    SetFullscreen(fullscreenToggle.isOn);
                });
            }
        }


        void UpdateUIToMatchSettings()
        {
            if(qualityLevelDropdown != null)
            {
                qualityLevelDropdown.value = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
            }

            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = Screen.fullScreen;
            }
        }


        void GetScreenResolutions()
        {
            if (resolutionDropdown == null) 
				return;

            resolutions = Screen.resolutions;

            resolutionDropdown.ClearOptions();

            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            int resolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                options.Add(new Dropdown.OptionData(resolutions[i].width + " x " + resolutions[i].height));

                if(resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    resolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = resolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }


        public void SetGraphicsQulality(int level)
        {
            QualitySettings.SetQualityLevel(level, applyExpensiveGraphicChanges);

            PlayerPrefs.SetInt("QualityLevel", level);
        }


        public void SetScreenResolution(Resolution res)
        {
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);

            PlayerPrefs.SetInt("ResoultionX", res.width);
            PlayerPrefs.SetInt("ResoultionY", res.height);
        }


        public void SetFullscreen(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;

            PlayerPrefs.SetInt("Fullscreen", fullscreen ? 0 : 1);
        }
    }
}