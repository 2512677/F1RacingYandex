using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using I2.Loc;

namespace RGSK
{
    public class MenuChampionshipPanel : MonoBehaviour
    {

        public ChampionshipData[] championships;
        private int championshipIndex;
        public RoundInformation[] roundInformation;

        public Text championshipName;
        public Text reward;
        public Text carName;
        public Image championshipImage;
        public Image championshipIcon;
        public Image raceTypeIcon;
        public Button nextChampionshipButton;
        public Button previousChampionshipButton;
        public Button startChampionship;
        public CarClass playerCar; // Предполагаем, что ссылка на машину игрока задаётся в инспекторе

        public MenuVehicleInstantiator vehicleInstantiator;


        // Публичные поля для Pop-up окна
        public GameObject popupWindow;     // Панель поп-апа (изначально неактивна)
        public Text popupMessageText;      // Текстовое поле для сообщения
        public Button popupOkButton;       // Кнопка "ОК" для закрытия поп-апа


        [Header("Назад")]
        public Button backButton;
        public GameObject previousPanel;

        void Start()
        {
            //Add button listeners
            if (nextChampionshipButton != null)
            {
                nextChampionshipButton.onClick.AddListener(delegate { AddChampionship(1); });
            }

            if (previousChampionshipButton != null)
            {
                previousChampionshipButton.onClick.AddListener(delegate { AddChampionship(-1); });
            }

            if (startChampionship != null)
            {
                startChampionship.onClick.AddListener(delegate { StartChampionship(); });
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(delegate { Back(); });
            }

            // Если в инспекторе не назначена машина, найдём её по тегу "Player"
            if (playerCar == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    playerCar = playerObj.GetComponent<CarClass>();
                    if (playerCar == null)
                    {
                        Debug.LogWarning("Объект с тегом 'Player' не содержит компонент CarClass!");
                    }
                }
                else
                {
                    Debug.LogWarning("Не удалось найти объект с тегом 'Player'!");
                }

                ClearRoundInformation();
                AddChampionship(0);
            }
        }


        public void AddChampionship(int direction)
        {
            // Move to the index in the direction of "direction"
            championshipIndex += direction;
            championshipIndex = Mathf.Clamp(championshipIndex, 0, championships.Length - 1);

            // Fill in the name
            if (championshipName != null)
            {
                championshipName.text = championships[championshipIndex].championshipName;
            }

            if (carName != null)
            {
                //carName.text = championships[championshipIndex].carName;
            }

            // Fill in the name
            if (reward != null)
            {
                reward.text = championships[championshipIndex].reward;
            }

            // Fill in the image
            if (championshipImage != null && championships[championshipIndex].championshipImage != null)
            {
                championshipImage.sprite = championships[championshipIndex].championshipImage;
            }

            // Fill in the Icon
            if (championshipIcon != null && championships[championshipIndex].championshipIcon != null)
            {
                championshipIcon.sprite = championships[championshipIndex].championshipIcon;
            }

            // Fill in the Icon
            if (raceTypeIcon != null && championships[championshipIndex].raceTypeIcon != null)
            {
                raceTypeIcon.sprite = championships[championshipIndex].raceTypeIcon;
            }

            // Clear round information and information on the current championship index
            ClearRoundInformation();
            for (int i = 0; i < championships[championshipIndex].championshipRounds.Count; i++)
            {
                if (i > roundInformation.Length - 1)
                    break;

                if (roundInformation[i].roundNumber != null)
                {
                    roundInformation[i].roundNumber.text = string.Format(LocalizationManager.GetTranslation("Round_Format"), i + 1);

                }

                if (roundInformation[i].trackName != null)
                {
                    roundInformation[i].trackName.text = championships[championshipIndex].championshipRounds[i].trackData.trackName;
                }

                if (roundInformation[i].raceType != null)
                {
                    // Use the GetDescription method to display the description in Russian
                    roundInformation[i].raceType.text = LocalizationManager.GetTranslation("RaceType_" + championships[championshipIndex].championshipRounds[i].raceType.ToString());

                }
            }

            // Disable the next button when we are on the last index
            if (nextChampionshipButton != null)
            {
                nextChampionshipButton.gameObject.SetActive(championshipIndex < championships.Length - 1);
            }

            // Disable the previous button when we are on the first index
            if (previousChampionshipButton != null)
            {
                previousChampionshipButton.gameObject.SetActive(championshipIndex > 0);
            }
        }

