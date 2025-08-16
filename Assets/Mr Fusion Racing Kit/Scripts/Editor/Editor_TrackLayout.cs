using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;

// Кастомный редактор для компонента TrackLayout
// Этот редактор позволяет настраивать параметры трассы в инспекторе и добавлять/удалять узлы трассы непосредственно из окна Scene.
[CustomEditor(typeof(TrackLayout))]
public class Editor_TrackBoundary : Editor
{
    // Ссылка на объект TrackLayout, для которого создаётся редактор
    TrackLayout _target;
    // Стиль для центрально выровненных меток
    GUIStyle centerLabelStyle;

    // Сериализованные свойства, отображаемые в инспекторе
    SerializedProperty defaultTrackWidth;  // Стандартная ширина трассы
    SerializedProperty loop;               // Замыкание трассы (повторение)
    SerializedProperty showSegments;       // Отображение сегментов трассы
    SerializedProperty showNodeIndexes;    // Отображение индексов узлов
    SerializedProperty visible;            // Видимость трассы
    SerializedProperty color;              // Цвет линии трассы

    // Параметры проекции цели для ИИ
    SerializedProperty minTargetDistance;  // Минимальное расстояние до цели
    SerializedProperty maxTargetDistance;  // Максимальное расстояние до цели

    // Параметры расчёта скорости узлов для ИИ
    SerializedProperty minSpeed;           // Минимальная скорость
    SerializedProperty maxSpeed;           // Максимальная скорость
    SerializedProperty cautionAngle;       // Угол замедления

    // Метод OnEnable вызывается при инициализации редактора
    void OnEnable()
    {
        _target = (TrackLayout)target;

        // Инициализируем стиль для центрально выровненных меток
        centerLabelStyle = new GUIStyle();
        centerLabelStyle.fontStyle = FontStyle.Normal;
        centerLabelStyle.normal.textColor = Color.white;
        centerLabelStyle.alignment = TextAnchor.MiddleCenter;

        // Для границы трассы не требуется сглаживание маршрута
        _target.smoothRoute = false;

        // Поиск сериализованных свойств по их именам
        defaultTrackWidth = serializedObject.FindProperty("defaultTrackWidth");
        loop = serializedObject.FindProperty("loop");
        showSegments = serializedObject.FindProperty("showSegments");
        showNodeIndexes = serializedObject.FindProperty("showNodeIndexes");
        visible = serializedObject.FindProperty("visible");
        color = serializedObject.FindProperty("color");

        minTargetDistance = serializedObject.FindProperty("minTargetDistance");
        maxTargetDistance = serializedObject.FindProperty("maxTargetDistance");

        minSpeed = serializedObject.FindProperty("minSpeed");
        maxSpeed = serializedObject.FindProperty("maxSpeed");
        cautionAngle = serializedObject.FindProperty("cautionAngle");
    }

