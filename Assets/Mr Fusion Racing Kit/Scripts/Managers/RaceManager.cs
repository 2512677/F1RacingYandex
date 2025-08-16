using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using I2.Loc;
using System;
using Gley.TrafficSystem; // для API и Events

namespace RGSK
{
    public class RaceManager : MonoBehaviour
    {
        public static RaceManager instance;

      

        // Перечисления
        public RaceState raceState; // Текущее состояние гонки
        public RaceState previousRaceState; // Предыдущее состояние гонки
        public RaceType raceType; // Тип гонки
        public RaceStartMode startMode; // Режим старта гонки
        public GridPositioningMode playerGridPositioningMode; // Режим позиционирования игрока на стартовой решётке
        public AIDifficultyLevel aiDifficultyLevel; // Уровень сложности искусственного интеллекта
        public RaceEndTimerStart raceEndTimerLogic; // Логика таймера окончания гонки
        public SpeedUnit speedUnit; // Единицы измерения скорости
        private PositioningType positioningType; // Тип позиционирования
        private CarClass.VehicleClass playerVehicleClass; // Класс автомобиля игрока


        [Header("Race Messages")]
        public string bestTimeInfo = "New Best Time";
        public string finalLapInfo = "Final Lap";
        public string invalidLapInfo = "Lap Invalidated";
        public string lapKnockoutDisqualifyInfo = "Knocked Out";
        public string checkpointDisqualifyInfo = "Checkpoint Missed";
        public string eliminationDisqualifyInfo = "Eliminated";
        public string defaultDisqualifyInfo = "Disqualified";
        public string raceEndMessage = "Race Finished";


        //Траффики 

        [Header("Traffic Integration")]
        [Tooltip("Перетащите сюда объект с TrafficComponent, он должен быть деактивирован до старта гонки")]
        public TrafficComponent trafficComponent;


        //Погоня 

        // ----- поля в блоке «Погоня»  (БЕЗ ключей!)
        public string capturedInfo = "Пойман";
        public string capturedAllInfo = "Все машины пойманы!";
        public string capturedFailInfo = "Не все машины пойманы";




        // ────────────────────────────────────────────────

        // ─────── в область переменных гонки Chase ───────
        private int capturedBots = 0;
        private int totalBots = 0;




        // Настройки гонки
        public bool loadRaceSettings; // Загрузка настроек гонки
        public int lapCount = 3; // Количество кругов
        public int opponentCount = 3; // Количество соперников
        public int playerStartPosition = 1; // Стартовая позиция игрока
        public float eliminationTimeStart = 30; // Время начала устранения гонщика
        public float checkpointTimeStart = 30; // Время начала чекпоинта
        public float enduranceTimeStart = 300; // Время начала гонки на выносливость
        public float driftTimeStart = 120; // Время начала дрифта
        public float raceEndTimerStart = 25; // Время начала таймера окончания гонки
        public float flyingStartSpeed = 100; // Скорость при летающем старте
        public float rollingStartSpeed = 75; // Скорость при скользящем старте
        public float endRaceDelay = 3; // Задержка окончания гонки

        [Range(0, 1)] public float postRaceSpeedMultiplier = 0.25f; // Множитель скорости после гонки
        public bool autoStartRace = true; // Автоматический старт гонки
        public bool autoStartReplay; // Автоматический старт реплея
        public bool nonCollisionRace; // Гонка без столкновений
        public bool postRaceCollisions = true; // Столкновения после гонки
        public bool enableRaceEndTimer; // Включение таймера окончания гонки
        public bool autoDriveTimeTrial; // Автоматическое управление в тайм-триале
        public bool flyingStart; // Летающий старт
        public bool useTimeLimit; // Использование ограничения по времени
        public bool finishEnduranceImmediately; // Немедленное завершение гонки на выносливость
        public bool enableCinematicCameraAfterFinish; // Включение кинематографической камеры после финиша
        public bool infiniteLaps { get; private set; } // Бесконечные круги
        public bool raceStarted { get; private set; } // Флаг начала гонки
        public bool raceFinished { get; private set; } // Флаг завершения гонки
        public bool isCountdownStarted { get; private set; } // Флаг начала обратного отсчёта
        public bool isRollingStart { get; private set; } // Флаг выполнения скользящего старта
        public bool endRaceTimerStarted { get; private set; } // Флаг запуска таймера окончания гонки
        public float raceEndTimer { get; private set; } // Таймер окончания гонки
        public float raceLimitTimer { get; private set; } // Таймер ограничения гонки
        public int numberOfRacersFinished { get; private set; } // Количество завершивших гонку гонщиков
        public int activeRacerCount { get; private set; } // Количество активных гонщиков
        private bool enduranceTimerOver; // Флаг окончания таймера выносливости
        private bool isTimeLimitAvailableForRaceType; // Флаг доступности ограничения по времени для типа гонки
        public Transform timeTrialStartPoint; // Точка старта тайм-триала
        public List<RacerStatistics> racerList { get; private set; } // Список статистики гонщиков
        public List<RacerStatistics> championshipList { get; private set; } // Список статистики чемпионата
        private List<Transform> gridPositionList; // Список позиций на стартовой решётке


        //Настройки плеера
        public GameObject playerVehiclePrefab;
        public string playerName = "Player";
        public Nationality playerNationality;
        public RacerStatistics playerStatistics { get; private set; }

        //Значки на миникарте/Имена гонщиков
        public List<GameObject> aiVehiclePrefabs;
        public AiDetails aiDetails;
        public AiDifficulty easyAiDifficulty;
        public AiDifficulty mediumAiDifficulty;
        public AiDifficulty hardAiDifficulty;
        private List<AIDetial> aiDetailsList = new List<AIDetial>();
        private int aiNameIndex = 1;

        //Значки на миникарте/Имена гонщиков
        public MinimapIcon playerMinimapIcon;
        public MinimapIcon opponentMinimapIcon;
        public RacerName racerName;

        //Настройки штрафа / неправильного пути
        public bool enableOfftrackPenalty;
        public int minWheelCountForOfftrack = 4;
        public bool forceWrongwayRespawn;
        public float wrongwayRespawnTime = 5;

        //Настройки догоняющего/резинового соединения
        public bool enableCatchup;
        [Range(0.1f, 1)] public float catchupStrength = 0.1f;
        public float minCatchupRange = 10;
        public float maxCatchupRange = 100;

        //Настройки Призрачного Транспортного Средства
        public bool enableGhostVehicle = true;
        private bool isGhostAvailableForRaceType;
        public Shader ghostVehicleShader;
        public Material ghostVehicleMaterial;

        //Настройки обратного отсчета
        public int countdownFrom = 3;
        public int countdownDelay = 1;
        public CountdownInfo[] countdownAudio;
        private float internalCountdownTimer;
        private int countdownTime;

        //Целевое время/счет
        public float targetTimeGold;
        public float targetTimeSilver;
        public float targetTimeBronze;
        public float targetScoreGold;
        public float targetScoreSilver;
        public float targetScoreBronze;

        //Погоня
        public int maxSpikes = 5;
        public int maxRoadblocks = 3;

        //Настройки возрождения
        public RespawnSettings respawnSettings;

        //Slipstream Settings
        public SlipstreamSettings slipstreamSettings;

        //Настройки дрифт-гонки
        public DriftRaceSettings driftRaceSettings;

        // Параметры музыки"
        public bool startMusicAfterCountdown;
        public AudioClip postRaceMusic;
        public bool loopPostRaceMusic;
        private MusicPlayer musicPlayer;

        //Messages
        public bool showRaceInfoMessages = true;
        public bool showWhenRacerFinishes = true;
        public bool showSplitTimes = true;
        public bool showVehicleAheadAndBehindGap = true;
        public bool addPositionToRaceEndMessage;

        // Ссылки
        private CameraManager cameraManager;
        private IInputManager inputManager;

        // События
        public static UnityAction OnVehicleSpawn;
        public static UnityAction OnRaceStart;
        public static UnityAction OnPlayerFinish;
        public static UnityAction OnRacePositionsChange;
        public static UnityAction<RaceState> OnRaceStateChange;
        public static UnityAction<int> OnRaceCountdown;
        internal bool enableVehicleInput;

