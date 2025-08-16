using System.Collections;
using UnityEngine;

/// <summary>
/// ������, ����� ����� ������� ������ RoadBlock � ��������� �������.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class RoadBlockVisual : MonoBehaviour
{
    Transform player;
    float despawnDelay;
    bool playerPassed;
    RoadBlockPoint owner;

    public void Init(RoadBlockPoint point, Transform playerT, float delay)
    {
        owner = point;
        player = playerT;
        despawnDelay = delay;
    }

    void OnTriggerExit(Collider other)
    {
        if (playerPassed || other.transform != player) return;
        playerPassed = true;
        StartCoroutine(DespawnRoutine());
    }

    IEnumerator DespawnRoutine()
    {
        yield return new WaitForSeconds(despawnDelay);
        gameObject.SetActive(false);   // �������� ���� (����� ��� �������� used)
    }
}
