using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RGSK
{
    public class Recorder : MonoBehaviour
    {
        [HideInInspector]
        public List<RecordableObject> recordableObjects = new List<RecordableObject>(); // Список объектов для записи
        [HideInInspector]
        public int playbackSpeed; // Скорость воспроизведения
        [HideInInspector]
        public int currentFrame; // Текущий кадр
        [HideInInspector]
        public int totalFrames; // Общее количество кадров
        [HideInInspector]
        public bool record; // Флаг записи
        [HideInInspector]
        public bool playback; // Флаг воспроизведения

        // Метод записи данных
        public void Record()
        {
            for (int i = 0; i < recordableObjects.Count; i++)
            {
                ReplayFrameData data = new ReplayFrameData(); // Создание нового кадра записи

                if (recordableObjects[i].transform != null)
                {
                    data.position = recordableObjects[i].transform.position; // Запись позиции
                    data.rotation = recordableObjects[i].transform.rotation; // Запись вращения
                }

                if (recordableObjects[i].rigidbody != null)
                {
                    data.velocity = recordableObjects[i].rigidbody.linearVelocity; // Запись скорости
                    data.angularVelocity = recordableObjects[i].rigidbody.angularVelocity; // Запись угловой скорости
                }

                if (recordableObjects[i].rgskVehicle != null)
                {
                    data.throttle = recordableObjects[i].rgskVehicle.throttleInput; // Запись газа
                    data.steer = recordableObjects[i].rgskVehicle.steerInput; // Запись руля
                    data.brake = recordableObjects[i].rgskVehicle.brakeInput; // Запись тормоза
                    data.handbrake = recordableObjects[i].rgskVehicle.handbrakeInput; // Запись ручного тормоза
                    data.engineRPM = recordableObjects[i].rgskVehicle.engineRPM; // Запись оборотов двигателя
                    data.gear = recordableObjects[i].rgskVehicle.currentGear; // Запись текущей передачи
                }

                if (recordableObjects[i].nitroController != null)
                {
                    data.nitroCapacity = recordableObjects[i].nitroController.capacity; // Запись ёмкости нитро
                    data.nitroEngaged = recordableObjects[i].nitroController.nitroEngaged; // Запись состояния нитро
                }

                if (recordableObjects[i].bikeRider != null)
                {
                    data.bikerAlive = recordableObjects[i].bikeRider.isAlive; // Запись состояния гонщика (жив/мертв)
                }

                recordableObjects[i].replayFrameData.Add(data); // Добавление данных кадра в список
            }
        }

        // Метод воспроизведения кадра записи
        public void PlaybackReplayFrame(RecordableObject target, ReplayFrameData frameData)
        {
            if (target.transform != null)
            {
                target.transform.position = frameData.position; // Установка позиции
                target.transform.rotation = frameData.rotation; // Установка вращения
            }

            if (target.rigidbody != null)
            {
                target.rigidbody.isKinematic = playbackSpeed != 1; // Установка кинематического состояния
                target.rigidbody.linearVelocity = frameData.velocity; // Установка скорости
                target.rigidbody.angularVelocity = frameData.angularVelocity; // Установка угловой скорости
            }

            if (target.rgskVehicle != null)
            {
                target.rgskVehicle.throttleInput = frameData.throttle; // Установка газа
                target.rgskVehicle.steerInput = frameData.steer; // Установка руля
                target.rgskVehicle.brakeInput = frameData.brake; // Установка тормоза
                target.rgskVehicle.handbrakeInput = frameData.handbrake; // Установка ручного тормоза
                target.rgskVehicle.engineRPM = frameData.engineRPM; // Установка оборотов двигателя
                target.rgskVehicle.currentGear = frameData.gear; // Установка текущей передачи               
            }

            if (target.nitroController != null)
            {
                target.nitroController.capacity = frameData.nitroCapacity; // Установка ёмкости нитро
                target.nitroController.nitroEngaged = frameData.nitroEngaged; // Установка состояния нитро
                target.nitroController.throttle = frameData.throttle; // Установка газа для нитро
            }

            if (target.bikeRider != null)
            {
                if (frameData.bikerAlive)
                {
                    target.bikeRider.DisableRagdoll(); // Отключение ragdoll, если гонщик жив
                }

                if (!frameData.bikerAlive)
                {
                    target.bikeRider.EnableRagdoll(); // Включение ragdoll, если гонщик мертв
                }
            }
        }

        // Метод воспроизведения записи
        public virtual void Playback()
        {
            // Увеличение текущего кадра на скорость воспроизведения
            currentFrame += 1 * playbackSpeed;
        }

        // Метод, вызываемый каждый физический кадр
        public virtual void FixedUpdate()
        {
            if (record)
            {
                Record(); // Запись данных
            }

            if (playback)
            {
                Playback(); // Воспроизведение данных
            }
        }

        // Метод получения текущего кадра
        public int GetCurrentFrame()
        {
            return currentFrame;
        }

        // Метод получения общего количества кадров
        public int GetTotalFrames()
        {
            if (recordableObjects.Count == 0)
                return 0;

            return recordableObjects[0].replayFrameData.Count;
        }
    }

    // Класс, представляющий объект, подлежащий записи
    [System.Serializable]
    public class RecordableObject
    {
        public Transform transform; // Трансформ объекта
        public Rigidbody rigidbody; // Rigidbody объекта
        public RCC_CarControllerV3 rgskVehicle; // Ссылка на компонент RCC_CarControllerV3
        public Nitro nitroController; // Ссылка на компонент Nitro
        public BikeRider bikeRider; // Ссылка на компонент BikeRider
        public List<ReplayFrameData> replayFrameData = new List<ReplayFrameData>(); // Список данных для воспроизведения
        internal global::RCC_CarControllerV4 rccVehicle; // Внутренний контроллер автомобиля
    }
}
