using UnityEngine;
using UnityEditor;
using RGSK;

// Кастомный редактор для компонента RaceUIManager
// Этот инспектор позволяет просматривать и настраивать все панели UI, а также обновлять их состояние
[CustomEditor(typeof(RaceUIManager))]
public class Editor_RaceUI : Editor
{
    // Ссылка на объект RaceUIManager
    RaceUIManager uiManager;

    // Сериализованные свойства, соответствующие полям RaceUIManager
    SerializedProperty preRacePanel;
    SerializedProperty startingGridPanel;
    SerializedProperty racePanel;
    SerializedProperty inRaceStandings;
    SerializedProperty driftPanel;
    SerializedProperty targetScorePanel;
    SerializedProperty pausePanel;
    SerializedProperty postRacePanel;
    SerializedProperty raceResultsPanel;
    SerializedProperty championshipResultsPanel;
    SerializedProperty otherResultsPanel;
    SerializedProperty replayPanel;
    SerializedProperty activePanel;
    SerializedProperty autoSelectButtons;

    // Метод OnEnable вызывается при инициализации редактора
    void OnEnable()
    {
        uiManager = (RaceUIManager)target;
        preRacePanel = serializedObject.FindProperty("preRacePanel");
        startingGridPanel = serializedObject.FindProperty("startingGridPanel");
        racePanel = serializedObject.FindProperty("racePanel");
        inRaceStandings = serializedObject.FindProperty("inRaceStandings");
        driftPanel = serializedObject.FindProperty("driftPanel");
        targetScorePanel = serializedObject.FindProperty("targetScorePanel");
        pausePanel = serializedObject.FindProperty("pausePanel");
        postRacePanel = serializedObject.FindProperty("postRacePanel");
        raceResultsPanel = serializedObject.FindProperty("raceResultsPanel");
        championshipResultsPanel = serializedObject.FindProperty("championshipResultsPanel");
        otherResultsPanel = serializedObject.FindProperty("otherResultsPanel");
        replayPanel = serializedObject.FindProperty("replayPanel");
        activePanel = serializedObject.FindProperty("activePanel");
        autoSelectButtons = serializedObject.FindProperty("autoSelectButtons");
    }

    // Основной метод отрисовки пользовательского интерфейса инспектора
    public override void OnInspectorGUI()
    {
        // Обновляем сериализованный объект для синхронизации данных
        serializedObject.Update();

        EditorGUILayout.LabelField("Настройки Race UI", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Отображение полей с соответствующими подписями
        EditorGUILayout.PropertyField(preRacePanel, new GUIContent("Панель перед гонкой"));
        EditorGUILayout.PropertyField(startingGridPanel, new GUIContent("Панель стартовой решетки"));
        EditorGUILayout.PropertyField(racePanel, new GUIContent("Панель гонки"));
        EditorGUILayout.PropertyField(inRaceStandings, new GUIContent("Таблица позиций в гонке"));
        EditorGUILayout.PropertyField(driftPanel, new GUIContent("Панель дрифта"));
        EditorGUILayout.PropertyField(targetScorePanel, new GUIContent("Панель целевого счета"));
        EditorGUILayout.PropertyField(pausePanel, new GUIContent("Панель паузы"));
        EditorGUILayout.PropertyField(postRacePanel, new GUIContent("Панель после гонки"));
        EditorGUILayout.PropertyField(raceResultsPanel, new GUIContent("Панель результатов гонки"));
        EditorGUILayout.PropertyField(championshipResultsPanel, new GUIContent("Панель результатов чемпионата"));
        EditorGUILayout.PropertyField(otherResultsPanel, new GUIContent("Панель прочих результатов"));
        EditorGUILayout.PropertyField(replayPanel, new GUIContent("Панель повтора"));
        EditorGUILayout.PropertyField(activePanel, new GUIContent("Активная панель"));
        EditorGUILayout.PropertyField(autoSelectButtons, new GUIContent("Авто выбор кнопок"));

        EditorGUILayout.Space();
        // Кнопка для обновления панелей в зависимости от типа гонки
        if (GUILayout.Button("Обновить панели по типу гонки"))
        {
            uiManager.UpdatePanelsBasedOnRaceType();
        }
        // Кнопка для скрытия всех панелей
        if (GUILayout.Button("Скрыть все панели"))
        {
            uiManager.HideAllPanels();
            uiManager.activePanel = null;
        }

        // Применяем изменения к сериализованному объекту
        serializedObject.ApplyModifiedProperties();
    }
}