        void Awake()
        {

            instance = this;



            if (loadRaceSettings)
            {
                new GameObject("DataLoader").AddComponent<DataLoader>();
            }

            if (ChampionshipManager.instance != null)
            {
                raceType = ChampionshipManager.instance.CurrentRound().raceType;
                lapCount = ChampionshipManager.instance.CurrentRound().laps;
                opponentCount = ChampionshipManager.instance.championshipRacers.Count - 1;
            }

            // Чтение "EliminationTime", "CheckpointTime", "EnduranceTime", "DriftTime" (уже было)
            if (PlayerPrefs.HasKey("EliminationTime"))
                eliminationTimeStart = PlayerPrefs.GetFloat("EliminationTime");

            if (PlayerPrefs.HasKey("CheckpointTime"))
                checkpointTimeStart = PlayerPrefs.GetFloat("CheckpointTime");

            if (PlayerPrefs.HasKey("EnduranceTime"))
                enduranceTimeStart = PlayerPrefs.GetFloat("EnduranceTime");

            if (PlayerPrefs.HasKey("DriftTime"))
                driftTimeStart = PlayerPrefs.GetFloat("DriftTime");

            // === Новые строки Time Attack ===
            if (PlayerPrefs.HasKey("TargetTimeGold"))
                targetTimeGold = PlayerPrefs.GetFloat("TargetTimeGold");

            if (PlayerPrefs.HasKey("TargetTimeSilver"))
                targetTimeSilver = PlayerPrefs.GetFloat("TargetTimeSilver");

            if (PlayerPrefs.HasKey("TargetTimeBronze"))
                targetTimeBronze = PlayerPrefs.GetFloat("TargetTimeBronze");

            // === Новые строки Drift ===
            if (PlayerPrefs.HasKey("TargetScoreGold"))
                targetScoreGold = PlayerPrefs.GetFloat("TargetScoreGold");

            if (PlayerPrefs.HasKey("TargetScoreSilver"))
                targetScoreSilver = PlayerPrefs.GetFloat("TargetScoreSilver");

            if (PlayerPrefs.HasKey("TargetScoreBronze"))
                targetScoreBronze = PlayerPrefs.GetFloat("TargetScoreBronze");


        }




        void Start()
        {

            // Заменяем строки локализованными значениями
            bestTimeInfo = LocalizationManager.GetTranslation("RaceMessages/BestTimeInfo");
            finalLapInfo = LocalizationManager.GetTranslation("RaceMessages/FinalLapInfo");
            invalidLapInfo = LocalizationManager.GetTranslation("RaceMessages/InvalidLapInfo");
            lapKnockoutDisqualifyInfo = LocalizationManager.GetTranslation("RaceMessages/LapKnockoutDisqualifyInfo");
            checkpointDisqualifyInfo = LocalizationManager.GetTranslation("RaceMessages/CheckpointDisqualifyInfo");
            eliminationDisqualifyInfo = LocalizationManager.GetTranslation("RaceMessages/EliminationDisqualifyInfo");
            defaultDisqualifyInfo = LocalizationManager.GetTranslation("RaceMessages/DefaultDisqualifyInfo");
            raceEndMessage = LocalizationManager.GetTranslation("RaceMessages/RaceEndMessage");
            racerList = new List<RacerStatistics>();
            championshipList = new List<RacerStatistics>();
            cameraManager = FindObjectOfType<CameraManager>();
            musicPlayer = FindObjectOfType<MusicPlayer>();
            inputManager = InputManager.instance;
            LoadLocalizedMessages();

            //Создайте эти игровые объекты, чтобы поддерживать чистоту наследственности
            new GameObject("RGSK_Trackers");
            new GameObject("RGSK_MinimapIcons");
            new GameObject("RGSK_RacerNames");

            //Настройка значений на основе типа гонки
            ConfigureRaceTypeConditions();

            //Загрузить доступные позиции сетки
            LoadGridPositions();

            //Создать игрока / транспортные средства ИИ
            if (ChampionshipManager.instance == null)
            {
                SpawnPlayer();
                SpawnAI();
            }
            else
            {
                //В чемпионатах позвольте ChampionshipManager управлять спавном
                ChampionshipManager.instance.SpawnVehicles();
            }

            //Обработка предгоночных конфигураций
            PreRaceConfiguration();
        }


        private void LoadLocalizedMessages()
        {
            bestTimeInfo = LocalizationManager.GetTranslation("RaceMessages/BestTimeInfo");
            finalLapInfo = LocalizationManager.GetTranslation("RaceMessages/FinalLapInfo");
            invalidLapInfo = LocalizationManager.GetTranslation("RaceMessages/InvalidLapInfo");
            lapKnockoutDisqualifyInfo = LocalizationManager.GetTranslation("RaceMessages/LapKnockoutDisqualifyInfo");
            checkpointDisqualifyInfo = LocalizationManager.GetTranslation("RaceMessages/CheckpointDisqualifyInfo");
            eliminationDisqualifyInfo = LocalizationManager.GetTranslation("RaceMessages/EliminationDisqualifyInfo");
            defaultDisqualifyInfo = LocalizationManager.GetTranslation("RaceMessages/DefaultDisqualifyInfo");
            raceEndMessage = LocalizationManager.GetTranslation("RaceMessages/RaceEndMessage");

            capturedInfo = LocalizationManager.GetTranslation("RacePanel/CapturedInfo");
            capturedAllInfo = LocalizationManager.GetTranslation("RacePanel/CapturedAllInfo");
            capturedFailInfo = LocalizationManager.GetTranslation("RacePanel/CapturedFailInfo");
        }

        // Если язык изменяется динамически, вызовите этот метод
        public void OnLanguageChanged()
        {
            LoadLocalizedMessages();
        }


        private GameObject GetAIVehicleOfSameClass()
        {
            // Создаём список всех машин AI с тем же классом, что у игрока
            List<GameObject> matchingAIVehicles = new List<GameObject>();

            foreach (GameObject aiVehicle in aiVehiclePrefabs)
            {
                CarClass carClassComponent = aiVehicle.GetComponent<CarClass>();
                if (carClassComponent != null && carClassComponent.carClass == playerVehicleClass)
                {
                    matchingAIVehicles.Add(aiVehicle);
                }
            }

            if (matchingAIVehicles.Count > 0)
            {
                // Случайным образом выбираем одну машину из подходящих
                int randomIndex = UnityEngine.Random.Range(0, matchingAIVehicles.Count);
                //Debug.Log($"Выбрана AI машина класса {playerVehicleClass}: {matchingAIVehicles[randomIndex].name}");
                return matchingAIVehicles[randomIndex];
            }

            //Debug.LogWarning("Не удалось найти машину AI с таким же классом, как у игрока.");
            return null; // Если нет машин нужного класса
        }



        public void SpawnVehicles()
        {
            // Спавним машину игрока
            if (playerVehiclePrefab != null)
            {
                SpawnVehicle(playerVehiclePrefab, 0, true); // Позиция 0 для игрока
            }
            else
            {
                //Debug.LogError("Сборка автомобиля игрока не назначена.!");
                return;
            }

            // Спавним машины AI
            for (int i = 1; i <= opponentCount; i++)
            {
                GameObject aiVehicle = GetAIVehicleOfSameClass();
                if (aiVehicle != null)
                {
                    SpawnVehicle(aiVehicle, i, false); // Позиции для ботов начиная с 1
                }
                else
                {
                    //Debug.LogError("Ошибка: Не удалось найти AI машину с классом игрока!");
                }
            }
        }




        void LoadGridPositions()
        {
            gridPositionList = new List<Transform>();

            GridPositions gridPositions = FindObjectOfType<GridPositions>();
            if (gridPositions == null)
                return;

            //Убедитесь, что количество противников никогда не меньше 0
            if (opponentCount < 0 || aiVehiclePrefabs.Count == 0)
                opponentCount = 0;

            int count = 0;
            Transform[] temp = gridPositions.GetComponentsInChildren<Transform>();
            foreach (Transform t in temp)
            {
                if (t != gridPositions.transform)
                {
                    //Добавить позицию на стартовой решетке для каждого гонщика, игнорируя тех, которые не нужны
                    if (count < opponentCount + 1)
                    {
                        gridPositionList.Add(t);
                        count++;
                    }
                }
            }
        }


