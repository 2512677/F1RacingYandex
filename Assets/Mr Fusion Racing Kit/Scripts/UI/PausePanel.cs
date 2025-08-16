using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System; // Array.FindIndex, StringComparison

namespace RGSK
{
    public class PausePanel : MonoBehaviour
    {
        [Header("Кнопки")]
        public Button resumeButton;
        public Button restartButton;
        public Button watchReplayButton;
        public Button exitButton;

        [Header("Exit Settings (Optional)")]
        [Tooltip("Если true, при выходе будет переключен профиль физики RCC v4")]
        public bool requireBehaviorTypeOnExit = false;

        [Tooltip("Имя профиля из RCC_Settings.behaviorTypes[].behaviorName (например: \"Simulator\", \"Arcade\", \"Racing\"). Используется, если exitBehaviorIndex < 0.")]
        public string exitBehaviorName = "Simulator";

        [Tooltip("Если >= 0, имеет приоритет над exitBehaviorName и будет установлен как behaviorSelectedIndex.")]
        public int exitBehaviorIndex = -1;

        void Start()
        {
            AddButtonListeners();
        }

        void AddButtonListeners()
        {
            // var adController = FindObjectOfType<CSharpSampleController>();

            if (resumeButton != null && RaceManager.instance)
                resumeButton.onClick.AddListener(delegate { RaceManager.instance.UnPause(); });

            if (restartButton != null && SceneController.instance)
            {
                restartButton.onClick.AddListener(delegate
                {
                    // if (adController != null) adController.ShowInterstitial();
                    SceneController.instance.ReloadScene();
                });
            }

            if (watchReplayButton != null && ReplayManager.instance != null)
                watchReplayButton.onClick.AddListener(delegate { ReplayManager.instance.WatchReplay(); });

            if (exitButton != null && SceneController.instance)
            {
                exitButton.onClick.AddListener(() =>
                {
                    // if (adController != null) adController.ShowInterstitial();

                    if (requireBehaviorTypeOnExit)
                        ApplyRCCBehaviorOnExit();

                    SceneController.instance.ExitToMenu();
                });
            }
        }

        private void ApplyRCCBehaviorOnExit()
        {
            var set = RCC_Settings.Instance;
            if (set == null || set.behaviorTypes == null || set.behaviorTypes.Length == 0)
            {
                Debug.LogWarning("PausePanel: RCC_Settings.behaviorTypes пуст — профиль физики установить нельзя.");
                return;
            }

            int idx = -1;

            // Приоритет индекса
            if (exitBehaviorIndex >= 0 && exitBehaviorIndex < set.behaviorTypes.Length)
            {
                idx = exitBehaviorIndex;
            }
            else
            {
                // По имени
                string name = string.IsNullOrEmpty(exitBehaviorName) ? "Simulator" : exitBehaviorName;
                idx = Array.FindIndex(set.behaviorTypes, b =>
                    b != null && b.behaviorName.Equals(name, StringComparison.OrdinalIgnoreCase));

                if (idx < 0) idx = 0; // запасной вариант
            }

            set.overrideBehavior = true;
            set.behaviorSelectedIndex = idx;

            // Совместимость со старой логикой, если где-то читаете PlayerPrefs
            PlayerPrefs.SetInt("BehaviorType", idx);
            PlayerPrefs.Save();

            string logName = (idx >= 0 && idx < set.behaviorTypes.Length && set.behaviorTypes[idx] != null)
                ? set.behaviorTypes[idx].behaviorName
                : $"index {idx}";
            Debug.Log("PausePanel: Физика переключена на профиль '" + logName + "'");
        }
    }
}
