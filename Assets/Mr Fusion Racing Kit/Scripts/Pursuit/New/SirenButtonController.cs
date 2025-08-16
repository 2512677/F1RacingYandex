using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Обработчик UI-кнопок в режиме Chase:
/// • Переключить сирену/поиск цели
/// • Бросить шип-ленту
/// • Активировать ближайший дорожный блок
/// </summary>
public class SirenButtonController : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] Button sirenToggleButton;    // вкл/выкл сирену
    [SerializeField] Button spikeButton;          // бросить шипы
    [SerializeField] Button roadblockButton;      // вызвать блок

    [SerializeField] Button helicopterButton;  // новая кнопка для вертолёта

    void Start()
    {
        // ←↓↓↓ подписываем все кнопки ↓↓↓→
        if (sirenToggleButton)
            sirenToggleButton.onClick.AddListener(() =>
                PursuitTargetManager.instance.ToggleSirenAndTarget());

        if (spikeButton)
            spikeButton.onClick.AddListener(() =>
                PursuitTargetManager.instance.DeploySpikes());

        if (roadblockButton)
            roadblockButton.onClick.AddListener(() =>
                PursuitTargetManager.instance.ActivateNearestRoadBlock());

          helicopterButton.onClick.AddListener(() =>
       PursuitTargetManager.instance.CallHelicopter());
    }

    void Update()
    {
        // 1 — переключить сирену/поиск цели
        if (Input.GetKeyDown(KeyCode.Alpha1))
            PursuitTargetManager.instance.ToggleSirenAndTarget();

        // 2 — бросить шип-ленту
        if (Input.GetKeyDown(KeyCode.Alpha2))
            PursuitTargetManager.instance.DeploySpikes();

        // 3 — активировать ближайший дорожный блок
        if (Input.GetKeyDown(KeyCode.Alpha3))
            PursuitTargetManager.instance.ActivateNearestRoadBlock();

        if (Input.GetKeyDown(KeyCode.Alpha4))
            PursuitTargetManager.instance.CallHelicopter();
    }
}
