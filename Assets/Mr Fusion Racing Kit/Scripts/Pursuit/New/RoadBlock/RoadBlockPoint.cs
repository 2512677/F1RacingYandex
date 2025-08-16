using UnityEngine;

/// <summary>
/// Якорь-точка возможного дорожного блока.
/// Ставится вручную в сцене прямо на дороге.
/// </summary>
public class RoadBlockPoint : MonoBehaviour
{
    [Tooltip("Номер ближайшего узла главного Waypoint-кольца")]
    public int waypointIndex;

    [Tooltip("Префаб визуального блока (две поперёк стоящие машины + шип-лента)")]
    public GameObject visualPrefab;

    [Tooltip("После проезда игрока через сколько секунд убрать блок")]
    public float despawnDelay = 30f;

    [HideInInspector] public bool used;          // задействован ли уже

    /// <summary>Инстанцирует визуал и запускает логику деспавна.</summary>
    public void Activate(Transform player)
    {
        if (used || visualPrefab == null) return;

        var go = Instantiate(visualPrefab, transform.position, transform.rotation);
        used = true;

        // ждём, пока игрок пересечёт Box-триггер RoadBlockVisual
        var rbVis = go.GetComponent<RoadBlockVisual>();
        if (rbVis) rbVis.Init(this, player, despawnDelay);
    }
}
