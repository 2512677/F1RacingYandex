using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameplaySettings : MonoBehaviour
{
    public Dropdown transmissionDropdown;
    public Dropdown speedUnitDropdown;


	void Start ()
    {
        AddListeners();
        UpdateUIToMatchSettings();
    }


    void AddListeners()
    {
        if (transmissionDropdown != null)
        {
            transmissionDropdown.onValueChanged.AddListener(delegate
            {
                SetTransmission(transmissionDropdown.value);
            });
        }

        if (speedUnitDropdown != null)
        {
            speedUnitDropdown.onValueChanged.AddListener(delegate
            {
                SetSpeedUnit(speedUnitDropdown.value);
            });
        }
    }


    void UpdateUIToMatchSettings()
    {
        if (transmissionDropdown != null)
        {
            transmissionDropdown.value = PlayerPrefs.GetInt("Transmission");
        }

        if (speedUnitDropdown != null)
        {
            speedUnitDropdown.value = PlayerPrefs.GetInt("SpeedUnit");
        }
    }


    public void SetTransmission(int value)
    {
        PlayerPrefs.SetInt("Transmission", value);
    }


    public void SetSpeedUnit(int value)
    {
        PlayerPrefs.SetInt("SpeedUnit", value);
    }
}
