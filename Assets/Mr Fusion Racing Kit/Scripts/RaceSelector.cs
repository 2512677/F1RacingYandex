using RGSK;
using UnityEngine;
using UnityEngine.UI;

public class RaceSelector : MonoBehaviour
{
    [Header("��������� �����")]
    [Tooltip("���������� ������������� ����� (������ ��������� � ���, ��� ������ � CareerData)")]
    public string raceID;

    [Tooltip("���� true, ����� ���������� �������������")]
    public bool lockedByDefault = false;

    [Tooltip("ID �����, ������� ����� ������ ��� ������������� ���� (���� �������������)")]
    public string requiredRaceID;

    [Header("���������� � ������")]
    [Tooltip("��������� ����� ������ ��� ������� � ����� (���� �� ���������, ������ None)")]
    public CarClass.VehicleClass requiredCarClass = CarClass.VehicleClass.None;

    [Header("UI ��������")]
    [Tooltip("������, ������������, ��� ����� ��������� (��������, ������� '��������')")]
    public GameObject completedText;

    [Tooltip("������ �����, ������������, ��� ����� ����������")]
    public GameObject lockIcon;

    [Header("��������� ������")]
    [Tooltip("������ �� CareerData ��� ������� �����")]
    public CareerData careerData;

    [Tooltip("������ ������ � CareerData, ��������������� ���� �����")]
    public int raceIndex;

    void Start()
    {
        UpdateRaceUI();
    }

    // ���������� UI: ���������� ����� "��������" �/��� ������ ����� � ����������� �� �������
    public void UpdateRaceUI()
    {
        // ���������� ����� "��������", ���� ��� ����� ��� ��������
        bool isCompleted = PlayerData.instance.playerData.completedRaces.Contains(raceID);
        if (completedText != null)
            completedText.SetActive(isCompleted);

        // �������� ������������� �� ������� ����������� ���������� �����
        bool isUnlockedByRace = !lockedByDefault ||
            (lockedByDefault && !string.IsNullOrEmpty(requiredRaceID) &&
             PlayerData.instance.playerData.completedRaces.Contains(requiredRaceID));

        // ��������, ������������� �� ��������� ����� ������ ����������
        bool isCarClassOk = CheckCarClass();

        // ����� ��������� ����������������, ���� ��������� ��� �������
        bool isUnlocked = isUnlockedByRace && isCarClassOk;

        if (lockIcon != null)
            lockIcon.SetActive(!isUnlocked);

        Debug.Log($"Race {raceID}: isUnlockedByRace={isUnlockedByRace}, isCarClassOk={isCarClassOk} => isUnlocked={isUnlocked}");
    }

    // �������� ���������� ������ ������
    bool CheckCarClass()
    {
        // ���� ��������� ����� �� ����� (None) � ������� ��������� �����������
        if (requiredCarClass == CarClass.VehicleClass.None)
            return true;

        CarClass.VehicleClass playerCarClass = GetPlayerCarClass();
        return (playerCarClass == requiredCarClass);
    }

    // ��������� ������ ��������� ������ ������.
    // �������� ���� ����� �� ���� ������: ��������, ����� ���� ������ ����� ��� ����� ������ �� �����.
    CarClass.VehicleClass GetPlayerCarClass()
    {
        // ������ (���� ���� VehicleDatabase):
        // var vehicleData = VehicleDatabase.Instance.GetVehicleByID(PlayerData.instance.playerData.vehicleID);
        // return vehicleData.carClass;

        // ������ (���� ������ ������ ��� �� �����):
        // CarClass car = FindObjectOfType<PlayerCarController>()?.GetComponent<CarClass>();
        // if (car != null)
        //     return car.carClass;

        // ���� ������� �������������� � ���������� None � �������� �� ���� ������!
        Debug.LogWarning("GetPlayerCarClass: ���������� ��������� ������ ������ ��� ������.");
        return CarClass.VehicleClass.None;
    }

    // �����, ���������� ��� ������ ����� (��������, ��� ������� ������)
    public void OnRaceSelected()
    {
        UpdateRaceUI(); // ��������� ��������� UI ����� �������

        bool isUnlockedByRace = !lockedByDefault ||
            (lockedByDefault && !string.IsNullOrEmpty(requiredRaceID) &&
             PlayerData.instance.playerData.completedRaces.Contains(requiredRaceID));
        bool isCarClassOk = CheckCarClass();
        bool isUnlocked = isUnlockedByRace && isCarClassOk;

        if (!isUnlocked)
        {
            // ���� ����� �������������, ������� ��������������� ��������� � �������
            if (!isUnlockedByRace)
                Debug.Log("����� �������������: ���������� ������ ���������� �����.");
            else if (!isCarClassOk)
                Debug.Log("����� ����������: ��������� ����� ������ �� ������������� ����������.");
            return;
        }

        // ���� ��� ������� ���������, ��������� ����� ����� CareerData (��. CareerData.cs)
        if (careerData != null)
        {
            Debug.Log($"������ ����� {raceID} (roundIndex: {raceIndex})");
            careerData.StartRace(raceIndex);
        }
        else
        {
            Debug.LogError("������ �� CareerData �� �����������!");
        }
    }
}
