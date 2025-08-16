using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RGSK
{
    public class MobileSettings : MonoBehaviour
    {
        public Dropdown mobileSteerType;

        void Start()
        {
			UpdateUIToMatchSettings ();

            if(mobileSteerType != null)
            {
				mobileSteerType.onValueChanged.AddListener(delegate 
                {
                    UpdateMobileControls(mobileSteerType.value);
                });
            }
        }


		void UpdateUIToMatchSettings()
		{
			if (mobileSteerType != null) 
			{
				mobileSteerType.value = PlayerPrefs.GetInt("MobileControlValue");
            }
		}


        void UpdateMobileControls(int value)
        {
            PlayerPrefs.SetInt("MobileControlValue", value);
        }


    }
}
