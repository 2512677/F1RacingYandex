using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class SetTargetFPS : MonoBehaviour
    {
        public int targetFramerate = 60;

        void Start()
        {
            Application.targetFrameRate = targetFramerate;
        }
    }
}