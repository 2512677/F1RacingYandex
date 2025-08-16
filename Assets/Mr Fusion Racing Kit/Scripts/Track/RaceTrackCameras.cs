using UnityEngine;
using System.Collections.Generic;

namespace RGSK
{
    public class RaceTrackCameras : MonoBehaviour
    {
        public float offset = 1;
        public Color gizmoColor = Color.yellow;
        public bool visible = true;

        private void OnDrawGizmos()
        {
            if (!visible)
                return;

            Gizmos.color = gizmoColor;

            if(transform.childCount > 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Gizmos.DrawWireSphere(transform.GetChild(i).position, 0.75f);
                }
            }
        }
    }
}
