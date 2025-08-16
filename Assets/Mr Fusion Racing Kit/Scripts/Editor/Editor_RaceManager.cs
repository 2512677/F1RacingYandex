using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;
using Gley.TrafficSystem; // если нужно явно указывать неймспейс

// Кастомный редактор для компонента RaceManager
// Этот редактор позволяет настраивать параметры гонки, игрока и ИИ через вкладки в инспекторе Unity.
[CustomEditor(typeof(RaceManager))]
public class Editor_RaceManager : Editor
{
    // Ссылка на целевой объект RaceManager
    RaceManager _target;
    // Массив названий вкладок для переключения между разделами настроек
    string[] toolbarTabs = { "Настройки гонки", "Настройки игрока", "Настройки ИИ" };
    // Стиль для центрально выровненных заголовков
    GUIStyle centerLabelStyle;
    // Индекс выбранной вкладки
    int editorTab;

    // ======= Новое свойство для TrafficComponent =======
    SerializedProperty trafficComponent; // ссылка на TrafficComponent

    // Настройки гонки – перечисления (Enums)
    SerializedProperty raceType;                 // Тип гонки
    SerializedProperty startMode;                // Режим старта
    SerializedProperty playerGridPositioningMode;  // Режим позиционирования игрока на стартовой решётке
    SerializedProperty aiDifficultyLevel;        // Уровень сложности ИИ
    SerializedProperty raceEndTimerLogic;        // Логика таймера завершения гонки
    SerializedProperty speedUnit;                // Единицы измерения скорости

    // Погоня
    SerializedProperty maxSpikes;
    SerializedProperty maxRoadblocks;

    // Настройки игрока
    SerializedProperty playerVehiclePrefab;      // Префаб транспортного средства игрока
    SerializedProperty playerStartPosition;      // Стартовая позиция игрока
    SerializedProperty playerName;               // Имя игрока
    SerializedProperty playerNationality;        // Национальность игрока

    // Настройки ИИ
    SerializedProperty aiVehiclePrefabs;         // Префабы транспортных средств ИИ
    SerializedProperty aiDetails;                // Дополнительные настройки ИИ
    SerializedProperty easyAiDifficulty;         // Настройки сложности для лёгкого уровня
    SerializedProperty mediumAiDifficulty;       // Настройки сложности для среднего уровня
    SerializedProperty hardAiDifficulty;         // Настройки сложности для сложного уровня

    // Настройки миникарты / имён гонщиков
    SerializedProperty playerMinimapIcon;        // Иконка игрока на миникарте
    SerializedProperty opponentMinimapIcon;      // Иконка соперника на миникарте
    SerializedProperty racerName;                // Имя гонщика

    // Настройки гонки – числовые значения
    SerializedProperty loadRaceSettings;         // Флаг загрузки настроек гонки
    SerializedProperty lapCount;                 // Количество кругов
    SerializedProperty opponentCount;            // Количество соперников
    SerializedProperty checkpointTimeStart;      // Время контрольного пункта при старте
    SerializedProperty eliminationTimeStart;     // Время начала отсева
    SerializedProperty enduranceTimeStart;       // Время для гонок на выносливость
    SerializedProperty driftTimeStart;           // Время для дрифт-гонок
    SerializedProperty rollingStartSpeed;        // Скорость при разбеговом старте
    SerializedProperty useTimeLimit;             // Флаг использования ограничения по времени
    SerializedProperty postRaceSpeedMultiplier;  // Множитель скорости после гонки
    SerializedProperty autoStartRace;            // Флаг автоматического старта гонки
    SerializedProperty nonCollisionRace;         // Гонка без столкновений
    SerializedProperty postRaceCollisions;       // Столкновения после гонки
    SerializedProperty enableRaceEndTimer;       // Включить таймер завершения гонки
    SerializedProperty raceEndTimerStart;        // Время начала таймера завершения гонки
    SerializedProperty autoDriveTimeTrial;       // Автоматическое вождение для гонки на время
    SerializedProperty flyingStart;              // Флаг летящего старта
    SerializedProperty flyingStartSpeed;         // Скорость при летящем старте
    SerializedProperty finishEnduranceImmediately; // Флаг немедленного завершения гонки на выносливость
    SerializedProperty endRaceDelay;             // Задержка после завершения гонки
    SerializedProperty enableCinematicCameraAfterFinish; // Включить кинематографическую камеру после финиша
    SerializedProperty autoStartReplay;          // Автоматический запуск повтора
    SerializedProperty timeTrialStartPoint;      // Точка старта для гонок на время

