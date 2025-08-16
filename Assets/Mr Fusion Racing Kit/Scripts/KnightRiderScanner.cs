using UnityEngine;

/// <summary>
/// Knight Rider-сканер: красная полоса ездит слева-направо и обратно
/// + синхронизированный звук сканирования.
/// Повесь скрипт на объект с MeshRenderer.
/// Материал должен иметь текстуру, которую можно сдвигать (Albedo или Emission).
/// </summary>
[RequireComponent(typeof(Renderer))]
[DisallowMultipleComponent]
public class KnightRiderScanner : MonoBehaviour
{
    [Header("Визуальный сканер")]
    [Tooltip("Сколько циклов (туда-обратно) в секунду")]
    [SerializeField] private float scanSpeed = 1f;

    [Tooltip("Диапазон смещения текстуры по X (0-1)")]
    [SerializeField] private float offsetRange = 1f;

    // ────────────────────────────────────────────────────────────────
    [Header("Звук сканера")]                       // ─── NEW ───
    [Tooltip("Источник звука. Если оставить пустым — возьмётся автоматически.")]
    [SerializeField] private AudioSource scanAudio;

    [Tooltip("Стерео-панорамирование вместе с движением луча")]
    [SerializeField] private bool panWithBeam = true;

    [Tooltip("Воспроизводить ли звук сразу при старте?")]
    [SerializeField] private bool playOnStart = true;
    // ────────────────────────────────────────────────────────────────

    public Material _mat;
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    private void Awake()
    {
        // копия материала, чтобы не трогать sharedMaterial
        _mat = GetComponent<Renderer>().material;

        // ─── NEW ───  если не назначили вручную, ищем AudioSource на этом же объекте
        if (!scanAudio) scanAudio = GetComponent<AudioSource>();

        // если нашли и надо запускать — делаем loop и Play()
        if (scanAudio && playOnStart)
        {
            scanAudio.loop = true;
            scanAudio.Play();
        }
    }

    private void Update()
    {
        // t = 0→1→0
        float t = Mathf.PingPong(Time.time * scanSpeed, 1f);

        // сдвигаем текстуру
        _mat.SetTextureOffset(MainTex, new Vector2(t * offsetRange, 0f));

        // ─── NEW ───  панорамирование стерео из -1 (лево) → 1 (право)
        if (scanAudio && panWithBeam)
        {
            scanAudio.panStereo = Mathf.Lerp(-1f, 1f, t);
        }
    }

#if UNITY_EDITOR
    private void OnDestroy()
    {
        if (_mat != null)
            _mat.SetTextureOffset(MainTex, Vector2.zero);
    }
#endif
}
