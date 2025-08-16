// Assets/Editor/Editor_TrackNode.cs

using UnityEngine;
using UnityEditor;
using RGSK;

/// <summary>
/// Кастомный редактор для TrackNode: отображает инспектор и кнопку сброса cornerType.
/// </summary>
[CustomEditor(typeof(TrackNode))]
public class Editor_TrackNode : Editor
{
    // Ссылка на редактируемый TrackNode
    private TrackNode nodeTarget;

    // Параметры инспектора, как были у вас
    private void OnEnable()
    {
        nodeTarget = (TrackNode)target;
    }

    public override void OnInspectorGUI()
    {
        // Рисуем стандартный инспектор TrackNode (поля Left Width, Right Width, Corner Type и т.д.)
        base.OnInspectorGUI();

        // Дополнительный раздел с кнопкой сброса cornerType
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Кастомный редактор TrackNode", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Можете добавить здесь любые кнопки, подсказки и т.д.", MessageType.Info);

        if (GUILayout.Button("Сбросить cornerType = None"))
        {
            Undo.RecordObject(nodeTarget, "Reset cornerType");
            nodeTarget.cornerType = CornerType.None;
            EditorUtility.SetDirty(nodeTarget);
        }
    }

    private void OnSceneGUI()
    {
        // В оригинальном скрипте вы рисовали текст, если cornerType != None.
        // Оставляем эту логику без изменений:

        if (nodeTarget.cornerType != CornerType.None)
        {
            Vector3 worldPos = nodeTarget.transform.position + Vector3.up * 1.5f;
            Handles.Label(worldPos, $"[Editor_TrackNode] {nodeTarget.cornerType}");
        }

        // Никаких иконок "W" тут мы не рисуем – теперь это делает статический DrawGizmo ниже.
    }
}


/// <summary>
/// Статический класс, который рисует иконки “W” над каждым TrackNode вне зависимости от его выделения.
/// Помещается в тот же файл, но может быть и в отдельном.
/// </summary>
public static class TrackNodeGizmo
{
    // Сместить иконку на 1.5 метра вверх от позиции узла.
    private const float ICON_WORLD_OFFSET_Y = 1.5f;

    // Пороговая скорость (km/h). Если targetSpeed < 50 → красная, иначе белая.
    private const float SPEED_THRESHOLD = 50f;

    /// <summary>
    /// Этот метод Unity вызывает автоматически в Scene View для каждого TrackNode,
    /// потому что мы указали и для выбранных, и для невыбранных:
    /// </summary>
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    private static void DrawWaypointIcon(TrackNode node, GizmoType gizmoType)
    {
        // 1) Ищем компонент RacingLineNode на том же GameObject
        RacingLineNode racingNode = node.GetComponent<RacingLineNode>();
        if (racingNode == null)
        {
            // Если нет RacingLineNode, просто выходим, не рисуем "W"
            return;
        }

        // 2) Берём targetSpeed из RacingLineNode
        float ts = racingNode.targetSpeed;
        bool useRed = ts < SPEED_THRESHOLD;

        // 3) Вычисляем мировую позицию чуть выше узла (Height = ICON_WORLD_OFFSET_Y)
        Vector3 iconWorldPos = node.transform.position + Vector3.up * ICON_WORLD_OFFSET_Y;

        // 4) Формируем путь к PNG-файлу:  
        //    Предполагаем, что вы скопировали два файла сюда:
        //      Assets/Gizmo/rgk_gizmo_waypoint_b.png   (красная "W")
        //      Assets/Gizmo/rgk_gizmo_waypoint_n.png   (белая "W")
        string iconPath = useRed
            ? "Assets/Gizmo/rgk_gizmo_waypoint_b.png"
            : "Assets/Gizmo/rgk_gizmo_waypoint_n.png";

        // 5) Рисуем иконку стандартным Gizmos.DrawIcon:
        //    allowScaling = true, чтобы иконка не растягивалась при приближении камеры.
        Gizmos.DrawIcon(iconWorldPos, iconPath, allowScaling: true);
    }
}
