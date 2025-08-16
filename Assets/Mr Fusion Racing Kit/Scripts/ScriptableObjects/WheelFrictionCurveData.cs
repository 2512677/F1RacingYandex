using UnityEngine;
using System.Collections;

namespace RGSK
{
    // Атрибут позволяет создавать объект данных кривых трения колес через меню в Unity Editor
    [CreateAssetMenu(fileName = "WheelFrcitionCurveData", menuName = "RGSK/Resources/Wheel Friction Curve Data", order = 1)]
    public class WheelFrictionCurveData : ScriptableObject
    {
        private static WheelFrictionCurveData _instance;

        // Свойство для получения экземпляра WheelFrictionCurveData (паттерн Singleton)
        public static WheelFrictionCurveData Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Resources.Load("RGSK/ScriptableObjects/WheelFrcitionCurveData") as WheelFrictionCurveData;
                return _instance;
            }
        }

        public FrictionCurves[] wheelFrictionCurves; // Массив кривых трения колес

        // Метод для получения кривой трения по идентификатору
        public FrictionCurves GetFrictionCurve(int id)
        {
            for (int i = 0; i < wheelFrictionCurves.Length; i++)
            {
                if (wheelFrictionCurves[i].id == id)
                {
                    return wheelFrictionCurves[i];
                }
            }

            return null; // Возвращает null, если кривая не найдена
        }

        /// <summary>
        /// Получает идентификатор кривой трения для заданного типа гонки.
        /// </summary>
        /// <param name="raceType">Тип гонки для поиска кривой трения.</param>
        /// <returns>Идентификатор кривой трения или 0, если не найдено.</returns>
        public int GetIDForRaceType(RaceType raceType)
        {
            for (int i = 0; i < wheelFrictionCurves.Length; i++)
            {
                for (int y = 0; y < wheelFrictionCurves[i].appliedRaceTypes.Length; y++)
                {
                    if (wheelFrictionCurves[i].appliedRaceTypes[y] == raceType)
                        return wheelFrictionCurves[i].id;
                }
            }

            return 0; // Возвращает 0, если кривая не найдена
        }

        // Внутренний класс для хранения данных кривых трения
        [System.Serializable]
        public class FrictionCurves
        {
            public string name; // Название кривой трения

            [Space(10)]
            public int id; // Уникальный идентификатор кривой трения
            public RaceType[] appliedRaceTypes; // Массив типов гонок, к которым применяется кривая трения

            [Space(10)]
            [Header("Кривая трения вперед")]
            public float forwardExtremumSlip;     // Экстремальное скольжение вперед
            public float forwardExtremumValue;    // Экстремальное значение трения вперед
            public float forwardAsymptoteSlip;    // Асимптотическое скольжение вперед
            public float forwardAsymptoteValue;   // Асимптотическое значение трения вперед

            [Header("Кривая бокового трения")]
            public float sidewaysExtremumSlip;    // Экстремальное скольжение в стороны
            public float sidewaysExtremumValue;   // Экстремальное значение трения в стороны
            public float sidewaysAsymptoteSlip;   // Асимптотическое скольжение в стороны
            public float sidewaysAsymptoteValue;  // Асимптотическое значение трения в стороны
        }
    }
}
