// ─────────────────────────────────────────────────────────────────────────────
// PursuitTargetManager.cs  (full length, + добавлена инициализация вертолёта)
// Поддерживает: сирену, поиск целей, шип-ленту, дорожные блоки (вариант 5)
// ─────────────────────────────────────────────────────────────────────────────

using System.Linq;
using UnityEngine;
using RGSK;        // TrackLayout / RaceManager

public class PursuitTargetManager : MonoBehaviour
{
    public static PursuitTargetManager instance;          // Singleton

    public Helicopter helicopter;  // назначить через инспектор

    // ────────────────────────────────────────────────────────────────
    #region Scene References
    [Header("Scene References")]
    public TrackLayout trackLayout;                       // кэш трассы
    public TCS_LightsSiren siren;                         // мигалки + звук
    #endregion
    // ────────────────────────────────────────────────────────────────

    #region Target Search
    [Header("Поиск Нарущителя")]
    [Header("Радиусь Поиска")]

    public float searchRadius = 30f;

    [Header("Угол поиска")]// радиус поиска бота
    public float searchAngle = 70f;                      // угол конуса
    [HideInInspector] public BotHealth currentTarget;     // текущая цель
    #endregion

    // ────────────────────────────────────────────────────────────────
    #region Spikes
    [Header("Шипы")]

    [Header("Префаб Шипа")]
    public GameObject spikePrefab;

    [Header("Смещение шипа")]
    public float spikeOffset = 2f;

    [Header("Продолжительность жизни")]
    public float spikeLifetime = 10f;

    [Header("Макс. Шипы")]
    public int maxSpikes = 5;

    [Header("Перезарядка шипа")]
    const float spikeCooldown = 2f;                     // перезарядка

    [Header("Последний раз время шипа")]
    float lastSpikeTime = -999f;

    [Header("Оставшиеся шипы")]
    public int remainingSpikes;
    #endregion
    // ────────────────────────────────────────────────────────────────
    #region Roadblocks
    [Header("Дорожные Блоки")]

    [Header("Лимит Блоков")]
    public int maxRoadblocks = 3;                   // лимит вызовов

    [Header("Кулдаун Кнопки")]
    public float roadblockCooldown = 15f;                 // кулдаун кнопки


    [Header("Использованные дорожные блоки")]
    public int usedRoadblocks;

    [Header("Последний раз время дорожные блоки")]
    public float lastRoadblockTime = -999f;
    #endregion
    // ────────────────────────────────────────────────────────────────

    #region Internal Cache
    Transform playerT;                                   // игрок
    Transform[] trackNodes;                              // массив узлов
    int TotalNodes => trackNodes?.Length ?? 1;           // защита от null
    #endregion

    // ========================================================================
    #region Initialisation
    void Awake()
    {
        // Сохраняем singleton
        instance = this;

        // Если ссылку на вертолёт не назначили вручную, ищем в сцене
        if (helicopter == null)
        {
            helicopter = FindObjectOfType<Helicopter>();
            if (helicopter == null)
                Debug.LogError("PursuitTargetManager: Helicopter не найден в сцене");
        }

        // Если нашли, то сразу деактивируем (он может быть включён в инспекторе)
        if (helicopter != null)
            helicopter.gameObject.SetActive(false);
    }

    void Start()
    {
        maxSpikes = PlayerPrefs.GetInt("MaxSpikes", maxSpikes);
        remainingSpikes = maxSpikes;
        maxRoadblocks = PlayerPrefs.GetInt("MaxRoadblocks", maxRoadblocks);
        usedRoadblocks = 0;
        lastRoadblockTime = -999f;

        // Находим ссылки, если не назначены
        if (!trackLayout) trackLayout = FindObjectOfType<TrackLayout>();
        if (!siren) siren = FindObjectOfType<TCS_LightsSiren>();

        playerT = RaceManager.instance.playerStatistics.transform;

        // Кэшируем все TrackNode в порядке их индекса
        if (trackLayout)
            trackNodes = trackLayout.GetComponentsInChildren<TrackNode>()
                                    .Select(n => n.transform).ToArray();

        remainingSpikes = maxSpikes;
    }
    #endregion
    // ========================================================================

    // ────────────────────────────────────────────────────────────────
    #region TrackNode helpers
    /// <summary>Ближайший узел к позиции.</summary>
    int GetClosestNodeIndex(Vector3 pos)
    {
        if (trackNodes == null || trackNodes.Length == 0) return 0;

        int best = 0;
        float bestSq = float.MaxValue;

        for (int i = 0; i < trackNodes.Length; i++)
        {
            float sq = (trackNodes[i].position - pos).sqrMagnitude;
            if (sq < bestSq) { bestSq = sq; best = i; }
        }
        return best;
    }

    /// <summary>Индекс узла, где сейчас находится игрок.</summary>
    int PlayerNodeIndex => playerT ? GetClosestNodeIndex(playerT.position) : 0;
    #endregion
    // ────────────────────────────────────────────────────────────────