        void ConfigureRaceTypeConditions()
        {
            isGhostAvailableForRaceType = raceType == RaceType.TimeAttack || raceType == RaceType.TimeTrial;
            isTimeLimitAvailableForRaceType = raceType == RaceType.Endurance || raceType == RaceType.Elimination || raceType == RaceType.Drift;

            infiniteLaps = false;
            lapCount = Mathf.Clamp(lapCount, 1, lapCount);
            positioningType = PositioningType.PositionScore;

            // Вызов метода для текущего типа гонки
            switch (raceType)
            {
                case RaceType.Sprint:
                    ConfigureSprint();
                    break;

                case RaceType.LapKnockout:
                    ConfigureLapKnockout();
                    break;

                case RaceType.Checkpoint:
                    ConfigureCheckpoint();
                    break;

                case RaceType.SpeedTrap:
                    ConfigureSpeedTrap();
                    break;

                case RaceType.TimeTrial:
                    ConfigureTimeTrial();
                    break;

                case RaceType.TimeAttack:
                    ConfigureTimeAttack();
                    break;

                case RaceType.Elimination:
                    ConfigureElimination();
                    break;

                case RaceType.Endurance:
                    ConfigureEndurance();
                    break;

                case RaceType.Drag:
                    ConfigureDrag();
                    break;

                case RaceType.Drift:
                    ConfigureDrift();
                    break;
                case RaceType.Chase:
                    ConfigureChaseRace();
                    break;
                case RaceType.Pursuit:
                    ConfigurePursuitRace();
                    break;
                case RaceType.Leader:
                    // Задаём нужное количество кругов, например, 3
                    lapCount = 3;
                    // Возможно, отключите элементы, связанные с догонялками, штрафами и т.д.
                    // Также можно задать дополнительные флаги, если потребуется.
                    break;


                default:
                    //Debug.LogWarning($"Тип гонки {raceType} не поддерживается!");
                    break;
            }
        }


        void ConfigureSprint()
        {
            lapCount = 1;

        }

        void ConfigureLapKnockout()
        {
            opponentCount = Mathf.Clamp(opponentCount, 1, opponentCount);
            lapCount = opponentCount;
            enableRaceEndTimer = false;
        }
        void ConfigureCheckpoint()
        {
            // Логика для гонки по контрольным точкам
        }
        void ConfigureSpeedTrap()
        {
            positioningType = PositioningType.Speed;
            lapCount = 1;
        }
        void ConfigureTimeTrial()
        {
            opponentCount = 0;
            infiniteLaps = true;
            if (flyingStart)
                autoStartRace = true;
        }
        void ConfigureTimeAttack()
        {
            opponentCount = 0;
            addPositionToRaceEndMessage = false;
            lapCount = 1;
        }
        void ConfigureElimination()
        {
            useTimeLimit = true;
            raceLimitTimer = eliminationTimeStart;
            opponentCount = Mathf.Clamp(opponentCount, 1, opponentCount);
        }
        void ConfigureEndurance()
        {
            useTimeLimit = true;
            raceLimitTimer = enduranceTimeStart;
            infiniteLaps = true;
        }
        void ConfigureDrag()
        {
            opponentCount = 1;
            lapCount = 1;
        }
        void ConfigureDrift()
        {
            opponentCount = 0;
            addPositionToRaceEndMessage = false;
            if (useTimeLimit)
            {
                raceLimitTimer = driftTimeStart;
            }

        }



        private void ConfigureChaseRace()
        {
            totalBots = opponentCount;
            var ptm = PursuitTargetManager.instance;
            if (ptm != null)
            {
                ptm.maxSpikes = PlayerPrefs.GetInt("MaxSpikes", ptm.maxSpikes);
                ptm.remainingSpikes = ptm.maxSpikes;
                ptm.maxRoadblocks = PlayerPrefs.GetInt("MaxRoadblocks", ptm.maxRoadblocks);
                ptm.usedRoadblocks = 0;
                ptm.lastRoadblockTime = -999f;
            }

            // Игрок всегда стартует первым
            playerGridPositioningMode = GridPositioningMode.First;
            playerStartPosition = 1;

            // До срабатывания триггера игрок не может ехать

        }


        private void ConfigurePursuitRace()
        {
            totalBots = opponentCount;
            capturedBots = 0;  // ← пока ни одна не поймана


            /* здесь будет остальная логика погони
               (спавн копов, отключение респаунов и т.д.) */
        }




        void SpawnPlayer()
        {
            if (playerVehiclePrefab == null)
            {
                //Debug.LogWarning("Транспортное средство игрока не назначено!");
                return;
            }

            //Зафиксировать выбранную позицию
            playerStartPosition = Mathf.Clamp(playerStartPosition, 1, gridPositionList.Count);

            //Определяем позицию появления игрока
            int pos =
                playerGridPositioningMode == GridPositioningMode.Random ? UnityEngine.Random.Range(0, gridPositionList.Count) :
                playerGridPositioningMode == GridPositioningMode.Select ? playerStartPosition - 1 :
                playerGridPositioningMode == GridPositioningMode.First ? 0 :
                gridPositionList.Count - 1;

            SpawnVehicle(playerVehiclePrefab, pos, true);
        }


        void SpawnAI()
        {
            //Создать имена ИИ
            if (aiDetails != null)
            {
                foreach (AIDetial detial in aiDetails.aiDetials)
                {
                    aiDetailsList.Add(detial);
                }
            }

            int vehicleIndex = 0;

            for (int i = 0; i < opponentCount; i++)
            {
                if (aiVehiclePrefabs.Count == 0)
                    break;

                //Выберите случайную начальную позицию для этого ИИ
                int spawnposition = UnityEngine.Random.Range(0, gridPositionList.Count);

                //Spawn the vehicle
                SpawnVehicle(aiVehiclePrefabs[vehicleIndex], spawnposition, false);

                //Создать следующее транспортное средство в списке в следующей итерации
                vehicleIndex++;
                vehicleIndex = vehicleIndex % aiVehiclePrefabs.Count;
            }
        }


        public void SpawnVehicle(GameObject vehicle, int position, bool isPlayer)
        {
            if (gridPositionList.Count == 0)
                return;

            // Фильтрация машин AI перед спавном
            if (!isPlayer)
            {
                GameObject filteredAIVehicle = GetAIVehicleOfSameClass();
                if (filteredAIVehicle != null)
                {
                    vehicle = filteredAIVehicle; // Используем случайно выбранную машину подходящего класса
                }
                else
                {
                    //Debug.LogError("Не удалось найти машину AI с нужным классом. Используется первая машина из списка.");
                    vehicle = aiVehiclePrefabs[0]; // Запасной вариант
                }
            }

            // Создаём экземпляр префаба
            GameObject clone = Instantiate(vehicle, gridPositionList[position].position, gridPositionList[position].rotation);

            RacerStatistics racerStatistics = clone.GetComponent<RacerStatistics>();
            if (racerStatistics != null)
            {
                // Добавить к количеству активных гонщиков
                activeRacerCount++;

                // Добавить гонщика в список
                racerList.Add(racerStatistics);

                // Сброс значений гонщика
                racerStatistics.Reset();

                if (racerStatistics.IsPlayer())
                {
                    // Назначить данные игрока
                    playerStatistics = racerStatistics;
                    racerStatistics.racerInformation.racerName = playerName;
                    racerStatistics.racerInformation.nationality = playerNationality;

                    ConfigurePlayerVehicleForRaceType();

                    // Определяем класс машины игрока
                    CarClass carClassComponent = clone.GetComponent<CarClass>();
                    if (carClassComponent != null)
                    {
                        playerVehicleClass = carClassComponent.carClass;
                        //Debug.Log($"Класс машины игрока: {playerVehicleClass}");
                    }
                    else
                    {
                        //Debug.LogWarning("CarClass не найден на машине игрока!");
                    }
                }
                else
                {
                    // Назначить данные AI
                    AIDetial details = GetRandomAIDetails();
                    racerStatistics.racerInformation.racerName = details.name;
                    racerStatistics.racerInformation.nationality = details.nationality;
                }
            }

            // Создание иконок миникарты
            if (playerMinimapIcon != null && opponentMinimapIcon != null)
            {
                MinimapIcon m = isPlayer ? Instantiate(playerMinimapIcon) : Instantiate(opponentMinimapIcon);
                m.target = clone.transform;
                m.transform.parent = GameObject.Find("RGSK_MinimapIcons").transform;
            }

            // Создание имён гонщиков
            if (racerName != null)
            {
                RacerName names = Instantiate(racerName);
                names.racer = clone.GetComponent<RacerStatistics>();
                names.transform.SetParent(GameObject.Find("RGSK_RacerNames").transform, false);
            }

            // Добавить компонент респауна
            if (respawnSettings.enableRespawns)
            {
                Respawner respawner = clone.AddComponent<Respawner>();
                respawner.respawnSettings = respawnSettings;
            }

            // Установить таймер контрольных точек для гонок по контрольным точкам
            if (raceType == RaceType.Checkpoint)
            {
                racerStatistics.checkpointTimer = checkpointTimeStart;
            }

            // Настройка коллайдеров
            if (nonCollisionRace)
            {
                clone.SetColliderLayer("IgnoreCollision");
            }
            else
            {
                clone.SetColliderLayer("Vehicle");
            }

            // Вызвать событие спавна
            OnVehicleSpawn?.Invoke();

            // Добавить автомобиль в список целей камеры
            cameraManager?.AddVehicle(clone.transform);

            // Обновить позиции гонки
            UpdateRacePositions();

            // Удалить позицию из списка, чтобы другие машины не спавнились в том же месте
            gridPositionList.RemoveAt(position);
        }






