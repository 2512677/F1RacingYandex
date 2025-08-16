using UnityEngine;

[System.Serializable]
public class RacerInformation
{
    [Header("Информация о гонщике")]
    public string racerName;
    public Nationality nationality;

    [Header("Информация о транспортном средстве")]
    public string vehicleName;
}
