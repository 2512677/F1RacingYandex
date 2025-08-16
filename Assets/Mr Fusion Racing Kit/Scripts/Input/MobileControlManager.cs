using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class MobileControlManager : MonoBehaviour
    {
        private StandardTouchInputManager inputManager;
        public MobileControlType mobileControlType;
        public GameObject touchControlLayout;
        public GameObject tiltControlLayout;
        public GameObject wheelControlLayout;
        public bool loadPlayerPreference = true;
        private bool showTouchPanel, showTiltPanel, showWheelPanel;


        void Start()
        {
            inputManager = FindObjectOfType<StandardTouchInputManager>();

            if (loadPlayerPreference)
            {
                mobileControlType = (MobileControlType)PlayerPrefs.GetInt("MobileControlValue");
            }

			switch(mobileControlType)
            {
                case MobileControlType.Touch:
                    showTouchPanel = true;
                    showTiltPanel = false;
                    showWheelPanel = false;
                    break;

                case MobileControlType.Tilt:
                    showTouchPanel = false;
                    showTiltPanel = true;
                    showWheelPanel = false;
                    break;

                case MobileControlType.Wheel:
                    showTouchPanel = false;
                    showTiltPanel = false;
                    showWheelPanel = true;
                    break;

            }          

            UpdateControlPanels();       
        }

        
        public void UpdateControlPanels()
        {
            HideAllControlPanels();

            if (showTouchPanel && touchControlLayout != null)
            {
                showTiltPanel = false;
                showWheelPanel = false;
                touchControlLayout.SetActive(true);
            }

            if (showTiltPanel && tiltControlLayout != null)
            {
                showTouchPanel = false;
                showWheelPanel = false;
                tiltControlLayout.SetActive(true);
            }

            if (showWheelPanel && wheelControlLayout != null)
            {
                showTouchPanel = false;
                showTiltPanel = false;
                wheelControlLayout.SetActive(true);               
            }

            if(inputManager != null)
            {
                inputManager.UpdateActionsDictionary();
            }
        }


        void HideAllControlPanels()
        {
            if (touchControlLayout != null)
            {
                touchControlLayout.SetActive(false);
            }

            if (tiltControlLayout != null)
            {
                tiltControlLayout.SetActive(false);
            }

            if (wheelControlLayout != null)
            {
                wheelControlLayout.SetActive(false);
            }
        }
    }


    public enum MobileControlType
    {
        Touch,
        Tilt,
        Wheel
    }
}