        public void SpawnChampionshipVehicle(GameObject vehicle, int position, int points, string name, Nationality nationality, bool isPlayer)
        {
            if (gridPositionList.Count == 0)
                return;

            //Создаем экземпляр префаба
            GameObject clone = (GameObject)Instantiate(vehicle,
                gridPositionList[position].position, gridPositionList[position].rotation);

            RacerStatistics racerStatistics = clone.GetComponent<RacerStatistics>();
            if (racerStatistics != null)
            {
                //Добавить к количеству активных гонщиков
                activeRacerCount++;

                //Добавить это в список гонщиков
                racerList.Add(racerStatistics);

                //Сбросить значения в скрипте
                racerStatistics.Reset();

                //В чемпионате добавить эту гонку в список чемпионата
                championshipList.Add(racerStatistics);
                racerStatistics.championshipPoints = points;

                if (racerStatistics.IsPlayer())
                {
                    //Назначить данные игрока
                    playerStatistics = racerStatistics;
                    ConfigurePlayerVehicleForRaceType();
                }

                //Назначить детали
                racerStatistics.racerInformation.racerName = name;
                racerStatistics.racerInformation.nationality = nationality;

                //Установить сложность ИИ
                AiLogic ai = clone.GetComponent<AiLogic>();
                if (ai != null)
                {
                    switch (aiDifficultyLevel)
                    {
                        case AIDifficultyLevel.Easy:
                            if (easyAiDifficulty != null)
                            {
                                ai.SetDifficulty(easyAiDifficulty);
                            }
                            break;

                        case AIDifficultyLevel.Medium:
                            if (mediumAiDifficulty != null)
                            {
                                ai.SetDifficulty(mediumAiDifficulty);
                            }
                            break;

                        case AIDifficultyLevel.Hard:
                            if (hardAiDifficulty != null)
                            {
                                ai.SetDifficulty(hardAiDifficulty);
                            }
                            break;
                    }
                }

                //Создание иконок миникарты
                if (playerMinimapIcon != null && opponentMinimapIcon != null)
                {
                    MinimapIcon m = isPlayer ? Instantiate(playerMinimapIcon) as MinimapIcon : Instantiate(opponentMinimapIcon) as MinimapIcon;
                    m.target = clone.transform;
                    m.transform.parent = GameObject.Find("RGSK_MinimapIcons").transform;
                }

                //Создание имен гонщиков
                if (racerName != null)
                {
                    RacerName names = Instantiate(racerName) as RacerName;
                    names.racer = clone.GetComponent<RacerStatistics>();
                    names.transform.SetParent(GameObject.Find("RGSK_RacerNames").transform, false);
                }

                //Добавить компонент респауна
                if (respawnSettings.enableRespawns)
                {
                    Respawner respawner = clone.AddComponent<Respawner>();
                    respawner.respawnSettings = respawnSettings;
                }

                //Установить таймер контрольных точек для гонок по контрольным точкам
                if (raceType == RaceType.Checkpoint)
                {
                    racerStatistics.checkpointTimer = checkpointTimeStart;
                }
            }

            if (nonCollisionRace)
            {
                //Установить коллайдеры этого транспортного средства на слой "IgnoreCollision" для гонок без столкновений
                clone.SetColliderLayer("IgnoreCollision");
            }
            else
            {
                //Установить коллайдеры этого транспортного средства на слой "Транспортное средство"
                clone.SetColliderLayer("Vehicle");
            }

            //Вызвать это событие для каждого созданного транспортного средства
            if (OnVehicleSpawn != null)
            {
                OnVehicleSpawn.Invoke();
            }

            //Добавить это транспортное средство в список целей камеры
            if (cameraManager != null)
            {
                cameraManager.AddVehicle(clone.transform);
            }

            //Обновляем позиции для каждого появления транспортного средства
            UpdateRacePositions();
        }

        public void SpawnCareerVehicle(GameObject vehicle, int position, string name, Nationality nationality, bool isPlayer)
        {
            if (gridPositionList.Count == 0)
                return;

            // Создаем экземпляр префаба
            GameObject clone = Instantiate(vehicle, gridPositionList[position].position, gridPositionList[position].rotation);

            RacerStatistics racerStatistics = clone.GetComponent<RacerStatistics>();
            if (racerStatistics != null)
            {
                // Добавить к количеству активных гонщиков
                activeRacerCount++;

                // Добавить это в список гонщиков
                racerList.Add(racerStatistics);

                // Сбросить значения в скрипте
                racerStatistics.Reset();

                if (racerStatistics.IsPlayer())
                {
                    // Назначить данные игрока
                    playerStatistics = racerStatistics;
                    ConfigurePlayerVehicleForRaceType();
                }

                // Назначить детали
                racerStatistics.racerInformation.racerName = name;
                racerStatistics.racerInformation.nationality = nationality;

                // Установить сложность ИИ
                AiLogic ai = clone.GetComponent<AiLogic>();
                if (ai != null)
                {
                    switch (aiDifficultyLevel)
                    {
                        case AIDifficultyLevel.Easy:
                            if (easyAiDifficulty != null)
                            {
                                ai.SetDifficulty(easyAiDifficulty);
                            }
                            break;

                        case AIDifficultyLevel.Medium:
                            if (mediumAiDifficulty != null)
                            {
                                ai.SetDifficulty(mediumAiDifficulty);
                            }
                            break;

                        case AIDifficultyLevel.Hard:
                            if (hardAiDifficulty != null)
                            {
                                ai.SetDifficulty(hardAiDifficulty);
                            }
                            break;
                    }
                }

                // Создание иконок на миникарте
                if (playerMinimapIcon != null && opponentMinimapIcon != null)
                {
                    MinimapIcon m = isPlayer ? Instantiate(playerMinimapIcon) : Instantiate(opponentMinimapIcon);
                    m.target = clone.transform;
                    m.transform.parent = GameObject.Find("RGSK_MinimapIcons").transform;
                }

                // Создание имен гонщиков
                if (racerName != null)
                {
                    RacerName names = Instantiate(racerName);
                    names.racer = clone.GetComponent<RacerStatistics>();
                    names.transform.SetParent(GameObject.Find("RGSK_RacerNames").transform, false);
                }

                // Добавить компонент респауна
                if (respawnSettings.enableRespawns)
                {
                    Respawner respawner = clone.AddComponent<Respawner>();
                    respawner.respawnSettings = respawnSettings;
                }

                // Установить таймер контрольных точек для гонок по контрольным точкам
                if (raceType == RaceType.Checkpoint)
                {
                    racerStatistics.checkpointTimer = checkpointTimeStart;
                }
            }

            if (nonCollisionRace)
            {
                // Установите коллайдеры этого транспортного средства на слой «IgnoreCollision» для гонок без столкновений
                clone.SetColliderLayer("IgnoreCollision");
            }
            else
            {
                // Устанавливаем коллайдеры этого транспортного средства на слой «Транспортное средство»
                clone.SetColliderLayer("Vehicle");
            }

            // Запускать это событие для каждого созданного транспортного средства
            OnVehicleSpawn?.Invoke();

            // Добавить это транспортное средство в список целей камеры
            cameraManager?.AddVehicle(clone.transform);

            // Обновить позиции для каждого появления транспортного средства
            UpdateRacePositions();
        }



