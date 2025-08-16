using UnityEngine;

// Класс для хранения данных состояния автомобиля на каждом кадре записи
[System.Serializable]
public class ReplayFrameData
{
    public Vector3 position;         // Позиция автомобиля в пространстве
    public Quaternion rotation;      // Вращение автомобиля
    public Vector3 velocity;         // Скорость автомобиля
    public Vector3 angularVelocity;  // Угловая скорость автомобиля
    public float throttle;           // Вход газа (акселератора)
    public float brake;              // Вход тормоза
    public float steer;              // Вход руля
    public float handbrake;          // Вход ручного тормоза
    public float engineRPM;          // Обороты двигателя
    public int gear;                 // Текущая передача
    public float nitroCapacity;      // Ёмкость нитро
    public bool nitroEngaged;        // Состояние нитро (включено/выключено)
    public bool bikerAlive;          // Состояние гонщика (жив/мертв)
}
