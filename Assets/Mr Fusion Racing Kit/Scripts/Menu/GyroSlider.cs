using UnityEngine;
using UnityEngine.UI;

public class GyroSlider : MonoBehaviour
{
    private Slider thisSlider;

    private void OnEnable()
    {
        thisSlider = GetComponent<Slider>();

        // Устанавливаем диапазон слайдера в коде (если не хотите делать это вручную в инспекторе)
        thisSlider.minValue = 0f;
        thisSlider.maxValue = 3f;

        // Читаем сохранённое значение (float), по умолчанию пусть будет 2f
        float savedGyro = PlayerPrefs.GetFloat("GyroFloat", 2f);

        // Присваиваем слайдеру
        thisSlider.value = savedGyro;
    }

    public void valChanged()
    {
        // Берём текущее значение
        float val = thisSlider.value;

        // Сохраняем как float
        PlayerPrefs.SetFloat("GyroFloat", val);

        // Присваиваем в RCC_Settings
        RCC_Settings.Instance.gyroSensitivity = val;

        // Для надёжности сохраняем сразу
        PlayerPrefs.Save();
    }
}