    // Настройки обратного отсчета
    SerializedProperty countdownFrom;            // Начальное число обратного отсчета
    SerializedProperty countdownDelay;           // Задержка обратного отсчета
    SerializedProperty countdownAudio;           // Аудио для обратного отсчета
    SerializedProperty startMusicAfterCountdown; // Запуск музыки после обратного отсчета

    // Настройки догонялок
    SerializedProperty enableCatchup;            // Включить систему догонялок
    SerializedProperty catchupStrength;          // Сила догонялок
    SerializedProperty minCatchupRange;          // Минимальный диапазон догонялок
    SerializedProperty maxCatchupRange;          // Максимальный диапазон догонялок

    // Настройки штрафов
    SerializedProperty enableOfftrackPenalty;    // Включить штраф за выход за пределы трассы
    SerializedProperty minWheelCountForOfftrack; // Минимальное количество колес, необходимых для определения выхода за пределы
    SerializedProperty forceWrongwayRespawn;     // Принудительный респаун при движении в неправильном направлении
    SerializedProperty wrongwayRespawnTime;      // Время респауна при движении в неправильном направлении

    // Настройки респауна
    SerializedProperty respawnSettings;          // Параметры респауна

    // Настройки слипстрима
    SerializedProperty slipstreamSettings;       // Параметры слипстрима

    // Настройки дрифт-гонок
    SerializedProperty driftRaceSettings;        // Параметры дрифт-гонок

    // Настройки призрачного транспортного средства (Ghost)
    SerializedProperty enableGhostVehicle;       // Включить призрачное транспортное средство
    SerializedProperty ghostVehicleMaterial;     // Материал призрачного транспортного средства
    SerializedProperty ghostVehicleShader;       // Шейдер призрачного транспортного средства

    // Настройки целевого времени / счета
    SerializedProperty targetTimeGold;           // Целевое время для золота (Time Attack)
    SerializedProperty targetTimeSilver;         // Целевое время для серебра
    SerializedProperty targetTimeBronze;         // Целевое время для бронзы
    SerializedProperty targetScoreGold;          // Целевой счет для золота (для дрифта)
    SerializedProperty targetScoreSilver;        // Целевой счет для серебра
    SerializedProperty targetScoreBronze;        // Целевой счет для бронзы

    // Настройки музыки
    SerializedProperty postRaceMusic;            // Музыка после гонки
    SerializedProperty loopPostRaceMusic;        // Зациклить музыку после гонки

    // Настройки сообщений
    SerializedProperty showRaceInfoMessages;       // Отображать информационные сообщения гонки
    SerializedProperty showWhenRacerFinishes;        // Отображать сообщение, когда гонщик финиширует
    SerializedProperty showSplitTimes;             // Отображать промежуточные времена
    SerializedProperty showVehicleAheadAndBehindGap; // Отображать разницу между транспортными средствами впереди и сзади
    SerializedProperty addPositionToRaceEndMessage;  // Добавлять позицию в сообщение о завершении гонки
    SerializedProperty bestTimeInfo;               // Информация о лучшем времени
    SerializedProperty finalLapInfo;               // Информация о последнем круге
    SerializedProperty invalidLapInfo;             // Информация о невалидном круге
    SerializedProperty raceEndMessage;             // Сообщение о завершении гонки
    SerializedProperty lapKnockoutDisqualifyInfo;    // Информация о дисквалификации в гонках по кругам
    SerializedProperty checkpointDisqualifyInfo;     // Информация о дисквалификации по контрольному пункту
    SerializedProperty eliminationDisqualifyInfo;      // Информация о дисквалификации в отсевах
    SerializedProperty defaultDisqualifyInfo;         // Информация по умолчанию о дисквалификации

