using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;

// Кастомный редактор для компонента RacingLine
[CustomEditor(typeof(RacingLine))]
public class Editor_RacingLine : Editor
{
    // Ссылка на экземпляр RacingLine, с которым работаем
    RacingLine _target;
    // Стиль для центрально выровненной метки
    GUIStyle centerLabelStyle;

    // Сериализованные свойства для отображения в инспекторе
    SerializedProperty loop;             // Опция замыкания трассы
    SerializedProperty smoothRoute;      // Опция сглаживания маршрута
    SerializedProperty showNodeIndexes;  // Отображение индексов узлов
    SerializedProperty visible;          // Видимость трассы
    SerializedProperty smoothness;       // Параметр сглаженности линии
    SerializedProperty color;            // Цвет линии

    // Параметры проекции цели
    SerializedProperty minTargetDistance; // Минимальное расстояние до цели
    SerializedProperty maxTargetDistance; // Максимальное расстояние до цели

    // Параметры расчёта скорости узлов
    SerializedProperty minSpeed;        // Минимальная скорость
    SerializedProperty maxSpeed;        // Максимальная скорость
    SerializedProperty cautionAngle;    // Угол замедления

    // Метод OnEnable вызывается при активации редактора
    void OnEnable()
    {
        // Приведение target к типу RacingLine
        _target = (RacingLine)target;

        // Инициализация стиля для центральной метки
        centerLabelStyle = new GUIStyle();
        centerLabelStyle.fontStyle = FontStyle.Normal;
        centerLabelStyle.normal.textColor = Color.white;
        centerLabelStyle.alignment = TextAnchor.MiddleCenter;

        // Поиск сериализованных свойств по их именам
        loop = serializedObject.FindProperty("loop");
        smoothRoute = serializedObject.FindProperty("smoothRoute");
        visible = serializedObject.FindProperty("visible");
        showNodeIndexes = serializedObject.FindProperty("showNodeIndexes");
        smoothness = serializedObject.FindProperty("smoothness");
        color = serializedObject.FindProperty("color");

        minTargetDistance = serializedObject.FindProperty("minTargetDistance");
        maxTargetDistance = serializedObject.FindProperty("maxTargetDistance");

        minSpeed = serializedObject.FindProperty("minSpeed");
        maxSpeed = serializedObject.FindProperty("maxSpeed");
        cautionAngle = serializedObject.FindProperty("cautionAngle");
    }

    // Основной метод отрисовки пользовательского инспектора
    public override void OnInspectorGUI()
    {
        // Обновляем сериализованный объект для синхронизации значений
        serializedObject.Update();

        // Вывод информационного сообщения для пользователя
        EditorGUILayout.HelpBox("Используйте Shift + левую кнопку мыши для размещения узлов гоночной линии", MessageType.Info);

        // Отрисовка основных настроек
        EditorGUILayout.PropertyField(visible);
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(loop);
        EditorGUILayout.PropertyField(smoothRoute);
        EditorGUILayout.PropertyField(showNodeIndexes);
        EditorGUILayout.PropertyField(smoothness);
        EditorGUILayout.PropertyField(color);

        // Раздел настроек проекции цели
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Проекция цели", centerLabelStyle);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(minTargetDistance);
        EditorGUILayout.PropertyField(maxTargetDistance);

        // Раздел расчёта скорости узлов
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Расчёт скорости узлов", centerLabelStyle);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(minSpeed);
        EditorGUILayout.PropertyField(maxSpeed);
        EditorGUILayout.PropertyField(cautionAngle);

        // Кнопка для расчёта скоростей узлов
        if (GUILayout.Button("Рассчитать скорости узлов"))
        {
            _target.CalculateNodeSpeeds();
        }

        // Дополнительные действия
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Space(5);

        // Кнопка для добавления дочерних узлов
        if (GUILayout.Button("Добавить дочерние узлы"))
        {
            // Получаем дочерние узлы объекта
            _target.GetChildNodes();
            // Добавляем компонент RacingLineNode для каждого найденного узла
            for (int i = 0; i < _target.nodes.Count; i++)
            {
                _target.nodes[i].gameObject.AddComponent<RacingLineNode>();
            }
        }

        if (GUILayout.Button("Отрегулируйте вращение узла"))
        {
            _target.AdjustNodeRotation();
        }

        // Кнопка для удаления всех узлов
        if (GUILayout.Button("Удалить все узлы"))
        {
            // Если нет дочерних объектов, выход из метода
            if (_target.transform.childCount == 0) return;

            // Перебираем все дочерние узлы и удаляем их
            foreach (Transform node in _target.nodes.ToArray())
            {
                // Записываем операцию в Undo для возможности отката
                Undo.RecordObject(_target, "Удалён узел гоночной линии");
                _target.nodes.Remove(node);
                Undo.DestroyObjectImmediate(node.gameObject);
            }
        }

        // Применяем все изменения к сериализованному объекту
        serializedObject.ApplyModifiedProperties();
    }

    // Метод для отрисовки элементов в окне Scene
    void OnSceneGUI()
    {
        // Выполняем проверку кликов мыши в сцене
        SceneViewRaycast();

        // Если включено отображение индексов узлов, выводим их в Scene view
        if (_target.showNodeIndexes)
        {
            for (int i = 0; i < _target.transform.childCount; i++)
            {
                // Формируем строку с индексом узла и его скоростью
                string info = i + " (" + (int)_target.transform.GetChild(i).GetComponent<RacingLineNode>().targetSpeed + " км/ч)";
                // Отображаем метку с информацией над узлом
                Handles.Label(_target.nodes[i].position, info);
            }
        }
    }

    // Метод для обработки кликов мышью в Scene view
    void SceneViewRaycast()
    {
        // Получаем текущее событие
        Event e = Event.current;

        // Если нажата левая кнопка мыши с зажатым Shift и событие MouseDown
        if (e.button == 0 && e.type == EventType.MouseDown && e.shift)
        {
            // Устанавливаем hotControl, чтобы другие объекты не реагировали на клик
            GUIUtility.hotControl = 1;

            // Выполняем лучевой каст (raycast) из позиции мыши в мир
            Ray sceneRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(sceneRay, out hit))
            {
                // Если столкновение произошло с объектом, у которого collider не является триггером
                if (!hit.collider.isTrigger)
                {
                    // Создаём новый узел по месту клика
                    RacingLineNode newNode = new GameObject("Node ").AddComponent<RacingLineNode>();
                    // Регистрируем создание объекта для возможности отката (Undo)
                    Undo.RegisterCreatedObjectUndo(newNode.gameObject, "Создан узел гоночной линии");

                    // Устанавливаем позицию нового узла с небольшим смещением по оси Y
                    newNode.transform.position = hit.point + new Vector3(0, 0.25f, 0);
                    // Делаем новый узел дочерним объектом текущего RacingLine
                    newNode.transform.parent = _target.transform;
                    // Добавляем номер узла к его имени
                    newNode.name += _target.transform.childCount;

                    // Регистрируем изменение объекта для Undo
                    Undo.RecordObject(_target, "Добавлен узел гоночной линии");

                    // Добавляем новый узел в список узлов трассы
                    _target.nodes.Add(newNode.transform);
                    // Корректируем повороты узлов после добавления нового
                    _target.AdjustNodeRotation();
                }
            }
        }

        // Если событие MouseUp, сбрасываем hotControl
        if (e.button == 0 && e.type == EventType.MouseUp)
        {
            GUIUtility.hotControl = 0;
        }
    }
}
