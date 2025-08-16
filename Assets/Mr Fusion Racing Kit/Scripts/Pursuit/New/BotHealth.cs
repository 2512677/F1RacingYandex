using System.Collections;
using UnityEngine;
using RGSK;   // Respawner, RaceManager

/// <summary>
/// HP, звёзды, дым и «поимка» бота. Урон принимается
///   ТОЛЬКО если бот выбран PursuitTargetManager-ом.
/// Работает лишь в режиме Chase.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BotHealth : MonoBehaviour
{
    /* ─────────────────────  НАСТРОЙКИ  ───────────────────── */
    [Header("HP")]
    public int maxHP = 100;          // 100 % = четыре звезды
    public int hitDamage = 25;           // упрощённо −25 % за удар

    [Header("Визуал (одна группа)")]
    public GameObject visualsRoot;        // контейнер: звёзды + дым
    public GameObject[] stars;            // ссылки на 4 звезды
    public GameObject smoke;            // дым / огонь (Inactive)

    [Header("Компоненты (можно оставить пустыми)")]
    public RCC_CarControllerV3 rcc;
    public RCCAIInput ai;

    /* ─────────────────────  ВНУТРЕННИЕ  ───────────────────── */
    public bool dead { get; private set; }   // для внешней логики
    int hp;
    float lastHit;
    const float cooldownHit = .25f;          // анти-дубль
    const float minImpact = 5f;            // минимальный удар (м/с)

    /* ========================================================= */
    #region INITIALISATION
    void Awake()
    {
        // Chase-гонка? — если нет, полностью выключаем скрипт и визуал
        if (RaceManager.instance == null || RaceManager.instance.raceType != RaceType.Chase)
        {
            if (visualsRoot) visualsRoot.SetActive(false);
            enabled = false;
            return;
        }

        if (!rcc) rcc = GetComponent<RCC_CarControllerV3>();
        if (!ai) ai = GetComponent<RCCAIInput>();

        hp = maxHP;
        UpdateStars();
        SetVisualsActive(false);                 // до выбора цели не показываем
    }

    /// <summary>Включить / выключить группу звёзд и дыма.</summary>
    public void SetVisualsActive(bool state)
    {
        if (visualsRoot && visualsRoot.activeSelf != state)
            visualsRoot.SetActive(state);
    }
    #endregion
    /* ========================================================= */
    #region DAMAGE
    void OnCollisionEnter(Collision col)
    {
        if (dead) return;

        // принимать урон только если этот бот сейчас выбран целью
        if (PursuitTargetManager.instance == null) return;
        if (PursuitTargetManager.instance.currentTarget != this) return;

        // доп. фильтры
        if (!col.transform.root.CompareTag("Player")) return;
        if (col.relativeVelocity.magnitude < minImpact) return;
        if (Time.time - lastHit < cooldownHit) return;

        lastHit = Time.time;
        TakeDamage(hitDamage);
    }

    void TakeDamage(int dmg)
    {
        hp = Mathf.Max(hp - dmg, 0);
        UpdateStars();

        if (hp <= maxHP * 0.2f && smoke) smoke.SetActive(true);
        if (hp == 0) Kill();
    }

    void UpdateStars()
    {
        if (stars == null || stars.Length == 0) return;
        int on = Mathf.CeilToInt((float)hp / maxHP * stars.Length);
        for (int i = 0; i < stars.Length; i++)
            if (stars[i]) stars[i].SetActive(i < on);
    }


    public void ForceDestroy()
    {
        if (!dead) TakeDamage(hp);   // мгновенно обнуляем HP
    }

    #endregion
    /* ========================================================= */
    #region DEATH / CAPTURE
    void Kill()
    {
        dead = true;

        // AI больше не работает
        if (ai) Destroy(ai);

        // глушим машину
        if (rcc)
        {
            rcc.throttleInput = rcc.steerInput = 0f;
            rcc.brakeInput = rcc.handbrakeInput = 1f;
            rcc.engineRunning = false;
        }

        // слегка гася физику, чтобы упала естественно
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.linearDamping = rb.angularDamping = 4f;
            rb.linearVelocity = rb.angularVelocity = Vector3.zero;
        }

        // запрет на автореспавн
        Respawner resp = GetComponent<Respawner>();
        if (resp) Destroy(resp);

        if (smoke) smoke.SetActive(true);

        // помечаем в статистике, чтобы UI мог написать «(Пойман)»
        var stats = GetComponent<RacerStatistics>();
        if (stats) stats.disqualified = true;

        // сообщаем системам
        if (RaceManager.instance) RaceManager.instance.RegisterBotCaptured();
        if (PursuitTargetManager.instance) PursuitTargetManager.instance.OnTargetCaptured();
    }
    #endregion
}
