using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace RGSK
{
    public class ReplayPanel : MonoBehaviour
    {
        private ReplayManager replayManager;

        public GameObject replayControlPanel;	
        public Slider replaySlider;
        public Text currentTime;
        public Text totalTime;
        public Button pauseButton;
        public Button fastForwardButton;
        public Button rewindButton;
        public Button cameraButton;
        public Button previousVehicle, nextVehicle;
        public Button exitReplayButton;
        public Button hideControlPanelButton;
        public Button showControlPanelButton;
        public bool autoHideControlPanel;
        public float autoHideTimeout = 5;
        private float autoHideTimer;
        private bool isPointerOverControlPanel;
        private bool isSliderDown;

        void Start()
        {
            //Захватываем ссылку на менеджер воспроизведения
            replayManager = ReplayManager.instance;

            //Добавить прослушиватели кнопок для назначенных кнопок
            AddButtonListeners();
            autoHideTimer = autoHideTimeout;
        }


        void Update()
        {
            if (replayManager == null)
                return;

            if (replaySlider != null && !isSliderDown)
            {
                //Обновить значение ползунка
                replaySlider.value = replayManager.GetCurrentFrame();
            }

            if (currentTime != null)
            {
                //Обновить текущее время
                currentTime.text = Helper.FormatTime(replayManager.currentFrame * Time.fixedDeltaTime, TimeFormat.MinSec);
            }

            //Обновить автоматическое скрытие
            if (replayControlPanel != null && autoHideControlPanel && !isPointerOverControlPanel)
            {
                if (autoHideTimer > 0)
                {
                    autoHideTimer -= Time.deltaTime;
                    if (autoHideTimer <= 0)
                    {
                        HideControlPanel();
                    }
                }
            }
        }
			

        public void SliderPressed()
        {
            //Установите это логическое значение в true, когда ползунок удерживается, чтобы остановить обновление его значения
            isSliderDown = true;
        }


		public void SliderReleased()
		{
            //Установить кадр воспроизведения на соответствующий кадр на ползунке при его отпускании
            if (replayManager != null)
			{
				replayManager.SetTimeFromSlider(replaySlider.value);
			}

			isSliderDown = false;
		}


        public void UpdateValuesFromReplayManager()
        {
            //Если null, берем ссылку на ReplayManager
            if (replayManager == null)
            {
                replayManager = ReplayManager.instance;
            }

            //Установите максимальное значение ползунка в OnEnable, если панель была активна при запуске
            SetSliderMaxValue();

            //Установить текст общего времени, если назначено
            if (totalTime != null)
            {
                totalTime.text = Helper.FormatTime(replayManager.totalFrames * Time.fixedDeltaTime, TimeFormat.MinSec);
            }
        }


        void SetSliderMaxValue()
        {
            if (replaySlider != null && replayManager != null)
            {
                //Установите максимальное значение ползунка на общее количество кадров воспроизведения
                replaySlider.maxValue = replayManager.totalFrames;
            }
        }


        public void ShowControlPanel()
        {
            if (replayControlPanel == null)
                return;

            //возврат, если он уже активен
            if (replayControlPanel.activeSelf)
                return;

            //Показать панель управления
            replayControlPanel.SetActive(true);

            //Скрыть кнопки показа
            if (showControlPanelButton != null)
                showControlPanelButton.gameObject.SetActive(false);         
        }


        public void HideControlPanel()
        {
            if (replayControlPanel == null)
                return;

            //возврат, если он уже деактивирован
            if (!replayControlPanel.activeSelf)
                return;

            //Hide the control panel
            replayControlPanel.SetActive(false);

            //Показать кнопку скрытия
            if (hideControlPanelButton != null)
                hideControlPanelButton.gameObject.SetActive(true);
        }


        public void PointerOverControlPanel(bool isOver)
        {
            isPointerOverControlPanel = isOver;

            if (isPointerOverControlPanel)
            {
                //Показать панель управления
                ShowControlPanel();

                //Сбросить таймер автоматического скрытия
                autoHideTimer = autoHideTimeout;
            }
        }


        void AddButtonListeners()
        {
            if (pauseButton != null)
            {
                if(replayManager != null)
                    pauseButton.onClick.AddListener(delegate { replayManager.PauseReplay(); });
            }

            if (fastForwardButton != null)
            {
                if (replayManager != null)
                    fastForwardButton.onClick.AddListener(delegate { replayManager.AdjustPlaybackSpeed(2); });
            }

            if (rewindButton != null)
            {
                if (replayManager != null)
                    rewindButton.onClick.AddListener(delegate { replayManager.AdjustPlaybackSpeed(-2); });
            }

            if (exitReplayButton != null)
            {
                if (replayManager != null)
                    exitReplayButton.onClick.AddListener(delegate { replayManager.ExitReplay(); });
            }

            CameraManager camManager = FindObjectOfType<CameraManager>();
            if (cameraButton != null)
            {
                if (camManager != null)
                    cameraButton.onClick.AddListener(delegate { camManager.SwitchReplayCameras(); });
            }

            if (previousVehicle != null)
            {
                if (camManager != null)
                    previousVehicle.onClick.AddListener(delegate { camManager.SwitchTargetVehicle(-1); });
            }

            if (nextVehicle != null)
            {
                if (camManager != null)
                   nextVehicle.onClick.AddListener(delegate { camManager.SwitchTargetVehicle(1); });
            }

            if(showControlPanelButton != null)
            {
                showControlPanelButton.onClick.AddListener(delegate { ShowControlPanel(); });
            }

            if (hideControlPanelButton != null)
            {
                hideControlPanelButton.onClick.AddListener(delegate { HideControlPanel(); });
            }
        }
    }
}
