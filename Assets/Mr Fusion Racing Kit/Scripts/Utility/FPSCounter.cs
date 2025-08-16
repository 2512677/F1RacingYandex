using System;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class FPSCounter : MonoBehaviour
    {
        public Text fpsText;
        public bool fpsColor = true;

        const float fpsMeasurePeriod = 0.5f;
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
        private int m_CurrentFps;
        const string display = "{0} FPS";
        
        void Start()
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
        }


        void Update()
        {
            if (fpsText != null)
            {
                //measure average frames per second
                m_FpsAccumulator++;
                if (Time.realtimeSinceStartup > m_FpsNextPeriod)
                {
                    m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
                    m_FpsAccumulator = 0;
                    m_FpsNextPeriod += fpsMeasurePeriod;
                    fpsText.text = string.Format(display, m_CurrentFps);
                }

                //color based on FPS
                if (fpsColor)
                    fpsText.color = m_CurrentFps > 30 ? Color.green : Color.yellow;
            }
        }
    }
}
