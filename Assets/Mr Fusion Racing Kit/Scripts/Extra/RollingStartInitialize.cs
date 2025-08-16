using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class RollingStartInitialize : MonoBehaviour
    {
        private int finishLineCount;
        private float lastTriggerEnter = 0;

        void OnTriggerEnter(Collider col)
        {
            var trigger = col.GetComponent<RaceTrigger>();

            if (trigger != null)
            {
                if (trigger.triggerType == RaceTriggerType.FinishLine)
                {
                    if (Time.time > lastTriggerEnter)
                    {
                        lastTriggerEnter = Time.time + 1;
                        finishLineCount++;

                        //The vehicle has passed the start/finish line, so start the race
                        if (finishLineCount >= 2)
                        {
                            RaceManager.instance.StartRace();
                            Destroy(this);
                        }
                    }
                }
            }
        }
    }
}