using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using System;

namespace RGSK
{
    public class AudioSettings : MonoBehaviour
    {
        public Slider masterVolumeSlider;
        public Slider sfxVolumeSlider;
        public Slider musicVolumeSlider;
        // Новый слайдер для регулировки звука мотора
        public Slider engineVolumeSlider;

        private AudioMixer mixer;
        // Ссылка на объект со звуком мотора
        //private RealisticEngineSound_mobile engineSound;

        void Start()
        {
            mixer = GlobalSettings.Instance.gameAudioMixer;
            //engineSound = FindObjectOfType<RealisticEngineSound_mobile>();

            UpdateUIToMatchSettings();   // читаем PlayerPrefs → ставим значения на слайдеры
            ApplySavedVolumes();         // ← НОВОЕ: применяем эти же значения к микшеру

            AddListeners();
        }

        /// <summary>
        /// Применяет текущие значения слайдеров к AudioMixer и к звуку мотора.
        /// Вызывается один раз при открытии окна.
        /// </summary>
        void ApplySavedVolumes()
        {
            if (masterVolumeSlider) SetMasterVolume(masterVolumeSlider.value);
            if (sfxVolumeSlider) SetSFXVolume(sfxVolumeSlider.value);
            if (musicVolumeSlider) SetMusicVolume(musicVolumeSlider.value);
            if (engineVolumeSlider) SetEngineVolume(engineVolumeSlider.value);
        }


        void AddListeners()
        {
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.onValueChanged.AddListener(delegate
                {
                    SetMasterVolume(masterVolumeSlider.value);
                });
            }

            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.onValueChanged.AddListener(delegate
                {
                    SetSFXVolume(sfxVolumeSlider.value);
                });
            }

            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.onValueChanged.AddListener(delegate
                {
                    SetMusicVolume(musicVolumeSlider.value);
                });
            }

            // Добавляем слушатель для слайдера звука мотора
            if (engineVolumeSlider != null)
            {
                engineVolumeSlider.onValueChanged.AddListener(delegate
                {
                    SetEngineVolume(engineVolumeSlider.value);
                });
            }
        }

        void UpdateUIToMatchSettings()
        {
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1);
            }

            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            }

            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            }

            // Загружаем сохранённое значение для звука мотора
            if (engineVolumeSlider != null)
            {
                engineVolumeSlider.value = PlayerPrefs.GetFloat("EngineVolume", 1);
            }
        }

        public void SetMasterVolume(float volume)
        {
            float dB = Helper.LinearToDecibel(volume);
            Debug.Log($"Master Volume: {volume} -> {dB} dB");

            if (mixer != null)
            {
                mixer.SetFloat("Master_Volume", dB);
            }
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }

        public void SetSFXVolume(float volume)
        {
            float dB = Helper.LinearToDecibel(volume);
            if (mixer != null)
            {
                mixer.SetFloat("SFX_Volume", dB);
            }
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }

        public void SetMusicVolume(float volume)
        {
            float dB = Helper.LinearToDecibel(volume);
            if (mixer != null)
            {
                mixer.SetFloat("Music_Volume", dB);
            }
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }

        // Новый метод для регулировки звука мотора
        public void SetEngineVolume(float volume)
        {
            Debug.Log($"Engine Volume: {volume}");
          //  if (engineSound != null)
            {
                // Обновляем masterVolume в скрипте звука мотора
               // engineSound.masterVolume = volume;
            }
            PlayerPrefs.SetFloat("EngineVolume", volume);
        }
    }
}
