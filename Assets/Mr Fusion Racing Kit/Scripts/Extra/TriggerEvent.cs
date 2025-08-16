using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace RGSK
{
    public class TriggerEvent : MonoBehaviour
    {
        public bool timed;
        public float duration;
        public UnityEvent theEvent;

        void Start()
        {
            if (timed)
            {
                Invoke("InvokeEvent", duration);
            }
        }


        public void InvokeEvent()
        {
            theEvent.Invoke();
        }
    }
}