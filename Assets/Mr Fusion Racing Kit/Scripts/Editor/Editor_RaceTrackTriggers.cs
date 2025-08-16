using UnityEngine;
using UnityEditor;
using System.Collections;
using RGSK;

// Кастомный редактор для компонента RaceTrackTriggers
// Этот редактор позволяет настраивать триггеры гонок в инспекторе и в окне сцены,
// а также добавлять, удалять и редактировать триггеры прямо в сцене.
[CustomEditor(typeof(RaceTrackTriggers))]
public class Editor_RaceTrackTriggers : Editor
{
    RaceTrackTriggers _target;

    SerializedProperty triggerType;
    SerializedProperty autoSelectTrigger;

    // Метод OnEnable вызывается при активации редактора и инициализирует сериализованные свойства
    void OnEnable()
    {
        _target = (RaceTrackTriggers)target;

        triggerType = serializedObject.FindProperty("triggerType");
        autoSelectTrigger = serializedObject.FindProperty("autoSelectTrigger");
    }

    // Основной метод отрисовки пользовательского инспектора
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Информационное сообщение для пользователя
        EditorGUILayout.HelpBox("Используйте Shift + левую кнопку мыши для размещения новых триггеров гонок", MessageType.Info);

        // Отображение настроек триггера в инспекторе
        EditorGUILayout.PropertyField(triggerType);
        EditorGUILayout.PropertyField(autoSelectTrigger);

        // Разделитель
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // Кнопка для удаления всех триггеров (удаляются все дочерние объекты)
        if (GUILayout.Button("Удалить всё"))
        {
            foreach (Transform sp in _target.transform.GetComponentsInChildren<Transform>())
            {
                if (sp != _target.transform)
                {
                    Undo.DestroyObjectImmediate(sp.gameObject);
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    // Метод для отрисовки элементов в окне Scene
    void OnSceneGUI()
    {
        SceneViewRaycast();
    }

    // Метод для обработки кликов мышью в окне Scene
    void SceneViewRaycast()
    {
        Event e = Event.current;

        // Если нажата левая кнопка мыши с зажатым Shift и событие MouseDown
        if (e.button == 0 && e.type == EventType.MouseDown && e.shift)
        {
            // Убедимся, что другие объекты не обрабатывают клик
            GUIUtility.hotControl = 1;

            // Выполняем лучевой каст (raycast) из позиции мыши в мир
            Ray sceneRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(sceneRay, out hit))
            {
                if (!hit.collider.isTrigger)
                {
                    // Создаем новый объект-триггер на месте клика
                    GameObject newObject = new GameObject(_target.triggerType.ToString());
                    Undo.RegisterCreatedObjectUndo(newObject, "Создан триггер гонок");
                    newObject.AddComponent<BoxCollider>();
                    newObject.GetComponent<BoxCollider>().size = new Vector3(30, 10, 1);
                    newObject.GetComponent<BoxCollider>().isTrigger = true;
                    newObject.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                    newObject.AddComponent<RaceTrigger>();
                    newObject.GetComponent<RaceTrigger>().triggerType = _target.triggerType;
                    newObject.transform.position = hit.point + new Vector3(0, 5, 0);
                    newObject.transform.parent = _target.transform;

                    if (_target.autoSelectTrigger)
                    {
                        // Выбираем созданный триггер в редакторе
                        Selection.activeObject = newObject;

                        // Сбрасываем hotControl
                        GUIUtility.hotControl = 0;
                    }
                }
            }
        }

        // Сбрасываем hotControl при отпускании мыши
        if (e.button == 0 && e.type == EventType.MouseUp)
        {
            GUIUtility.hotControl = 0;
        }
    }
}