        void ConfigurePlayerVehicleForRaceType()
        {
            if (playerStatistics == null)
                return;

            switch (raceType)
            {
                case RaceType.TimeTrial:
                    //Добавляем компонент инициализации гонки на время
                    playerStatistics.gameObject.AddComponent<TimeTrialInitialize>();

                    //Переместить автомобиль в начальную точку гонки на время
                    if (timeTrialStartPoint != null)
                    {
                        playerStatistics.transform.position = timeTrialStartPoint.position;
                        playerStatistics.transform.rotation = timeTrialStartPoint.rotation;
                    }

                    //Переведите автомобиль в режим ИИ, чтобы подъехать к стартовой линии
                    if (autoDriveTimeTrial)
                    {
                        PlayerToAI();
                    }

                    //Начните с быстрого круга, придав автомобилю некоторую скорость
                    if (flyingStart)
                    {
                        Rigidbody rigid = playerStatistics.GetComponent<Rigidbody>();
                        if (rigid != null)
                        {
                            rigid.linearVelocity = (rigid.transform.forward * flyingStartSpeed) / 3.6f;
                        }
                    }

                    break;

                case RaceType.Drift:
                    //Добавляем контроллер точки дрифта
                    if (!playerStatistics.gameObject.GetComponent<DriftPointsManager>())
                    {
                        playerStatistics.gameObject.AddComponent<DriftPointsManager>();
                    }
                    break;
            }
        }


        void PreRaceConfiguration()
        {
            //Создать призрачный автомобиль, если он доступен
            if (isGhostAvailableForRaceType && enableGhostVehicle)
            {
                CreateGhostVehicleManager();
            }

            //Автоматически начать обратный отсчет гонки
            if (autoStartRace)
            {
                BeginCountdown();
            }
            else
            {
                //Иначе, войти в состояние PreRace
                SwitchRaceState(RaceState.PreRace);
            }
        }


        public void BeginCountdown()
        {
            SwitchRaceState(RaceState.Race);
            StartCoroutine(Countdown());
        }


        IEnumerator Countdown()
        {
            //Пропустить обратный отсчет в типе гонки TimeTrial
            if (raceType == RaceType.TimeTrial)
            {
                raceStarted = true;
                yield break;
            }

            yield return new WaitForSeconds(countdownDelay);

            //Пропустить обратный отсчет для скользящих стартов
            if (startMode == RaceStartMode.RollingStart)
            {
                BeginRollingStart();
                yield break;
            }

            isCountdownStarted = true;

            countdownTime = countdownFrom + 1;

            while (countdownTime > 0)
            {
                internalCountdownTimer -= Time.deltaTime;

                if (internalCountdownTimer <= 0)
                {
                    countdownTime -= 1;
                    internalCountdownTimer = 1;

                    if (countdownTime > 0)
                    {
                        //Воспроизвести звук обратного отсчета
                        PlayCountdownAudio(countdownTime);

                        //Запустить событие обратного отсчета
                        if (OnRaceCountdown != null)
                        {
                            OnRaceCountdown(countdownTime);
                        }
                    }
                }

                yield return null;
            }

            isCountdownStarted = false;

            //Обработка логики начала гонки
            StartRace();
        }


        public void StartRace()
        {
            //Debug.Log("StartRace: Запуск гонки начат.");
            Time.timeScale = 1f;
            // 1) Считываем настройки "Настройки догоняющего"
            if (PlayerPrefs.HasKey("EnableCatchup"))
                enableCatchup = PlayerPrefs.GetInt("EnableCatchup") == 1;

            if (PlayerPrefs.HasKey("CatchupStrength"))
                catchupStrength = PlayerPrefs.GetFloat("CatchupStrength");

            if (PlayerPrefs.HasKey("MinCatchupRange"))
                minCatchupRange = PlayerPrefs.GetFloat("MinCatchupRange");

            if (PlayerPrefs.HasKey("MaxCatchupRange"))
                maxCatchupRange = PlayerPrefs.GetFloat("MaxCatchupRange");

            // 2) Считываем настройки сложности ботов
            if (PlayerPrefs.HasKey("AiDifficulty"))
            {
                string difficulty = PlayerPrefs.GetString("AiDifficulty");
                if (System.Enum.TryParse(difficulty, out AIDifficultyLevel parsedDifficulty))
                {
                    aiDifficultyLevel = parsedDifficulty;
                }
                else
                {
                    //Debug.LogWarning("StartRace: Ошибка чтения AiDifficulty из PlayerPrefs!");
                }
            }

            // Чтение настроек штрафов
            if (PlayerPrefs.HasKey("EnableOfftrackPenalty"))
                enableOfftrackPenalty = PlayerPrefs.GetInt("EnableOfftrackPenalty") == 1;

            if (PlayerPrefs.HasKey("MinWheelCountForOfftrack"))
                minWheelCountForOfftrack = PlayerPrefs.GetInt("MinWheelCountForOfftrack");

            if (PlayerPrefs.HasKey("ForceWrongwayRespawn"))
                forceWrongwayRespawn = PlayerPrefs.GetInt("ForceWrongwayRespawn") == 1;

            if (PlayerPrefs.HasKey("WrongwayRespawnTime"))
                wrongwayRespawnTime = PlayerPrefs.GetFloat("WrongwayRespawnTime");

            // 3) Считываем настройки времен для типов гонок
            if (PlayerPrefs.HasKey("EliminationTime"))
                eliminationTimeStart = PlayerPrefs.GetFloat("EliminationTime", eliminationTimeStart);

            if (PlayerPrefs.HasKey("CheckpointTime"))
                checkpointTimeStart = PlayerPrefs.GetFloat("CheckpointTime", checkpointTimeStart);

            if (PlayerPrefs.HasKey("EnduranceTime"))
                enduranceTimeStart = PlayerPrefs.GetFloat("EnduranceTime", enduranceTimeStart);

            if (PlayerPrefs.HasKey("DriftTime"))
                driftTimeStart = PlayerPrefs.GetFloat("DriftTime", driftTimeStart);

            // Инициализация трафика после старта гонки
            if (trafficComponent != null)
            {
                trafficComponent.gameObject.SetActive(true);
            }

            //Debug.Log($"StartRace: Таймер для Elimination: {eliminationTimeStart}, Checkpoint: {checkpointTimeStart}");

            // 6) Запускаем гонку для всех гонщиков
            for (int i = 0; i < racerList.Count; i++)
            {
                racerList[i].Reset();
                racerList[i].started = true;

                if (playerStatistics != null && playerStatistics.GetComponent<global::RCC_CarControllerV4>() != null)
                {
                    playerStatistics.GetComponent<global::RCC_CarControllerV4>().engineRunning = true;
                }
            }

            //Debug.Log("StartRace: Все гонщики запущены.");

            // 8) Воспроизвести звук начала гонки
            PlayCountdownAudio(0);

            // 9) Событие обратного отсчёта (перед запуском)
            if (OnRaceCountdown != null)
            {
                OnRaceCountdown(0);
            }

            // 10) Событие начала гонки
            if (OnRaceStart != null)
            {
                OnRaceStart();
            }

            if (startMusicAfterCountdown)
            {
                if (musicPlayer != null)
                {
                    musicPlayer.StartMusic();
                }
            }

            // 12) Установить флаг начала гонки
            raceStarted = true;
            //Debug.Log("StartRace: Гонка началась, raceStarted установлено в true.");

            // Проверка, чтобы raceLimitTimer и другие переменные были корректно инициализированы
            //Debug.Log($"StartRace: Значение raceLimitTimer: {raceLimitTimer}, useTimeLimit: {useTimeLimit}");

            // 13) Инициализация таймера Elimination
            if (raceType == RaceType.Elimination)
            {
                raceLimitTimer = eliminationTimeStart;
                //Debug.Log($"StartRace: Таймер для Elimination установлен на {raceLimitTimer} секунд.");
            }
        }



