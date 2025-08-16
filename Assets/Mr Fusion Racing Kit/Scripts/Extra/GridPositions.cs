using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class GridPositions : MonoBehaviour
    {
        public Color gizmoColor = new Color(1, 0, 0, 1);
        public float offset = 0.5f; //Placement offset on the y axis
        public Vector2 defaultRotation = new Vector2();
        public bool visible = true;

        void OnDrawGizmos()
        {
            if (visible)
            {
                Gizmos.color = gizmoColor;

                for (int i = 0; i < transform.childCount; i++)
                {
                    Gizmos.DrawWireSphere(transform.GetChild(i).position, 0.4f);
                    Gizmos.DrawLine(transform.GetChild(i).localPosition,
                        transform.GetChild(i).forward * 2 + transform.GetChild(i).localPosition);
                }
            }
        }


        public void UpdatePositionAndRotation()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                //Update postion
                Vector3 pos = transform.GetChild(i).localPosition;
                pos.y = offset;
                transform.GetChild(i).localPosition = pos;

                //Update rotation
                transform.GetChild(i).localEulerAngles = defaultRotation;
            }
        }
    }
}