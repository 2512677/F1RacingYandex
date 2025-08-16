using UnityEngine;

// Атрибут позволяет создавать объект данных трека через меню в Unity Editor
[CreateAssetMenu(fileName = "Новый трек", menuName = "MRFE/Новые данные о треке", order = 1)]
public class TrackData : ScriptableObject
{
    public string scene;                // Имя сцены, связанной с треком
    public string trackName;            // Название трека
    public string trackDescription;     // Описание трека
    public string trackKm;              // Пробег трека (в километрах), возможно, как строка для формата
    public float trackLength;           // Длина трека в метрах или километрах
    public Sprite trackImage;           // Изображение трека для UI
    public Sprite trackMinimap;         // Мини-карта трека для UI
    public Sprite trackIcon;            // Иконка трека для отображения в списках
    public string uniqueID;             // Уникальный идентификатор трека
}
