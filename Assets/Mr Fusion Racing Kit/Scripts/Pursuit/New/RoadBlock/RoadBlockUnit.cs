/*************************************************************************
 * RoadBlockUnit.cs
 * -----------------------------------------------------------------------
 *  ▸  Активируется PursuitTargetManager’ом (Activate()).
 *  ▸  Когда в триггер въезжает объект, чей root содержит компонент
 *     PursuitTargetManager (игрок), запускается таймер.
 *  ▸  Через despawnDelay секунд корневой визуал road-блока выключается,
 *     коллайдер-триггер отключается.
 ************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RoadBlockUnit : MonoBehaviour
{
    /* ─ ПАРАМЕТРЫ ─ */

    [Header("Привязка к трассе")]
    public int nodeIndex = 0;
    public TrackNode node;                          // просто ссылка для инспектора

    [Header("Визуал")]
    public GameObject visualRoot;

    [Header("Деспавн")]
    [Tooltip("Через сколько секунд скрыть road-блок после проезда игрока")]
    public float despawnDelay = 30f;

    /* ─ СЛУЖЕБНОЕ ─ */

    [HideInInspector] public bool used;             // активирован ли уже
    static readonly List<RoadBlockUnit> pool = new();

    BoxCollider trigger;
    bool timerStarted;

    /* ─ ИНИЦИАЛИЗАЦИЯ ─ */

    void Awake()
    {
        pool.Add(this);

        if (!visualRoot) visualRoot = gameObject;

        trigger = GetComponent<BoxCollider>();
        trigger.isTrigger = true;

        visualRoot.SetActive(false);
        trigger.enabled = false;
    }

    void OnDestroy() => pool.Remove(this);

    /* ─ API ДЛЯ МЕНЕДЖЕРА ─ */

    public static RoadBlockUnit FindNext(int playerNode, int totalNodes)
    {
        RoadBlockUnit best = null;
        int bestDelta = int.MaxValue;

        foreach (var b in pool)
        {
            if (b.used) continue;
            int delta = (b.nodeIndex - playerNode + totalNodes) % totalNodes;
            if (delta == 0) continue;
            if (delta < bestDelta) { bestDelta = delta; best = b; }
        }
        return best;
    }

    public void Activate()
    {
        if (used) return;

        used = true;
        visualRoot.SetActive(true);
        trigger.enabled = true;
    }

    /* ─ ТРИГГЕР ─ */

    void OnTriggerEnter(Collider other)
    {
        // проверяем PursuitTargetManager у корня объекта
        bool isPlayer = other.transform.root.GetComponent<PursuitTargetManager>() != null;

        if (!timerStarted && isPlayer)
        {
            timerStarted = true;
            StartCoroutine(DeactivateAfterDelay());
        }
    }

    /* ─ ДЕСПАВН ─ */

    IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(despawnDelay);

        visualRoot.SetActive(false);
        trigger.enabled = false;
    }
}
