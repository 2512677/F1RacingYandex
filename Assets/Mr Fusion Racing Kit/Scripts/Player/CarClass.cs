using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarClass : MonoBehaviour
{
    // ������������ ��� ������� �����
    public enum VehicleClass
    {
        None,       /// �������� ����� "None" ��� ���������� ��������
        Sport,
        Perfomance,
        Super,
        Hyper,
        Hatchback,
        Drift,
        Offroad,
        Muscle,
        Cult,
        Copcar,
    }

    [Header("����� ������")]
    public VehicleClass carClass; // ������� ����� ������ � ����������

    // ����� � ������� ��� ������������
    private void Awake()
    {
        Debug.Log($"������ {gameObject.name} ��������� � ������ {carClass}");
    }
}
