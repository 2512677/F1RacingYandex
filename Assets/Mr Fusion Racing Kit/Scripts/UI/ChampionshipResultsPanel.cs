using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RGSK
{
    public class ChampionshipResultsPanel : RaceEntry
    {
        public Text championshipRound;

        void Start()
        {
            if(championshipRound != null)
            {
                if (ChampionshipManager.instance != null)
                {
                    championshipRound.text = ChampionshipManager.instance.roundIndex + 1 + "/" + ChampionshipManager.instance.championshipRounds.Count;
                }
            }
        }


        public void UpdateChampionshipResults()
        {
			if (RaceManager.instance == null)
                return;

			for (int i = 0; i < RaceManager.instance.championshipList.Count; i++)
            {
                if (i > raceEntry.Count - 1)
                    break;

                //Racer position text
                if (raceEntry[i].position != null)
                {
					raceEntry[i].position.text = RaceManager.instance.championshipList[i].championshipPosition.ToString();
                }

                //Racer name text
                if (raceEntry[i].name != null)
                {
					raceEntry[i].name.text = RaceManager.instance.championshipList[i].racerInformation.racerName;
                }

                //Vehicle name text
                if (raceEntry[i].vehicle != null)
                {
					raceEntry[i].vehicle.text = RaceManager.instance.championshipList[i].GetVehicle();
                }

                //Racer nationality
                if (raceEntry[i].nationality != null)
                {
					raceEntry[i].nationality.sprite = Helper.GetCountryFlag(RaceManager.instance.championshipList[i].GetNationality());

                    if (raceEntry[i].nationality.sprite != null)
                        raceEntry[i].nationality.enabled = true;
                }

                //Total points
                if (raceEntry[i].totalPoints != null)
                {
					raceEntry[i].totalPoints.text = RaceManager.instance.championshipList[i].championshipPoints.ToString();
                }
            }
        }
    }
}
