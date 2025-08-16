using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor; // Нужно для рисования в сцене через Handles
#endif

/// <summary>
/// Перечисление вариантов стрелок/поворотов.
/// </summary>
public enum CornerType
{
    None,           // Не показываем иконку
    Down,
    Up,
    Left,
    Right,
    LeftEasy,
    RightEasy,
    LeftUp45,
    RightUp45,
    LeftDown45,
    RightDown45,
    Left90,
    Right90,
    LeftHairpin,
    RightHairpin,
    LeftS,
    RightS,
    RoundaboutLeft,
    RoundaboutRight
}

/// <summary>
/// Связка: какой CornerType соответствует какому Sprite
/// </summary>
[System.Serializable]
public class CornerSpriteGizmo
{
    public CornerType cornerType;
    public Sprite cornerSprite;
}

public class TrackNode : MonoBehaviour
{
    [Header("Ширина дороги в этом узле")]
    public float leftWidth = 5;
    public float rightWidth = 5;

    /// <summary>
    /// Тип поворота/иконки, которую хотим отобразить на гизмо
    /// </summary>
    public CornerType cornerType = CornerType.None;

    /// <summary>
    /// Массив, в котором мы сопоставляем CornerType → Sprite
    /// </summary>
    [Header("Настройки иконок для Gizmos")]
    public CornerSpriteGizmo[] cornerSpriteGizmos;

    [Tooltip("Показывать ли иконку поворота над узлом в Scene View?")]
    public bool showCornerIcon = true;

    /// <summary>
    /// (Опционально) Сколько метров поднять иконку над узлом, чтобы её было видно
    /// </summary>
    public float iconHeight = 1.5f;

    /// <summary>
    /// Расстояние по трассе (заполняется внешним кодом, если нужно)
    /// </summary>
    public float distanceAtNode { get; set; }

    void OnDrawGizmos()
    {
        // 1) Рисуем стандартные гизмо-линии для ширины дороги
        DrawLaneGizmos();

#if UNITY_EDITOR
        // 2) Показываем иконку, если cornerType != None
        // и если включён флажок showCornerIcon
        if (showCornerIcon && cornerType != CornerType.None)
        {
            // Пытаемся найти спрайт по cornerType
            Sprite foundSprite = GetSpriteForCornerType(cornerType);
            if (foundSprite != null)
            {
                // Превращаем sprite в Texture2D (для Handles.Label)
                Texture2D iconTex = foundSprite.texture;
                if (iconTex != null)
                {
                    // Позиция над узлом, чтобы иконка не врезалась в асфальт
                    Vector3 iconPos = transform.position + Vector3.up * iconHeight;

                    // Рисуем иконку через Handles.Label
                    Handles.Label(iconPos, new GUIContent(iconTex));
                }
            }
            else
            {
                // Если не нашли спрайт для cornerType, покажем текст:
                Vector3 textPos = transform.position + Vector3.up * (iconHeight + 0.2f);
                Handles.Label(textPos, $"[Нет спрайта для {cornerType}]");
            }
        }
#endif
    }

    /// <summary>
    /// Рисует линии ширины (лево/право) в режиме Gizmos,
    /// как было в исходном варианте.
    /// </summary>
    private void DrawLaneGizmos()
    {
        TrackLayout parent = GetComponentInParent<TrackLayout>();
        if (parent != null)
        {
            Gizmos.color = parent.color;
            if (parent.showSegments)
            {
                Gizmos.DrawLine(transform.position, transform.position + (transform.right * rightWidth));
                Gizmos.DrawLine(transform.position, transform.position + (-transform.right * leftWidth));

                Gizmos.DrawWireSphere(transform.position + (transform.right * rightWidth), 0.25f);
                Gizmos.DrawWireSphere(transform.position + (-transform.right * leftWidth), 0.25f);
            }
        }
    }

    /// <summary>
    /// Ищет в массиве cornerSpriteGizmos тот, где cornerType совпадает.
    /// </summary>
    private Sprite GetSpriteForCornerType(CornerType type)
    {
        // Пробегаем массив
        for (int i = 0; i < cornerSpriteGizmos.Length; i++)
        {
            if (cornerSpriteGizmos[i].cornerType == type)
            {
                return cornerSpriteGizmos[i].cornerSprite;
            }
        }
        // Если не нашли
        return null;
    }
}
