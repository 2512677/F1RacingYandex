using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using I2;
using I2.Loc;

namespace RGSK
{
    public class OtherResultsPanel : MonoBehaviour
    {
        [Header("Результаты Временная атака")]
        public GameObject timeAttackResultsPanel;
        public Text totalTime;
        public string totalTimePrefix = "Time: ";

        [Header("Результаты дрифт-гонок")]
        public GameObject driftResultsPanel;
        public Text totalPoints;
        public Text bestDrift;
        public Text longestDrift;
        public string totalPointsPrefix = "Total Drift Points: ";
        public string bestDriftPrefix = "Best Drift: ";
        public string longestDriftPrefix = "Longest Drift: ";

        [Header("Целевые показатели")]
        public Text targetScoreGold;
        public Text targetScoreSilver;
        public Text targetScoreBronze;


        public void UpdateResults()
        {
            switch(RaceManager.instance.raceType)
            {
                case RaceType.TimeAttack:
                    UpdateTimeAttackResults();
                    break;

                case RaceType.Drift:
                    UpdateDriftResults();
                    break;
            }
        }


        void UpdateTimeAttackResults()
        {
            if (timeAttackResultsPanel != null)
            {
                timeAttackResultsPanel.SetActive(true);
            }

            if (driftResultsPanel != null)
            {
                driftResultsPanel.SetActive(false);
            }

            if (totalTime != null)
            {
                string localizedTimePrefix = LocalizationManager.GetTranslation("ResultsPanel/TotalTimePrefix");
                totalTime.text = localizedTimePrefix + Helper.FormatTime(RaceManager.instance.playerStatistics.totalRaceTime);
            }

            if (targetScoreGold != null)
            {
                targetScoreGold.text = Helper.FormatTime(RaceManager.instance.targetTimeGold);
            }
            if (targetScoreSilver != null)
            {
                targetScoreSilver.text = Helper.FormatTime(RaceManager.instance.targetTimeSilver);
            }
            if (targetScoreBronze != null)
            {
                targetScoreBronze.text = Helper.FormatTime(RaceManager.instance.targetTimeBronze);
            }
        }


        void UpdateDriftResults()
        {
            if (timeAttackResultsPanel != null)
            {
                timeAttackResultsPanel.SetActive(false);
            }

            if (driftResultsPanel != null)
            {
                driftResultsPanel.SetActive(true);
            }

            DriftPointsManager driftPoints = FindObjectOfType<DriftPointsManager>();
            if (driftPoints == null)
                return;

            if (totalPoints != null)
            {
                string localizedPointsPrefix = LocalizationManager.GetTranslation("ResultsPanel/TotalPointsPrefix");
                totalPoints.text = localizedPointsPrefix + driftPoints.totalDriftPoints.ToString("N0");
            }

            if (bestDrift != null)
            {
                string localizedBestDriftPrefix = LocalizationManager.GetTranslation("ResultsPanel/BestDriftPrefix");
                bestDrift.text = localizedBestDriftPrefix + driftPoints.bestDrift.ToString("N0");
            }

            if (longestDrift != null)
            {
                string localizedLongestDriftPrefix = LocalizationManager.GetTranslation("ResultsPanel/LongestDriftPrefix");
                longestDrift.text = localizedLongestDriftPrefix + Helper.FormatTime(driftPoints.longestDrift);
            }

            if (targetScoreGold != null)
            {
                targetScoreGold.text = RaceManager.instance.targetScoreGold.ToString();
            }
            if (targetScoreSilver != null)
            {
                targetScoreSilver.text = RaceManager.instance.targetScoreSilver.ToString();
            }
            if (targetScoreBronze != null)
            {
                targetScoreBronze.text = RaceManager.instance.targetScoreBronze.ToString();
            }
        }
    }
}