        // Метод, который обновляет ссылку на машину игрока, если в сцене найден активный объект с тегом "Player"
        public void RefreshPlayerCar()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null && playerObj.activeInHierarchy)
            {
                playerCar = playerObj.GetComponent<CarClass>();
                if (playerCar != null)
                {
                    Debug.Log("[MenuChampionshipPanel] Машина обновлена: " + playerCar.gameObject.name);
                }
                else
                {
                    Debug.LogWarning("[MenuChampionshipPanel] Объект с тегом 'Player' не имеет компонента CarClass!");
                }
            }
            else
            {
                Debug.LogWarning("[MenuChampionshipPanel] Активный объект с тегом 'Player' не найден!");
            }
        }



        public void StartChampionship()
        {
            // 1) Обновляем машину игрока
            if (vehicleInstantiator != null)
            {
                GameObject currentVehicle = vehicleInstantiator.GetCurrentVehicle();
                if (currentVehicle != null)
                {
                    playerCar = currentVehicle.GetComponent<CarClass>();
                    Debug.Log("[MenuChampionshipPanel] Машина обновлена: " + playerCar.gameObject.name);
                }
            }

            // 2) Проверяем класс машины
            CarClass.VehicleClass playerCarClass = playerCar != null ? playerCar.carClass : CarClass.VehicleClass.None;
            CarClass.VehicleClass requiredClass = championships[championshipIndex].requiredCarClass;
            if (playerCarClass != requiredClass)
            {
                popupMessageText.text = requiredClass.ToString();
                popupWindow.SetActive(true);
                popupOkButton.onClick.RemoveAllListeners();
                popupOkButton.onClick.AddListener(() => popupWindow.SetActive(false));
                return;
            }

            Debug.Log("[DEBUG] Начало чемпионата. Сохраняем награды...");

            // 3) Подготавливаем список наград
            var champRewards = championships[championshipIndex].raceRewards.ToArray();
            ChampionshipData.pendingRewards = champRewards;
            RaceRewards.Instance?.SetRewards(champRewards);
            Debug.Log($"[DEBUG] Награды за чемпионат установлены: {champRewards.Length}");

            // 4) Создаём и сохраняем ChampionshipManager
            GameObject go = new GameObject("Championship");
            DontDestroyOnLoad(go);                                        // ← вот эта строка!
            ChampionshipManager newChamp = go.AddComponent<ChampionshipManager>();
            newChamp.championshipData = championships[championshipIndex];

            // 5) Сохраняем ID текущего раунда и флаг режима
            var round = newChamp.CurrentRound();
            PlayerPrefs.SetString("CurrentRaceID", round.championshipID);
            PlayerPrefs.SetInt("IsChampionshipMode", 1);
            PlayerPrefs.Save();
            Debug.Log($"[DEBUG] CurrentRaceID='{round.championshipID}', режим чемпионата включён");

            // 6) Загружаем сцену раунда
            if (SceneController.instance != null)
            {
                SceneController.instance.LoadScene(round.trackData.scene);
            }
            else
            {
                Debug.LogWarning("SceneController.instance == null, не могу загрузить сцену!");
            }
        }











        public void Back()
        {
            if (previousPanel != null)
            {
                gameObject.SetActive(false);
                previousPanel.SetActive(true);
            }
        }


        void ClearRoundInformation()
        {
            for (int i = 0; i < roundInformation.Length; i++)
            {
                if (roundInformation[i].roundNumber != null)
                {
                    roundInformation[i].roundNumber.text = string.Empty;
                }

                if (roundInformation[i].trackName != null)
                {
                    roundInformation[i].trackName.text = string.Empty;
                }

                if (roundInformation[i].raceType != null)
                {
                    roundInformation[i].raceType.text = string.Empty;
                }
            }
        }


        [System.Serializable]
        public class RoundInformation
        {
            public Text roundNumber;
            public Text trackName;
            public Text raceType;
        }
    }
}