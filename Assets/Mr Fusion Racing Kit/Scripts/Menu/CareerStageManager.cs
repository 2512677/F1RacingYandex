using UnityEngine;

namespace RGSK
{
    public class CareerStageManager : MonoBehaviour
    {
        public CareerData careerStage; // Изменено с CareerStage на CareerData

        void Start()
        {
            if (careerStage != null)
            {
               // Debug.Log("Начало уровня карьеры: " + careerStage.levelName);
            }
            else
            {
                Debug.LogWarning("Уровень карьеры не задан!");
            }
        }
    }
}
    