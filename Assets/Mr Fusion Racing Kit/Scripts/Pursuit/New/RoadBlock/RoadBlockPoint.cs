using UnityEngine;

/// <summary>
/// �����-����� ���������� ��������� �����.
/// �������� ������� � ����� ����� �� ������.
/// </summary>
public class RoadBlockPoint : MonoBehaviour
{
    [Tooltip("����� ���������� ���� �������� Waypoint-������")]
    public int waypointIndex;

    [Tooltip("������ ����������� ����� (��� ������ ������� ������ + ���-�����)")]
    public GameObject visualPrefab;

    [Tooltip("����� ������� ������ ����� ������� ������ ������ ����")]
    public float despawnDelay = 30f;

    [HideInInspector] public bool used;          // ������������ �� ���

    /// <summary>������������ ������ � ��������� ������ ��������.</summary>
    public void Activate(Transform player)
    {
        if (used || visualPrefab == null) return;

        var go = Instantiate(visualPrefab, transform.position, transform.rotation);
        used = true;

        // ���, ���� ����� ��������� Box-������� RoadBlockVisual
        var rbVis = go.GetComponent<RoadBlockVisual>();
        if (rbVis) rbVis.Init(this, player, despawnDelay);
    }
}
