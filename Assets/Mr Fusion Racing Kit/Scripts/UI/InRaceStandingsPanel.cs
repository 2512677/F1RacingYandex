using UnityEngine;
using System.Collections.Generic;
using I2.Loc;
using System.Linq;

namespace RGSK
{
    public class InRaceStandingsPanel : RaceEntry
    {
        public UIPositionDisplayMode positionDisplay;
        public UIGapDisplayMode gapDisplay;
        public Color playerColor = Color.green;
        public Color opponentColor = Color.white;
        private float lastUpdate;
        private float gap;
        [Header("Chase Settings")]
        // Ключ локализации из I2 (RacePanel/CapturedInfo)
        [SerializeField] private string capturedInfoKey = "RacePanel/CapturedInfo";
        // Суффикс, который будет подтягивать перевод
        private string capturedSuffix;
        void Start()
        {
            // Инициализируем суффикс из локализации
            capturedSuffix = " (" + LocalizationManager.GetTranslation(capturedInfoKey) + ")";

            // Деактивируем все записи в начале
            for (int i = 0; i < Entries.Length; i++)
                Entries[i].SetActive(false);
        }


        void Update()
        {
            if (RaceManager.instance == null)
                return;

            if (Time.time <= lastUpdate)
                return;

            lastUpdate = Time.time + 0.25f;

            // 1) получаем список, исключая игрока-полицейского в Chase
            var racers = RaceManager.instance.racerList;
            if (RaceManager.instance.raceType == RaceType.Chase)
                racers = racers.Where(r => !r.isPlayer).ToList();

            // 2) для каждой строки считаем «gap»
            for (int i = 0; i < racers.Count && i < raceEntry.Count; i++)
            {
                if (raceEntry[i].gap == null) continue;

                // пропускаем строку игрока (актуально для других режимов)
                if (racers[i] == RaceManager.instance.playerStatistics)
                {
                    raceEntry[i].gap.text = "";
                    continue;
                }

                // вычисляем разрыв
                float gapVal = (gapDisplay == UIGapDisplayMode.Time)
                    ? RaceManager.instance.GetTimeGapBetween(racers[i])
                    : RaceManager.instance.GetTrackDistanceBetween(racers[i]);

                string gapText;
                if (gapDisplay == UIGapDisplayMode.Time)
                {
                    gapText = gapVal > 0
                        ? "-" + gapVal.ToString("F1")
                        : Mathf.Abs(gapVal).ToString("F1");
                }
                else // расстояние в метрах
                {
                    int meters = Mathf.Abs(Mathf.RoundToInt(gapVal));
                    gapText = gapVal > 0
                        ? "-" + meters + "m"
                        : meters + "m";
                }

                raceEntry[i].gap.text = gapText;
            }

            // 3) скрываем лишние строки, если таблица длиннее количества ботов
            for (int i = racers.Count; i < raceEntry.Count && i < Entries.Length; i++)
                Entries[i].SetActive(false);
        }




        public void UpdateStandings()
        {
            if (RaceManager.instance == null)
                return;

            // 1) формируем список без игрока-полицейского в Chase
            var racers = RaceManager.instance.racerList;
            if (RaceManager.instance.raceType == RaceType.Chase)
                racers = racers.Where(r => !r.isPlayer).ToList();

            // 2) сортируем по позиции
            racers.Sort((a, b) => a.Position.CompareTo(b.Position));

            // 3) заполняем таблицу
            for (int i = 0; i < racers.Count && i < raceEntry.Count; i++)
            {
                // ── позиция ────────────────────────────────────────────────
                if (raceEntry[i].position != null)
                {
                    raceEntry[i].position.text =
                        (positionDisplay == UIPositionDisplayMode.Default ||
                         positionDisplay == UIPositionDisplayMode.PositionOnly)
                            ? racers[i].Position.ToString()
                            : Helper.AddOrdinal(racers[i].Position);
                }

                // ── имя + состояние ───────────────────────────────────────
                if (raceEntry[i].name != null)
                {
                    raceEntry[i].name.text = racers[i].racerInformation.racerName;
                    raceEntry[i].name.color = racers[i].isPlayer ? playerColor : opponentColor;

                    bool dq = racers[i].disqualified;
                    bool isChase = RaceManager.instance.raceType == RaceType.Chase;

                    if (dq && isChase)
                        raceEntry[i].name.text += capturedSuffix;   // «(Captured)»
                    else if (dq)
                        raceEntry[i].name.text += " (DNF)";
                }

                // ── SpeedTrap (если режим) ─────────────────────────────────
                if (RaceManager.instance.raceType == RaceType.SpeedTrap &&
                    raceEntry[i].speedtrapSpeed != null)
                {
                    raceEntry[i].speedtrapSpeed.text =
                        racers[i].totalSpeed.ToString("F1") +
                        RaceManager.instance.speedUnit;
                }

                // ── активируем строку, если была скрыта ───────────────────
                if (!Entries[i].activeSelf && racers.Count > 1)
                    Entries[i].SetActive(true);
            }

            // 4) скрываем лишние строки
            for (int i = racers.Count; i < raceEntry.Count && i < Entries.Length; i++)
                Entries[i].SetActive(false);
        }
    }
}