using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;

// Этот класс является кастомным инспектором для объекта типа PlayerData
[CustomEditor(typeof(PlayerData))]
public class Editor_PlayerData : Editor
{
    // Ссылка на целевой объект PlayerData
    PlayerData _target;

    // Метод вызывается при включении инспектора
    void OnEnable()
    {
        _target = (PlayerData)target;  // Приводим объект к типу PlayerData
    }

    // Переопределённый метод для отрисовки пользовательского интерфейса в инспекторе
    public override void OnInspectorGUI()
    {
        // Рисуем стандартный интерфейс инспектора для PlayerData
        DrawDefaultInspector();

        // Добавляем горизонтальную разделительную линию
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // Кнопка для сброса данных
        if (GUILayout.Button("Сброс данных"))
        {
            _target.ResetData();  // Вызываем метод сброса данных у объекта PlayerData
        }

        // Закомментированный код для удаления файла с данными (если понадобится)
        //if (GUILayout.Button("Delete Data"))
        //{
        //    _target.DeleteSaveFile();
        //}
    }
}
