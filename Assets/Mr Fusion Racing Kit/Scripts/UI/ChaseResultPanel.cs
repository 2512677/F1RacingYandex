using RGSK;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;          // замените на TMPro, если нужно

/// <summary>
/// Панель итогов погони: показывает статистику, без собственной кнопки Continue.
/// Переходом занимается PostRacePanel.
/// </summary>
public class ChaseResultPanel : MonoBehaviour
{
    [Header("UI")]
    public Text capturedLabel;      // «Captured: X»
    public Text escapedLabel;       // «Escaped:  Y»
    public Text roadblockLabel;     // «Road-blocks: used/max»
    public Text spikesLabel;        // «Spike strips: used/max»

    void OnEnable() => Refresh();

    // ----------------------------------------------------------------
    // Обновление статистики
    // ----------------------------------------------------------------
    void Refresh()
    {
        var rm = RaceManager.instance;
        var ptm = PursuitTargetManager.instance;
        if (rm == null || ptm == null) return;

        // поймано / убежало
        int captured = FindObjectsOfType<BotHealth>().Count(b => b.dead);
        int total = rm.opponentCount;
        int escaped = Mathf.Max(total - captured, 0);

        // использованные блок-посты / шип-ленты
        int rbUsed = ptm.maxRoadblocks - ptm.GetRemainingRoadblocks();
        int spUsed = ptm.maxSpikes - ptm.GetRemainingSpikes();

        if (capturedLabel) capturedLabel.text = $"{captured}";
        if (escapedLabel) escapedLabel.text = $"{escaped}";
        if (roadblockLabel) roadblockLabel.text = $"{rbUsed}/{ptm.maxRoadblocks}";
        if (spikesLabel) spikesLabel.text = $"{spUsed}/{ptm.maxSpikes}";
    }
}
