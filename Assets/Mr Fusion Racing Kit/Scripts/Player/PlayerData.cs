using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Events;
using System.Threading.Tasks; // Добавлено для работы с async/await



namespace RGSK
{
    public class PlayerData : MonoBehaviour
    {
        public static event Action OnDataLoaded; // Событие, вызываемое после загрузки данных
        public static PlayerData instance; // Ссылка на текущий экземпляр PlayerData
        public static UnityAction OnPlayerLevelUp; // Событие при повышении уровня игрока
        public bool AdsRemoved => playerData.adsRemoved;

        public static PlayerData Instance { get; private set; }

        public int currentDailyDay = 1; // Текущий день дейликов
        public string lastDailyRaceTime; // Время последнего прохождения гонки

        [Header("Стандартные Данные")]
        public DataContainer defaultValues = new DataContainer(); // Начальные значения данных игрока

        [Header("Данные игрока")]
        public DataContainer playerData = new DataContainer(); // Текущие данные игрока



        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject); // Не уничтожать объект при смене сцен

              
              
            }
            else if (instance != this)
            {
                Destroy(gameObject); // Уничтожаем дублирующий объект
            }

            LoadData(); // Загрузка данных из файла (локальный файл инициализируется при запуске)
        }


        public void SaveDailyProgress()
        {
            PlayerPrefs.SetInt("CurrentDailyDay", currentDailyDay);
            PlayerPrefs.SetString("LastDailyRaceTime", lastDailyRaceTime);
            PlayerPrefs.Save();
            SaveDailyProgress();
        }

        public void RemoveAds()
       {
       playerData.adsRemoved = true;
       SaveData();
       }


       



        public void RaiseDataLoadedEvent()
        {
            OnDataLoaded?.Invoke();
        }

        public void LoadDailyProgress()
        {
            currentDailyDay = PlayerPrefs.GetInt("CurrentDailyDay", 1);
            lastDailyRaceTime = PlayerPrefs.GetString("LastDailyRaceTime", "");
        }

        public void SaveRaceResult(string raceID)
        {
            Debug.Log($"SaveRaceResult called for raceID: {raceID}");
            if (!playerData.completedRaces.Contains(raceID))
            {
                playerData.completedRaces.Add(raceID);
                SaveData();
                Debug.Log($"Гонка '{raceID}' сохранена. Текущий список: {string.Join(", ", playerData.completedRaces)}");
            }
            else
            {
                Debug.Log($"Гонка '{raceID}' уже была завершена.");
            }
        }

        // Сохранить попытку (win/lose) и время (UTC)
        public void SaveDailyRaceAttempt(int dayIndex, bool isWin)
        {
            string timeKey = $"DailyRace_{dayIndex}_LastTime";
            string timeValue = DateTime.UtcNow.ToString("o");
            PlayerPrefs.SetString(timeKey, timeValue);

            string winKey = $"DailyRace_{dayIndex}_IsWin";
            int winVal = isWin ? 1 : 0;
            PlayerPrefs.SetInt(winKey, winVal);

            PlayerPrefs.Save();
        }

        // Получить время последней попытки (UTC)
        public DateTime GetLastAttemptTime(int dayIndex)
        {
            string key = $"DailyRace_{dayIndex}_LastTime";
            string saved = PlayerPrefs.GetString(key, "");
            if (string.IsNullOrEmpty(saved))
                return DateTime.MinValue;

            if (DateTime.TryParse(saved, out DateTime parsed))
                return parsed;

            return DateTime.MinValue;
        }

        // Узнать, выигран ли день
        public bool IsDailyRaceWin(int dayIndex)
        {
            string winKey = $"DailyRace_{dayIndex}_IsWin";
            int val = PlayerPrefs.GetInt(winKey, -1);
            // -1 => не играли вообще, 0 => проигрыш, 1 => победа

            return (val == 1);
        }

        public void AddCurrency(float amount)
        {
            playerData.playerCurrency += amount;
            SaveData();
        }

        public bool SpendCurrency(float amount)
        {
            if (playerData.playerCurrency >= amount)
            {
                playerData.playerCurrency -= amount;
                SaveData();
                return true; // Покупка успешна
            }
            return false; // Недостаточно средств
        }

        // Метод для добавления очков опыта
        public void AddXP(float amount)
        {
            playerData.totalPlayerXP += amount;
            SaveData();
        }

        //// Изменённый асинхронный метод Start: сначала ждём облачную синхронизацию, затем обновляем UI
        private async void Start()
        {

           /// await SyncCloudData();
           // FindObjectOfType<PlayerSettings>()?.UpdateUIToMatchSettings();
        }

        // Вспомогательный метод для вызова асинхронной синхронизации
        private async Task SyncCloudData()
        {
            await SyncWithCloudSave();
        }

        // Метод для синхронизации локальных данных с облачным сохранением.
        // Если облачные данные существуют, они перезаписывают локальные.
        // Если нет – локальные данные сохраняются в облаке.
        public async Task SyncWithCloudSave()
        {
            // Предполагаем, что у тебя в сцене есть объект CloudSaveManager
            CloudSaveManager cloudSaveManager = FindObjectOfType<CloudSaveManager>();
            if (cloudSaveManager == null)
            {
                Debug.LogWarning("CloudSaveManager не найден. Синхронизация не выполнена.");
                return;
            }

            // Пытаемся загрузить данные из облака по ключу "playerData"
            DataContainer cloudData = await cloudSaveManager.LoadGameAsync<DataContainer>("playerData");
            if (cloudData != null)
            {
                // Если облачные данные есть, обновляем локальные данные
                playerData = cloudData;
                Debug.Log("Данные из облака успешно синхронизированы с локальными данными.");
            }
            else
            {
                // Если облачных данных нет, сохраняем локальные данные в облаке
                await cloudSaveManager.SaveGameAsync("playerData", playerData);
                Debug.Log("Локальные данные успешно сохранены в облаке.");
            }

            // После синхронизации обновляем локальный файл сохранения
            SaveData();
            // Вызываем событие, чтобы уведомить UI о том, что данные обновлены
            RaiseDataLoadedEvent();
        }

        // Метод для повышения уровня
        private void LevelUp()
        {
            playerData.playerXPLevel++; // Обращаемся через playerData
            playerData.totalPlayerXP = 0; // Сбрасываем текущий опыт
            Debug.Log($"PlayerData: Уровень повышен! Новый уровень: {playerData.playerXPLevel}");
        }

        private float GetXPForNextLevel()
        {
            return playerData.playerXPLevel * 100; // Всё обращение через playerData
        }

        public void AddPlayerCurrecny(float amount)
        {
            playerData.playerCurrency += amount;
            SaveData();

            Debug.Log($"Player currency updated to: {playerData.playerCurrency}");

            var playerSettings = FindObjectOfType<PlayerSettings>();
            if (playerSettings != null)
            {
                playerSettings.UpdateUIToMatchSettings();
                Debug.Log("PlayerSettings UI updated.");
            }

            var careerManager = FindObjectOfType<CareerManager>();
            if (careerManager != null)
            {
                careerManager.UpdateEventUI();
                Debug.Log("Event UI updated.");
            }
        }

        public void AddPlayerXP(float points)
        {
            playerData.totalPlayerXP += points; // Увеличиваем общий опыт
            playerData.currentLevelXP += points; // Увеличиваем опыт на текущем уровне

            if (playerData.currentLevelXP >= playerData.nextLevelXP) // Проверка на повышение уровня
            {
                AddPlayerXPLevel();
            }

            SaveData(); // Сохранение данных
        }

        void AddPlayerXPLevel()
        {
            playerData.playerXPLevel++;
            playerData.currentLevelXP = 0;
            playerData.nextLevelXP *= playerData.nextLevelXpMultiplier;
        }

        public void UnlockItem(string itemID)
        {
            if (playerData.items.Contains(itemID))
                return;

            playerData.items.Add(itemID);

            VehicleDatabase.Instance?.UnlockVehicle(itemID);
            SaveData();
        }


        /// <summary>
        /// Разблокировать машину без списания денег.
        /// </summary>
        /// <param name="uniqueID">Уникальный идентификатор машины из VehicleDatabase.</param>
        public void UnlockCar(string uniqueID)
        {
            Debug.Log($"[UnlockCar] called for {uniqueID}");
            UnlockItem(uniqueID);
            VehicleDatabase.Instance.SyncVehicleData(this);
            if (MenuVehicleInstantiator.Instance != null)
                MenuVehicleInstantiator.Instance.RevertPlayerVehicle();
        }



        public void SavePlayerName(string newName)
        {
            playerData.playerName = newName;
            SaveData();
        }

        public void SavePlayerNationality(Nationality newNationality)
        {
            playerData.playerNationality = newNationality;
            SaveData();
        }

        public void SaveSelectedVehicle(string id)
        {
            playerData.vehicleID = id;
            SaveData();
        }

        public bool IsItemUnlocked(string id)
        {
            return playerData.items != null && playerData.items.Contains(id);
        }

        public void AddSPB(int amount)
        {
            Debug.Log($"Добавление SpeedBoost: {amount}");
            playerData.speedBoost += amount;
            SaveData();
            Debug.Log($"Новый баланс SpeedBoost: {playerData.speedBoost}");
        }

        public int GetSPB()
        {
            return (int)playerData.speedBoost;
        }

        public void AddItem(string item)
        {
            if (playerData.items == null)
            {
                playerData.items = new List<string>();
            }

            if (!playerData.items.Contains(item))
            {
                playerData.items.Add(item);
                SaveData();
                Debug.Log($"PlayerData: Предмет '{item}' добавлен в список разблокированных.");
            }
            else
            {
                Debug.Log($"PlayerData: Предмет '{item}' уже существует в списке.");
            }
        }

        public void UnlockStage(string stageName)
        {
            if (!playerData.unlockedStages.Contains(stageName))
            {
                playerData.unlockedStages.Add(stageName);
                SaveData();
                Debug.Log($"PlayerData: Stage {stageName} сохранён как разблокированный. Все разблокированные уровни: {string.Join(", ", playerData.unlockedStages)}");
            }
            else
            {
                Debug.Log($"PlayerData: Stage {stageName} уже разблокирован.");
            }
        }

        /// <summary>
        /// Сохраняет результат одного раунда чемпионата отдельно от обычных гонок.
        /// </summary>
        public void SaveChampionshipRoundResult(string roundID)
        {
            if (!playerData.completedChampionshipRounds.Contains(roundID))
            {
                playerData.completedChampionshipRounds.Add(roundID);
                SaveData();
                Debug.Log($"Championship round '{roundID}' saved. Total: {playerData.completedChampionshipRounds.Count}");
            }
            else
            {
                Debug.Log($"Championship round '{roundID}' was already completed.");
            }
        }

        /// <summary>
        /// Проверяет, завершён ли раунд чемпионата.
        /// </summary>
        public bool IsChampionshipRoundCompleted(string roundID)
        {
            return playerData.completedChampionshipRounds.Contains(roundID);
        }

        public void SaveData()
        {
            try
            {
                Debug.Log("SaveData: Сохраняем данные игрока...");
                Debug.Log($"Unlocked Stages Before Save: {string.Join(", ", playerData.unlockedStages)}");

                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(Application.persistentDataPath + "/savefile.dat");

                // Копируем все поля в временный объект DataContainer
                DataContainer data = new DataContainer
                {
                    playerName = playerData.playerName,
                    playerNationality = playerData.playerNationality,
                    playerCurrency = playerData.playerCurrency,
                    playerXPLevel = playerData.playerXPLevel,
                    totalPlayerXP = playerData.totalPlayerXP,
                    currentLevelXP = playerData.currentLevelXP,
                    nextLevelXP = playerData.nextLevelXP,
                    items = new List<string>(playerData.items),
                    vehicleID = playerData.vehicleID,
                    unlockedStages = new List<string>(playerData.unlockedStages),
                    speedBoost = playerData.speedBoost,
                    completedRaces = new List<string>(playerData.completedRaces),
                    vehicleColors = new List<SavedVehicleColor>(playerData.vehicleColors), // <-- ДОДАНО
                    adsRemoved = playerData.adsRemoved,
                    completedChampionshipRounds = new List<string>(playerData.completedChampionshipRounds),




                };

                bf.Serialize(file, data);
                file.Close();

                Debug.Log($"PlayerData успешно сохранён. Unlocked Stages After Save: {string.Join(", ", data.unlockedStages)}");
            }
            catch (Exception e)
            {
                Debug.LogError("Error saving data: " + e.Message);
            }
        }

        public void LoadData()
        {
            if (!File.Exists(Application.persistentDataPath + "/savefile.dat"))
            {
                Debug.LogWarning("Save file not found, resetting data to defaults.");
                ResetData();
                return;
            }

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/savefile.dat", FileMode.Open);

                DataContainer data = (DataContainer)bf.Deserialize(file);
                file.Close();

                playerData.playerName = data.playerName;
                playerData.playerNationality = data.playerNationality;
                playerData.playerCurrency = data.playerCurrency;
                playerData.playerXPLevel = data.playerXPLevel;
                playerData.totalPlayerXP = data.totalPlayerXP;
                playerData.currentLevelXP = data.currentLevelXP;
                playerData.nextLevelXP = data.nextLevelXP;
                playerData.items = new List<string>(data.items);
                playerData.vehicleID = data.vehicleID;
                playerData.unlockedStages = data.unlockedStages;
                playerData.speedBoost = data.speedBoost;
                playerData.completedRaces = new List<string>(data.completedRaces);
                playerData.adsRemoved = data.adsRemoved;
                playerData.completedChampionshipRounds = new List<string>(data.completedChampionshipRounds);



                // <-- ВОТ! Загружаем массив цветов
                playerData.vehicleColors = new List<SavedVehicleColor>(data.vehicleColors);

                Debug.Log($"PlayerData успешно загружен. Unlocked Stages After Load: {string.Join(", ", playerData.unlockedStages)}");
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading data: " + e.Message);
            }
        }

        public void ResetData()
        {
            playerData.playerName = defaultValues.playerName;
            playerData.playerNationality = Nationality.Other;
            playerData.playerCurrency = defaultValues.playerCurrency;
            playerData.totalPlayerXP = defaultValues.totalPlayerXP;
            playerData.playerXPLevel = defaultValues.playerXPLevel;
            playerData.currentLevelXP = defaultValues.currentLevelXP;
            playerData.nextLevelXP = defaultValues.nextLevelXP;
            playerData.nextLevelXpMultiplier = defaultValues.nextLevelXpMultiplier;
            playerData.speedBoost = defaultValues.speedBoost;
            playerData.completedRaces = new List<string>(defaultValues.completedRaces);
            PlayerPrefs.DeleteKey("MachineIntroShown");

            // Полностью очищаем список разблокированных предметов
            playerData.items = new List<string>();

            // Уникальный идентификатор автомобиля, который разблокирован по умолчанию
            string defaultVehicleID = "golf";
            if (!playerData.items.Contains(defaultVehicleID))
            {
                playerData.items.Add(defaultVehicleID);
                Debug.Log($"Машина с уникальным ID '{defaultVehicleID}' разблокирована по умолчанию.");
            }

            if (!playerData.unlockedStages.Contains("Sezon1"))
                playerData.unlockedStages.Add("Sezon1");

            foreach (var item in defaultValues.items)
            {
                if (!playerData.items.Contains(item))
                {
                    playerData.items.Add(item);
                }
            }

            playerData.vehicleID = defaultValues.vehicleID;

            // <-- Сбрасываем цвета автомобилей
            playerData.vehicleColors = new List<SavedVehicleColor>();

            SaveData();

            if (GlobalSettings.Instance.vehicleDatabase != null)
            {
                GlobalSettings.Instance.vehicleDatabase.SyncVehicleData(this);
            }

            Debug.Log("Player data and vehicle data have been reset!");
        }




        // ======================= ДОБАВЛЕННЫЕ НОВЫЕ МЕТОДЫ =======================

        /// <summary>
        /// Сохраняет цвет для конкретной машины в списке vehicleColors (в HEX).
        /// </summary>
        public void SetVehicleColor(string vehicleID, Color color)
        {
            if (playerData.vehicleColors == null)
                playerData.vehicleColors = new List<SavedVehicleColor>();

            // Конвертируем цвет в HEX (без "#")
            string hexColor = ColorUtility.ToHtmlStringRGB(color);

            // Ищем, есть ли уже запись для этого vehicleID
            var existing = playerData.vehicleColors.Find(c => c.vehicleID == vehicleID);
            if (existing != null)
            {
                existing.colorHEX = hexColor;
            }
            else
            {
                // Добавляем новую запись
                SavedVehicleColor newColorEntry = new SavedVehicleColor
                {
                    vehicleID = vehicleID,
                    colorHEX = hexColor
                };
                playerData.vehicleColors.Add(newColorEntry);
            }

            // Сохраняем файл
            SaveData();
        }

        /// <summary>
        /// Загружает цвет для конкретной машины. Если не найден — возвращаем defaultColor.
        /// </summary>
        public Color GetVehicleColor(string vehicleID, Color defaultColor)
        {
            if (playerData.vehicleColors == null)
                playerData.vehicleColors = new List<SavedVehicleColor>();

            var existing = playerData.vehicleColors.Find(c => c.vehicleID == vehicleID);
            if (existing != null)
            {
                if (ColorUtility.TryParseHtmlString("#" + existing.colorHEX, out Color c))
                {
                    return c;
                }
                else
                {
                    Debug.LogWarning($"[PlayerData] Ошибка парсинга цвета {existing.colorHEX} для {vehicleID}. Возвращаем defaultColor.");
                    return defaultColor;
                }
            }

            return defaultColor; // Если нет записи, возвращаем дефолт
        }

        // Удаление файла с сохраненными данными
        public void DeleteSaveFile()
        {
            if (File.Exists(Application.persistentDataPath + "/savefile.dat"))
            {
                File.Delete(Application.persistentDataPath + "/savefile.dat");
            }
        }
    }

    /// <summary>
    /// Класс-структура для хранения связи vehicleID -> HEX-цвет.
    /// </summary>
    [Serializable]
    public class SavedVehicleColor
    {
        public string vehicleID;
        public string colorHEX;
    }

    [Serializable]
    public class DataContainer
    {
        public string playerName;
        public Nationality playerNationality;
        public List<string> unlockedStages = new List<string>();
        public float playerCurrency;
        public float totalPlayerXP;
        public int playerXPLevel;
        public float currentLevelXP;
        public float nextLevelXP;
        public float nextLevelXpMultiplier;
        public List<string> items = new List<string>();
        public string vehicleID;
        public int speedBoost;
        public List<string> completedRaces = new List<string>();
        // Для результатов чемпионата
        public List<string> completedChampionshipRounds = new List<string>();
        public bool adsRemoved;



        // <-- ДОБАВЛЕНО: теперь храним цвета всех машин (через список).
        public List<SavedVehicleColor> vehicleColors = new List<SavedVehicleColor>();
    }
}
