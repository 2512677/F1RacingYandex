// Assets/Editor/Editor_GridPositions.cs

using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;

// Кастомный редактор для компонента GridPositions.
// Позволяет настраивать параметры сетки в инспекторе и добавлять новые позиции 
// сетки через Shift+Click в Scene View. Также теперь отрисовывает иконку спавна ("P") 
// над каждой созданной позицией.
[CustomEditor(typeof(GridPositions))]
public class Editor_GridPositions : Editor
{
    // Ссылка на редактируемый компонент GridPositions
    private GridPositions _target;

    // Сериализованные свойства, отображаемые в инспекторе
    private SerializedProperty offset;     // Смещение по Y для размещения точек сетки
    private SerializedProperty gizmoColor; // Цвет Gizmo (если используется)
    private SerializedProperty visible;    // Включение/выключение видимости сетки

    // ====== НОВЫЕ ПОЛЯ ДЛЯ ОТРИСОВКИ ИКОНКИ SPAWN ("P") ======
    // Текстура иконки спавна (rgk_gizmo_spawn.png) – будем загружать из Assets/Gizmo
    private Texture2D spawnIcon;

    // Размер иконки в пикселях на экране (можно менять под свои нужды)
    private const float ICON_SCREEN_SIZE = 48f;

    // Смещение по оси Y (в world-space), чтобы иконка рисовалась немного выше точки
    private const float ICON_WORLD_OFFSET_Y = 1.5f;
    // ==========================================================

    // Вызывается Unity при инициализации редактора
    void OnEnable()
    {
        _target = (GridPositions)target;

        // Находим сериализованные поля (если они у вас есть в GridPositions)
        offset = serializedObject.FindProperty("offset");
        gizmoColor = serializedObject.FindProperty("gizmoColor");
        visible = serializedObject.FindProperty("visible");

        // ====== ЗАГРУЗКА ТЕКСТУРЫ SPAWN ICON ======
        // Предполагается, что файл лежит по пути "Assets/Gizmo/rgk_gizmo_spawn.png"
        // Если папка называется иначе, исправьте путь соответствующим образом.
        spawnIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(
            "Assets/Gizmo/rgk_gizmo_spawn.png"
        );
        // Если файл не найден, spawnIcon останется null, и иконка не будет отрисовываться.
        // =====================================================
    }

    // Рисуем пользовательский интерфейс инспектора для GridPositions
    public override void OnInspectorGUI()
    {
        // Обновляем сериализованный объект, чтобы синхронизировать данные
        serializedObject.Update();

        // Выводим информационное сообщение
        EditorGUILayout.HelpBox(
            "Используйте Shift + левая кнопка мыши для размещения новых позиций сетки",
            MessageType.Info
        );

        // Отображаем поле видимости сетки, если оно есть
        if (visible != null)
        {
            EditorGUILayout.PropertyField(visible);
            GUILayout.Space(10);
        }

        // Отображаем свойства offset и gizmoColor (если они есть)
        if (offset != null)
            EditorGUILayout.PropertyField(offset);

        if (gizmoColor != null)
            EditorGUILayout.PropertyField(gizmoColor);

        GUILayout.Space(10);
        // Выводим количество дочерних позиций (узлов) сетки
        EditorGUILayout.LabelField("Позиции сетки: " + _target.transform.childCount);

        // Разделитель
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // Кнопка для удаления всех позиций в сетке
        if (GUILayout.Button("Удалить всё"))
        {
            // Удаляем всех дочерних объектов под GridPositions
            for (int i = _target.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = _target.transform.GetChild(i);
                Undo.DestroyObjectImmediate(child.gameObject);
            }
        }

        // Применяем изменения, сделанные в инспекторе
        serializedObject.ApplyModifiedProperties();
    }

    // Рисует элементы в Scene View: обрабатывает клики иконкой, а затем отрисовывает spawn icon
    void OnSceneGUI()
    {
        // 1) Обработка Shift+Click для создания новой точки сетки
        SceneViewRaycast();

        // 2) Отрисовка иконки "P" над каждой созданной позицией сетки
        if (_target == null || spawnIcon == null)
            return;

        // (Если вы захотите менять цвет линий или прочих Gizmo, можете установить Handles.color здесь)
        // Например: Handles.color = _target.gizmoColor;

        // Проходим по всем дочерним объектам (каждый – это точка сетки)
        for (int i = 0; i < _target.transform.childCount; i++)
        {
            Transform child = _target.transform.GetChild(i);
            if (child == null) continue;

            // 2.1) Вычисляем мировую позицию чуть выше объекта:
            Vector3 worldPos = child.position + Vector3.up * ICON_WORLD_OFFSET_Y;

            // 2.2) Конвертируем мировую позицию в экранные GUI-координаты:
            Vector2 guiPoint = HandleUtility.WorldToGUIPoint(worldPos);

            // 2.3) Задаём размер иконки в пикселях (ширина = высота):
            float size = ICON_SCREEN_SIZE;

            // 2.4) Формируем прямоугольник, центрированный по guiPoint:
            Rect iconRect = new Rect(
                guiPoint.x - size / 2f,
                guiPoint.y - size / 2f,
                size,
                size
            );

            // 2.5) Рисуем текстуру через GUI.DrawTexture
            Handles.BeginGUI();
            GUI.DrawTexture(iconRect, spawnIcon);
            Handles.EndGUI();
        }
    }

    // Обработка Shift+Click в Scene View – добавление позиции сетки
    void SceneViewRaycast()
    {
        Event e = Event.current;
        // Если нажата левая кнопка мыши с зажатым Shift и событие MouseDown
        if (e.button == 0 && e.type == EventType.MouseDown && e.shift)
        {
            // Блокируем другие обработчики UI
            GUIUtility.hotControl = 1;

            // Лучевой каст из позиции курсора мыши
            Ray sceneRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(sceneRay, out hit))
            {
                // Если столкновение произошло с объектом, чей Collider не является триггером
                if (!hit.collider.isTrigger)
                {
                    // Создаём новый пустой GameObject с именем "P"
                    GameObject newSpawnpoint = new GameObject("P");
                    Undo.RegisterCreatedObjectUndo(newSpawnpoint, "Создана позиция сетки");

                    // Устанавливаем позицию (hit.point + смещение offset по Y)
                    newSpawnpoint.transform.position = hit.point + Vector3.up * (_target.offset);
                    // Устанавливаем вращение, заданное в GridPositions.defaultRotation
                    newSpawnpoint.transform.eulerAngles = _target.defaultRotation;
                    // Делаем его дочерним для объекта GridPositions
                    newSpawnpoint.transform.parent = _target.transform;
                    // Добавляем порядковый номер к имени
                    newSpawnpoint.name += _target.transform.childCount;
                }
            }
        }

        // При отпускании кнопки мыши сбрасываем hotControl
        if (e.button == 0 && e.type == EventType.MouseUp)
        {
            GUIUtility.hotControl = 0;
        }
    }
}
