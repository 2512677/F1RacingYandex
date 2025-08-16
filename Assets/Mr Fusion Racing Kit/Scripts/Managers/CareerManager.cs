// =============================
//  CareerManager.cs (полный, 599 строк)
//  Добавлены поля championButton, championLocked и метод OnChampionButtonClicked()
// =============================

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
//using Firebase.Analytics;
//using UnityEditor.SceneManagement;

namespace RGSK
{
    /// <summary>
    /// Способы разблокировки события:
    ///  - ByFullCompletion: только при 100% прохождении предыдущего
    ///  - BySpeedBoost:     только за speedBoost
    ///  - Both:             либо 100%, либо speedBoost
    /// </summary>
    public enum UnlockMethod
    {
        ByFullCompletion,
        BySpeedBoost,
        Both
    }

    public class CareerManager : MonoBehaviour
    {
        public static CareerManager instance;

        [Header("Career Stages")]
        public List<CareerStage> careerStages = new List<CareerStage>();
        public int currentStageIndex = 0; // <-- возможно используется, не удаляем

        [Header("Popup Window")]
        public GameObject popupWindow; // Ссылка на панель (Canvas) для сообщений

        // ADDED // Новое поле для подстановки только числа (например, "100%", "300 SPB").
        [Header("Dynamic Value Text")]
        public Text dynamicValueText;  // <-- скрипт будет подставлять сюда чистое число

        // ADDED // ChampionButton, чтобы кликом запускать/блокировать чемпионат
        [Header("Champion Button")]
        public Button championButton;     // <-- Привяжите в инспекторе (если нужно)
        public bool championLocked = true; // <-- true = заблокирован

        private RaceRewards.Rewards pendingRewards;

        private void Awake()
        {
            // Проверяем, существует ли экземпляр CareerManager
            if (instance != null && instance != this)
            {
                Debug.Log("[CareerManager] Уничтожение дубликата CareerManager.");
                Destroy(gameObject); // Уничтожаем дубликат
                return;
            }

            instance = this; // Сохраняем текущий экземпляр
            DontDestroyOnLoad(gameObject); // Делаем объект постоянным между сценами

            // ADDED // Привяжем обработчик к championButton, если он задан
            if (championButton != null)
            {
                championButton.onClick.AddListener(OnChampionButtonClicked);
            }
        }

        /// <summary>
        /// Показать всплывающее окно с сообщением msg (как было у вас).
        /// Ищет компонент Text внутри popupWindow и задаёт ему текст msg.
        /// (Вы можете им тоже пользоваться, если нужно.)
        /// </summary>
        public void ShowPopupMessage(string msg)
        {
            if (popupWindow == null)
            {
                Debug.LogWarning("[CareerManager] popupWindow не назначен в инспекторе!");
                return;
            }

            // 1) Включаем окно
            popupWindow.SetActive(true);

            // 2) Находим в нем Text-объект и ставим msg
            Text txt = popupWindow.GetComponentInChildren<Text>();
            if (txt != null)
                txt.text = msg;
            else
                Debug.LogWarning("[CareerManager] Не найден компонент Text в дочерних объектах popupWindow!");
        }

        // NEW METHOD: Показать в popup-е **только** число/строку (например "100%", "300 SPB"),
        //             а основной текст ("Чтобы разблокировать...") вы сами прописываете в другом Text или статически в окне.
        public void ShowNeededValue(string neededValue)
        {
            if (popupWindow == null)
            {
                Debug.LogWarning("[CareerManager] popupWindow не назначен в инспекторе!");
                return;
            }

            // Включаем окно
            popupWindow.SetActive(true);

            // Если в Inspector вы привязали отдельный Text (dynamicValueText) — подставляем туда нужное значение
            if (dynamicValueText != null)
            {
                dynamicValueText.text = neededValue;
            }
            else
            {
                Debug.LogWarning("[CareerManager] dynamicValueText не назначен! Некуда подставить число.");
            }
        }

