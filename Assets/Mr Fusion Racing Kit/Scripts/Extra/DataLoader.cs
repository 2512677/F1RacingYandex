using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class DataLoader : MonoBehaviour
    {
        void Awake()
        {
            LoadPlayerSettings();
            LoadRaceSettings();
        }


        public void LoadPlayerSettings()
        {
            if (PlayerData.instance == null)
                return;

            //Загрузить транспортное средство игрока
            RaceManager.instance.playerVehiclePrefab = GlobalSettings.Instance.vehicleDatabase.GetVehicle(PlayerData.instance.playerData.vehicleID).vehicle;

            //Загрузить имя игрока
            RaceManager.instance.playerName = PlayerData.instance.playerData.playerName;
            RaceManager.instance.playerNationality = PlayerData.instance.playerData.playerNationality;
        }


        public void LoadRaceSettings()
        {
            //Тип гонки нагрузки
            if (PlayerPrefs.HasKey("RaceType"))
                RaceManager.instance.raceType = (RaceType)PlayerPrefs.GetInt("RaceType");

            //Загрузка кругов
            if (PlayerPrefs.HasKey("LapCount"))
                RaceManager.instance.lapCount = PlayerPrefs.GetInt("LapCount");

            //Загрузка счетчика противников
            if (PlayerPrefs.HasKey("OpponentCount"))
                RaceManager.instance.opponentCount = PlayerPrefs.GetInt("OpponentCount");

            //Загрузить сложность ИИ
            if (PlayerPrefs.HasKey("AIDifficultyLevel"))
                RaceManager.instance.aiDifficultyLevel = (AIDifficultyLevel)PlayerPrefs.GetInt("AIDifficultyLevel");

            //Загрузка единицы скорости
            if (PlayerPrefs.HasKey("SpeedUnit"))
                RaceManager.instance.speedUnit = (SpeedUnit)PlayerPrefs.GetInt("SpeedUnit");
        }
    }
}