        void BeginRollingStart()
        {
            isRollingStart = true;
            raceStarted = true;
            racerList[0].gameObject.AddComponent<RollingStartInitialize>();

            PlayerToAI();
        }




        void PlayCountdownAudio(int num)
        {
            for (int i = 0; i < countdownAudio.Length; i++)
            {
                if (countdownAudio[i].countdownNumber == num)
                {
                    if (AudioManager.instance != null)
                    {
                        AudioManager.instance.PlayClip(countdownAudio[i].countdownSound);
                    }
                }
            }
        }


        void Update()
        {
            if (!raceFinished)
            {
                CalculateRacerPositions();
            }

            if (endRaceTimerStarted)
            {
                UpdateEndRaceTimer();
            }

            if (useTimeLimit && isTimeLimitAvailableForRaceType)
            {
                UpdateRaceLimitTimer();
            }


            if (Input.GetKeyDown(KeyCode.M)) // Тестовое завершение гонки
            {
                //Debug.Log("Клавиша M нажата. Гонка завершена для тестирования.");

                // 1) Принудительно завершаем гонку для всех
                ForceFinish();

                // 2) Обновляем позиции гонщиков
                UpdateRacePositions();

                // 3) Пусть RaceManager «оформит» финиш игрока
                if (playerStatistics != null)
                {
                    RegisterRacerFinish(playerStatistics);
                }

                // 4) Сохраняем «статические» данные гонки (как раньше)
                string trackName = SceneManager.GetActiveScene().name;
                int playerPosition = playerStatistics != null ? playerStatistics.Position : 1;
                PlayerPrefs.SetString("LastRaceTrack", trackName);
                PlayerPrefs.SetInt("LastRacePosition", playerPosition);
                PlayerPrefs.Save();

                //Debug.Log($"Сохранены данные гонки: Трасса={trackName}, Позиция={playerPosition}");
            }

            // Обработка паузы
            if (inputManager != null)
            {
                if (inputManager.GetButtonDown(0, InputAction.Pause))
                {
                    if (raceState == RaceState.Race)
                    {
                        Pause();
                    }
                    else if (raceState == RaceState.Pause)
                    {
                        UnPause();
                    }
                }
            }
        }


        void UpdateRaceLimitTimer()
        {
            //Debug.Log("UpdateRaceLimitTimer called");
            if (!raceStarted || raceFinished || raceState != RaceState.Race)
                return;

            raceLimitTimer -= Time.deltaTime;

            if (raceLimitTimer <= 0)
            {
                switch (raceType)
                {
                    case RaceType.Elimination:
                        if (activeRacerCount > 1)
                        {
                            //Reset the elimination timer
                            raceLimitTimer = eliminationTimeStart;

                            //Eliminate the racer in last place
                            DisqualifyRacer(GetRacerInLastPlace());
                        }
                        break;

                    case RaceType.Endurance:
                        if (!finishEnduranceImmediately)
                        {
                            if (!enduranceTimerOver)
                            {
                                enduranceTimerOver = true;

                                //Set the final lap to the leaders current lap
                                lapCount = GetRacerInFirstPlace().lap;

                                //The race is now on the final lap so set infinte laps to false
                                infiniteLaps = false;

                                //Show final lap info and update the UI lap text
                                if (RacePanel.instance != null)
                                {
                                    RacePanel.instance.ShowRaceInfoMessage(finalLapInfo);
                                    RacePanel.instance.UpdateLapText();
                                }
                            }
                        }
                        else
                        {
                            //Finish the race immediately
                            ForceFinish();
                        }
                        break;

                    default:
                        //Finish the race
                        ForceFinish();
                        break;
                }
            }
        }








        void UpdateEndRaceTimer()
        {
            if (raceState == RaceState.Replay)
                return;

            raceEndTimer -= Time.deltaTime;

            //Как только таймер дойдет до 0, дисквалифицируйте не закончивших гонку гонщиков
            if (raceEndTimer <= 0)
            {
                endRaceTimerStarted = false;
                enableRaceEndTimer = false;
                DisqualifyUnfinishedRacers();
            }
        }


        void CalculateRacerPositions()
        {
            //Создаем временный список, который позже будет использоваться для проверки того, произошло ли изменение позиции
            List<RacerStatistics> tempList = racerList;

            //Управление позиционированием гонщика на основе типа позиционирования
            switch (positioningType)
            {
                case PositioningType.PositionScore:
                    racerList = racerList.OrderByDescending(x => x.racePositionScore).ToList();
                    break;

                case PositioningType.Speed:
                    racerList = racerList.OrderByDescending(x => x.totalSpeed).ToList();
                    break;
            }

            //При изменении порядка в списке обновляем позиции гонщиков
            if (!tempList.SequenceEqual(racerList))
            {
                UpdateRacePositions();
            }
        }


        public void UpdateRacePositions()
        {
            // Сортировка гонщиков по их текущему порядку (например, по списку racerList)
            for (int i = 0; i < racerList.Count; i++)
            {
                racerList[i].Position = i + 1; // Обновляем позицию для каждого гонщика
            }

            // Лог для отладки
            if (playerStatistics != null)
            {
                //Debug.Log($"Позиция игрока обновлена: {playerStatistics.Position}");
            }

            // Вызов события, если оно назначено
            OnRacePositionsChange?.Invoke();
        }



        public void UpdateChampionshipPositions()
        {
            for (int i = 0; i < racerList.Count; i++)
            {
                //Add the points
                racerList[i].championshipPoints += ChampionshipManager.instance.GetPointsForPosition(racerList[i].Position);
                ChampionshipManager.instance.SetPointsForRacer(racerList[i].racerInformation.racerName, racerList[i].championshipPoints);
            }

            //Order the championship list by descending putting the racers with the most points first
            championshipList = championshipList.OrderByDescending(x => x.championshipPoints).ToList();

            //Set the championship position of the racers
            for (int i = 0; i < championshipList.Count; i++)
            {
                championshipList[i].championshipPosition = (i + 1);
            }

            if (OnRacePositionsChange != null)
            {
                OnRacePositionsChange();
            }
        }


        public void RegisterBotCaptured()
        {
            // работаем только в погоне
            if (raceType != RaceType.Chase) return;

            // счётчик пойманных
            capturedBots = Mathf.Clamp(capturedBots + 1, 0, totalBots);

            // обновляем HUD  «Нужно поймать N / M»
            if (RacePanel.instance)
                RacePanel.instance.UpdatePursuitCounter(capturedBots, totalBots);

            // сообщение «захвачен» / «Caught»  (берётся из локализатора по ключу capturedInfo)
            if (showRaceInfoMessages && RacePanel.instance)
                RacePanel.instance.ShowRaceInfoMessage(capturedInfo);

            // все цели пойманы
            if (capturedBots >= totalBots)
            {
                // крупное финальное сообщение – тоже через локализатор
                if (showRaceInfoMessages && RacePanel.instance)
                    RacePanel.instance.ShowRaceEndMessage(capturedAllInfo);

                // штатное завершение гонки
                ForceFinish();          // если используешь EndRaceRoutine, замени на StartCoroutine(EndRaceRoutine());
            }
        }




