using UnityEngine;
using System.Collections;

namespace RGSK
{
    // Атрибут позволяет создавать объект данных автомобиля через меню в Unity Editor
    [CreateAssetMenu(fileName = "Новый автомобиль", menuName = "MRFE/New Vehicle Data", order = 1)]
    public class VehicleData2 : ScriptableObject
    {
        // Ссылка на основной объект транспортного средства
        public GameObject vehicle;

        // Ссылка на объект транспортного средства для ИИ
        public GameObject aiVehicle;

        // Ссылка на объект транспортного средства для меню
        public GameObject menuVehicle;

        [Space(10)]
        // Уникальный идентификатор транспортного средства
        public string uniqueID;

        // Класс транспортного средства (например, спортивный, гоночный и т.д.)
        public string vehicleClass;

        [Space(10)]
        // Название транспортного средства
        public string vehicleName;

        // Описание транспортного средства, отображаемое в виде многострочного текста
        [TextArea(2, 5)]
        public string vehicleDescription;

        [Header("Технические характеристики")]
        // Максимальная скорость транспортного средства (значение от 0 до 1)
        [Range(0, 1)]
        public float topSpeed;

        // Ускорение транспортного средства (значение от 0 до 1)
        [Range(0, 1)]
        public float acceleration;

        // Управляемость транспортного средства (значение от 0 до 1)
        [Range(0, 1)]
        public float handling;

        // Тормозные характеристики транспортного средства (значение от 0 до 1)
        [Range(0, 1)]
        public float braking;

        // Масса транспортного средства
        public float mass;

        // Мощность двигателя транспортного средства
        public float power;

        [Header("Прочее")]
        // Цена транспортного средства
        public float price;

        // Материал кузова транспортного средства
        public Material bodyMaterial;
    }
}
