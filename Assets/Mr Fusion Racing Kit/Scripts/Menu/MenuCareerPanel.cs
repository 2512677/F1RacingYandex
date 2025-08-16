using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RGSK
{
    public class MenuCareerPanel : MonoBehaviour
    {
        public static MenuCareerPanel Instance; // Синглтон для доступа

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadCareerEvents(CareerData careerData)
        {
            if (careerData == null)
            {
                Debug.LogError("MenuCareerPanel: CareerData не назначено!");
                return;
            }

            foreach (var round in careerData.careerRounds)
            {
                Debug.Log($"Загрузка события: {round.raceType} - {round.laps} кругов");
            }
        }

        private void StartRace(CareerData.CareerRound round)
        {
            if (round == null)
            {
                Debug.LogError("MenuCareerPanel: Round не назначен!");
                return;
            }

            PlayerPrefs.SetInt("RaceType", (int)round.raceType);
            PlayerPrefs.SetInt("LapCount", round.laps);
            PlayerPrefs.SetInt("OpponentCount", round.opponentCount);

            if (SceneController.instance != null)
            {
                SceneController.instance.LoadScene(round.trackData.trackName);
            }
            else
            {
                Debug.LogWarning("SceneController не найден.");
            }
        }
    }
}
