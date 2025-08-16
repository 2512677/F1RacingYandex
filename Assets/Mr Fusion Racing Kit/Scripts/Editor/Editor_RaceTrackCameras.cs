using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;

// Настраиваем редактор для компонента RaceTrackCameras
[CustomEditor(typeof(RaceTrackCameras))]
public class Editor_RaceTrackCameras : Editor
{
    // Ссылка на объект RaceTrackCameras, для которого применяется данный редактор
    RaceTrackCameras _target;

    // Сериализованные свойства для управления отображением в инспекторе
    SerializedProperty offset;
    SerializedProperty gizmoColor;
    SerializedProperty visible;

    // Метод вызывается при инициализации редактора
    void OnEnable()
    {
        // Приведение объекта target к типу RaceTrackCameras
        _target = (RaceTrackCameras)target;

        // Поиск сериализованных свойств по именам для дальнейшей работы в инспекторе
        offset = serializedObject.FindProperty("offset");
        gizmoColor = serializedObject.FindProperty("gizmoColor");
        visible = serializedObject.FindProperty("visible");
    }

    // Переопределение метода отрисовки интерфейса инспектора
    public override void OnInspectorGUI()
    {
        // Обновление данных сериализованного объекта
        serializedObject.Update();

        // Начало отслеживания изменений в инспекторе
        EditorGUI.BeginChangeCheck();

        // Вывод информационного окна с подсказкой (перевод строки на русский)
        EditorGUILayout.HelpBox("Используйте 'Shift + Левая кнопка мыши' для размещения камер трека", MessageType.Info);

        // Отображение свойства видимости
        EditorGUILayout.PropertyField(visible);
        GUILayout.Space(10);

        // Отображение свойств смещения и цвета гуиджи (gizmo)
        EditorGUILayout.PropertyField(offset);
        EditorGUILayout.PropertyField(gizmoColor);

        GUILayout.Space(10);
        // Вывод количества дочерних объектов (камер) в иерархии
        EditorGUILayout.LabelField("Всего камер: " + _target.transform.childCount);

        // Разделительная линия в виде горизонтального слайдера
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // Кнопка для удаления всех дочерних камер
        if (GUILayout.Button("Удалить всё"))
        {
            // Перебор всех дочерних объектов, ассоциированных с _target
            foreach (Transform child in _target.transform.GetComponentsInChildren<Transform>())
            {
                // Исключаем сам объект _target из удаления
                if (child != _target.transform)
                {
                    // Удаление объекта с регистрацией действия для возможности отмены
                    Undo.DestroyObjectImmediate(child.gameObject);
                }
            }
        }

        // Применение всех изменений к сериализованному объекту
        serializedObject.ApplyModifiedProperties();
    }

    // Метод для отрисовки элементов в окне сцены
    void OnSceneGUI()
    {
        // Вызов обработки событий мыши в сцене
        SceneViewRaycast();
    }

    // Метод для обработки кликов мыши в окне сцены
    void SceneViewRaycast()
    {
        // Получаем текущее событие (клик, перемещение и т.д.)
        Event e = Event.current;

        // Если нажата левая кнопка мыши с удержанием клавиши Shift
        if (e.button == 0 && e.type == EventType.MouseDown && e.shift)
        {
            // Блокируем возможность клика по другим элементам во время этого действия
            GUIUtility.hotControl = 1;

            // Преобразуем положение мыши в луч (Ray) для определения точки на сцене
            Ray sceneRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;

            // Если луч пересекает физический объект на сцене
            if (Physics.Raycast(sceneRay, out hit))
            {
                // Проверяем, что столкнувшийся коллайдер не является триггером
                if (!hit.collider.isTrigger)
                {
                    // Создаем новый игровой объект для камеры трека
                    GameObject newTrackCam = new GameObject("Track Camera ");
                    // Регистрируем создание объекта, чтобы можно было отменить действие
                    Undo.RegisterCreatedObjectUndo(newTrackCam, "Создана камера трека");

                    // Устанавливаем позицию нового объекта: место удара луча со смещением по оси Y, заданным в offset
                    newTrackCam.transform.position = hit.point + new Vector3(0, _target.offset, 0);
                    // Назначаем родительский объект так, чтобы объект оказался внутри структуры _target
                    newTrackCam.transform.parent = _target.transform;
                    // Изменяем имя объекта, добавляя количество уже существующих дочерних объектов
                    newTrackCam.name += _target.transform.childCount;

                    // Добавляем компонент TrackCamera, который отвечает за поведение камеры трека
                    newTrackCam.AddComponent<TrackCamera>();
                }
            }
        }

        // Если левая кнопка мыши отпущена
        if (e.button == 0 && e.type == EventType.MouseUp)
        {
            // Сброс горячего элемента управления, чтобы вернуть стандартное поведение интерфейса
            GUIUtility.hotControl = 0;
        }
    }
}
