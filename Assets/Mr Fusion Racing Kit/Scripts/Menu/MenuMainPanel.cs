using Firebase.Analytics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//using Firebase.Analytics;

namespace RGSK
{
    public class MenuMainPanel : MonoBehaviour
    {
        public UnityAction VehicleSelectedEvent; // Событие для уведомления о выборе автомобиля

        public QuickRacePanel quickRacePanel;
        public DailyRacePanel dailePanel;

        public EventPanel eventPanel;
        public MenuChampionshipPanel championshipPanel;
        public VehicleSelectionPanel vehicleSeletPanel;
        public TuningMenuUIController tuningmenuPanel;
        public CarPaintPanel carTuningPanel;
        public MenuOptionsPanel optionsPanel;
        public MenuCareerPanel menuCareerPanel;
        public MenuIAPPanel iapPanel;
        public QuitPanel quitPanel;
        public PlayerSettings playerPanel;



        public Button quickRaceButton;
        public Button dailyButton;
        public Button eventButton;
        public Button championshipButton;
        public Button vehicleSelectButton;
        public Button tuningMenuButton;
        public Button carTuningButton;
        public Button optionsButton;
        public Button careerButton;
        public Button iapButton;
        public Button quitButton;
        public Button playerButton;

        private bool hasSelectedVehicle = false;

        void Start()
        {
            // Получаем контроллер рекламы из сцены
           // var adController = FindObjectOfType<CSharpSampleController>();
            // Добавляем слушателей для кнопок, как и раньше
            if (dailyButton != null)
                dailyButton.onClick.AddListener(delegate { ShowPanel("Daily"); });

            if (eventButton != null)
                eventButton.onClick.AddListener(delegate {
                  //  if (adController != null) adController.ShowInterstitial();
                    ShowPanel("Event");
                });

            if (quickRaceButton != null)
                quickRaceButton.onClick.AddListener(delegate {
                 //   if (adController != null) adController.ShowInterstitial();
                    ShowPanel("QuickRace");
                });

            if (championshipButton != null)
                championshipButton.onClick.AddListener(delegate {
                   // if (adController != null) adController.ShowInterstitial();
                    ShowPanel("Championship");
                });

            if (vehicleSelectButton != null)
                vehicleSelectButton.onClick.AddListener(delegate { ShowPanel("VehicleSelect"); });

            if (tuningMenuButton != null)
                tuningMenuButton.onClick.AddListener(delegate { ShowPanel("TuningMenu"); });

            if (carTuningButton != null)
                carTuningButton.onClick.AddListener(delegate { ShowPanel("Car Tuning"); });

            if (optionsButton != null)
                optionsButton.onClick.AddListener(delegate { ShowPanel("Options"); });

            if (careerButton != null)
                careerButton.onClick.AddListener(delegate {
                  //  if (adController != null) adController.ShowInterstitial();
                    ShowPanel("CareerStage");
                });

            if (iapButton != null)
                iapButton.onClick.AddListener(delegate { ShowPanel("IAP"); });

            if (playerButton != null)
                playerButton.onClick.AddListener(delegate { ShowPanel("PlayerOptions"); });

            if (quitButton != null)
                quitButton.onClick.AddListener(Quit);

            // Если игрок заходит в игру впервые (автомобиль не выбран), сразу показываем панель выбора автомобиля
            if (string.IsNullOrEmpty(PlayerData.instance.playerData.vehicleID))
            {
                // Подписываемся на событие выбора автомобиля, чтобы обработать его
                vehicleSeletPanel.VehicleSelectedEvent += HandleVehicleSelected;
                // Показываем панель выбора автомобиля
                ShowPanel("VehicleSelect");
                Debug.Log("Первый вход в игру: показываем панель выбора автомобиля.");
            }
        }


        // Пример вызова события, когда автомобиль выбран
        public void SelectVehicle(string vehicleID)
        {
            // Логика выбора автомобиля
            Debug.Log($"Vehicle {vehicleID} selected.");
            PlayerData.instance.SaveSelectedVehicle(vehicleID);

            // Вызываем событие
            VehicleSelectedEvent?.Invoke();
        }

        public void Discord()
        {
            Application.OpenURL("https://discord.gg/DG6PcF6h");
        }

        public void privacy()
        {
            Application.OpenURL("https://sevenwolf.uz/Privacy.html");
        }


        private void OnCareerButtonClicked()
        {
            // Получаем контроллер рекламы и показываем межстраничную рекламу
          //  var adController = FindObjectOfType<CSharpSampleController>();
           // if (adController != null)
               // adController.ShowInterstitial();

            // Проверяем, выбрана ли машина у игрока
            if (string.IsNullOrEmpty(PlayerData.instance.playerData.vehicleID))
            {
                // Если машина не выбрана, переходим в панель выбора машины
                ShowPanel("VehicleSelect");
                Debug.Log("No vehicle selected. Redirecting to Vehicle Selection.");
            }
            else
            {
                // Если машина выбрана, переходим в меню карьеры
                ShowPanel("CareerStage");
                Debug.Log($"Selected Vehicle: {PlayerData.instance.playerData.vehicleID}. Redirecting to Career Menu.");
            }
        }


        private void HandleVehicleSelected()
        {
            // Логика при выборе автомобиля
            hasSelectedVehicle = true;

            // Отписываемся от события, чтобы избежать дублирования
            vehicleSeletPanel.VehicleSelectedEvent -= HandleVehicleSelected;

            // Переход в панель карьеры
            ShowPanel("CareerStage");
        }



        public void ShowPanel(string panel)
        {
            // Скрываем саму главную панель
            gameObject.SetActive(false);

            // Скрываем ВСЕ дочерние панели
            quickRacePanel?.gameObject.SetActive(false);
            dailePanel?.gameObject.SetActive(false);
            eventPanel?.gameObject.SetActive(false);
            championshipPanel?.gameObject.SetActive(false);
            vehicleSeletPanel?.gameObject.SetActive(false);
            tuningmenuPanel?.gameObject.SetActive(false);
            carTuningPanel?.gameObject.SetActive(false);
            optionsPanel?.gameObject.SetActive(false);
            menuCareerPanel?.gameObject.SetActive(false);
            iapPanel?.gameObject.SetActive(false);
            playerPanel?.gameObject.SetActive(false);
            quitPanel?.gameObject.SetActive(false);

            // Открываем нужную панель
            switch (panel)
            {
                case "Daily":
                    dailePanel.gameObject.SetActive(true);
                    break;
                case "Event":
                    eventPanel.gameObject.SetActive(true);
                    break;
                case "QuickRace":
                    quickRacePanel.gameObject.SetActive(true);
                    break;
                case "Championship":
                    championshipPanel.gameObject.SetActive(true);
                    break;
                case "VehicleSelect":
                    vehicleSeletPanel.gameObject.SetActive(true);
                    break;
                case "TuningMenu":
                    tuningmenuPanel.gameObject.SetActive(true);
                    break;
                case "Car Tuning":
                    carTuningPanel.gameObject.SetActive(true);
                    break;
                case "Options":
                    optionsPanel.gameObject.SetActive(true);
                    break;
                case "CareerStage":
                    menuCareerPanel.gameObject.SetActive(true);
                    break;
                case "IAP":
                    iapPanel.gameObject.SetActive(true);
                    break;
                case "PlayerOptions":
                    playerPanel.gameObject.SetActive(true);
                    break;
                case "Quit":
                    quitPanel.gameObject.SetActive(true);
                    break;
            }
        }



        public void Quit()
        {
            Application.Quit();
        }
    }
}
