using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RGSK
{
    public class DriftPanel : MonoBehaviour
    {
        private DriftPointsManager driftPointController;
        public Text currentDriftPoints; //Text that shows the players current drift points (while drifting)
        public Text totalDriftPoints; //Text that shows the players total drift points
        public Text driftMultiplier; //Text that shows the player's point multiplier value
        public Text driftInfo; //Text that shows Completed & Failed drift information


        void Start()
        {
            //Clear the assigned texts
            if (totalDriftPoints != null)
            {
                totalDriftPoints.text = "Points:";
            }

            if (currentDriftPoints != null)
            {
                currentDriftPoints.text = string.Empty;
            }

            if (driftMultiplier != null)
            {
                driftMultiplier.text = string.Empty;
            }

            if (driftInfo != null)
            {
                driftInfo.text = string.Empty;
            }
        }


        void Update()
        {
            if (driftPointController == null)
            {
                FindDriftPointController();
            }

            UpdateCurrentDriftPoints();
        }


        void UpdateCurrentDriftPoints()
        {
            if (driftPointController == null)
                return;

            //If the vehicle is drifting, update the CurrentDriftPoints text to display how many
            //points the player has accumulated
            if (driftPointController.drifting)
            {
                if (currentDriftPoints != null && driftPointController.currentDriftPoints > 0)
                {
                    currentDriftPoints.text = "+ " + driftPointController.currentDriftPoints.ToString("N0");
                }
            }
        }


        public void UpdateTotalDriftPoints()
        {
            if (driftPointController == null)
                return;

            //After completing a drift, update all the neccessary texts with the player's points
            if (totalDriftPoints)
                totalDriftPoints.text = "Points: " + driftPointController.totalDriftPoints.ToString("N0");

            if (currentDriftPoints)
                currentDriftPoints.text = string.Empty;

            if (driftMultiplier)
                driftMultiplier.text = string.Empty;

            if (driftInfo)
            {
                //Show that the player has completed the drift
                driftInfo.color = Color.green;
                driftInfo.text = "Drift Complete\n+" + driftPointController.currentDriftPoints.ToString("N0") + " pts";
                Invoke("ClearDriftInfo", 2);
            }
        }


        public void UpdateDriftMultipier()
        {
            if (driftPointController == null)
                return;

            //Update the DriftMultiplier text whenever the player achieves a point multipier
            if (driftMultiplier)
                driftMultiplier.text = "x" + driftPointController.driftMultiplier;
        }


        public void UpdateDriftFailInfo()
        {
            if (driftInfo)
            {
                //Show that the player has failed the drift
                driftInfo.color = Color.red;
                driftInfo.text = "Drift Failed";
                Invoke("ClearDriftInfo", 2);
            }

            //Clear the point & multiplier texts
            if (currentDriftPoints != null)
            {
                currentDriftPoints.text = string.Empty;
            }

            if (driftMultiplier != null)
            {
                driftMultiplier.text = string.Empty;
            }
        }


        void ClearDriftInfo()
        {
            driftInfo.text = string.Empty;
        }


        void FindDriftPointController()
        {
            if (driftPointController != null)
                return;

            if (FindObjectOfType<DriftPointsManager>())
            {
                driftPointController = FindObjectOfType<DriftPointsManager>();
            }
        }


        void OnEnable()
        {
            RaceManager.OnVehicleSpawn += FindDriftPointController;
        }


        void Disable()
        {
            RaceManager.OnVehicleSpawn -= FindDriftPointController;
        }
    }
}