        private void Start()
        {
            // Загружаем данные игрока перед использованием
            PlayerData.instance.LoadData();

            Debug.Log($"Загруженные разблокированные уровни: {string.Join(", ", PlayerData.instance.playerData.unlockedStages)}");

            foreach (CareerStage stage in careerStages)
            {
                // Инициализация кнопок и обновление состояния этапа
                stage.InitializeButton(this);

                // Обновляем прогресс гонок для каждого этапа
                stage.UpdateRaceProgress();

                // Обновляем текст стоимости этапа, если он есть
                stage.UpdatePriceText();

                // Проверяем, есть ли текст прогресса, и выводим в лог текущее значение
                if (stage.progressText != null)
                {
                    Debug.Log($"Прогресс для этапа {stage.stageName}: {stage.progressText.text}");
                }
            }

            Debug.Log("Инициализация этапов завершена.");

            // ADDED // Если чемпионат заблокирован, отключим или спрячем кнопку
            if (championButton != null)
            {
                championButton.gameObject.SetActive(!championLocked);
            }
        }

        // ADDED //
        private void OnChampionButtonClicked()
        {
            if (championLocked)
            {
                // Показываем окно, что чемпионат закрыт
                ShowPopupMessage("Чемпионат ещё заблокирован!");
            }
            else
            {
                // Иначе запускаем какую-то логику чемпионата
                ShowPopupMessage("Запуск Чемпионата! (пример)");
                // Или SceneManager.LoadScene("ChampionshipScene");
            }
        }

        public void UpdateEventUI()
        {
            Debug.Log("Обновление интерфейса событий начато.");

            foreach (CareerStage stage in careerStages)
            {
                // Старая логика блокировки/разблокировки по unlockedStages:
                if (PlayerData.instance.playerData.unlockedStages.Contains(stage.stageName))
                {
                    stage.isLocked = false;
                    if (stage.lockObject != null)
                        stage.lockObject.SetActive(false);
                }
                else
                {
                    stage.isLocked = true;
                    if (stage.lockObject != null)
                        stage.lockObject.SetActive(true);
                }

                stage.UpdateRaceProgress();

                if (stage.isLocked && stage.lockObject != null)
                {
                    stage.lockObject.SetActive(true);
                }
                else if (!stage.isLocked && stage.lockObject != null)
                {
                    stage.lockObject.SetActive(false);
                }
            }

            Debug.Log("Обновление интерфейса событий завершено.");
        }

        public void LoadCareerStage(int stageIndex)
        {
            // Проверяем валидность индекса
            if (stageIndex < 0 || stageIndex >= careerStages.Count)
            {
                Debug.LogError($"CareerManager: Stage with index {stageIndex} does not exist.");
                return;
            }

            // Сохраняем и получаем текущий этап
            currentStageIndex = stageIndex;
            CareerStage currentStage = GetCurrentStage();
            if (currentStage == null)
            {
                Debug.LogError("CareerManager: Current stage is null!");
                return;
            }

            Debug.Log($"CareerManager: Loading stage {currentStage.stageName}.");

            // Показываем панели без логов в Firebase
            TogglePanels(currentStage.mainCareerPanel, currentStage.eventPanel);

            // Загружаем события этапа
            LoadCareerEvents(currentStage);
        }


        public void LoadCareerEvents(CareerStage stage)
        {
            if (MenuCareerPanel.Instance != null)
            {
                MenuCareerPanel.Instance.LoadCareerEvents(stage.careerData);
            }
            else
            {
                Debug.LogError("CareerManager: MenuCareerPanel.Instance not found!");
            }
        }

        private void OnApplicationQuit()
        {
            PlayerData.instance.SaveData();
            Debug.Log("CareerManager: Данные сохранены при выходе из игры.");
        }

