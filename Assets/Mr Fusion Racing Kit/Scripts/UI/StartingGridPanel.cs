using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class StartingGridPanel : RaceEntry
    {
        public void UpdateStartingGrid()
        {
			if (RaceManager.instance == null)
                return;

			//Only update the starting grid when the race has not started
			if (RaceManager.instance.raceStarted)
                return;

			for (int i = 0; i < RaceManager.instance.racerList.Count; i++)
            {
                if (i > raceEntry.Count - 1)
                    break;

                //Display the racer's position
                if (raceEntry[i].position != null)
                {
					raceEntry[i].position.text = RaceManager.instance.racerList[i].Position.ToString();
                }

                //Display the racer's name
                if (raceEntry[i].name != null)
                {
					raceEntry[i].name.text = RaceManager.instance.racerList[i].GetName();
                }

                //Display the racer's vehicle
                if (raceEntry[i].vehicle != null)
                {
					raceEntry[i].vehicle.text = RaceManager.instance.racerList[i].GetVehicle();
                }

                //Display the racer's country flag
                if (raceEntry[i].nationality != null)
                {
					raceEntry[i].nationality.sprite = Helper.GetCountryFlag(RaceManager.instance.racerList[i].GetNationality());

                    if(raceEntry[i].nationality.sprite != null)
                        raceEntry[i].nationality.enabled = true;
                }
            }
        }
    }
}