using UnityEngine;
using System.Collections.Generic;

namespace RGSK
{
    // Атрибут позволяет создавать объект данных чемпионата через меню в Unity Editor
    [CreateAssetMenu(fileName = "Новый чемпионат", menuName = "MRFE/New Championship Data", order = 1)]
    public class ChampionshipData : ScriptableObject
    {
        public static RaceRewards.Rewards[] pendingRewards;

        public string championshipID;                 // Уникальный ID чемпионата
        public string championshipName;               // Название чемпионата
        public string reward;
        public string carName;                        // Сумма/авто
        public Sprite championshipImage;              // Изображение чемпионата для UI
        public Sprite raceTypeIcon;
        public Sprite championshipIcon;               // Иконка чемпионата
        public List<ChampionshipRound> championshipRounds = new List<ChampionshipRound>(); // Раунды
        public List<ChampionshipRacer> championshipRacers = new List<ChampionshipRacer>(); // Участники
        public int[] championshipPoints;              // Очки
        public CarClass.VehicleClass requiredCarClass;

        [Header("Награды")]
        public List<RaceRewards.Rewards> raceRewards;

        // ───────────────────────────────────────────────────────────────────────────────
        // ★ Требование физики (RCC v4): вместо устаревшего RCC_Settings.BehaviorType.*
        // ───────────────────────────────────────────────────────────────────────────────
        [Header("Physics Behavior Requirement (Optional)")]
        [Tooltip("Если true, при входе в чемпионат будет автоматически установлен нужный профиль физики RCC v4.")]
        public bool requireBehaviorType = false;

        [Tooltip("Имя профиля из RCC_Settings.behaviorTypes[].behaviorName (например: \"Simulator\", \"Arcade\", \"Racing\"). " +
                 "Если индекс < 0, используется это поле.")]
        public string requiredBehaviorName = "Racing";

        [Tooltip("Если >= 0, имеет приоритет над именем и будет использован как behaviorSelectedIndex в RCC_Settings.")]
        public int requiredBehaviorIndex = -1;
    }
}
