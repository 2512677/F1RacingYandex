using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class TimeTrialInitialize : MonoBehaviour
    {
        private float lastTriggerEnter;

        void OnTriggerEnter(Collider col)
        {
            if (Time.time > lastTriggerEnter)
            {
                lastTriggerEnter = Time.time + 1;

                var trigger = col.GetComponent<RaceTrigger>();

                if (trigger != null)
                {
                    if (trigger.triggerType == RaceTriggerType.FinishLine)
                    {
                        //Транспортное средство прошло стартовую точку, начинайте гонку
                        if (RaceManager.instance != null)
                        {
                            RaceManager.instance.StartRace();
                            RaceManager.instance.AIToPlayer();
                        }

                        Destroy(this);
                    }
                }
            }
        }
    }
}