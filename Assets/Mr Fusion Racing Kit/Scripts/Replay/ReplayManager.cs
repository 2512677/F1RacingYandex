using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RGSK
{
    public class ReplayManager : Recorder
    {
        public static ReplayManager instance; // Статический экземпляр менеджера воспроизведения
        private RaceManager raceManager; // Ссылка на менеджер гонок
        private TrackSurface trackSurface; // Ссылка на поверхность трассы 

        public bool enableReplay = true; // Флаг включения воспроизведения

        [Header("Дополнительно")]
        public bool onlyRecordFinalLap; // Флаг записи только последнего круга
        public float maximumReplayTime = 0; // Максимальное время воспроизведения

        // Метод, вызываемый при активации объекта
        void Awake()
        {
            instance = this;
        }

        // Метод, вызываемый при запуске сцены
        void Start()
        {
            raceManager = RaceManager.instance;
            trackSurface = FindObjectOfType<TrackSurface>();
        }

        // Инициализация воспроизведения
        public void InitializeReplay()
        {
            if (!enableReplay)
                return;

            FindRaceVehicles(); // Поиск транспортных средств гонок
            FindMiscellaneousObjects(); // Поиск прочих записываемых объектов

            if (!onlyRecordFinalLap)
                BeginRecording(); // Начало записи, если не указано иное
        }

        // Начало записи данных
        public void BeginRecording()
        {
            record = true;
        }

        // Остановка записи данных
        public void StopRecording()
        {
            record = false;
            playback = false;
            totalFrames = GetTotalFrames(); // Установка общего количества кадров
        }

        // Поиск транспортных средств гонок и добавление их в список записываемых объектов
        void FindRaceVehicles()
        {
            if (raceManager == null)
            {
                Debug.Log(this + " не смог найти транспортные средства гонок. Пожалуйста, убедитесь, что в вашей сцене есть RaceManager");
                return;
            }

            for (int i = 0; i < raceManager.racerList.Count; i++)
            {
                RecordableObject vehicle = new RecordableObject();
                Transform transform = raceManager.racerList[i].transform;

                vehicle.transform = transform;
                vehicle.rigidbody = transform.GetComponent<Rigidbody>();
                vehicle.rgskVehicle = transform.GetComponent<RCC_CarControllerV3>();
                vehicle.rccVehicle = transform.GetComponent<global::RCC_CarControllerV4>();
                vehicle.nitroController = transform.GetComponent<Nitro>();
                vehicle.bikeRider = transform.GetComponentInChildren<BikeRider>();

                recordableObjects.Add(vehicle);
            }
        }

        // Поиск прочих объектов, помеченных тегом "RecordableObject", и добавление их в список записываемых объектов
        void FindMiscellaneousObjects()
        {
            // Поиск прочих записываемых объектов по тегу и добавление их в список
            GameObject[] miscellaneousGOs = GameObject.FindGameObjectsWithTag("RecordableObject");

            for (int i = 0; i < miscellaneousGOs.Length; i++)
            {
                RecordableObject misc = new RecordableObject();
                Transform transform = miscellaneousGOs[i].transform;

                misc.transform = transform;
                misc.rigidbody = transform.GetComponent<Rigidbody>();

                recordableObjects.Add(misc);
            }
        }

        // Метод, вызываемый каждый физический кадр
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // Если максимальное время воспроизведения больше 0, ограничить время воспроизведения заданным значением
            if (maximumReplayTime > 0)
            {
                // Удаление самых старых данных кадра из каждого записываемого объекта, чтобы сохранить воспроизведение в пределах ограниченного времени
                for (int i = 0; i < recordableObjects.Count; i++)
                {
                    if (recordableObjects[i].replayFrameData.Count > (int)(maximumReplayTime / Time.fixedDeltaTime))
                    {
                        recordableObjects[i].replayFrameData.RemoveAt(0);
                    }
                }
            }
        }

        // Метод воспроизведения записи
        public override void Playback()
        {
            base.Playback();

            // Принудительно установить скорость воспроизведения на 1, если мы перемотали менее чем на 2 кадра
            if (currentFrame <= 2)
            {
                AdjustPlaybackSpeed(1);
            }

            // Перебор всех записываемых объектов
            for (int i = 0; i < recordableObjects.Count; i++)
            {
                if (currentFrame < totalFrames)
                {
                    // Вызов метода воспроизведения кадра из базового класса с параметрами записываемого объекта
                    PlaybackReplayFrame(recordableObjects[i], recordableObjects[i].replayFrameData[currentFrame]);
                }
                else
                {
                    // Перезапуск воспроизведения, когда все кадры были воспроизведены
                    RestartReplay();
                }
            }
        }

        #region REPLAY_CONTROLS

        // Метод для просмотра воспроизведения
        public void WatchReplay()
        {
            if (recordableObjects.Count == 0 || GetTotalFrames() < 1)
                return;

            // Переключение в состояние воспроизведения
            raceManager.SwitchRaceState(RaceState.Replay);

            // Установка общего количества кадров на общее количество записанных кадров
            totalFrames = GetTotalFrames();

            // Убедиться, что скорость воспроизведения установлена на 1
            AdjustPlaybackSpeed(1);

            // В случае просмотра из паузы, убедиться, что аудио установлено на 1
            AudioListener.volume = 1;

            // Затемнение экрана
            if (currentFrame < 2)
            {
                if (FindObjectOfType<ScreenFader>())
                    FindObjectOfType<ScreenFader>().DoFadeOut(0.5f);
            }

            // Обновление интерфейса пользователя
            ReplayPanel panel = FindObjectOfType<ReplayPanel>();
            if (panel != null)
            {
                panel.UpdateValuesFromReplayManager();
            }

            // Деактивация следов шин
            if (trackSurface != null)
            {
                trackSurface.ToggleSkidmarkVisibility(false);
            }

            // Отключение компонентов гонки и начало воспроизведения
            raceManager.DisableRaceComponents(false);
            record = false;
            playback = true;
        }

        // Метод приостановки воспроизведения
        public void PauseReplay()
        {
            playbackSpeed = (playbackSpeed > 0 || playbackSpeed < 0) ? 0 : 1; // Переключение скорости воспроизведения между 0 и 1
            Time.timeScale = playbackSpeed; // Установка временной шкалы

            // Регулировка громкости звуковых эффектов транспортных средств
            if (AudioManager.instance != null)
            {
                AudioManager.instance.ToggleAudioGroupMute("SFX_Volume", playbackSpeed != 1);
            }
        }

        // Метод для регулировки скорости воспроизведения
        public void AdjustPlaybackSpeed(int speed)
        {
            if (playbackSpeed == speed)
            {
                playbackSpeed = 1;
            }
            else
            {
                playbackSpeed = speed;
            }

            // Регулировка временной шкалы
            if (Time.timeScale < 1)
            {
                Time.timeScale = 1.0f;
            }

            // Регулировка громкости звуковых эффектов транспортных средств
            if (AudioManager.instance != null)
            {
                AudioManager.instance.ToggleAudioGroupMute("SFX_Volume", playbackSpeed != 1);
            }
        }

        // Метод для установки времени воспроизведения из слайдера
        public void SetTimeFromSlider(float t)
        {
            currentFrame = (int)t;
        }

        // Метод выхода из воспроизведения
        public void ExitReplay()
        {
            AdjustPlaybackSpeed(1); // Установка скорости воспроизведения на 1

            raceManager.SwitchRaceState(raceManager.previousRaceState); // Переключение обратно в предыдущее состояние гонки

            if (raceManager.raceState == RaceState.Pause)
            {
                // Возврат к последнему записанному кадру
                for (int i = 0; i < recordableObjects.Count; i++)
                {
                    if (recordableObjects[i].rigidbody != null)
                    {
                        recordableObjects[i].rigidbody.isKinematic = false;
                    }

                    PlaybackReplayFrame(recordableObjects[i], recordableObjects[i].replayFrameData[totalFrames - 1]);
                }

                // При возвращении в меню паузы установить временную шкалу и громкость на 0
                Time.timeScale = 0.0f;
                AudioListener.volume = 0;

                // Повторное включение компонентов гонки
                raceManager.DisableRaceComponents(true);

                // Повторное включение следов шин на трассе
                if (trackSurface != null)
                {
                    trackSurface.ToggleSkidmarkVisibility(true);
                }

                // Возврат в состояние записи
                record = true;
                playback = false;
                currentFrame = 0;
            }
        }
        #endregion

        // Метод перезапуска воспроизведения
        void RestartReplay()
        {
            // Затемнение экрана
            if (FindObjectOfType<ScreenFader>())
                FindObjectOfType<ScreenFader>().DoFadeOut(0.5f);

            playbackSpeed = 1; // Установка скорости воспроизведения на 1
            currentFrame = 0; // Сброс текущего кадра
        }

        // Метод проверки записи только последнего круга
        void CheckFinalLap()
        {
            if (!onlyRecordFinalLap)
                return;

            if (!record && raceManager.lapCount > 1 && raceManager.playerStatistics.IsOnFinalLap())
            {
                BeginRecording(); // Начало записи, если игрок находится на последнем круге
            }
        }

        // Метод, вызываемый при активации компонента
        void OnEnable()
        {
            RaceManager.OnRaceStart += InitializeReplay; // Подписка на событие начала гонки
            RaceManager.OnPlayerFinish += StopRecording; // Подписка на событие завершения игроком гонки
            RacerStatistics.OnEnterNewLap += CheckFinalLap; // Подписка на событие начала нового круга
        }

        // Метод, вызываемый при отключении компонента
        void OnDisable()
        {
            RaceManager.OnRaceStart -= InitializeReplay; // Отписка от события начала гонки
            RaceManager.OnPlayerFinish -= StopRecording; // Отписка от события завершения игроком гонки
            RacerStatistics.OnEnterNewLap -= CheckFinalLap; // Отписка от события начала нового круга
        }
    }
}
