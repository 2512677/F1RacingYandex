using UnityEngine;
using UnityEngine.UI;

public class GyroSlider : MonoBehaviour
{
    private Slider thisSlider;

    private void OnEnable()
    {
        thisSlider = GetComponent<Slider>();

        // ������������� �������� �������� � ���� (���� �� ������ ������ ��� ������� � ����������)
        thisSlider.minValue = 0f;
        thisSlider.maxValue = 3f;

        // ������ ���������� �������� (float), �� ��������� ����� ����� 2f
        float savedGyro = PlayerPrefs.GetFloat("GyroFloat", 2f);

        // ����������� ��������
        thisSlider.value = savedGyro;
    }

    public void valChanged()
    {
        // ���� ������� ��������
        float val = thisSlider.value;

        // ��������� ��� float
        PlayerPrefs.SetFloat("GyroFloat", val);

        // ����������� � RCC_Settings
        RCC_Settings.Instance.gyroSensitivity = val;

        // ��� ��������� ��������� �����
        PlayerPrefs.Save();
    }
}
