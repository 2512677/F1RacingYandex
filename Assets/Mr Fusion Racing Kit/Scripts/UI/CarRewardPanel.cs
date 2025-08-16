using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    /// <summary>
    /// Панель отображения награды — выигранной машины.
    /// Показывает превью-спрайт и название модели машины.
    /// </summary>
    public class CarRewardPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("Изображение превью выигранной машины")]
        public Image previewImage;

        [Tooltip("Текст названия выигранной машины")]
        public Text carNameText;

        /// <summary>
        /// Отображает панель с превью и названием машины по уникальному идентификатору.
        /// </summary>
        /// <param name="carID">Уникальный идентификатор машины из VehicleDatabase</param>
        public void Show(string carID)
        {
            // Подтягиваем данные машины из базы
            var v = VehicleDatabase.Instance.GetVehicle(carID);
            if (v != null && v.previewSprite != null)
            {
                previewImage.sprite = v.previewSprite;
                carNameText.text = v.ModelName;
                gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"CarRewardPanel: превью для машины '{carID}' не найдено.");
                Hide();
            }
        }

        /// <summary>
        /// Скрывает панель.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
