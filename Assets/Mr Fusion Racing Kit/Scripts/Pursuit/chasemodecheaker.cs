using System.Collections;
using UnityEngine;
using RGSK;               //  ������������ ��� RSKK (Racing Game Starter Kit)

public class chasemodecheaker : MonoBehaviour
{
    [Tooltip("������, ������� ���� �������� ������ � ������ Chase")]
    public GameObject chasemodeobject;

    void Start()
    {
        StartCoroutine(CheckRaceType());
    }

    private IEnumerator CheckRaceType()
    {
        // ���, ���� RaceManager �������� � �����
        while (RaceManager.instance == null)
            yield return null;

        // ���� �������� ��� ����� = Chase � ���������� ������
        bool isChase = RaceManager.instance.raceType == RaceType.Chase;

        if (chasemodeobject)
            chasemodeobject.SetActive(isChase);

        // ��� ���� ������ �� �����
        enabled = false;
    }
}