        public void StartRace(int roundIndex)
        {
            var currentStage = GetCurrentStage();

            if (currentStage == null || currentStage.careerData == null)
            {
                Debug.LogError("CareerManager: Current stage or career data is missing!");
                return;
            }

            if (roundIndex < 0 || roundIndex >= currentStage.careerData.careerRounds.Count)
            {
                Debug.LogError("CareerManager: Invalid round index!");
                return;
            }

            var round = currentStage.careerData.careerRounds[roundIndex];

            if (round.raceRewards == null || round.raceRewards.Count == 0)
            {
                Debug.LogError("CareerManager: No rewards defined for this round!");
                return;
            }

            var rewardsArray = round.raceRewards.ToArray();
            RaceRewards.Instance?.SetRewards(rewardsArray);

            // Сохраняем настройки гонки
            PlayerPrefs.SetInt("RaceType", (int)round.raceType);
            PlayerPrefs.SetInt("LapCount", round.laps);
            PlayerPrefs.SetInt("OpponentCount", round.opponentCount);

            // ВАЖНО: сохраняем raceID, чтобы потом RaceManager мог взять именно его
            PlayerPrefs.SetString("CurrentRaceID", round.raceID);
            PlayerPrefs.Save();

            // Подписываемся на событие загрузки сцены и загружаем саму трассу
            SceneManager.sceneLoaded += OnRaceSceneLoaded;
            SceneManager.LoadScene(round.trackData.trackName);
        }

        private void OnRaceSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("CareerManager: Race scene loaded. Rewards will be displayed based on player position.");
            SceneManager.sceneLoaded -= OnRaceSceneLoaded;
        }

        public void UnlockStageWithPurchase(int stageIndex)
        {
            if (stageIndex < 0 || stageIndex >= careerStages.Count)
            {
                Debug.LogError("CareerManager: Некорректный индекс уровня.");
                return;
            }

            CareerStage stage = careerStages[stageIndex];
            // Пример вызова: stage.TryUnlockStage() — если оставили старую логику
            // Однако здесь убрано/закомментировано.

            SaveProgress();
            Debug.Log($"Событие {stage.stageName} разблокировано. Обновляем UI.");
            UpdateEventUI();
        }

        public void CompleteRace(string raceID, int playerPosition)
        {
            if (playerPosition != 1) return;

            if (!PlayerData.instance.playerData.completedRaces.Contains(raceID))
            {
                PlayerData.instance.playerData.completedRaces.Add(raceID);
                PlayerData.instance.SaveData();
            }

            CareerStage currentStage = GetCurrentStage();
            if (currentStage != null)
            {
                currentStage.UpdateRaceProgress();
            }
        }

        private bool AllRacesWithFirstPlace()
        {
            CareerStage currentStage = GetCurrentStage();
            if (currentStage == null)
            {
                Debug.LogError("CareerManager: Current stage is null!");
                return false;
            }

            Debug.Log("CareerManager: All races completed with 1st place!");
            return true;
        }

        private void SaveProgress()
        {
            string progressKey = $"Stage_{currentStageIndex}_RacesCompleted";
            PlayerData.instance.playerData.items.RemoveAll(item => item.StartsWith(progressKey));
            PlayerData.instance.SaveData();
        }

        /// <summary>
        /// Разблокировать следующий этап (по имени), если текущее событие прошло на 100%.
        /// </summary>
        public void TryUnlockNextStageByFullCompletion(string nextStageName)
        {
            if (string.IsNullOrEmpty(nextStageName))
            {
                Debug.LogWarning($"[CareerManager] nextStageName не задан, не можем разблокировать следующий этап.");
                return;
            }

            CareerStage nextStage = careerStages.Find(st => st.stageName == nextStageName);
            if (nextStage == null)
            {
                Debug.LogWarning($"[CareerManager] Событие с именем {nextStageName} не найдено.");
                return;
            }

            if (!nextStage.isLocked)
            {
                Debug.Log($"[CareerManager] Событие {nextStageName} уже разблокировано!");
                return;
            }

            nextStage.isLocked = false;
            if (nextStage.lockObject != null)
                nextStage.lockObject.SetActive(false);

            PlayerData.instance.UnlockStage(nextStageName);
            PlayerData.instance.SaveData();

            Debug.Log($"[CareerManager] Автоматически разблокирован {nextStageName}, т.к. предыдущее событие пройдено на 100%.");
            UpdateEventUI();
        }

        private void UnlockNextStage()
        {
            if (currentStageIndex + 1 < careerStages.Count)
            {
                CareerStage nextStage = careerStages[currentStageIndex + 1];
                if (nextStage.isLocked)
                {
                    nextStage.isLocked = false;
                    if (nextStage.lockObject != null)
                    {
                        nextStage.lockObject.SetActive(false);
                        Debug.Log($"CareerManager: Unlocked next stage: {nextStage.stageName}");
                    }
                    else
                    {
                        Debug.LogWarning($"CareerManager: Lock object is missing for stage {nextStage.stageName}!");
                    }
                }
            }
            else
            {
                Debug.LogWarning("CareerManager: No more stages to unlock.");
            }
        }

