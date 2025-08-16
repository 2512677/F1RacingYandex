using UnityEngine;
using System.Collections;
using RGSK;

[RequireComponent(typeof(AudioSource))]
public class HelicopterChase : MonoBehaviour
{
    [Header("Настройки погони")]
    [Header("Скорость полёта вертолёта (м/с)")]
    public float speed = 10f;
    [Header("Скорость поворота вертолёта (градусы в секунду)")]
    public float rotationSpeed = 120f;
    [Header("Максимальный радиус поиска ботов")]
    public float detectionRadius = 50f;

    [Header("Настройки высоты")]
    [Header("Желаемая высота полёта над трассой (от поверхности трассы)")]
    public float targetHeight = 20f;
    [Header("Минимальная допустимая высота над уровнем мира")]
    public float minHeight = 10f;
    [Header("Максимальная допустимая высота над уровнем мира")]
    public float maxHeight = 100f;

    [Header("Настройки ротора")]
    [Header("Массив трансформ роторов (лопастей)")]
    public Transform[] rotorBlades;
    [Header("Скорость вращения лопастей (градусы в секунду)")]
    public float bladeRotationSpeed = 360f;

    [Header("Audio Settings")]
    [Header("AudioSource вертолёта (будет автоматически найден)")]
    public AudioSource helicopterAudioSource;
    [Header("Максимальное расстояние, на котором слышен звук")]
    public float maxSoundDistance = 50f;

    private AiLogic currentTargetLogic;
    private BotHealth currentTargetHealth;
    private Transform currentTargetTransform;
    private Transform playerTransform;
    private float initialAudioVolume;
    private TrackLayout trackLayout;

    void Start()
    {
        // Найти игрока
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        // AudioSource
        if (helicopterAudioSource == null)
            helicopterAudioSource = GetComponent<AudioSource>();
        if (helicopterAudioSource != null)
            initialAudioVolume = helicopterAudioSource.volume;

        // Находим трассу
        trackLayout = FindObjectOfType<TrackLayout>();
    }

    void Update()
    {
        // Обновляем цель
        UpdateTarget();

        // Если цель арестована — исчезаем
        if (currentTargetHealth != null && currentTargetHealth.dead)
        {
            Destroy(gameObject);
            return;
        }

        if (currentTargetLogic != null)
        {
            MoveAlongTrackToTarget();
        }

        RotateBlades();
        UpdateAudioVolume();
    }

    private void UpdateTarget()
    {
        AiLogic[] bots = FindObjectsOfType<AiLogic>();
        AiLogic nearest = null;
        float bestDist = detectionRadius;

        foreach (var bot in bots)
        {
            float d = Vector3.Distance(transform.position, bot.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                nearest = bot;
            }
        }

        if (nearest != null && nearest != currentTargetLogic)
        {
            currentTargetLogic = nearest;
            currentTargetTransform = nearest.transform;
            currentTargetHealth = nearest.GetComponent<BotHealth>();
        }
    }

    private void MoveAlongTrackToTarget()
    {
        Vector3 targetPosOnTrack;

        // Если есть трасса и у бота задана точка на ней
        if (currentTargetLogic.racingLineTarget != null && trackLayout != null)
        {
            targetPosOnTrack = currentTargetLogic.racingLineTarget.position;
        }
        else
        {
            targetPosOnTrack = currentTargetTransform.position;
        }

        // Высота полета над трассой
        float worldTargetHeight = targetPosOnTrack.y + targetHeight;
        // Клэмп по min/max
        worldTargetHeight = Mathf.Clamp(worldTargetHeight, minHeight, maxHeight);

        Vector3 desiredPos = new Vector3(
            targetPosOnTrack.x,
            worldTargetHeight,
            targetPosOnTrack.z
        );

        // Двигаем вертолет
        transform.position = Vector3.MoveTowards(
            transform.position,
            desiredPos,
            speed * Time.deltaTime
        );

        // Плавный поворот в сторону движения
        Vector3 direction = desiredPos - transform.position;
        if (direction.sqrMagnitude > 0.01f)
        {
            var lookRot = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                lookRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void RotateBlades()
    {
        if (rotorBlades == null) return;
        foreach (var blade in rotorBlades)
        {
            if (blade != null)
                blade.Rotate(Vector3.up, bladeRotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void UpdateAudioVolume()
    {
        if (helicopterAudioSource == null || playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        float factor = Mathf.Clamp01(1f - (dist / maxSoundDistance));
        helicopterAudioSource.volume = initialAudioVolume * factor;
    }
}
