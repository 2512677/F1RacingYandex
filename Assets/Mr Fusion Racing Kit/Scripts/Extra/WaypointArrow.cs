using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class WaypointArrow : MonoBehaviour
    {
        public float rotateSpeed = 5;
        public Vector3 offset;

        void Update()
        {
            if (RaceManager.instance)
            {
                Vector3 targetDirection = RaceManager.instance.playerStatistics.GetCurrentNode().position - RaceManager.instance.playerStatistics.transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Vector3 rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed).eulerAngles;
                transform.rotation = Quaternion.Euler(0, rotation.y, 0);
            }
        }
    }
}