        public void RegisterRacerFinish(RacerStatistics racer)
        {
            // 1. Обновляем позиции всех гонщиков
            UpdateRacePositions();

            // 2. Проверяем, завершена ли гонка
            raceFinished = AllRacersFinished();

            // 3. Отключаем коллизии для финишировавшего гонщика (если отключены postRaceCollisions)
            if (!postRaceCollisions)
            {
                racer.gameObject.SetColliderLayer("IgnoreCollision");
            }

            // 4. Проверяем, если финишировал игрок
            if (racer == playerStatistics)
            {
                int playerPosition = racer.Position;

                // Если режим гонки Drift, переопределяем позицию игрока на основании очков дрифта
                if (raceType == RaceType.Drift)
                {
                    DriftPointsManager driftPoints = FindObjectOfType<DriftPointsManager>();
                    int driftPosition = 4; // по умолчанию худший результат
                    if (driftPoints != null)
                    {
                        float points = driftPoints.totalDriftPoints;
                        Debug.Log("RegisterRacerFinish: Total Drift Points = " + points);
                        if (points >= targetScoreGold) driftPosition = 1;
                        else if (points >= targetScoreSilver) driftPosition = 2;
                        else if (points >= targetScoreBronze) driftPosition = 3;
                        else driftPosition = 4;
                    }
                    else
                    {
                        Debug.LogWarning("RegisterRacerFinish: DriftPointsManager не найден, устанавливаем позицию по умолчанию (4).");
                    }
                    playerStatistics.Position = driftPosition;
                    playerPosition = driftPosition;
                    Debug.Log("RegisterRacerFinish: Позиция игрока для режима Drift установлена в " + driftPosition);
                }

                // --- Новая логика выбора ID для сохранения ---
                bool isChampionship = PlayerPrefs.GetInt("IsChampionshipMode", 0) == 1;
                string idToSave = PlayerPrefs.GetString("CurrentRaceID", "defaultID");
                Debug.Log($"RegisterRacerFinish: сохраняем ID = {idToSave} (championship={isChampionship})");
                // --- /Новая логика ---

                // Если игрок занял 1 место, сохраняем результат
                if (playerPosition == 1)
                {
                    if (isChampionship)
                        PlayerData.instance.SaveChampionshipRoundResult(idToSave);
                    else
                        PlayerData.instance.SaveRaceResult(idToSave);

                    // Сбрасываем флаг чемпионата, чтобы следующий вызов шёл в обычном режиме
                    if (isChampionship)
                    {
                        PlayerPrefs.SetInt("IsChampionshipMode", 0);
                        PlayerPrefs.Save();
                    }
                }

                // 5. Если это дейлик (ID содержит "Day" — например "Day3")
                if (idToSave.StartsWith("Day", StringComparison.OrdinalIgnoreCase))
                {
                    int dayIndex = Mathf.Max(0, int.Parse(idToSave.Substring(3)) - 1);
                    bool isWin = (playerPosition == 1);
                    PlayerData.instance.SaveDailyRaceAttempt(dayIndex, isWin);
                    Debug.Log($"[RegisterRacerFinish] Дейлик сохранен! День: {dayIndex}, Победа={isWin}");
                }

                // 6. Сообщаем CareerManager о завершении гонки
                CareerManager cm = FindObjectOfType<CareerManager>();
                if (cm != null)
                {
                    cm.CompleteRace(idToSave, playerPosition);
                    CareerStage currentStage = cm.GetCurrentStage();
                    if (currentStage != null)
                        currentStage.UpdateRaceProgress();
                }

                if (RacePanel.instance != null)
                {
                    if (raceType == RaceType.Chase)               // ← Chase
                    {
                        string msg = (capturedBots >= totalBots)
                                     ? capturedAllInfo            // «Все машины пойманы!»
                                     : capturedFailInfo;          // «Не все машины пойманы»
                        RacePanel.instance.ShowRaceEndMessage(msg);
                    }
                    else                                          // ← все остальные типы гонок
                    {
                        string localized = LocalizationManager.GetTranslation("RaceMessages/RacePositionMessage");
                        if (string.IsNullOrEmpty(localized) || !localized.Contains("{0}"))
                            localized = "Вы заняли {0} место";

                        try
                        {
                            string formatted = string.Format(localized, Helper.AddOrdinal(playerPosition));
                            RacePanel.instance.ShowRaceEndMessage(formatted);
                        }
                        catch (FormatException)
                        {
                            RacePanel.instance.ShowRaceEndMessage("Вы заняли место, но сообщение не смогло быть отформатировано.");
                        }
                    }
                }

                // 7-А. Chase: игрок финишировал, но не все машины пойманы
                if (raceType == RaceType.Chase && capturedBots < totalBots)
                {
                    if (RacePanel.instance != null)
                        RacePanel.instance.ShowRaceInfoMessage(capturedFailInfo); // «Не все машины пойманы»
                }

                // 8. Запускаем завершение гонки
                StartCoroutine(EndRaceRoutine());
            }
            else
            {
                // Логика для других гонщиков
                string otherName = racer.GetName();
                int otherPos = racer.Position;
                if (RacePanel.instance != null)
                {
                    string localizedInfo = LocalizationManager.GetTranslation("RaceMessages/OtherRacerPositionMessage");
                    if (string.IsNullOrEmpty(localizedInfo) || !localizedInfo.Contains("{0}") || !localizedInfo.Contains("{1}"))
                        localizedInfo = "{0}  {1}";

                    try
                    {
                        string formattedInfo = string.Format(localizedInfo, otherName, Helper.AddOrdinal(otherPos));
                        RacePanel.instance.ShowRaceInfoMessage(formattedInfo);
                    }
                    catch (FormatException)
                    {
                        RacePanel.instance.ShowRaceInfoMessage($"{otherName} завершил гонку.");
                    }
                }
            }

            // 9. Увеличиваем счетчик финишировавших
            numberOfRacersFinished++;

            // 10. Проверяем, нужно ли запускать таймер завершения гонки
            if (enableRaceEndTimer)
            {
                bool startTimer = raceEndTimerLogic == RaceEndTimerStart.After1st
                    ? numberOfRacersFinished > 0
                    : numberOfRacersFinished >= racerList.Count / 2;

                if (startTimer && !endRaceTimerStarted)
                {
                    raceEndTimer = raceEndTimerStart;
                    endRaceTimerStarted = true;
                }
            }
        }








        public void DisqualifyRacer(RacerStatistics racer)
        {
            //Disqualify the racer
            racer.disqualified = true;

            //Update race positions when a racer is disqualified
            UpdateRacePositions();

            //set this vehicle to ingnore collisions with other vehicles
            racer.gameObject.SetColliderLayer("IgnoreCollision");

            //subtract from the active racer count
            activeRacerCount--;

            //Show a message based on the race type
            string info = raceType == RaceType.Checkpoint ? checkpointDisqualifyInfo :
                raceType == RaceType.LapKnockout ? lapKnockoutDisqualifyInfo :
                raceType == RaceType.Elimination ? eliminationDisqualifyInfo : defaultDisqualifyInfo;

            //Insert the racers name to the message
            if (!racer.isPlayer)
            {
                info = info.Insert(0, racer.GetName() + " ");
            }

            //Show the message on screen
            if (RacePanel.instance != null)
            {
                RacePanel.instance.ShowRaceInfoMessage(info);
            }

            //End the race if the player gets disqualified
            if (racer == playerStatistics)
            {
                //Trigger this event when the player finishes
                if (OnPlayerFinish != null)
                {
                    OnPlayerFinish.Invoke();
                }

                //The player has been disqualified so force finish the race
                ForceFinish();

                //Show the end race message
                if (RacePanel.instance != null)
                {
                    RacePanel.instance.ShowRaceEndMessage(info);
                }

                //Start the End Race routine
                StartCoroutine(EndRaceRoutine());

                //Switch to AI control
                PlayerToAI();
            }

            //End the race if theres only 1 racer left
            if (activeRacerCount <= 1)
            {
                ForceFinish();
            }
        }


        IEnumerator EndRaceRoutine()
        {
            //Enable the cinematic camera
            if (enableCinematicCameraAfterFinish)
            {
                if (cameraManager != null)
                {
                    cameraManager.EnableCinematicCamera();
                }
            }

            //Post race music
            if (musicPlayer != null && postRaceMusic != null)
            {
                musicPlayer.OverrideMusicClip(postRaceMusic, loopPostRaceMusic);
            }

            yield return new WaitForSeconds(endRaceDelay);

            //Switch to the post race state
            SwitchRaceState(RaceState.PostRace);

            //Begin the replay if auto start replay is set to true
            if (autoStartReplay && ReplayManager.instance != null)
            {
                ReplayManager.instance.WatchReplay();
            }
        }


        public void PlayerToAI()
        {
            if (playerStatistics == null)
                return;

            AiLogic AILogic = playerStatistics.GetComponent<AiLogic>();
            RCCAIInput aiInput = playerStatistics.GetComponent<RCCAIInput>();
            RCCPlayerInput playerInput = playerStatistics.GetComponent<RCCPlayerInput>();

            if (AILogic != null)
                AILogic.enabled = true;
            else
                playerStatistics.gameObject.AddComponent<AiLogic>();

            if (aiInput != null)
                aiInput.enabled = true;
            else
                playerStatistics.gameObject.AddComponent<RCCAIInput>();

            if (playerInput != null)
                playerInput.enabled = false;
        }


