using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
using RGSK;            // пространство имён твоего VehicleDatabase
using static CarClass; // позволяет писать Sport вместо CarClass.Sport

// ─────────────────────────────────────────────────────────────────────────────
//  Редактор базы машин
// ─────────────────────────────────────────────────────────────────────────────
[CustomEditor(typeof(VehicleDatabase))]
public class VehicleDatabaseEditor : Editor
{
    private SerializedProperty vehiclesProp;                         // сам массив
    private Dictionary<VehicleClass, ReorderableList> lists;         // списки по классам

    private void OnEnable()
    {
        vehiclesProp = serializedObject.FindProperty("vehicles");
        lists = new Dictionary<VehicleClass, ReorderableList>();

        // Для каждого значения enum VehicleClass создаём отдельный ReorderableList
        foreach (VehicleClass cls in System.Enum.GetValues(typeof(VehicleClass)))
        {
            lists[cls] = CreateListForClass(cls);
        }
    }

    // -------------------------------------------------------------------------
    //  Создание списка для конкретного класса
    // -------------------------------------------------------------------------
    private ReorderableList CreateListForClass(VehicleClass cls)
    {
        // Вью-обёртка над исходным массивом: оставляет только элементы нужного класса
        var filtered = new FilteredList(vehiclesProp, cls);

        var list = new ReorderableList(
            filtered,                               // источник данных
            typeof(SerializedProperty),             // тип элементов
            draggable: true,
            displayHeader: true,
            displayAddButton: true,
            displayRemoveButton: true);

        // ───── Заголовок ─────
        list.drawHeaderCallback = rect =>
            // выводим название класса + количество
            EditorGUI.LabelField(rect, $"{cls} ({filtered.Count})");

        // ───── Отрисовка элемента ─────
        list.drawElementCallback = (rect, index, active, focused) =>
        {
            var prop = (SerializedProperty)filtered[index];
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                prop,
                includeChildren: true);
        };

        // ───── Корректная высота для развёрнутых элементов ─────
        list.elementHeightCallback = index =>
        {
            var prop = (SerializedProperty)filtered[index];
            return EditorGUI.GetPropertyHeight(prop, true) + 4;
        };

        // ───── Перетаскивание внутри списка ─────
        list.onReorderCallbackWithDetails = (rl, oldIndex, newIndex) =>
        {
            serializedObject.Update();
            int globalOld = filtered.GetGlobalIndex(oldIndex);
            int globalNew = filtered.GetGlobalIndex(newIndex);
            vehiclesProp.MoveArrayElement(globalOld, globalNew);
            serializedObject.ApplyModifiedProperties();
            filtered.Rebuild();                 // обновляем индексы после перемещения
        };

        // ───── Добавление элемента ─────
        list.onAddCallback = rl =>
        {
            serializedObject.Update();
            int insertPos = vehiclesProp.arraySize;
            vehiclesProp.InsertArrayElementAtIndex(insertPos);

            // присваиваем класс новому элементу
            var newElem = vehiclesProp.GetArrayElementAtIndex(insertPos);
            newElem.FindPropertyRelative("CarClass").enumValueIndex = (int)cls;

            serializedObject.ApplyModifiedProperties();
            filtered.Rebuild();                 // сразу отобразится в списке
        };

        // ───── Удаление элемента ─────
        list.onRemoveCallback = rl =>
        {
            serializedObject.Update();
            int removeGlobal = filtered.GetGlobalIndex(rl.index);
            vehiclesProp.DeleteArrayElementAtIndex(removeGlobal);
            serializedObject.ApplyModifiedProperties();
            filtered.Rebuild();
        };

        return list;
    }

    // -------------------------------------------------------------------------
    //  Инспектор
    // -------------------------------------------------------------------------
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Рисуем все списки по порядку
        foreach (var kv in lists)
        {
            kv.Value.DoLayoutList();
            EditorGUILayout.Space();
        }

        // ─── Новая секция: количество машин по каждому классу ───
        EditorGUILayout.LabelField("Количество машин по классам:", EditorStyles.boldLabel);
        foreach (VehicleClass cls in System.Enum.GetValues(typeof(VehicleClass)))
        {
            EditorGUILayout.LabelField($"• {cls}: {GetClassCount(cls)}");
        }

        EditorGUILayout.Space();

        // ─── Общий счётчик (оставляем, как было) ───
        EditorGUILayout.LabelField(
            "Общее количество машин в базе:",
            vehiclesProp.arraySize.ToString(),
            EditorStyles.boldLabel);

        serializedObject.ApplyModifiedProperties();
    }

    // -------------------------------------------------------------------------
    //  Подсчитать, сколько машин заданного класса в массиве
    // -------------------------------------------------------------------------
    private int GetClassCount(VehicleClass cls)
    {
        int cnt = 0;
        for (int i = 0; i < vehiclesProp.arraySize; i++)
        {
            var e = vehiclesProp.GetArrayElementAtIndex(i);
            if ((VehicleClass)e.FindPropertyRelative("CarClass").enumValueIndex == cls)
                cnt++;
        }
        return cnt;
    }

    // ──────────────────────────────────────────────────────────────────────────
    //  FilteredList: вью-обёртка над SerializedProperty-массивом,
    //                показывающая только элементы заданного класса
    // ──────────────────────────────────────────────────────────────────────────
    private class FilteredList : IList
    {
        private SerializedProperty sourceArray;
        private VehicleClass cls;
        private List<int> indices;

        public FilteredList(SerializedProperty arrayProp, VehicleClass cls)
        {
            this.sourceArray = arrayProp;
            this.cls = cls;
            Rebuild();
        }

        // Перестраиваем список индексов, когда массив меняется
        public void Rebuild()
        {
            indices = new List<int>();
            for (int i = 0; i < sourceArray.arraySize; i++)
            {
                var e = sourceArray.GetArrayElementAtIndex(i);
                if ((VehicleClass)e.FindPropertyRelative("CarClass").enumValueIndex == cls)
                    indices.Add(i);
            }
        }

        // Получить индекс в исходном массиве по локальному индексу
        public int GetGlobalIndex(int filteredIndex) => indices[filteredIndex];

        // -------------------- IList реализация --------------------
        public object this[int index]
        {
            get => sourceArray.GetArrayElementAtIndex(indices[index]);
            set { /* not used */ }
        }

        public int Count => indices.Count;
        public bool IsReadOnly => false;

        public IEnumerator GetEnumerator()
        {
            foreach (var i in indices)
                yield return sourceArray.GetArrayElementAtIndex(i);
        }

        #region NotUsed members
        public bool IsFixedSize => false;
        public bool IsSynchronized => false;
        public object SyncRoot => null;
        public int Add(object value) { throw new System.NotSupportedException(); }
        public bool Contains(object value) { return false; }
        public void Clear() { }
        public int IndexOf(object value) { return -1; }
        public void Insert(int index, object value) { }
        public void Remove(object value) { }
        public void RemoveAt(int index) { }
        public void CopyTo(System.Array array, int index) { }
        #endregion
    }
}
