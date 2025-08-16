using UnityEngine;
using UnityEngine.UI;
using I2.Loc;
using System;   // для Array.FindIndex и StringComparison

namespace RGSK
{
    /// <summary>
    /// Управление последовательностью окон после гонки:
    /// RaceResults → ChampionshipResults (если есть) → RaceRewards → CarReward → Menu
    /// </summary>
    public class PostRacePanel : MonoBehaviour
    {
        private RaceResultsPanel raceResultsPanel;
        private ChampionshipResultsPanel championshipResultsPanel;
        private OtherResultsPanel otherRaceResultsPanel;
        private RaceRewardsPanel raceRewardsPanel;
        private CarRewardPanel carRewardPanel;
        private ChaseResultPanel chaseResultPanel;

        public Text raceEndTimer;
        private string currentPanel;

        // ✱ Новые поля для логики показа CarReward только один раз
        private string carRewardID;
        private bool carWasUnlockedAtStart;

        [Header("Buttons")]
        public Button continueButton;
        public Button watchReplayButton;
        public Button restartRaceButton;

        [Header("Continue Settings (Optional)")]
        [Tooltip("Если true, при нажатии на кнопку Continue переключаем физику на указанный профиль RCC v4.")]
        public bool requireBehaviorTypeOnContinue = false;

        [Tooltip("Имя профиля поведения из RCC_Settings.behaviorTypes (например: \"Simulator\", \"Arcade\", и т.п.).")]
        public string continueBehaviorName = "Simulator";

        void Start()
        {
            // подписываем кнопки и собираем дочерние панели
            AddButtonListeners();
            FindPanels();

            // если режим CHASE → открываем ChaseResult
            if (RaceManager.instance &&
                RaceManager.instance.raceType == RaceType.Chase)
            {
                ShowPanel("ChaseResult");        // ← исправлено имя панели
            }
            else
            {
                ShowPanel("RaceResults");
            }

            // обнуляем таймер
            if (raceEndTimer != null)
                raceEndTimer.text = string.Empty;
        }

        void FindPanels()
        {
            raceResultsPanel = GetComponentInChildren<RaceResultsPanel>(true);
            championshipResultsPanel = GetComponentInChildren<ChampionshipResultsPanel>(true);
            otherRaceResultsPanel = GetComponentInChildren<OtherResultsPanel>(true);
            raceRewardsPanel = GetComponentInChildren<RaceRewardsPanel>(true);
            chaseResultPanel = GetComponentInChildren<ChaseResultPanel>(true);
            carRewardPanel = GetComponentInChildren<CarRewardPanel>(true);
        }

        void Update()
        {
            if (RaceManager.instance && raceEndTimer != null)
            {
                if (RaceManager.instance.endRaceTimerStarted && !RaceManager.instance.raceFinished)
                {
                    string localized = LocalizationManager.GetTranslation("RaceMessages/RaceEndTimer");
                    if (string.IsNullOrEmpty(localized)) localized = "Race End: ";
                    raceEndTimer.text = localized + (int)RaceManager.instance.raceEndTimer;
                }
                else
                {
                    raceEndTimer.text = string.Empty;
                }
            }
        }

