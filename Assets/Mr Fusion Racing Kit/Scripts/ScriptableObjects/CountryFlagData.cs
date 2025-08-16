using UnityEngine;
using System.Collections;

namespace RGSK
{
    // Атрибут позволяет создавать объект данных флагов стран через меню в Unity Editor
    [CreateAssetMenu(fileName = "CountryFlagData", menuName = "MRFE/Resources/Country Flag Data", order = 1)]
    public class CountryFlagData : ScriptableObject
    {
        private static CountryFlagData _instance;

        // Свойство для получения экземпляра CountryFlagData (паттерн Singleton)
        public static CountryFlagData Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Resources.Load("RGSK/ScriptableObjects/CountryFlagData") as CountryFlagData;
                return _instance;
            }
        }

        public Flags[] countryFlags; // Массив флагов стран

        // Метод для получения флага страны по ее национальности
        public Sprite GetFlag(Nationality country)
        {
            for (int i = 0; i < countryFlags.Length; i++)
            {
                if (country == countryFlags[i].nationality)
                {
                    if (countryFlags[i].flag != null)
                        return countryFlags[i].flag;
                }
            }

            return null; // Возвращает null, если флаг не найден
        }

        // Внутренний класс для хранения данных флага страны
        [System.Serializable]
        public class Flags
        {
            public Nationality nationality; // Национальность страны
            public Sprite flag;             // Спрайт флага страны
        }
    }
}
