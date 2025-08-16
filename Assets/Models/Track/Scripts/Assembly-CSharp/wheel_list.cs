using System;
using UnityEngine;

public class wheel_list : MonoBehaviour
{
    [Serializable]
    public class Wheels
    {
        public string wheelName;        // Название колеса
        public GameObject WheelPrefab;  // Префаб колеса
        public int price;               // Цена колеса
    }

    public Wheels[] wheels;

    private void Start()
    {
        InitializeWheels();
        Debug.Log("Доступные колеса:");
        PrintWheelsInfo();
    }

    private void Update()
    {
        // Обновление каждый кадр (если необходимо)
    }

    // Инициализация колес, если они не заданы в инспекторе
    private void InitializeWheels()
    {
        if (wheels == null || wheels.Length == 0)
        {
            Debug.Log("Список колес пуст. Заполняем значениями по умолчанию.");
            wheels = new Wheels[2];

            wheels[0] = new Wheels();
            wheels[0].wheelName = "Standard Wheel";
            wheels[0].price = 1000;
            // Загрузка префаба колеса по имени из папки Resources
            wheels[0].WheelPrefab = Resources.Load<GameObject>("StandardWheel");

            wheels[1] = new Wheels();
            wheels[1].wheelName = "Sport Wheel";
            wheels[1].price = 1500;
            wheels[1].WheelPrefab = Resources.Load<GameObject>("SportWheel");
        }
    }

    // Метод для получения информации о колесе по индексу
    public Wheels GetWheel(int index)
    {
        if (index >= 0 && index < wheels.Length)
        {
            return wheels[index];
        }
        else
        {
            Debug.LogWarning("Индекс колеса вне диапазона: " + index);
            return null;
        }
    }

    // Метод для покупки колеса
    public bool PurchaseWheel(int index)
    {
        Wheels wheel = GetWheel(index);
        if (wheel != null)
        {
            int playerMoney = PlayerPrefs.GetInt("PlayerMoney", 0);
            if (playerMoney >= wheel.price)
            {
                playerMoney -= wheel.price;
                PlayerPrefs.SetInt("PlayerMoney", playerMoney);
                PlayerPrefs.SetInt("WheelUnlocked_" + wheel.wheelName, 1);
                PlayerPrefs.Save();
                Debug.Log("Колесо " + wheel.wheelName + " успешно куплено.");
                return true;
            }
            else
            {
                Debug.Log("Недостаточно средств для покупки колеса: " + wheel.wheelName);
                return false;
            }
        }
        return false;
    }

    // Метод для проверки, разблокировано ли колесо
    public bool IsWheelUnlocked(int index)
    {
        Wheels wheel = GetWheel(index);
        if (wheel != null)
        {
            return PlayerPrefs.GetInt("WheelUnlocked_" + wheel.wheelName, 0) == 1;
        }
        return false;
    }

    // Метод для печати информации о всех колесах (для отладки)
    private void PrintWheelsInfo()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            Wheels wheel = wheels[i];
            Debug.Log($"[{i}] {wheel.wheelName} - Цена: {wheel.price} - Префаб: {(wheel.WheelPrefab != null ? wheel.WheelPrefab.name : "null")}");
        }
    }
}