        /// <summary>
        /// Обработка нажатия Continue: переключение между этапами.
        /// </summary>
        void Continue()
        {
            // 1) Переключаем профиль поведения RCC, если нужно (RCC v4)
            if (requireBehaviorTypeOnContinue)
            {
                var set = RCC_Settings.Instance;
                if (set != null && set.behaviorTypes != null && set.behaviorTypes.Length > 0)
                {
                    int idx = Array.FindIndex(set.behaviorTypes, b =>
                        b != null && b.behaviorName.Equals(continueBehaviorName, StringComparison.OrdinalIgnoreCase));

                    if (idx < 0) idx = 0; // запасной вариант

                    set.overrideBehavior = true;
                    set.behaviorSelectedIndex = idx;

                    // сохраним индекс для совместимости со старой логикой
                    PlayerPrefs.SetInt("BehaviorType", idx);
                    PlayerPrefs.Save();
                }
            }

            // 2) Собираем reward.items и определяем флаг наличия машины
            string[] items = null;
            var rewards = RaceRewards.Instance.currentRewards;
            if (rewards != null && RaceManager.instance != null)
            {
                int pos = RaceManager.instance.playerStatistics.Position;
                if (RaceManager.instance.raceType == RaceType.TimeAttack)
                    pos = RaceManager.instance.GetTimeAttackPosition();
                else if (RaceManager.instance.raceType == RaceType.Drift)
                    pos = RaceManager.instance.GetDriftRacePosition();

                if (pos >= 1 && pos <= rewards.Length)
                    items = rewards[pos - 1].items;
            }
            bool hasCarReward = items != null && items.Length > 0;

            // 2a) Запоминаем состояние разблокировки ДО старта панели наград
            if (currentPanel == "RaceResults" && hasCarReward)
            {
                carRewardID = items[0];
                carWasUnlockedAtStart = PlayerData.instance.IsItemUnlocked(carRewardID);
            }

            // 3) Переключаем панели
            switch (currentPanel)
            {
                case "RaceResults":
                    if (ChampionshipManager.instance != null)
                    {
                        RaceManager.instance.UpdateChampionshipPositions();
                        ShowPanel("ChampionshipResults");
                    }
                    else if (raceRewardsPanel != null)
                    {
                        ShowPanel("RaceRewards");
                    }
                    else
                    {
                        LoadMenuScene();
                    }
                    break;

                case "ChampionshipResults":
                    if (!ChampionshipManager.instance.IsFinalRound())
                    {
                        ChampionshipManager.instance.LoadNextRound();
                    }
                    else if (raceRewardsPanel != null)
                    {
                        ShowPanel("RaceRewards");
                    }
                    else
                    {
                        LoadMenuScene();
                    }
                    break;

                case "RaceRewards":
                    // Показываем CarReward только если машина не была уже разблокирована
                    if (hasCarReward && !carWasUnlockedAtStart && carRewardPanel != null)
                    {
                        carRewardPanel.Show(carRewardID);
                        currentPanel = "CarReward";
                    }
                    else
                    {
                        LoadMenuScene();
                    }
                    break;

                case "ChaseResult":
                    if (raceRewardsPanel != null)
                    {
                        ShowPanel("RaceRewards");
                    }
                    else
                    {
                        LoadMenuScene();
                    }
                    break;

                case "CarReward":
                    LoadMenuScene();
                    break;
            }
        }

        /// <summary>
        /// Активирует указанную панель, скрывая все остальные.
        /// </summary>
        void ShowPanel(string panel)
        {
            currentPanel = panel;
            raceResultsPanel?.gameObject.SetActive(false);
            otherRaceResultsPanel?.gameObject.SetActive(false);
            championshipResultsPanel?.gameObject.SetActive(false);
            raceRewardsPanel?.gameObject.SetActive(false);
            chaseResultPanel?.gameObject.SetActive(false);
            carRewardPanel?.Hide();

            switch (panel)
            {
                case "RaceResults":
                    bool special = RaceManager.instance.raceType == RaceType.Drift ||
                                   RaceManager.instance.raceType == RaceType.TimeAttack;
                    raceResultsPanel.gameObject.SetActive(!special);
                    otherRaceResultsPanel.gameObject.SetActive(special);
                    break;

                case "ChaseResult":
                    // показываем панель погонь
                    chaseResultPanel.gameObject.SetActive(true);
                    // скрываем текстовый таймер
                    if (raceEndTimer != null)
                    {
                        raceEndTimer.text = string.Empty;
                        raceEndTimer.gameObject.SetActive(false);
                    }
                    break;

                case "ChampionshipResults":
                    championshipResultsPanel.gameObject.SetActive(true);
                    break;

                case "RaceRewards":
                    raceRewardsPanel.gameObject.SetActive(true);
                    break;

                case "CarReward":
                    // Активируется в Continue()
                    break;
            }
        }

        void LoadMenuScene()
        {
            if (SceneController.instance != null)
                SceneController.instance.ExitToMenu();
            else
                Debug.LogWarning("SceneController не найден — выход в меню невозможен.");
        }

        void AddButtonListeners()
        {
            // Получаем контроллер рекламы
            // var adController = FindObjectOfType<CSharpSampleController>();

            // Continue — теперь с показом межстраничного объявления
            continueButton?.onClick.AddListener(delegate {
                // if (adController != null) adController.ShowInterstitial();
                Continue();
            });

            // Смотрим повтор (без изменений)
            watchReplayButton?.onClick.AddListener(() => ReplayManager.instance?.WatchReplay());

            // Перезапуск гонки — тоже с показом межстраничного объявления
            restartRaceButton?.onClick.AddListener(delegate {
                // if (adController != null) adController.ShowInterstitial();
                SceneController.instance?.ReloadScene();
            });
        }
    }
}