    // Основной метод отрисовки пользовательского интерфейса инспектора
    public override void OnInspectorGUI()
    {
        // Обновляем сериализованный объект для синхронизации данных
        serializedObject.Update();

        // Выводим информационное сообщение для пользователя
        EditorGUILayout.HelpBox("Используйте «Shift + Левая кнопка мыши» для размещения узлов пути.", MessageType.Info);

        // Отрисовка основных настроек трассы
        EditorGUILayout.PropertyField(visible);
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(defaultTrackWidth);
        EditorGUILayout.PropertyField(loop);
        EditorGUILayout.PropertyField(showSegments);
        EditorGUILayout.PropertyField(showNodeIndexes);
        EditorGUILayout.PropertyField(color);

        GUILayout.Space(10);
        // Выводим длину трассы в километрах и милях (с использованием вспомогательного метода)
        EditorGUILayout.LabelField("Длина пути: " + Helper.MeterToKm(_target.length).ToString("F2") + " Км / " + Helper.MeterToMiles(_target.length).ToString("F2") + " Миля");

        // Раздел проекции цели для ИИ
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Проекция цели ИИ", centerLabelStyle);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(minTargetDistance);
        EditorGUILayout.PropertyField(maxTargetDistance);

        // Раздел расчёта скорости узлов для ИИ
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Расчёт скорости узла ИИ", centerLabelStyle);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(minSpeed);
        EditorGUILayout.PropertyField(maxSpeed);
        EditorGUILayout.PropertyField(cautionAngle);

        // Кнопка для расчёта скорости узлов трассы
        if (GUILayout.Button("Рассчитать скорость узла"))
        {
            _target.CalculateNodeSpeeds();
        }

        // Раздел «Разное»
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Разное", centerLabelStyle);

        // Кнопка для добавления дочерних узлов (узлов трассы)
        if (GUILayout.Button("Добавить дочерние узлы"))
        {
            _target.GetChildNodes();
            for (int i = 0; i < _target.nodes.Count; i++)
            {
                _target.nodes[i].gameObject.AddComponent<TrackNode>();
            }
        }

        // Кнопка для регулировки вращения узлов
        if (GUILayout.Button("Отрегулировать вращение узлов"))
        {
            _target.AdjustNodeRotation();
        }

        // Кнопка для удаления всех узлов трассы
        if (GUILayout.Button("Удалить все узлы"))
        {
            if (_target.transform.childCount == 0) return;

            foreach (Transform node in _target.nodes.ToArray())
            {
                // Регистрируем действие для возможности отмены (Undo)
                Undo.RecordObject(_target, "Удалён узел трека");
                _target.nodes.Remove(node);
                Undo.DestroyObjectImmediate(node.gameObject);
            }
        }

        // Применяем все изменения к сериализованному объекту
        serializedObject.ApplyModifiedProperties();
    }

    // Метод отрисовки элементов в окне Scene
    void OnSceneGUI()
    {
        // Обработка кликов мышью для создания узлов трассы
        SceneViewRaycast();

        // Если отображение индексов узлов включено, выводим их в окне Scene
        if (_target.showNodeIndexes)
        {
            for (int i = 0; i < _target.transform.childCount; i++)
            {
                // Формируем информацию об узле (индекс и целевая скорость в KPH)
                string info = i + " (" + (int)_target.transform.GetChild(i).GetComponent<RacingLineNode>().targetSpeed + " KPH)";
                // Отображаем метку над узлом
                Handles.Label(_target.nodes[i].position, info);
            }
        }
    }

    // Метод для обработки кликов мышью в окне Scene и создания новых узлов трассы
    void SceneViewRaycast()
    {
        Event e = Event.current;

        // Если нажата левая кнопка мыши с зажатым Shift и событие MouseDown
        if (e.button == 0 && e.type == EventType.MouseDown && e.shift)
        {
            // Обеспечиваем, чтобы другие объекты не реагировали на клик
            GUIUtility.hotControl = 1;

            // Выполняем лучевой каст (raycast) из позиции мыши в мир
            Ray sceneRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(sceneRay, out hit))
            {
                // Если столкновение произошло с объектом, у которого collider не является триггером
                if (!hit.collider.isTrigger)
                {
                    // Создаем новый узел трассы на месте клика
                    TrackNode newNode = new GameObject("Node ").AddComponent<TrackNode>();
                    // Добавляем компонент RacingLineNode для управления дополнительными параметрами узла
                    newNode.gameObject.AddComponent<RacingLineNode>();
                    // Регистрируем создание нового узла для возможности отмены (Undo)
                    Undo.RegisterCreatedObjectUndo(newNode.gameObject, "Созданный узел трека");

                    // Устанавливаем позицию нового узла с небольшим смещением по оси Y
                    newNode.transform.position = hit.point + new Vector3(0, 0.25f, 0);
                    // Делает новый узел дочерним объектом текущей трассы
                    newNode.transform.parent = _target.transform;
                    // Добавляем порядковый номер к имени нового узла
                    newNode.name += _target.transform.childCount;
                    // Устанавливаем ширину узла по левому и правому краю равной половине стандартной ширины трассы
                    newNode.leftWidth = _target.defaultTrackWidth / 2;
                    newNode.rightWidth = _target.defaultTrackWidth / 2;

                    // Регистрируем изменение объекта _target для возможности отмены (Undo)
                    Undo.RecordObject(_target, "Добавлен узел трека");

                    // Добавляем новый узел в список узлов трассы
                    _target.nodes.Add(newNode.transform);
                    // Корректируем вращение узлов после добавления нового узла
                    _target.AdjustNodeRotation();
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
