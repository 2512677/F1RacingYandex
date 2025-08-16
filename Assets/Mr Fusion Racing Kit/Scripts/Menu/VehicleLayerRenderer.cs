using UnityEngine;
using System.Collections.Generic;

public class VehicleLayerRenderer : MonoBehaviour
{
    public GameObject vehicleSelectMenu; // Меню, которое активирует или деактивирует машины
    private List<GameObject> cars = new List<GameObject>(); // Список для хранения объектов CarMenu

    void Start()
    {
        // Находим и сохраняем все объекты CarMenu в начале
        GameObject[] carObjects = GameObject.FindGameObjectsWithTag("CarMenu");
        foreach (GameObject car in carObjects)
        {
            cars.Add(car);
            car.SetActive(false); // Деактивируем все машины при старте
        }
    }

    void Update()
    {
        // Активируем машины, только если vehicleSelectMenu активно
        if (vehicleSelectMenu != null && vehicleSelectMenu.activeSelf)
        {
            SetCarsActive(true);
        }
        else
        {
            SetCarsActive(false);
        }
    }

    // Метод для активации или деактивации всех сохранённых объектов CarMenu
    private void SetCarsActive(bool isActive)
    {
        foreach (GameObject car in cars)
        {
            if (car != null)
            {
                car.SetActive(isActive);
            }
        }
    }
}
