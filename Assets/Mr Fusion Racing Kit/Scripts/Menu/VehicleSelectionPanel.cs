using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace RGSK
{
    public class VehicleSelectionPanel : MonoBehaviour
    {
        private MenuVehicleInstantiator vehicleInstantiator;
        private IEnumerator moveBars;
        // Читаем сохранённый флаг: 1 ‒ уже показывали окно машины, 0 ‒ ещё нет
        ///static bool machineIntroShown = false;     // оставляем так


        [Header("Tech Specs")]
        public Image topSpeedBar;
        public Image accelerationBar;
        public Image handlingBar;
        public Image brakingBar;

        public Text BrandName; // Марка автомобиля

        public Text ModelName; // Модель автомобиля

       

        public Text CarClassNames; // Класс Авто



        public Button nextVehicle;
        public Button previousVehicle;
        public Button selectVehicle;
        public Button customizeVehicle;
        public GameObject colorPanel;
        public Button buyVehicle; // Кнопка покупки машины

        [Header("Назад")]
        public Button backButton;
        public GameObject previousPanel;

        [Header("Иконка Замка")]
        public GameObject lockIcon; // Иконка замка, если машина заблокирована

        [Header("Элементы  Меню")]
        public Text playerCurrencyText; // Отображение количества валюты игрока
        public GameObject insufficientFundsPanel; // Панель с сообщением о недостаточном количестве средств

        // в начале скрипта рядом с другими [Header]-полями
        [Header("UI – панель с кнопками классов")]
        public GameObject classTabsPanel;   // сюда перетащим объект ClassTabs

        [Header("UI – панель выбора машины")]
        public GameObject vehiclePanel;        // сюда перетащишь объект с машин/стрелками

        public Action VehicleSelectedEvent { get; internal set; }

        void Start()
        {
            vehicleInstantiator = FindObjectOfType<MenuVehicleInstantiator>();
            if (vehicleInstantiator == null)
                return;

            //Add button listeners
            if (nextVehicle != null)
            {
                nextVehicle.onClick.AddListener(delegate { vehicleInstantiator.CycleVehicles(1); });
                nextVehicle.onClick.AddListener(delegate { UpdateVehicleInformation(); });
            }

            if (previousVehicle != null)
            {
                previousVehicle.onClick.AddListener(delegate { vehicleInstantiator.CycleVehicles(-1); });
                previousVehicle.onClick.AddListener(delegate { UpdateVehicleInformation(); });
            }

            if (selectVehicle != null)
            {
                selectVehicle.onClick.AddListener(delegate { vehicleInstantiator.SetSelectedVehicle(); });
            }

            if (buyVehicle != null)
            {
                buyVehicle.onClick.AddListener(delegate { BuyVehicle(); });
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(delegate { Back(); });
            }

                
            

            UpdateVehicleInformation();
            UpdatePlayerCurrencyUI();
        }


        

        public void ShowClassTabs()
        {
            if (classTabsPanel) classTabsPanel.SetActive(true);
            if (vehiclePanel) vehiclePanel.SetActive(false);
        }

        public void UpdateVehicleInformation()
        {
            if (vehicleInstantiator == null)
                return;
            // Обновляем название модели автомобиля
            if (ModelName != null)
            {
                ModelName.text = vehicleInstantiator.GetVehicleData().ModelName;
            }

            if (!vehicleInstantiator.HasVehicleDatabase())
                return;

            // Получаем данные о текущем транспортном средстве
            VehicleDatabase.VehicleData currentVehicle = vehicleInstantiator.GetVehicleData();

            // Обновляем название марки автомобиля
            if (BrandName != null)
            {
                BrandName.text = currentVehicle.BrandName;
            }



           

            // // Класс Авто
            if (CarClassNames != null)
            {
                CarClassNames.text = currentVehicle.CarClassNames;
            }



         

            // Показываем/скрываем иконку замка в зависимости от того, заблокирован ли автомобиль
            if (lockIcon != null)
            {
                lockIcon.SetActive(currentVehicle.isLocked);
            }

            // Показываем/скрываем кнопку покупки, если автомобиль заблокирован
            if (buyVehicle != null)
            {
                buyVehicle.gameObject.SetActive(currentVehicle.isLocked);
                buyVehicle.GetComponentInChildren<Text>().text = "" + currentVehicle.unlockCost + " CR";
            }

            // Показываем/скрываем кнопку выбора, если автомобиль разблокирован
            if (selectVehicle != null)
            {
                selectVehicle.gameObject.SetActive(!currentVehicle.isLocked);
            }

            // Показываем/скрываем элементы окраски только если машина разблокирована и помечена как paintable
            bool canPaint = !currentVehicle.isLocked && currentVehicle.isPaintable;

            if (customizeVehicle != null)
                customizeVehicle.gameObject.SetActive(canPaint);

            if (colorPanel != null)
                colorPanel.gameObject.SetActive(canPaint);


            // Обновляем отображение полос производительности автомобиля
            if (moveBars != null)
            {
                StopCoroutine(moveBars);
            }

            moveBars = MovePerformanceBars(
                currentVehicle.topSpeed,
                currentVehicle.acceleration,
                currentVehicle.handling,
                currentVehicle.braking);

            StartCoroutine(moveBars);

            // Показываем/скрываем кнопку "следующий автомобиль", если это последний автомобиль в списке
            if (nextVehicle != null)
            {
                nextVehicle.gameObject.SetActive(!vehicleInstantiator.IsLastVehicleInList());
            }

            // Показываем/скрываем кнопку "предыдущий автомобиль", если это первый автомобиль в списке
            if (previousVehicle != null)
            {
                previousVehicle.gameObject.SetActive(!vehicleInstantiator.IsFirstVehicleInList());
            }
        }

        // VehicleSelectionPanel.cs
        public void OnClassButtonClicked(int classIndex)
        {
            var cls = (CarClass.VehicleClass)classIndex;

            if (vehicleInstantiator != null)
                vehicleInstantiator.ApplyClassFilter(cls);

            UpdateVehicleInformation();

            if (classTabsPanel) classTabsPanel.SetActive(false);
            if (vehiclePanel) vehiclePanel.SetActive(true);

           
            
        }






        public void BuyVehicle()
        {
            if (vehicleInstantiator == null)
                return;

            if (!vehicleInstantiator.HasVehicleDatabase())
                return;

            VehicleDatabase.VehicleData currentVehicle = vehicleInstantiator.GetVehicleData();

            if (PlayerData.instance != null && currentVehicle.isLocked)
            {
                if (PlayerData.instance.playerData.playerCurrency >= currentVehicle.unlockCost)
                {
                    // Списание стоимости автомобиля
                    PlayerData.instance.AddPlayerCurrecny(-currentVehicle.unlockCost);

                    // Разблокировка автомобиля в памяти
                    currentVehicle.isLocked = false;

                    // Сохранение разблокированного автомобиля
                    PlayerData.instance.UnlockItem(currentVehicle.uniqueID);

                    // Обновление интерфейса
                    UpdatePlayerCurrencyUI();
                    UpdateVehicleInformation();

                    Debug.Log("Машина " + currentVehicle.ModelName + " успешно куплена.");
                }
                else
                {
                    // Показать панель о недостатке средств
                    if (insufficientFundsPanel != null)
                    {
                        insufficientFundsPanel.SetActive(true);
                        StartCoroutine(HideInsufficientFundsPanel());
                    }
                    Debug.Log("Недостаточно средств для покупки " + currentVehicle.ModelName);
                }
            }
        }


        private void UpdatePlayerCurrencyUI()
        {
            if (PlayerData.instance != null && playerCurrencyText != null)
            {
                playerCurrencyText.text = "$" + PlayerData.instance.playerData.playerCurrency;
            }
        }

        private IEnumerator HideInsufficientFundsPanel()
        {
            yield return new WaitForSeconds(2f);
            if (insufficientFundsPanel != null)
            {
                insufficientFundsPanel.SetActive(false);
            }
        }

        IEnumerator MovePerformanceBars(float speed, float accel, float handle, float brake)
        {
            float timer = 0;
            float lerpSpeed = 1;

            while (timer < lerpSpeed)
            {
                timer += Time.deltaTime;

                if (topSpeedBar != null)
                {
                    topSpeedBar.fillAmount = Mathf.Lerp(topSpeedBar.fillAmount, speed, timer / lerpSpeed);
                }

                if (accelerationBar != null)
                {
                    accelerationBar.fillAmount = Mathf.Lerp(accelerationBar.fillAmount, accel, timer / lerpSpeed);
                }

                if (handlingBar != null)
                {
                    handlingBar.fillAmount = Mathf.Lerp(handlingBar.fillAmount, handle, timer / lerpSpeed);
                }

                if (brakingBar != null)
                {
                    brakingBar.fillAmount = Mathf.Lerp(brakingBar.fillAmount, brake, timer / lerpSpeed);
                }

                yield return null;
            }
        }

        void RevertPlayerVehicle()
        {
            if (vehicleInstantiator == null)
                return;

            if (!vehicleInstantiator.HasVehicleDatabase())
                return;

            //Revert the player vehicle if the selected vehicle does not match the saved vehicle
            if (PlayerData.instance.playerData.vehicleID != vehicleInstantiator.GetVehicleData().uniqueID)
                vehicleInstantiator.RevertPlayerVehicle();
        }

        public void Back()
        {
            RevertPlayerVehicle();

            if (previousPanel != null)
            {
                gameObject.SetActive(false);
                previousPanel.SetActive(true);
            }
        }


        void OnEnable()
        {
            bool firstLaunch = PlayerPrefs.GetInt("MachineIntroShown", 0) == 0;

            if (firstLaunch)
            {
                // ► Первый вход: показываем только машину
                if (classTabsPanel) classTabsPanel.SetActive(false);
                if (vehiclePanel) vehiclePanel.SetActive(true);

                // ► СРАЗУ помечаем, что окно машины уже показывали
                PlayerPrefs.SetInt("MachineIntroShown", 1);
                PlayerPrefs.Save();
            }
            else
            {
                // ► Все последующие входы: сразу панель классов
                if (classTabsPanel) classTabsPanel.SetActive(true);
                if (vehiclePanel) vehiclePanel.SetActive(false);
            }

            UpdateVehicleInformation();
            UpdatePlayerCurrencyUI();
        }




    }
}
