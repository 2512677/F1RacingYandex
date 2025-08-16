using UnityEngine;
using System.Collections;
using static CarClass;
using System.Collections.Generic;
using Firebase.Analytics;


namespace RGSK
{
    // Атрибут позволяет создавать объект данных чемпионата через меню в Unity Editor
    [CreateAssetMenu(fileName = "База данных автомобиля", menuName = "MRFE/Vehicle Database", order = 1)]
    [System.Serializable]
    public class VehicleInfo
    {
        public string uniqueID;    // напр. "Car_Onix_2023"
        public string carName;     // "Chevy Onix"
        public bool isUnlocked;    // true = доступен, false = заблокирован
        public List<string> items = new List<string>();  // ... любые другие поля (цена, характеристики и пр.)
    }

    public class VehicleDatabase : ScriptableObject
    {
        public static VehicleDatabase Instance; // <-- Добавлено
        public VehicleData[] vehicles; // Массив данных автомобилей

        private void OnEnable()
        {
            Instance = this; // <-- При включении/загрузке этот объект становится Singleton
        }

        // Метод для получения данных автомобиля по уникальному идентификатору
        public VehicleData GetVehicle(string id)
        {
            for (int i = 0; vehicles != null && i < vehicles.Length; i++)
            {
                if (vehicles[i].uniqueID == id)
                {
                    return vehicles[i];
                }
            }

            return null; // Возвращает null, если автомобиль не найден
        }

        /// <summary>
        /// Разблокирует машину по ID и логирует событие.
        /// </summary>
        public bool UnlockVehicle(string id)
        {
            var vehicle = GetVehicle(id);
            if (vehicle == null)
            {
                Debug.LogWarning($"[VehicleDatabase] Авто с ID '{id}' не найдено!");
                return false;
            }

            if (!vehicle.isLocked)
            {
                Debug.Log($"[VehicleDatabase] Машина '{vehicle.ModelName}' уже разблокирована.");
                return true;
            }

            vehicle.isLocked = false;
            Debug.Log($"[VehicleDatabase] Машина '{vehicle.ModelName}' (ID {id}) разблокирована!");

            // Логируем разблокировку в Firebase Analytics
            FirebaseAnalytics.LogEvent(
                "unlock_vehicle",
                new Parameter("vehicle_id", id),
                new Parameter("vehicle_name", vehicle.ModelName)
            );

            return true;
        }



        /// <summary>
        /// Синхронизирует статус блокировки автомобилей с данными игрока.
        /// </summary>
        /// <param name="playerData">Данные игрока для синхронизации.</param>
        public void SyncVehicleData(PlayerData playerData)
        {
            if (playerData == null)
            {
                Debug.LogError("Экземпляр PlayerData равен null. Невозможно синхронизировать данные автомобилей.");
                return;
            }

            string defaultVehicleID = "golf";
            int unlockedCount = 0;

            foreach (var vehicle in vehicles)
            {
                if (vehicle.uniqueID == defaultVehicleID)
                {
                    vehicle.isLocked = false;
                }
                else
                {
                    bool shouldUnlock = playerData.playerData.items.Contains(vehicle.uniqueID);
                    vehicle.isLocked = !shouldUnlock;
                }

                if (!vehicle.isLocked)
                    unlockedCount++;
            }

            Debug.Log($"VehicleDatabase: синхронизировано {unlockedCount}/{vehicles.Length} машин.");

            // Логируем результат синхронизации в Firebase Analytics
            FirebaseAnalytics.LogEvent(
                "sync_vehicle_data",
                new Parameter("total_vehicles", vehicles.Length),
                new Parameter("unlocked_vehicles", unlockedCount)
            );
        }

        // Внутренний класс для хранения данных автомобиля
        [System.Serializable]
        public class VehicleData
        {
            public string ModelName;  // Модель автомобиля
            public string BrandName;  // Марка автомобиля
            public string CarClassNames; // Класс Авто


            [Header("Preview Sprite")]
            public Sprite previewSprite;  // сюда в инспекторе задаём картинку машины

            // Новое поле для класса автомобиля
            public VehicleClass CarClass;

            [Space(10)]
            public string uniqueID; // Уникальный идентификатор автомобиля

            [Header("Прифаб")]
            public GameObject vehicle;     // Префаб автомобиля
            public GameObject aiVehicle;   // Префаб автомобиля для ИИ
            public GameObject menuVehicle; // Префаб автомобиля для меню

            [Header("Технические характеристики")]
            [Range(0, 1)]
            public float topSpeed;        // Максимальная скорость (нормализованное значение от 0 до 1)
            [Range(0, 1)]
            public float acceleration;    // Ускорение (нормализованное значение от 0 до 1)
            [Range(0, 1)]
            public float handling;        // Управляемость (нормализованное значение от 0 до 1)
            [Range(0, 1)]
            public float braking;         // Тормозные характеристики (нормализованное значение от 0 до 1)

            [Header("Прочее")]
            public Material[] bodyMaterials; // Материалы кузова автомобиля

            // Новые свойства для отображения массы, мощности и описания автомобиля
            public float mass;  // Масса автомобиля
            public float power; // Мощность двигателя

            public string vehicleDescription; // Описание автомобиля

            [Header("Разблокировать систему")]
            public bool isLocked = true; // Флаг, указывающий, заблокирована ли машина
            public float unlockCost;     // Стоимость разблокировки автомобиля

            [Header("Можно ли красить машину")]
            public bool isPaintable = true;   // <-- ВСТАВЬТЕ СЮДА


            [Header("Стоковый цвет машины")] // <-- ДОБАВЛЕНО
            public Color stockColor = Color.white; // <-- ДОБАВЛЕНО
        }
    }
}
