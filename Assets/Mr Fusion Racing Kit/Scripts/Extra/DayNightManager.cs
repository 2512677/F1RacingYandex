using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DayNightManager : MonoBehaviour 
{

    public enum TimeOfDay { Morning, Afternoon, Evening, Night}
    public TimeOfDay timeOfDay = TimeOfDay.Afternoon;

    [Header("Directional Light")]
    public Light directionalLight;

    [Header("Time Of Day Settings")]
    public TimeOfDaySettings morningSettings;
    public TimeOfDaySettings afternoonSettings;
    public TimeOfDaySettings eveningSettings;
    public TimeOfDaySettings nightSettings;

    //[Space(10)]
    //[Header("Time Progression")]
    //public bool timeProgression;
    //public float totalMinutesInDay = 24;
    //private float progression;

    int index;

    void Start () 
	{
        switch(timeOfDay)
        {
            case TimeOfDay.Morning:
                SetTimeOfDay(morningSettings);
                break;

            case TimeOfDay.Afternoon:
                SetTimeOfDay(afternoonSettings);
                break;

            case TimeOfDay.Evening:
                SetTimeOfDay(eveningSettings);
                break;

            case TimeOfDay.Night:
                SetTimeOfDay(nightSettings);
                break;
        }
	}
	


    void SetTimeOfDay(TimeOfDaySettings settings)
    {
        //Set the skybox
        if(settings.skyboxMaterial != null)
        {
            RenderSettings.skybox = settings.skyboxMaterial;
        }

        //Set the directional light's color
        if(directionalLight != null)
        {
            directionalLight.color = settings.directionalLightColor;
            directionalLight.intensity = settings.directionalLightIntensity;
        }


        //Set the ambient light color
        RenderSettings.ambientLight = settings.ambientColor;


        //Set the fog settings
        RenderSettings.fog = settings.useFog;
        if (settings.useFog)
        {          
            RenderSettings.fogColor = settings.fogColor;
            RenderSettings.fogMode = settings.fogMode;
            RenderSettings.fogDensity = settings.fogDensity;
        }
    }


    void TimeProgression()
    {
        //Time progression is a work in progress.
        //I need to figure out how to blend the skybox / light with the time

        //if(timeProgression)
        //{
            //progression = 360 / (totalMinutesInDay * 60f) * Time.deltaTime;
            //directionLight.RotateAround (directionLight.position, directionLight.right, progression);
        //}
    }



    [System.Serializable]
    public class TimeOfDaySettings
    {
        [Header("Skybox")]
        public Material skyboxMaterial;

        [Header("Directional Light")]
        public Color directionalLightColor = Color.gray;
        [Range(0,8)]public float directionalLightIntensity = 1f;

        [Header("Ambient Color")]
        public Color ambientColor = Color.gray;

        [Header("Fog")]
        public bool useFog;
        public Color fogColor = Color.gray;
        public FogMode fogMode = FogMode.Linear;
        public float fogDensity = 300;
    }
}
