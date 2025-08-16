using UnityEngine;
using System.Collections;
using RGSK;

[RequireComponent(typeof(AudioSource))]
public class HelicopterChase : MonoBehaviour
{
    [Header("��������� ������")]
    [Header("�������� ����� �������� (�/�)")]
    public float speed = 10f;
    [Header("�������� �������� �������� (������� � �������)")]
    public float rotationSpeed = 120f;
    [Header("������������ ������ ������ �����")]
    public float detectionRadius = 50f;

    [Header("��������� ������")]
    [Header("�������� ������ ����� ��� ������� (�� ����������� ������)")]
    public float targetHeight = 20f;
    [Header("����������� ���������� ������ ��� ������� ����")]
    public float minHeight = 10f;
    [Header("������������ ���������� ������ ��� ������� ����")]
    public float maxHeight = 100f;

    [Header("��������� ������")]
    [Header("������ ��������� ������� (��������)")]
    public Transform[] rotorBlades;
    [Header("�������� �������� �������� (������� � �������)")]
    public float bladeRotationSpeed = 360f;

    [Header("Audio Settings")]
    [Header("AudioSource �������� (����� ������������� ������)")]
    public AudioSource helicopterAudioSource;
    [Header("������������ ����������, �� ������� ������ ����")]
    public float maxSoundDistance = 50f;

    private AiLogic currentTargetLogic;
    private BotHealth currentTargetHealth;
    private Transform currentTargetTransform;
    private Transform playerTransform;
    private float initialAudioVolume;
    private TrackLayout trackLayout;

    void Start()
    {
        // ����� ������
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        // AudioSource
        if (helicopterAudioSource == null)
            helicopterAudioSource = GetComponent<AudioSource>();
        if (helicopterAudioSource != null)
            initialAudioVolume = helicopterAudioSource.volume;

        // ������� ������
        trackLayout = FindObjectOfType<TrackLayout>();
    }

    void Update()
    {
        // ��������� ����
        UpdateTarget();

        // ���� ���� ���������� � ��������
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

        // ���� ���� ������ � � ���� ������ ����� �� ���
        if (currentTargetLogic.racingLineTarget != null && trackLayout != null)
        {
            targetPosOnTrack = currentTargetLogic.racingLineTarget.position;
        }
        else
        {
            targetPosOnTrack = currentTargetTransform.position;
        }

        // ������ ������ ��� �������
        float worldTargetHeight = targetPosOnTrack.y + targetHeight;
        // ����� �� min/max
        worldTargetHeight = Mathf.Clamp(worldTargetHeight, minHeight, maxHeight);

        Vector3 desiredPos = new Vector3(
            targetPosOnTrack.x,
            worldTargetHeight,
            targetPosOnTrack.z
        );

        // ������� ��������
        transform.position = Vector3.MoveTowards(
            transform.position,
            desiredPos,
            speed * Time.deltaTime
        );

        // ������� ������� � ������� ��������
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