    #region Siren / Target
    /// <summary>Вкл/выкл сирену и поиск цели.</summary>
    public void ToggleSirenAndTarget()
    {
        if (!siren) return;

        bool newState = !siren.sirenOn;
        siren.sirenOn = newState;
        siren.flashLightsOn = newState;

        if (newState)
        {
            FindTarget();

            // ─── СРАЗУ ПЕРЕДАЁМ НОВУЮ ЦЕЛЬ (или null), чтобы вертолёт знал, за кем лететь:
            if (helicopter != null)
            {
                if (currentTarget != null)
                    helicopter.SetTarget(currentTarget.transform);
                else
                    helicopter.SetTarget(null);
            }
        }
        else
        {
            ClearTarget();
            siren.flashLightsOn = false;

            // ─── СРАЗУ СБРОСИМ ЦЕЛЬ У ВЕРТОЛЁТА, чтобы он перестал гоняться:
            if (helicopter != null)
                helicopter.SetTarget(null);
        }
    }


    #endregion
    // ────────────────────────────────────────────────────────────────
    #region Spikes
    /// <summary>Бросить шип-ленту за машиной.</summary>
    public void DeploySpikes()
    {
        if (!spikePrefab || remainingSpikes <= 0 || !playerT) return;
        if (Time.time - lastSpikeTime < spikeCooldown) return;

        lastSpikeTime = Time.time;

        Vector3 pos = playerT.position - playerT.forward * spikeOffset;
        Quaternion rot = playerT.rotation;

        var go = Instantiate(spikePrefab, pos, rot);
        StartCoroutine(DelayedAddSpikeStrip(go));
        Destroy(go, spikeLifetime);

        remainingSpikes--;
    }

    System.Collections.IEnumerator DelayedAddSpikeStrip(GameObject go)
    {
        yield return new WaitForSeconds(1f);
        if (go && go.activeInHierarchy && !go.GetComponent<RCCV3_SpikeStrip>())
            go.AddComponent<RCCV3_SpikeStrip>();
    }


    /// <summary>Вызвать вертолёт для преследования текущей цели.</summary>
    public void CallHelicopter()
    {
        // Если нет выбранной цели, ничего не делаем
        if (currentTarget == null) return;

        // Если ссылка на вертолёт отсутствует, ничего не делаем
        if (helicopter == null) return;

        // Ставим вертолёт над игроком (например, 10 единиц вверх)
        if (playerT != null)
            helicopter.transform.position = playerT.position + Vector3.up * 10f;

        // Активируем объект-вертолёт
        helicopter.gameObject.SetActive(true);

        // Назначаем цель (BotHealth → Transform)
        helicopter.SetTarget(currentTarget.transform);
    }

    public int GetRemainingSpikes() => remainingSpikes;
    #endregion
    // ────────────────────────────────────────────────────────────────
    #region Roadblocks
    /// <summary>Кнопка HUD вызывает ближайший вперёд RoadBlockUnit.</summary>
    public void ActivateNearestRoadBlock()
    {
        // лимит и кулдаун
        if (usedRoadblocks >= maxRoadblocks) return;
        if (Time.time - lastRoadblockTime < roadblockCooldown) return;

        // ищем первый неиспользованный блок дальше по трассе
        var cand = RoadBlockUnit.FindNext(PlayerNodeIndex, TotalNodes);
        if (!cand) return;

        cand.Activate();                 // включаем визуал + триггер
        usedRoadblocks++;
        lastRoadblockTime = Time.time;
    }

    public int GetRemainingRoadblocks() => maxRoadblocks - usedRoadblocks;
    #endregion
    // ────────────────────────────────────────────────────────────────
    #region Target Search implementation
    public void FindTarget()
    {
        ClearTarget();
        if (!playerT) return;

        float bestDist = Mathf.Infinity;
        BotHealth best = null;

        foreach (var b in FindObjectsOfType<BotHealth>())
        {
            if (b.dead) continue;
            Vector3 dir = b.transform.position - playerT.position;
            float dist = dir.magnitude;
            if (dist > searchRadius) continue;
            float angle = Vector3.Angle(playerT.forward, dir);
            if (angle > searchAngle) continue;
            if (dist < bestDist)
            {
                bestDist = dist;
                best = b;
            }
        }

        if (best != null)
        {
            currentTarget = best;
            currentTarget.SetVisualsActive(true);

            // ─── ОБНОВЛЯЕМ ЦЕЛЬ ДЛЯ ВЕРТОЛЁТА:
            if (helicopter != null)
                helicopter.SetTarget(currentTarget.transform);
        }
        else
        {
            currentTarget = null;

            // ─── ЕСЛИ НИКАКОЙ ЦЕЛИ НЕТ – СБРОСИМ У ВЕРТОЛЁТА:
            if (helicopter != null)
                helicopter.SetTarget(null);
        }
    }


    public void ClearTarget()
    {
        if (currentTarget != null)
            currentTarget.SetVisualsActive(false);
        currentTarget = null;

        // ─── ЗАВЕРШАЕМ ПРЕСЛЕДОВАНИЕ У ВЕРТОЛЁТА:
        if (helicopter != null)
            helicopter.SetTarget(null);
    }


    public void OnTargetCaptured()
    {
        ClearTarget();

        bool anyAlive = FindObjectsOfType<BotHealth>().Any(b => !b.dead);
        if (!anyAlive)
        {
            if (siren)
            {
                siren.sirenOn = false;
                siren.flashLightsOn = false;
            }
            return;
        }

        if (siren)
        {
            siren.sirenOn = true;
            siren.flashLightsOn = true;
        }
        Invoke(nameof(FindTarget), 0.3f);
    }
    #endregion
    // ────────────────────────────────────────────────────────────────
}