    // Метод OnEnable вызывается при инициализации редактора
    void OnEnable()
    {
        _target = (RaceManager)target;

        // Инициализируем стиль для центральных заголовков
        centerLabelStyle = new GUIStyle();
        centerLabelStyle.fontStyle = FontStyle.Normal;
        centerLabelStyle.normal.textColor = Color.white;
        centerLabelStyle.alignment = TextAnchor.MiddleCenter;

        // ======= Инициализируем новое свойство =======
        trafficComponent = serializedObject.FindProperty("trafficComponent");

        // Race Settings – перечисления
        raceType = serializedObject.FindProperty("raceType");
        startMode = serializedObject.FindProperty("startMode");
        playerGridPositioningMode = serializedObject.FindProperty("playerGridPositioningMode");
        aiDifficultyLevel = serializedObject.FindProperty("aiDifficultyLevel");
        raceEndTimerLogic = serializedObject.FindProperty("raceEndTimerLogic");
        speedUnit = serializedObject.FindProperty("speedUnit");

        // Настройка режим Погони
        maxSpikes = serializedObject.FindProperty("maxSpikes");
        maxRoadblocks = serializedObject.FindProperty("maxRoadblocks");

        // Настройки игрока
        playerVehiclePrefab = serializedObject.FindProperty("playerVehiclePrefab");
        playerStartPosition = serializedObject.FindProperty("playerStartPosition");
        playerName = serializedObject.FindProperty("playerName");
        playerNationality = serializedObject.FindProperty("playerNationality");

        // Настройки ИИ
        aiVehiclePrefabs = serializedObject.FindProperty("aiVehiclePrefabs");
        aiDetails = serializedObject.FindProperty("aiDetails");
        easyAiDifficulty = serializedObject.FindProperty("easyAiDifficulty");
        mediumAiDifficulty = serializedObject.FindProperty("mediumAiDifficulty");
        hardAiDifficulty = serializedObject.FindProperty("hardAiDifficulty");

        // Настройки миникарты / имён гонщиков
        playerMinimapIcon = serializedObject.FindProperty("playerMinimapIcon");
        opponentMinimapIcon = serializedObject.FindProperty("opponentMinimapIcon");
        racerName = serializedObject.FindProperty("racerName");

        // Race Settings – числовые значения
        loadRaceSettings = serializedObject.FindProperty("loadRaceSettings");
        lapCount = serializedObject.FindProperty("lapCount");
        opponentCount = serializedObject.FindProperty("opponentCount");
        checkpointTimeStart = serializedObject.FindProperty("checkpointTimeStart");
        eliminationTimeStart = serializedObject.FindProperty("eliminationTimeStart");
        enduranceTimeStart = serializedObject.FindProperty("enduranceTimeStart");
        driftTimeStart = serializedObject.FindProperty("driftTimeStart");
        rollingStartSpeed = serializedObject.FindProperty("rollingStartSpeed");
        raceEndTimerStart = serializedObject.FindProperty("raceEndTimerStart");
        flyingStartSpeed = serializedObject.FindProperty("flyingStartSpeed");
        endRaceDelay = serializedObject.FindProperty("endRaceDelay");
        postRaceSpeedMultiplier = serializedObject.FindProperty("postRaceSpeedMultiplier");
        autoStartRace = serializedObject.FindProperty("autoStartRace");
        nonCollisionRace = serializedObject.FindProperty("nonCollisionRace");
        postRaceCollisions = serializedObject.FindProperty("postRaceCollisions");
        enableRaceEndTimer = serializedObject.FindProperty("enableRaceEndTimer");
        useTimeLimit = serializedObject.FindProperty("useTimeLimit");
        autoDriveTimeTrial = serializedObject.FindProperty("autoDriveTimeTrial");
        flyingStart = serializedObject.FindProperty("flyingStart");
        finishEnduranceImmediately = serializedObject.FindProperty("finishEnduranceImmediately");
        enableCinematicCameraAfterFinish = serializedObject.FindProperty("enableCinematicCameraAfterFinish");
        autoStartReplay = serializedObject.FindProperty("autoStartReplay");
        timeTrialStartPoint = serializedObject.FindProperty("timeTrialStartPoint");

        // Настройки обратного отсчета
        countdownFrom = serializedObject.FindProperty("countdownFrom");
        countdownDelay = serializedObject.FindProperty("countdownDelay");
        countdownAudio = serializedObject.FindProperty("countdownAudio");
        startMusicAfterCountdown = serializedObject.FindProperty("startMusicAfterCountdown");

        // Настройки догонялок
        enableCatchup = serializedObject.FindProperty("enableCatchup");
        catchupStrength = serializedObject.FindProperty("catchupStrength");
        minCatchupRange = serializedObject.FindProperty("minCatchupRange");
        maxCatchupRange = serializedObject.FindProperty("maxCatchupRange");

        // Настройки штрафов
        enableOfftrackPenalty = serializedObject.FindProperty("enableOfftrackPenalty");
        minWheelCountForOfftrack = serializedObject.FindProperty("minWheelCountForOfftrack");
        forceWrongwayRespawn = serializedObject.FindProperty("forceWrongwayRespawn");
        wrongwayRespawnTime = serializedObject.FindProperty("wrongwayRespawnTime");

        // Настройки респауна
        respawnSettings = serializedObject.FindProperty("respawnSettings");

        // Настройки слипстрима
        slipstreamSettings = serializedObject.FindProperty("slipstreamSettings");

        // Настройки дрифт-гонок
        driftRaceSettings = serializedObject.FindProperty("driftRaceSettings");

        // Настройки призрачного транспортного средства
        enableGhostVehicle = serializedObject.FindProperty("enableGhostVehicle");
        ghostVehicleMaterial = serializedObject.FindProperty("ghostVehicleMaterial");
        ghostVehicleShader = serializedObject.FindProperty("ghostVehicleShader");

        // Настройки целевого времени/счёта
        targetTimeGold = serializedObject.FindProperty("targetTimeGold");
        targetTimeSilver = serializedObject.FindProperty("targetTimeSilver");
        targetTimeBronze = serializedObject.FindProperty("targetTimeBronze");
        targetScoreGold = serializedObject.FindProperty("targetScoreGold");
        targetScoreSilver = serializedObject.FindProperty("targetScoreSilver");
        targetScoreBronze = serializedObject.FindProperty("targetScoreBronze");

        // Настройки музыки
        postRaceMusic = serializedObject.FindProperty("postRaceMusic");
        loopPostRaceMusic = serializedObject.FindProperty("loopPostRaceMusic");

        // Настройки сообщений
        showRaceInfoMessages = serializedObject.FindProperty("showRaceInfoMessages");
        showWhenRacerFinishes = serializedObject.FindProperty("showWhenRacerFinishes");
        showSplitTimes = serializedObject.FindProperty("showSplitTimes");
        showVehicleAheadAndBehindGap = serializedObject.FindProperty("showVehicleAheadAndBehindGap");
        addPositionToRaceEndMessage = serializedObject.FindProperty("addPositionToRaceEndMessage");
        bestTimeInfo = serializedObject.FindProperty("bestTimeInfo");
        finalLapInfo = serializedObject.FindProperty("finalLapInfo");
        invalidLapInfo = serializedObject.FindProperty("invalidLapInfo");
        raceEndMessage = serializedObject.FindProperty("raceEndMessage");
        lapKnockoutDisqualifyInfo = serializedObject.FindProperty("lapKnockoutDisqualifyInfo");
        checkpointDisqualifyInfo = serializedObject.FindProperty("checkpointDisqualifyInfo");
        eliminationDisqualifyInfo = serializedObject.FindProperty("eliminationDisqualifyInfo");
        defaultDisqualifyInfo = serializedObject.FindProperty("defaultDisqualifyInfo");
    }

