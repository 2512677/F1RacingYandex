using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RGSK
{
    public class TargetScorePanel : MonoBehaviour
    {
        public Text gold;
        public Text silver;
        public Text bronze;

        void Start()
        {
            UpdateScores();
        }

        public void UpdateScores()
        {
			if(RaceManager.instance != null)
            {
				RaceType raceType = RaceManager.instance.raceType;
                if(raceType != RaceType.TimeAttack && raceType != RaceType.Drift)
                {
                    gameObject.SetActive(false);
                    return;
                }

                if(gold != null)
                {
					gold.text = raceType == RaceType.TimeAttack ? Helper.FormatTime(RaceManager.instance.targetTimeGold)
						: RaceManager.instance.targetScoreGold.ToString();
                }

                if(silver != null)
                {
					silver.text = raceType == RaceType.TimeAttack ? Helper.FormatTime(RaceManager.instance.targetTimeSilver)
						: RaceManager.instance.targetScoreSilver.ToString();
                }

                if(bronze != null)
                {
					bronze.text = raceType == RaceType.TimeAttack ? Helper.FormatTime(RaceManager.instance.targetTimeBronze)
						: RaceManager.instance.targetScoreBronze.ToString();
                }
            }
        }
    }
}