        public void AIToPlayer()
        {
            if (playerStatistics == null)
                return;

            AiLogic AILogic = playerStatistics.GetComponent<AiLogic>();
            RCCAIInput aiInput = playerStatistics.GetComponent<RCCAIInput>();
            RCCPlayerInput playerInput = playerStatistics.GetComponent<RCCPlayerInput>();

            if (AILogic != null)
                AILogic.enabled = false;

            if (aiInput != null)
                aiInput.enabled = false;

            if (playerInput != null)
                playerInput.enabled = true;
        }


        public void DisableRaceComponents(bool enable)
        {
            for (int i = 0; i < racerList.Count; i++)
            {
                //Toggle racer statistsics
                racerList[i].enabled = enable;

                //Toggle racer input
                AiLogic ai = racerList[i].GetComponent<AiLogic>();
                RCCAIInput aiInput = racerList[i].GetComponent<RCCAIInput>();
                RCCPlayerInput player = racerList[i].GetComponent<RCCPlayerInput>();

                if (ai != null)
                    ai.enabled = enable;

                if (aiInput != null)
                    aiInput.enabled = enable;

                if (player != null)
                    player.enabled = enable;
            }
        }


        void ForceFinish()
        {
            //Force a finish for all racers
            for (int i = 0; i < racerList.Count; i++)
            {
                if (!racerList[i].finished)
                {
                    racerList[i].FinishRace();
                }
            }
        }


        void DisqualifyUnfinishedRacers()
        {
            //DNF all unfinished racers
            for (int i = 0; i < racerList.Count; i++)
            {
                if (!racerList[i].finished)
                {
                    racerList[i].disqualified = true;
                    RegisterRacerFinish(racerList[i]);
                }
            }
        }


        public void CreateGhostVehicleManager()
        {
            GameObject gvm = new GameObject("Ghost Vehicle Manager");
            gvm.AddComponent<GhostVehicleManager>();
            gvm.GetComponent<GhostVehicleManager>().CreateGhostVehicle();
        }


        public void RespawnVehicle(Transform racer)
        {
            if (!raceStarted)
                return;

            Respawner respawner = racer.gameObject.GetComponent<Respawner>();
            if (respawner != null)
            {
                respawner.Respawn();
            }
        }


        void FixedUpdate()
        {
            // Логика ускорения / резинового пояса
            if (enableCatchup && racerList.Count > 1)
            {
                // Получить расстояние между гонщиками
                float distance = GetRacerInFirstPlace().totalDistance - GetRacerInLastPlace().totalDistance;

                // Сохранить ссылку на Rigidbody гонщика, занимающего первое место
                Rigidbody rigid = racerList[0].GetComponent<Rigidbody>();

                // Получить значение между 0 и 1 на основе расстояния
                float ratio = Mathf.InverseLerp(minCatchupRange, maxCatchupRange, distance);

                // Добавить обратную силу на основе отношения, чтобы замедлить гонщика в первом месте
                if (rigid != null)
                {
                    rigid.AddRelativeForce(Vector3.back * (ratio * (catchupStrength + 1)), ForceMode.Acceleration);
                }
            }
        }



        public void Pause()
        {
            SwitchRaceState(RaceState.Pause);
            Time.timeScale = 0.0f;
            AudioListener.volume = 0;
            if (musicPlayer != null)
            {
                musicPlayer.Pause();
            }
        }


        public void UnPause()
        {
            SwitchRaceState(RaceState.Race);
            Time.timeScale = 1.0f;
            AudioListener.volume = 1;
            if (musicPlayer != null)
            {
                musicPlayer.UnPause();
            }
        }


        public void SwitchRaceState(RaceState state)
        {
            previousRaceState = raceState;

            raceState = state;

            if (OnRaceStateChange != null)
            {
                OnRaceStateChange(state);
            }
        }


        public void CheckForBestLapTime(float lapTime)
        {
            string sceneName = GetCurrentScene();

            if (!PlayerPrefs.HasKey("BestTime" + "@" + sceneName))
            {
                SaveBestLapTime("BestTime", sceneName, lapTime);
            }
            else
            {
                float best = Helper.LoadBestLapTime("BestTime", sceneName);
                if (lapTime < best)
                {
                    SaveBestLapTime("BestTime", sceneName, lapTime);
                }
            }
        }


        void SaveBestLapTime(string key, string sceneName, float lapTime)
        {
            PlayerPrefs.SetFloat(key + "@" + sceneName, lapTime);

            if (RacePanel.instance != null)
            {
                RacePanel.instance.ShowRaceInfoMessage(bestTimeInfo);
            }
        }


        public float LoadOverallBestTimeForScene()
        {
            return Helper.LoadBestLapTime("BestTime", GetCurrentScene());
        }

        public void SetOpponentVehicles(List<GameObject> opponentVehicles)
        {
            if (opponentVehicles == null || opponentVehicles.Count == 0)
            {
                //Debug.LogWarning("Список машин соперников пуст.");
                return;
            }

            // Устанавливаем список машин соперников
            aiVehiclePrefabs = opponentVehicles;

            // Спавним AI
            SpawnAI();
        }


        public float GetTrackDistanceBetween(RacerStatistics to)
        {
            if (playerStatistics == null)
                return 0;

            return (playerStatistics.totalDistance - to.totalDistance);
        }


        public float GetDistanceBetween(RacerStatistics to)
        {
            if (playerStatistics == null)
                return 0;

            return Vector3.Distance(playerStatistics.transform.position, to.transform.position);
        }


        public float GetTimeGapBetween(RacerStatistics to)
        {
            if (playerStatistics == null)
                return 0;

            float dist = GetTrackDistanceBetween(to);
            float speed = (to.GetComponent<Rigidbody>().linearVelocity.magnitude + playerStatistics.GetComponent<Rigidbody>().linearVelocity.magnitude) / 2;
            speed = Mathf.Clamp(speed, 2, speed);

            return (dist / Mathf.Abs(speed));
        }


        public int GetTimeAttackPosition()
        {
            if (playerStatistics.totalRaceTime == 0)
                return 4;

            if (playerStatistics.totalRaceTime <= targetTimeGold)
                return 1;

            if (playerStatistics.totalRaceTime > targetTimeGold && playerStatistics.totalRaceTime <= targetTimeSilver)
                return 2;

            if (playerStatistics.totalRaceTime > targetTimeSilver && playerStatistics.totalRaceTime <= targetTimeBronze)
                return 3;

            return 4;
        }


        public int GetDriftRacePosition()
        {
            DriftPointsManager driftPoints = FindObjectOfType<DriftPointsManager>();

            if (driftPoints == null)
                return 4;

            if (driftPoints.totalDriftPoints == 0)
                return 4;

            if (driftPoints.totalDriftPoints >= targetScoreGold)
                return 1;

            if (driftPoints.totalDriftPoints < targetScoreGold && driftPoints.totalDriftPoints >= targetScoreSilver)
                return 2;

            if (driftPoints.totalDriftPoints < targetScoreSilver && driftPoints.totalDriftPoints >= targetScoreBronze)
                return 3;

            return 4;
        }


        AIDetial GetRandomAIDetails()
        {
            AIDetial temp = new AIDetial();
            temp.name = "AI " + aiNameIndex;
            temp.nationality = Nationality.Other;
            aiNameIndex++;

            if (aiDetailsList.Count > 0)
            {
                int i = UnityEngine.Random.Range(0, aiDetailsList.Count);
                temp = aiDetailsList[i];
                aiDetailsList.RemoveAt(i);
            }

            return temp;
        }


        bool AllRacersFinished()
        {
            bool finito = true;

            for (int i = 0; i < racerList.Count; i++)
            {
                if (!racerList[i].finished)
                    finito = false;
            }

            return finito;
        }


        public RacerStatistics GetRacerInFirstPlace()
        {
            return racerList[0];
        }


        public RacerStatistics GetRacerInLastPlace()
        {
            return racerList[activeRacerCount - 1];
        }


        public RacerStatistics GetRacerInPosition(int pos)
        {
            return racerList[pos - 1];
        }


        public string GetCurrentScene()
        {
            return SceneManager.GetActiveScene().name;
        }
    }

    [System.Serializable]
    public class CountdownInfo
    {
        public int countdownNumber;
        public AudioClip countdownSound;
    }


}