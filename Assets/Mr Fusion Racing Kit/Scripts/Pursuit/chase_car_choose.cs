using UnityEngine;

public class chase_car_choose : MonoBehaviour
{
	public GameObject CarOptionsWindow;

	public GameObject SelectTrackWindow;

	public string RaceTypes;

	public int lastcar;

	public int UpdateCarPref;

	public int SetTheCarAtStart;

	public int vncurrent;

	public bool chaseModeOn;

	private void Start()
	{
		vncurrent = PlayerPrefs.GetInt("Vehicle Number");
		if (vncurrent == 50)
		{
			SetTheCarAtStart = PlayerPrefs.GetInt("Vehicle Number Last");
			PlayerPrefs.SetInt("Vehicle Number", SetTheCarAtStart);
			PlayerPrefs.SetString("PlayerVehicle", SetTheCarAtStart.ToString());
		}
	}

	private void Update()
	{
		if (CarOptionsWindow.activeInHierarchy)
		{
			UpdateCarPref = PlayerPrefs.GetInt("Vehicle Number");
			PlayerPrefs.SetInt("Vehicle Number Last", UpdateCarPref);
		}
		if (SelectTrackWindow.activeInHierarchy)
		{
			RaceTypes = PlayerPrefs.GetString("RaceType");
			if (RaceTypes == "Chase")
			{
				chaseModeOn = true;
			}
			else
			{
				chaseModeOn = false;
			}
		}
	}

    public void Start_the_Chase_button()
    {
        if (chaseModeOn)
        {
            PlayerPrefs.SetInt("Vehicle Number", 50);
            PlayerPrefs.SetString("PlayerVehicle", "50");

            // фикс: помечаем тип гонки
            PlayerPrefs.SetString("RaceType", "Chase");
            PlayerPrefs.Save();
        }
    }

}
