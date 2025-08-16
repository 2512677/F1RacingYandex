using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    /// <summary>
    /// ������ ����������� ������� � ���������� ������.
    /// ���������� ������-������ � �������� ������ ������.
    /// </summary>
    public class CarRewardPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("����������� ������ ���������� ������")]
        public Image previewImage;

        [Tooltip("����� �������� ���������� ������")]
        public Text carNameText;

        /// <summary>
        /// ���������� ������ � ������ � ��������� ������ �� ����������� ��������������.
        /// </summary>
        /// <param name="carID">���������� ������������� ������ �� VehicleDatabase</param>
        public void Show(string carID)
        {
            // ����������� ������ ������ �� ����
            var v = VehicleDatabase.Instance.GetVehicle(carID);
            if (v != null && v.previewSprite != null)
            {
                previewImage.sprite = v.previewSprite;
                carNameText.text = v.ModelName;
                gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"CarRewardPanel: ������ ��� ������ '{carID}' �� �������.");
                Hide();
            }
        }

        /// <summary>
        /// �������� ������.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
