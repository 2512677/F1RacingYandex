using UnityEngine;
using System.Collections;
using I2.Loc;

namespace RGSK
{
    public class RaceResultsPanel : RaceEntry
    {
        public void UpdateRaceResults()
        {
            if (RaceManager.instance == null)
                return;

            bool isChase = RaceManager.instance.raceType == RaceType.Chase;
            // Локализованное слово «Пойман»
            string capturedInfo = LocalizationManager.GetTranslation("RacePanel/CapturedInfo");
            // Суффикс для Total Time
            string capturedSuffix = " (" + capturedInfo + ")";

            for (int i = 0; i < RaceManager.instance.racerList.Count; i++)
            {
                if (i > raceEntry.Count - 1)
                    break;

                // ───── позиция ─────
                if (raceEntry[i].position != null)
                    raceEntry[i].position.text = RaceManager.instance.racerList[i].Position.ToString();

                // ───── имя гонщика ─────
                if (raceEntry[i].name != null)
                {
                    // Просто имя без суффикса
                    raceEntry[i].name.text = RaceManager.instance.racerList[i].GetName();
                }

                // ───── имя машины ─────
                if (raceEntry[i].vehicle != null)
                    raceEntry[i].vehicle.text = RaceManager.instance.racerList[i].GetVehicle();

                // ───── флаг страны ─────
                if (raceEntry[i].nationality != null)
                {
                    raceEntry[i].nationality.sprite =
                        Helper.GetCountryFlag(RaceManager.instance.racerList[i].GetNationality());
                    raceEntry[i].nationality.enabled = raceEntry[i].nationality.sprite != null;
                }

                // ───── лучший круг ─────
                if (raceEntry[i].bestLap != null)
                    raceEntry[i].bestLap.text =
                        RaceManager.instance.racerList[i].bestLapTime > 0
                            ? Helper.FormatTime(RaceManager.instance.racerList[i].bestLapTime)
                            : "--:--.---";

                // ───── Total Time / DNF / Пойман ─────
                if (raceEntry[i].totalTime != null)
                {
                    if (RaceManager.instance.racerList[i].disqualified)
                        // для погони показываем «Пойман», иначе DNF
                        raceEntry[i].totalTime.text = isChase
                            ? capturedInfo
                            : "DNF";
                    else
                        raceEntry[i].totalTime.text = RaceManager.instance.racerList[i].finished
                            ? Helper.FormatTime(RaceManager.instance.racerList[i].totalRaceTime)
                            : "--:--.---";
                }

                // ───── отставание ─────
                if (raceEntry[i].gap != null)
                    raceEntry[i].gap.text =
                        RaceManager.instance.racerList[i].finished
                            ? "+ " + Helper.FormatTime(
                                RaceManager.instance.racerList[i].totalRaceTime -
                                RaceManager.instance.racerList[0].totalRaceTime,
                                TimeFormat.SecMs)
                            : "--:--.---";

                // ───── SpeedTrap ─────
                if (RaceManager.instance.raceType == RaceType.SpeedTrap &&
                    raceEntry[i].speedtrapSpeed != null)
                {
                    raceEntry[i].speedtrapSpeed.text =
                        RaceManager.instance.racerList[i].totalSpeed.ToString("F1")
                        + RaceManager.instance.speedUnit.ToString();
                }
            }
        }
    }
}