    // Основной метод отрисовки пользовательского интерфейса инспектора
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        // Отрисовка вкладок (панели инструментов) для переключения между разделами настроек
        editorTab = GUILayout.SelectionGrid(editorTab, toolbarTabs, 3);

        if (EditorGUI.EndChangeCheck())
        {
            GUI.FocusControl(null);
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // В зависимости от выбранной вкладки отображаем соответствующие настройки
        switch (editorTab)
        {
            // Настройки гонки
            case 0:
                EditorGUILayout.LabelField("Общие настройки гонки", centerLabelStyle);
                EditorGUILayout.Space();

                // === Здесь добавляем наше поле trafficComponent ===
                EditorGUILayout.PropertyField(trafficComponent);
                EditorGUILayout.Space();

                // --- Перечисления ---
                EditorGUILayout.PropertyField(raceType);
                EditorGUILayout.PropertyField(startMode);
                EditorGUILayout.PropertyField(raceEndTimerLogic);
                EditorGUILayout.PropertyField(speedUnit);

                // --- Значения ---
                if (_target.raceType == RaceType.LapKnockout || _target.raceType == RaceType.Endurance ||
                    _target.raceType == RaceType.Drag || _target.raceType == RaceType.Drift ||
                    _target.raceType == RaceType.TimeTrial || _target.raceType == RaceType.TimeAttack)
                {
                    EditorGUILayout.HelpBox("Этот тип гонки отменяет подсчет кругов и/или количество соперников.", MessageType.Info);
                }

                EditorGUILayout.PropertyField(lapCount);
                EditorGUILayout.PropertyField(opponentCount);

                GUILayout.Space(10);
                EditorGUILayout.PropertyField(checkpointTimeStart);
                EditorGUILayout.PropertyField(eliminationTimeStart);
                EditorGUILayout.PropertyField(enduranceTimeStart);
                EditorGUILayout.PropertyField(driftTimeStart);
                EditorGUILayout.PropertyField(rollingStartSpeed);
                EditorGUILayout.PropertyField(raceEndTimerStart);
                EditorGUILayout.PropertyField(flyingStartSpeed);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Погоня", centerLabelStyle);
                EditorGUILayout.PropertyField(maxSpikes);
                EditorGUILayout.PropertyField(maxRoadblocks);

                GUILayout.Space(10);
                EditorGUILayout.PropertyField(loadRaceSettings);
                EditorGUILayout.PropertyField(autoStartRace);
                EditorGUILayout.PropertyField(useTimeLimit);
                EditorGUILayout.PropertyField(autoDriveTimeTrial);
                EditorGUILayout.PropertyField(nonCollisionRace);
                EditorGUILayout.PropertyField(finishEnduranceImmediately);
                EditorGUILayout.PropertyField(enableRaceEndTimer);
                EditorGUILayout.PropertyField(flyingStart);

                EditorGUILayout.PropertyField(timeTrialStartPoint);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                // --- Настройки обратного отсчета ---
                EditorGUILayout.LabelField("Настройки обратного отсчета", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(countdownFrom);
                EditorGUILayout.PropertyField(countdownDelay);
                EditorGUILayout.PropertyField(countdownAudio, true);
                EditorGUILayout.PropertyField(startMusicAfterCountdown);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                // --- Настройки значков на миникарте / имен гонщиков ---
                EditorGUILayout.LabelField("Настройки значков на миникарте / имен гонщиков", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(playerMinimapIcon);
                EditorGUILayout.PropertyField(opponentMinimapIcon);
                EditorGUILayout.PropertyField(racerName);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                // --- Настройки догонялок ---
                EditorGUILayout.LabelField("Настройки догонялок", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(enableCatchup);
                EditorGUILayout.PropertyField(catchupStrength);
                EditorGUILayout.PropertyField(minCatchupRange);
                EditorGUILayout.PropertyField(maxCatchupRange);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                // --- Настройки штрафов ---
                EditorGUILayout.LabelField("Настройки штрафов", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(enableOfftrackPenalty);
                EditorGUILayout.PropertyField(forceWrongwayRespawn);
                EditorGUILayout.PropertyField(minWheelCountForOfftrack);
                EditorGUILayout.PropertyField(wrongwayRespawnTime);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                // --- Настройки респауна ---
                EditorGUILayout.LabelField("Параметры респауна", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(respawnSettings, true);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                // --- Настройки слипстрима ---
                EditorGUILayout.LabelField("Настройки слипстрима", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(slipstreamSettings, true);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                // --- Настройки дрифт-гонок ---
                EditorGUILayout.LabelField("Настройки дрифт-гонок", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(driftRaceSettings, true);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                // --- Настройки призрачного транспортного средства ---
                EditorGUILayout.LabelField("Настройки призрачного транспортного средства", centerLabelStyle);
                EditorGUILayout.Space();

                if (_target.ghostVehicleShader != null && _target.ghostVehicleMaterial != null)
                {
                    EditorGUILayout.HelpBox("Назначены как материал, так и шейдер призрачного транспортного средства. Пожалуйста, назначьте только один из них.", MessageType.Warning);
                }

                EditorGUILayout.PropertyField(enableGhostVehicle);
                EditorGUILayout.PropertyField(ghostVehicleMaterial);
                EditorGUILayout.PropertyField(ghostVehicleShader);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                // --- Настройки целевого времени / счета ---
                EditorGUILayout.LabelField("Настройки целевого времени / счета", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.HelpBox("В гонках типа Time Attack используются целевые времена.", MessageType.Info);
                EditorGUILayout.PropertyField(targetTimeGold);
                EditorGUILayout.PropertyField(targetTimeSilver);
                EditorGUILayout.PropertyField(targetTimeBronze);

                EditorGUILayout.HelpBox("В гонках типа «Дрифт» используются целевые баллы.", MessageType.Info);
                EditorGUILayout.PropertyField(targetScoreGold);
                EditorGUILayout.PropertyField(targetScoreSilver);
                EditorGUILayout.PropertyField(targetScoreBronze);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                // --- Настройки после гонки ---
                EditorGUILayout.LabelField("Настройки после гонки", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(endRaceDelay);
                EditorGUILayout.PropertyField(postRaceSpeedMultiplier);
                EditorGUILayout.PropertyField(postRaceMusic);
                EditorGUILayout.PropertyField(enableCinematicCameraAfterFinish);
                EditorGUILayout.PropertyField(autoStartReplay);
                EditorGUILayout.PropertyField(postRaceCollisions);
                EditorGUILayout.PropertyField(loopPostRaceMusic);
                break;

            // Настройки игрока
            case 1:
                EditorGUILayout.LabelField("Настройки игрока", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(playerVehiclePrefab);
                EditorGUILayout.PropertyField(playerName);
                EditorGUILayout.PropertyField(playerNationality);
                EditorGUILayout.PropertyField(playerGridPositioningMode);
                if (_target.playerGridPositioningMode == GridPositioningMode.Select)
                {
                    EditorGUILayout.PropertyField(playerStartPosition);
                }
                break;

            // Настройки ИИ
            case 2:
                EditorGUILayout.LabelField("Настройки ИИ", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(aiVehiclePrefabs, true);
                EditorGUILayout.PropertyField(aiDetails);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                EditorGUILayout.LabelField("Настройки сложности ИИ", centerLabelStyle);
                EditorGUILayout.PropertyField(aiDifficultyLevel);
                EditorGUILayout.PropertyField(easyAiDifficulty);
                EditorGUILayout.PropertyField(mediumAiDifficulty);
                EditorGUILayout.PropertyField(hardAiDifficulty);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
