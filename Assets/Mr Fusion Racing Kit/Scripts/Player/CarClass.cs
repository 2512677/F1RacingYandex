using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarClass : MonoBehaviour
{
    // Перечисление для классов машин
    public enum VehicleClass
    {
        None,       /// Добавлен класс "None" для начального значения
        Sport,
        Perfomance,
        Super,
        Hyper,
        Hatchback,
        Drift,
        Offroad,
        Muscle,
        Cult,
        Copcar,
    }

    [Header("Класс машины")]
    public VehicleClass carClass; // Укажите класс машины в инспекторе

    // Вывод в консоль для тестирования
    private void Awake()
    {
        Debug.Log($"Машина {gameObject.name} относится к классу {carClass}");
    }
}