        private void TogglePanels(GameObject currentPanel, GameObject nextPanel)
        {
            if (currentPanel != null)
                currentPanel.SetActive(false);

            if (nextPanel != null)
                nextPanel.SetActive(true);
        }

        public CareerStage GetCurrentStage()
        {
            return careerStages.Count > currentStageIndex ? careerStages[currentStageIndex] : null;
        }
    }


    [System.Serializable]
    public class CareerStage
    {
        public string stageName;
        public bool isLocked = true;
        public CareerData careerData;


        public Button stageButton;
        public GameObject lockObject;
        public GameObject mainCareerPanel;
        public GameObject eventPanel;

        public Text priceText;
        public Text progressText; // Ссылка на текстовый элемент UI

        // Новые поля для гибкой разблокировки
        [Header("Настройки разблокировки")]
        public UnlockMethod unlockMethod;
        public float speedBoostCost;
        public string nextStageName;
        [HideInInspector] public bool isCompleted;

        private int cachedProgress;

        // Новые поля для ChampionButton
        [Header("Champion Settings")]
        public Button championButton;       // Кнопка для чемпионата
        public bool championLocked = true;  // Флаг блокировки чемпионата

        public void UpdateRaceProgress()
        {
            int totalRaces = careerData != null ? careerData.careerRounds.Count : 0;
            int completedRaces = 0;

            if (careerData != null)
            {
                foreach (var round in careerData.careerRounds)
                {
                    if (PlayerData.instance.playerData.completedRaces.Contains(round.raceID))
                    {
                        completedRaces++;
                    }
                }
            }

            int progressPercentage = totalRaces > 0 ? (completedRaces * 100) / totalRaces : 0;
            cachedProgress = progressPercentage;

            if (progressText != null)
            {
                progressText.text = $"{progressPercentage}%";
            }

            Debug.Log($"[UpdateRaceProgress] Завершенные гонки: {completedRaces}/{totalRaces} ({progressPercentage}%) для события {stageName}");

            // Определяем, пройдено ли событие на 100%
            isCompleted = (progressPercentage >= 100);

            if (isCompleted && (unlockMethod == UnlockMethod.ByFullCompletion || unlockMethod == UnlockMethod.Both))
            {
                CareerManager.instance.TryUnlockNextStageByFullCompletion(nextStageName);
            }
        }

        private int GetProgressPercentage()
        {
            return cachedProgress;
        }

        public void InitializeButton(CareerManager careerManager)
        {
            if (stageButton != null)
            {
                stageButton.onClick.AddListener(() => OnStageButtonClick(careerManager));
            }

            if (lockObject != null)
            {
                lockObject.SetActive(isLocked);
            }

            // Инициализация ChampionButton, если он задан
            if (championButton != null)
            {
                championButton.onClick.AddListener(OnChampionButtonClicked);
                championButton.gameObject.SetActive(!championLocked);
            }

            UpdateRaceProgress();
            UpdatePriceText();
        }

        private void OnStageButtonClick(CareerManager careerManager)
        {
            if (isLocked)
            {
                // 1) Проверяем, что предыдущий сезон завершён:
                if (!IsPreviousStageCompleted())
                {
                    CareerManager.instance.ShowPopupMessage("Сначала завершите предыдущий сезон!");
                    return;
                }

                // 2) Дальше идёт ваша логика разблокировки по unlockMethod:
                int currentSPB = PlayerData.instance.playerData.speedBoost;
                int prog = GetProgressPercentage();

                switch (unlockMethod)
                {
                    case UnlockMethod.ByFullCompletion:
                        if (prog < 100)
                        {
                            int neededPercent = 100 - prog;
                            CareerManager.instance.ShowNeededValue($"{neededPercent}%");
                        }
                        else
                        {
                            CareerManager.instance.ShowNeededValue($"Уже {prog}%, странно!");
                        }
                        break;

                    case UnlockMethod.BySpeedBoost:
                        if (currentSPB >= speedBoostCost)
                        {
                            PlayerData.instance.AddSPB(-(int)speedBoostCost);
                            isLocked = false;
                            if (lockObject != null)
                                lockObject.SetActive(false);

                            PlayerData.instance.UnlockStage(stageName);
                            Debug.Log($"CareerStage: {stageName} успешно разблокирован за {speedBoostCost} SPB.");
                            PlayerData.instance.SaveData();

                            UpdatePriceText();
                            careerManager.LoadCareerStage(careerManager.careerStages.IndexOf(this));
                        }
                        else
                        {
                            int lackSPB = (int)(speedBoostCost - currentSPB);
                            CareerManager.instance.ShowNeededValue($"{lackSPB} SPB");
                        }
                        break;

                    case UnlockMethod.Both:
                        bool canUnlockByPercent = (prog >= 100);
                        bool canUnlockBySPB = (currentSPB >= speedBoostCost);

                        if (!canUnlockByPercent && !canUnlockBySPB)
                        {
                            int lackPercent = 100 - prog;
                            int lackSPB2 = (int)(speedBoostCost - currentSPB);
                            CareerManager.instance.ShowNeededValue($"{lackPercent}% / {lackSPB2} SPB");
                        }
                        else
                        {
                            if (canUnlockBySPB)
                            {
                                PlayerData.instance.AddSPB(-(int)speedBoostCost);
                                Debug.Log($"CareerStage: {stageName} разблокирован за {speedBoostCost} SPB.");
                            }
                            else
                            {
                                Debug.Log($"CareerStage: {stageName} разблокирован при 100% прогрессе!");
                            }

                            isLocked = false;
                            if (lockObject != null)
                                lockObject.SetActive(false);

                            PlayerData.instance.UnlockStage(stageName);
                            PlayerData.instance.SaveData();

                            UpdatePriceText();
                            careerManager.LoadCareerStage(careerManager.careerStages.IndexOf(this));
                        }
                        break;
                }

                return;
            }

            // Если не заблокирован
            careerManager.LoadCareerStage(careerManager.careerStages.IndexOf(this));
        }

        /// <summary>
        /// Проверяем, что предыдущий сезон (по индексу в careerStages) уже завершён.
        /// Например, требуем: он не заблокирован и isCompleted == true.
        /// </summary>
        private bool IsPreviousStageCompleted()
        {
            int myIndex = CareerManager.instance.careerStages.IndexOf(this);

            // Если это самый первый сезон (индекс 0), предыдущего нет — по желанию можно сразу вернуть true
            if (myIndex <= 0)
                return true;

            var prevStage = CareerManager.instance.careerStages[myIndex - 1];

            // Требуем, чтобы предыдущий сезон уже не был заблокирован и считался завершённым (progress 100%)
            // При необходимости поменяйте логику на ту, что подходит вашему проекту.
            return !prevStage.isLocked && prevStage.isCompleted;
        }


        /// <summary>
        /// Обработчик нажатия на ChampionButton.
        /// </summary>
        private void OnChampionButtonClicked()
        {
            if (championLocked)
            {
                // Выводим сообщение о блокировке чемпионата
                CareerManager.instance.ShowPopupMessage("Чемпионат для этого события ещё заблокирован!");
            }
            else
            {
                // Запускаем логику чемпионата (можно заменить на загрузку сцены или другую функциональность)
                CareerManager.instance.ShowPopupMessage($"Запуск Чемпионата для события: {stageName}");
                // Пример: SceneManager.LoadScene("ChampionshipScene");
            }
        }

        public void UpdatePriceText()
        {
            if (priceText != null)
            {
                if (!isLocked)
                {
                    priceText.text = "";
                }
                else
                {
                    switch (unlockMethod)
                    {
                        case UnlockMethod.ByFullCompletion:
                            priceText.text = "100%";
                            break;
                        case UnlockMethod.BySpeedBoost:
                            priceText.text = $"{speedBoostCost} SPEED BOOST";
                            break;
                        case UnlockMethod.Both:
                            priceText.text = $"{speedBoostCost} SPEED BOOST |  100%";
                            break;
                    }
                }
            }
        }
    }
}
