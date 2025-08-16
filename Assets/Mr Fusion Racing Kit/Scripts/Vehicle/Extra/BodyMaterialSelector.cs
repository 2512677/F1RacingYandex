// BodyMaterialSelector.cs
using UnityEngine;

/// <summary>
/// Применяет выбранный материал и цвет к заданным Renderer’ам машины.
/// </summary>
public class BodyMaterialSelector : MonoBehaviour
{
    [System.Serializable]
    public class ObjectToColor
    {
        [Tooltip("Renderer объекта, к которому будем применять материал")]
        public Renderer bodyRenderer;
        [Tooltip("Индекс материала в массиве Renderer.materials")]
        public int materialIndex;
    }

    [Header("Настройки окраски")]
    [Tooltip("Список Renderer’ов и индексов их материалов")]
    public ObjectToColor[] objectsToColor;

    [Tooltip("Возможные цвета тела машины")]
    public Color[] bodyColors;

    [Tooltip("Возможные базовые материалы (если пуст — используется из модели)")]
    public Material[] bodyMaterials;

    void Start()
    {
        ApplyMaterials();
    }

    /// <summary>
    /// Клонирует материал и задаёт ему случайный цвет из bodyColors.
    /// Если bodyMaterials не пуст — клонирует материал из него, иначе — из исходного Renderer.
    /// Если objectsToColor пуст — красит все дочерние Renderer слот 0.
    /// </summary>
    public void ApplyMaterials()
    {
        // выбор случайного цвета или дефолт белого
        Color selColor = (bodyColors != null && bodyColors.Length > 0)
            ? bodyColors[Random.Range(0, bodyColors.Length)]
            : Color.white;

        // выбор случайного базового материала (или null)
        Material selMat = (bodyMaterials != null && bodyMaterials.Length > 0)
            ? bodyMaterials[Random.Range(0, bodyMaterials.Length)]
            : null;

        bool painted = false;

        if (objectsToColor != null && objectsToColor.Length > 0)
        {
            foreach (var obj in objectsToColor)
            {
                if (obj == null || obj.bodyRenderer == null)
                    continue;

                Renderer rend = obj.bodyRenderer;
                Material[] mats = rend.materials;
                int idx = Mathf.Clamp(obj.materialIndex, 0, mats.Length - 1);

                // клонируем материал
                Material baseMat = selMat != null ? selMat : mats[idx];
                Material newMat = Instantiate(baseMat);
                SetColor(newMat, selColor);

                mats[idx] = newMat;
                rend.materials = mats;

                Debug.Log($"[BodyMaterialSelector] Окрасил {rend.gameObject.name} в слот {idx}");
                painted = true;
            }
        }

        if (!painted)
        {
            // fallback — красим все Renderer слот 0
            Renderer[] all = GetComponentsInChildren<Renderer>(true);
            foreach (var rend in all)
            {
                Material[] mats = rend.materials;
                if (mats.Length == 0)
                    continue;

                Material baseMat = selMat != null ? selMat : mats[0];
                Material newMat = Instantiate(baseMat);
                SetColor(newMat, selColor);

                mats[0] = newMat;
                rend.materials = mats;

                Debug.Log($"[BodyMaterialSelector] Fallback окрасил {rend.gameObject.name} слот 0");
            }
        }
    }

    private void SetColor(Material mat, Color col)
    {
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", col);
        else if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", col);
        else
            Debug.LogWarning($"[BodyMaterialSelector] Материал {mat.name} не содержит _Color/_BaseColor");
    }
}
