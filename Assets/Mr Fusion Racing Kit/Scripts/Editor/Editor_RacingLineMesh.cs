using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;

[CustomEditor(typeof(RacingLineMesh))]
public class Editor_RacingLineMesh : Editor
{
    // Ссылка на экземпляр RacingLineMesh, с которым работаем
    RacingLineMesh _target;

    // Метод OnEnable вызывается при активации редактора
    void OnEnable()
    {
        _target = (RacingLineMesh)target;
    }

    // Основной метод отрисовки пользовательского инспектора
    public override void OnInspectorGUI()
    {
        // Вывод предупреждающего сообщения о том, что текущая версия Racing Line Mesh не оптимизирована
        EditorGUILayout.HelpBox("Обратите внимание: Racing Line Mesh v" + Editor_Helper.version + " не оптимизирован.", MessageType.Warning);

        // Отрисовываем стандартный инспектор для объекта
        DrawDefaultInspector();

        // Разделитель (горизонтальная линия)
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // Кнопка для генерации меша трассы
        if (GUILayout.Button("Сгенерировать меш трассы"))
        {
            _target.GenerateRaceLine();
        }

        // Кнопка для объединения отдельных мешей трассы в один
        if (GUILayout.Button("Объединить меш трассы"))
        {
            _target.CombineMeshes();
        }

        // Кнопка для удаления меша трассы
        if (GUILayout.Button("Удалить меш трассы"))
        {
            _target.DeleteRaceLine();
        }
    }
}
