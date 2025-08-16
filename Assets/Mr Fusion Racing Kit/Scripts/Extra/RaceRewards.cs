using RGSK;
using UnityEngine;

/// <summary>
/// Объединённый скрипт RaceRewards, который:
/// 1) Хранит текущие награды (currentRewards).
/// 2) Проверяет дисквалификацию игрока (DNF) и не выдаёт наград, если "awardDNF" = false.
/// 3) Слушает событие завершения гонки и автоматически вызывает GiveRewards().
/// 4) Выдаёт валюту, опыт, буст скорости, предметы и т.д.
/// </summary>
public class RaceRewards : MonoBehaviour
{
    public static RaceRewards Instance;

    // Проверять, выдавать ли награду при DNF (Disqualified/Did Not Finish)
    public bool awardDNF = false;

    // Поля для отображения выданных наград (для отладки или UI)
    public float awardedCurrency { get; private set; }
    public float awardedXP { get; private set; }
    public int awardedSpeedBoost { get; private set; }

    // Текущий массив наград (для разных мест: 1-го, 2-го и т.д.)
    public Rewards[] currentRewards;

    [System.Serializable]
    public class Rewards
    {
        public int currency;
        public int xp;
        public int speedBoost;
        public string[] items;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // ★ подписываемся на события
    private void OnEnable()
    {
        RaceManager.OnRaceStart += RefreshRewards;   // ← выбираем актуальный пакет перед каждой гонкой
        RaceManager.OnPlayerFinish += GiveRewards;
    }

    private void OnDisable()
    {
        RaceManager.OnRaceStart -= RefreshRewards;
        RaceManager.OnPlayerFinish -= GiveRewards;
    }

    private void Start()
    {
        // ★ сразу подтягиваем актуальный пакет
        RefreshRewards();
    }

    // ★ НОВЫЙ метод — выбирает актуальные pendingRewards
    public void RefreshRewards()
    {
        // 1) чемпионат-награды
        if (ChampionshipData.pendingRewards != null && ChampionshipData.pendingRewards.Length > 0)
        {
            currentRewards = ChampionshipData.pendingRewards;

            //ChampionshipData.pendingRewards = null;   // ← ДОБАВЬ ЭТУ СТРОКУ
                                                      // она «съедает» пакет сразу после копирования
            Debug.Log("RaceRewards: загружены награды чемпионата.");
            return;
        }

        // 2) карьера-награды
        if (CareerData.pendingRewards != null && CareerData.pendingRewards.Length > 0)
        {
            currentRewards = CareerData.pendingRewards;
            Debug.Log("RaceRewards: загружены награды карьеры.");
            return;
        }

        // 3) ничего нет
        currentRewards = null;
        Debug.Log("RaceRewards: доступных наград нет.");
    }



    /// <summary>
    /// Основной метод для выдачи наград игроку,
    /// вызывается при финише гонки (OnPlayerFinish).
    /// </summary>
    public void GiveRewards()
    {
        // ★ подстраховка
        RefreshRewards();

        // 1. Определяем место игрока
        int position = RaceManager.instance.playerStatistics.Position;

        // 2. Учитываем другие типы гонок (TimeAttack, Drift и т.д.)
        if (RaceManager.instance.raceType == RaceType.TimeAttack)
        {
            position = RaceManager.instance.GetTimeAttackPosition();
        }
        else if (RaceManager.instance.raceType == RaceType.Drift)
        {
            position = RaceManager.instance.GetDriftRacePosition();
        }

        // 3. Проверяем дисквалификацию.
        //    Если игрок дисквалифицирован и awardDNF = false, то награды не выдаём.
        if (RaceManager.instance.playerStatistics.disqualified && !awardDNF)
        {
            Debug.Log("Игрок дисквалифицирован (DNF). Награды не выдаются.");
            return;
        }

        // ------------------------- НОВАЯ ПРОВЕРКА ДЛЯ TIME ATTACK -------------------------
        // Если это режим Time Attack, и позиция выше 3-го места => награду не даём
        if (RaceManager.instance.raceType == RaceType.TimeAttack && position > 3)
        {
            Debug.Log("Time Attack: игрок занял место 4 или хуже, награда = 0");
            return;
        }
        // ----------------------------------------------------------------------------------

        // 4. Проверяем, что массив наград не пуст и позиция игрока попадает в его диапазон
        if (currentRewards == null || currentRewards.Length == 0)
        {
            Debug.LogWarning("currentRewards пуст — наград нет.");
            return;
        }

        // Индекс в массиве: (position - 1)
        if (position - 1 < 0 || position - 1 >= currentRewards.Length)
        {
            Debug.LogWarning($"Для позиции {position} нет награды в currentRewards.");
            return;
        }

        // 5. Получаем из массива нужную награду
        Rewards reward = currentRewards[position - 1];

        // 6. Запоминаем, сколько выдали (для UI или отладки)
        awardedCurrency = reward.currency;
        awardedXP = reward.xp;
        awardedSpeedBoost = reward.speedBoost;

        // 7. Применяем к PlayerData (при условии, что PlayerData.instance существует)
        if (PlayerData.instance != null)
        {
            // Валюта
            PlayerData.instance.AddPlayerCurrecny(awardedCurrency);

            // Опыт
            PlayerData.instance.AddPlayerXP(awardedXP);

            // Буст скорости (если у вас есть соответствующий метод в PlayerData)
            // PlayerData.instance.AddSpeedBoost(awardedSpeedBoost);

            // Если есть items, вызываем PlayerData.instance.UnlockItem(item)
            if (reward.items != null && reward.items.Length > 0)
            {
                foreach (string itemID in reward.items)
                {
                    PlayerData.instance.UnlockCar(itemID);
                }
                VehicleDatabase.Instance.SyncVehicleData(PlayerData.instance);
            }
        }
        else
        {
            Debug.LogWarning("PlayerData.instance не найден: награду некуда добавить!");
        }

        // ★ сбрасываем использованный pending-массив, чтобы не залипал
        if (currentRewards == ChampionshipData.pendingRewards)
            ChampionshipData.pendingRewards = null;
        else if (currentRewards == CareerData.pendingRewards)
            CareerData.pendingRewards = null;
    }

    // Методы для получения/установки currentRewards, если нужно извне
    public Rewards[] GetRewards()
    {
        return currentRewards;
    }

    public void SetRewards(Rewards[] rewards)
    {
        currentRewards = rewards;
    }
}
