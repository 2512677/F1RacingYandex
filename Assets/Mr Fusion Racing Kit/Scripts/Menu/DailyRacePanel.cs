using RGSK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Скрипт DailyPanel: UI ежедневных гонок (дни 0..N),
///   • Проверяем, разблокирован ли день,
///   • Выводим таймер оставшегося времени,
///   • Логика на основе DateTime.UtcNow, не зависящая от часовых поясов.
/// </summary>
public class DailyRacePanel : MonoBehaviour
{
    [Header("UI Links")]
    public List<GameObject> dailyRaceButtons; // Кнопки
    public List<GameObject> lockPanels;       // "Замки"
    public Text[] unlockTimers;               // Тексты таймера

    /// <summary>
    /// Чтобы не обновлять таймер 60 раз в секунду,
    /// заведём счётчик, который раз в 1 секунду будет вызывать UpdateDailyStatus.
    /// </summary>
    private float refreshTimer = 0f;
    private const float REFRESH_INTERVAL = 1f;

    /// <summary>
    /// В методе Update() раз в кадр прибавляем deltaTime
    /// и когда счётчик достигнет 1 секунды — обновляем всё.
    /// </summary>
    void Update()
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= REFRESH_INTERVAL)
        {
            refreshTimer = 0f;
            UpdateDailyStatus();
        }
    }

    /// <summary>
    /// Обновляем статус всех дней: если заблокировано — показываем "HH:MM:SS",
    /// если разблокировано — очищаем таймер,
    /// если предыдущий не выигран — показываем "Заблокировано".
    /// </summary>
    private void UpdateDailyStatus()
    {
        for (int i = 0; i < dailyRaceButtons.Count; i++)
        {
            bool unlocked = CheckIfUnlocked(i);
            dailyRaceButtons[i].GetComponent<Button>().interactable = unlocked;
            lockPanels[i].SetActive(!unlocked);

            if (!unlocked)
            {
                TimeSpan remain = GetRemainingTime(i);
                if (remain == TimeSpan.MaxValue)
                {
                    unlockTimers[i].text = "Заблокировано";
                }
                else
                {
                    // Форматируем остаток как HH:MM:SS
                    unlockTimers[i].text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                        remain.Hours, remain.Minutes, remain.Seconds);
                }
            }
            else
            {
                unlockTimers[i].text = "";
            }
        }
    }

    /// <summary>
    /// Проверяем, доступен ли день dayIndex.
    /// Если никогда не играли:
    ///   • dayIndex=0 => открыт
    ///   • иначе нужен предыдущий Win + 24 часа
    ///
    /// Если уже играли:
    ///   • при Win больше не даём переигрывать,
    ///   • при Lose ждём 24 часа.
    /// </summary>
    private bool CheckIfUnlocked(int dayIndex)
    {
        DateTime lastTry = PlayerData.instance.GetLastAttemptTime(dayIndex);
        bool isWin = PlayerData.instance.IsDailyRaceWin(dayIndex);

        if (lastTry == DateTime.MinValue)
        {
            // Никогда не играли
            if (dayIndex == 0)
            {
                // Первый день — открыт
                return true;
            }
            else
            {
                // Нужно, чтобы предыдущий день был выигран + прошли сутки
                DateTime prevLast = PlayerData.instance.GetLastAttemptTime(dayIndex - 1);
                bool prevWin = PlayerData.instance.IsDailyRaceWin(dayIndex - 1);
                if (!prevWin) return false;

                DateTime unlockTime = prevLast.AddHours(24);
                return (DateTime.UtcNow >= unlockTime);
            }
        }
        else
        {
            // День уже пытались
            if (isWin)
            {
                // Если выигран — повторно не даём играть
                return false;
            }
            else
            {
                // Проигран => ждём 24 часа 
                DateTime unlockTime = lastTry.AddHours(24);
                return (DateTime.UtcNow >= unlockTime);
            }
        }
    }

    /// <summary>
    /// Сколько времени осталось до разблокировки.
    /// • Если уже unlocked => 0.
    /// • Если день "пройден (Win)" => MaxValue (не показываем таймер).
    /// • Иначе unlockTime - UtcNow, не меньше нуля.
    /// </summary>
    private TimeSpan GetRemainingTime(int dayIndex)
    {
        if (CheckIfUnlocked(dayIndex))
            return TimeSpan.Zero;

        DateTime lastTry = PlayerData.instance.GetLastAttemptTime(dayIndex);
        bool isWin = PlayerData.instance.IsDailyRaceWin(dayIndex);

        if (lastTry == DateTime.MinValue)
        {
            if (dayIndex == 0)
                return TimeSpan.Zero; // особый случай

            DateTime prevLast = PlayerData.instance.GetLastAttemptTime(dayIndex - 1);
            bool prevWin = PlayerData.instance.IsDailyRaceWin(dayIndex - 1);

            if (!prevWin)
                return TimeSpan.MaxValue;

            DateTime unlockTime = prevLast.AddHours(24);
            TimeSpan r = unlockTime - DateTime.UtcNow;
            return r.TotalSeconds > 0 ? r : TimeSpan.Zero;
        }
        else
        {
            if (isWin)
            {
                return TimeSpan.MaxValue;
            }
            else
            {
                DateTime unlockTime = lastTry.AddHours(24);
                TimeSpan remain = unlockTime - DateTime.UtcNow;
                return remain.TotalSeconds > 0 ? remain : TimeSpan.Zero;
            }
        }
    }
}
