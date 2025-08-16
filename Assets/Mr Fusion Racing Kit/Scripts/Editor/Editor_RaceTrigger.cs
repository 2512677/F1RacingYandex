using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using RGSK;

// Кастомный редактор для компонента RaceTrigger
// Этот редактор настраивает отображение свойств RaceTrigger в инспекторе Unity.
[CustomEditor(typeof(RaceTrigger))]
public class Editor_RaceTrigger : Editor
{
    // Ссылка на целевой объект RaceTrigger
    RaceTrigger _target;

    // Сериализованные свойства, используемые в инспекторе
    SerializedProperty triggerType;       // Тип триггера
    SerializedProperty nearestTrackNode;  // Ближайший узел трассы
    SerializedProperty index;             // Индекс триггера
    SerializedProperty addedTime;         // Добавленное время для контрольного пункта

    // Метод OnEnable вызывается при инициализации редактора
    void OnEnable()
    {
        // Приводим целевой объект к типу RaceTrigger
        _target = (RaceTrigger)target;

        // Находим сериализованные свойства по их именам
        triggerType = serializedObject.FindProperty("triggerType");
        nearestTrackNode = serializedObject.FindProperty("nearestTrackNode");
        index = serializedObject.FindProperty("index");
        addedTime = serializedObject.FindProperty("addedTime");
    }

    // Метод отрисовки пользовательского интерфейса инспектора
    public override void OnInspectorGUI()
    {
        // Обновляем сериализованный объект для синхронизации данных
        serializedObject.Update();

        // Отображаем поле для выбора типа триггера
        EditorGUILayout.PropertyField(triggerType);

        // Если тип триггера не является финишной линией
        if (_target.triggerType != RaceTriggerType.FinishLine)
        {
            // Если ближайший узел не назначен, выводим предупреждение
            if (_target.nearestTrackNode == null)
            {
                EditorGUILayout.HelpBox("Ближайший узел не назначен! Это может позволить проехать триггер в обратном направлении.", MessageType.Warning);
            }

            // Выводим информационное сообщение о назначении ближайшего узла
            EditorGUILayout.HelpBox("Ближайший узел для этого триггера. Узел должен находиться позади триггера.", MessageType.Info);
            // Отображаем поле для назначения ближайшего узла
            EditorGUILayout.PropertyField(nearestTrackNode);

            // Разделитель
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // Выводим информационное сообщение о необходимости уникального индекса
            EditorGUILayout.HelpBox("Это число должно быть уникальным для каждого типа триггера.", MessageType.Info);
            // Отображаем поле для ввода индекса
            EditorGUILayout.PropertyField(index);

            // Разделитель
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        // Если тип триггера равен контрольному пункту
        if (_target.triggerType == RaceTriggerType.Checkpoint)
        {
            // Выводим информационное сообщение о добавляемом времени
            EditorGUILayout.HelpBox("Количество времени, добавляемого, когда гонщик проходит этот контрольный пункт.", MessageType.Info);
            // Отображаем поле для ввода добавленного времени
            EditorGUILayout.PropertyField(addedTime);
        }

        // Применяем изменения к сериализованному объекту
        serializedObject.ApplyModifiedProperties();
    }
}
