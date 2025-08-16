using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class RaceRewardsPanel : MonoBehaviour
    {
        [Header("UI Text fields for showing rewards")]
        public Text moneyText;
        public Text xpText;
        public Text speedBoostText;
        public Text itemsText;

        [Header("Buttons")]
        public Button doubleRewardButton;   // Кнопка "2X"
        public Button collectButton;        // Кнопка "Collect" / "ОК"

        // Переменные для расчёта награды
        private int baseReward = 0;         // Базовая награда (если нужна другая логика)
        private int currentReward = 0;      // Текущая «отображаемая» награда (для денег)
        private bool wasDoubled = false;    // Флаг, что награда удвоена

        // Сохраним базовые данные награды, полученные из RaceRewards.Instance
        private int baseCurrency;
        private int baseXp;
        private int baseSpeedBoost;
        private string[] baseItems;

        // Флаг, чтобы награда не начислилась дважды
        private bool rewardTaken = false;

        // Ссылка на контроллер рекламы
       // private CSharpSampleController adController;

        private void OnEnable()
        {
            RaceRewards.Instance.RefreshRewards();           // ★ тянем актуальный пакет наград

            //     ▼ добавь эти две строки прямо следом ▼
            if (ChampionshipData.pendingRewards != null)    // чемпионский пакет только что взяли
                ChampionshipData.pendingRewards = null;     // «съедаем» его, чтобы не тянулся дальше

            // Вычисляем базовую награду – пример: 5000 (можно заменить своей логикой)
            baseReward = CalculateRaceReward();
            currentReward = baseReward;
            wasDoubled = false;

            // Сбрасываем UI, чтобы до загрузки всё не показывалось некорректно
            moneyText.text = currentReward.ToString();
            xpText.text = "0";
            speedBoostText.text = "0";
            itemsText.text = "";

            // Ищем контроллер рекламы
          //  adController = FindObjectOfType<CSharpSampleController>();
           // if (adController != null)
            {
                // Подписываемся на событие, когда реклама досмотрена
             //   adController.onUserRewardEarned += OnRewardedAdCompleted;
            }
            
            {
                Debug.LogWarning("RaceRewardsPanel: CSharpSampleController не найден в сцене!");
            }

            // Получаем позицию игрока из RaceManager и подготавливаем UI награды
            if (RaceManager.instance != null && RaceManager.instance.playerStatistics != null)
            {
                int playerPosition = RaceManager.instance.playerStatistics.Position;
                PrepareRewardsUI(playerPosition);
            }
            else
            {
                Debug.LogWarning("RaceRewardsPanel: RaceManager или playerStatistics не инициализированы.");
                moneyText.text = "0";
                xpText.text = "0";
                speedBoostText.text = "0";
                itemsText.text = "";
            }

            // Сброс флагов
            wasDoubled = false;
            rewardTaken = false;
        }

        private void OnDisable()
        {
            // Отписываемся от события, чтобы не было утечек памяти
          
        }

        /// <summary>
        /// Метод для кнопки "2X". Вызывает показ рекламы.
        /// </summary>
        public void OnClick_DoubleReward()
        {
            
            {
                Debug.LogWarning("OnClick_DoubleReward: adController не найден.");
                return;
            }

            if (wasDoubled)
            {
                Debug.Log("Награда уже удвоена, повторное удвоение не требуется.");
                return;
            }

            // Показываем rewarded-рекламу. Если пользователь досмотрит её до конца, сработает событие.
            
        }

        /// <summary>
        /// Событие, вызываемое после успешного просмотра рекламы.
        /// Здесь мы только обновляем отображаемую сумму (для денег).
        /// XP и Speed Boost не удваиваем.
        /// Панель при этом не закрывается.
        /// </summary>
        private void OnRewardedAdCompleted()
        {
            if (!wasDoubled)
            {
                wasDoubled = true;
                currentReward = baseCurrency * 2;
                moneyText.text = currentReward.ToString();
                Debug.Log($"Награда удвоена на экране: {baseCurrency} -> {currentReward}");
            }
        }

        /// <summary>
        /// Метод для кнопки "Collect" / "ОК". Начисляет награду игроку и закрывает панель.
        /// </summary>
        /// <summary>
        /// Метод для кнопки "Collect" / "ОК". Начисляет награду игроку, разблокирует выигранные машины и закрывает панель.
        /// </summary>
        /// <summary>
        /// Метод для кнопки "Collect" / "ОК". Начисляет награду игроку, разблокирует выигранные машины,
        /// помечает гонку как завершённую и закрывает панель.
        /// </summary>
        /// <summary>
        /// Кнопка Collect / OK: начисляем награды, разблокируем машины, сохраняем прогресс и сбрасываем RaceID,
        /// чтобы при повторном заходе в ту же гонку не думало, что награды уже были взяты.
        /// </summary>
        /// <summary>
        /// Метод для кнопки "Collect" / "ОК". Начисляет награду игроку, разблокирует выигранные машины,
        /// сохраняет прогресс гонки только при 1-м месте и закрывает панель.
        /// </summary>
        public void OnClick_CollectReward()
        {
            if (rewardTaken)
            {
                Debug.Log("Награда уже выдана! Повторная выдача отменена.");
                return;
            }
            rewardTaken = true;

            // 0) DNF
            bool disqualified = RaceManager.instance.playerStatistics.disqualified;
            bool awardDNF = RaceRewards.Instance.awardDNF;
            if (disqualified && !awardDNF)
            {
                Debug.Log("Игрок дисквалифицирован (DNF): награды не начисляются.");
                ChampionshipData.pendingRewards = null;
                gameObject.SetActive(false);
                return;
            }

            // 1) Проверка дубликата
            string currentRaceID = PlayerPrefs.GetString("CurrentRaceID", "");
            if (!string.IsNullOrEmpty(currentRaceID) &&
                PlayerData.instance.playerData.completedRaces.Contains(currentRaceID))
            {
                Debug.Log($"Награды за гонку '{currentRaceID}' уже получены. Панель закрывается.");
                PlayerPrefs.DeleteKey("CurrentRaceID");
                PlayerPrefs.Save();
                gameObject.SetActive(false);
                return;
            }

            // 2) Начисляем деньги, XP, SPB
            int finalCurrency = wasDoubled ? baseCurrency * 2 : baseCurrency;
            PlayerData.instance.AddPlayerCurrecny(finalCurrency);
            PlayerData.instance.AddXP(baseXp);
            PlayerData.instance.AddSPB(baseSpeedBoost);

            // 3) Разблокировка машин
            if (baseItems != null && baseItems.Length > 0)
            {
                foreach (var id in baseItems)
                {
                    PlayerData.instance.UnlockItem(id);
                    Debug.Log($"Unlocked item from reward panel: {id}");
                }
                VehicleDatabase.Instance.SyncVehicleData(PlayerData.instance);
            }

            // 4) Сохранение результата только при 1-м месте
            int playerPosition = RaceManager.instance.playerStatistics.Position;
            if (playerPosition == 1)
            {
                if (!string.IsNullOrEmpty(currentRaceID))
                    PlayerData.instance.SaveRaceResult(currentRaceID);
                Debug.Log("Гонка пройдена (1 место): следующий уровень разблокируется.");
            }
            else
                Debug.Log($"Гонка не засчитана (место {playerPosition}): уровень не разблокируется.");

            // 5) Сброс RaceID и обновление меню
            PlayerPrefs.DeleteKey("CurrentRaceID");
            PlayerPrefs.Save();
            var playerSettings = FindObjectOfType<PlayerSettings>();
            if (playerSettings != null)
                playerSettings.UpdateUIToMatchSettings();

            Debug.Log($"Выдали награду: {finalCurrency} монет, {baseXp} XP, +{baseSpeedBoost} SP (удвоение: {wasDoubled}).");

            // **Диагностический лог**:
            Debug.Log("OnClick_CollectReward: до вызова SetActive(false)");

            // 6) Закрываем панель
            gameObject.SetActive(false);
            Debug.Log("OnClick_CollectReward: после вызова SetActive(false)");
        }






        /// <summary>
        /// Альтернативный вариант для выдачи базовой награды (1х).
        /// </summary>
        public void OnClick_CollectBaseReward()
        {
            if (!rewardTaken)
            {
                rewardTaken = true;
                GiveReward(baseCurrency, baseXp, baseSpeedBoost, baseItems);
                ClosePanel();
            }
        }

        /// <summary>
        /// Альтернативный вариант выдачи награды после досмотра рекламы.
        /// Здесь происходит начисление удвоенной награды и обновление UI, но панель не закрывается автоматически.
        /// </summary>
        private void OnUserRewardEarned()
        {
            if (!rewardTaken)
            {
                wasDoubled = true;
                rewardTaken = true;
                currentReward = baseCurrency * 2;
                moneyText.text = currentReward.ToString();
                Debug.Log($"Награда удвоена (OnUserRewardEarned): {currentReward} монет");
                // Здесь панель остаётся открытой – игрок должен нажать Collect
            }
        }

        /// <summary>
        /// Метод для вычисления базовой награды (можно заменить своей логикой).
        /// </summary>
        private int CalculateRaceReward()
        {
            // В данном примере просто возвращаем 5000,
            // но вы можете изменить расчёт в зависимости от позиции и пр.
            return 5000;
        }

        /// <summary>
        /// Подготавливает UI с наградой, полученной из RaceRewards.Instance, по позиции игрока.
        /// </summary>
        private void PrepareRewardsUI(int playerPosition)
        {
            // 1. Проверка дисквалификации (DNF)
            bool disqualified = RaceManager.instance.playerStatistics.disqualified;
            bool awardDNF = RaceRewards.Instance.awardDNF;

            // Если игрок дисквалифицирован и awardDNF = false, сразу обнуляем UI
            if (disqualified && !awardDNF)
            {
                Debug.Log("DNF: награда обнулена в UI!");
                baseCurrency = 0;
                baseXp = 0;
                baseSpeedBoost = 0;
                baseItems = null;

                moneyText.text = "0";
                xpText.text = "0";
                speedBoostText.text = "0";
                itemsText.text = "";
                return;
            }

            // 2. Проверка Time Attack: если позиция > 3, обнуляем награду в UI
            if (RaceManager.instance.raceType == RaceType.TimeAttack && playerPosition > 3)
            {
                Debug.Log("Time Attack: игрок занял место 4 или хуже ⇒ награда = 0");
                baseCurrency = 0;
                baseXp = 0;
                baseSpeedBoost = 0;
                baseItems = null;

                moneyText.text = "0";
                xpText.text = "0";
                speedBoostText.text = "0";
                itemsText.text = "";
                return;
            }

            // 3. Проверка, есть ли награды
            var rewardsArray = RaceRewards.Instance.currentRewards;
            if (rewardsArray == null || rewardsArray.Length == 0)
            {
                Debug.LogWarning("RaceRewardsPanel: нет наград в currentRewards.");
                moneyText.text = "0";
                xpText.text = "0";
                speedBoostText.text = "0";
                itemsText.text = "";
                return;
            }

            // 4. Проверка границ
            if (playerPosition < 1 || playerPosition > rewardsArray.Length)
            {
                Debug.LogWarning($"RaceRewardsPanel: playerPosition {playerPosition} выходит за границы наград.");
                moneyText.text = "0";
                xpText.text = "0";
                speedBoostText.text = "0";
                itemsText.text = "";
                return;
            }

            // 5. Берём награду из массива
            var reward = rewardsArray[playerPosition - 1];

            baseCurrency = reward.currency;
            baseXp = reward.xp;
            baseSpeedBoost = reward.speedBoost;
            baseItems = reward.items;

            // 6. Отображаем в UI
            moneyText.text = baseCurrency.ToString();
            xpText.text = baseXp.ToString();
            speedBoostText.text = baseSpeedBoost.ToString();

            if (baseItems != null && baseItems.Length > 0)
                itemsText.text = string.Join(", ", baseItems);
            else
                itemsText.text = "";
        }


        /// <summary>
        /// Фактическое начисление награды в PlayerData.
        /// </summary>
        private void GiveReward(int currency, int xp, int speedBoost, string[] items)
        {
            PlayerData.instance.AddPlayerCurrecny(currency);
            PlayerData.instance.AddXP(xp);
            PlayerData.instance.AddSPB(speedBoost);

            if (items != null && items.Length > 0)
            {
                foreach (var item in items)
                {
                    PlayerData.instance.AddItem(item);
                }
            }

            PlayerData.instance.SaveData();

            Debug.Log($"Выдали награду: {currency} монет, {xp} XP, +{speedBoost} SP");
        }

        private void ClosePanel()
        {
            gameObject.SetActive(false);
        }
    }
}
