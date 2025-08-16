using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class SpikeController : MonoBehaviour
{
    [Tooltip("Через сколько секунд после спавна убрать физику и коллайдеры (кроме триггеров)")]
    public float disableDelay = 0.2f;

    [Tooltip("Интервал мигания материала в секундах")]
    public float flashInterval = 0.2f;

    [Tooltip("Материал-шаблон, который будет мигать (будет клонироваться)")]
    public Material flashMaterial;

    [Tooltip("Цвет эмиссии при мигании")]
    public Color emissionColor = Color.red;

    [Tooltip("Список Renderer'ов, которые должны мигать. Если пуст, возьмётся MeshRenderer этого объекта")]
    public Renderer[] targetRenderers;

    // Сохраняем оригинальные массивы материалов для каждого Renderer
    private List<Material[]> originalMaterialsList = new List<Material[]>();
    // Подготовленные массивы флеш-материалов для каждого Renderer
    private List<Material[]> flashMaterialsList = new List<Material[]>();

    private void Start()
    {
        // Если не указаны вручную — ищем единственный Renderer на этом объекте
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            var rend = GetComponent<Renderer>();
            if (rend != null)
                targetRenderers = new[] { rend };
        }

        // Копируем оригинальные материалы и формируем флеш-массивы
        foreach (var rend in targetRenderers)
        {
            if (rend == null) continue;

            // Сохраняем оригинал
            originalMaterialsList.Add(rend.materials);

            // Формируем флеш-массив
            if (flashMaterial != null)
            {
                // Клонируем шаблонный материал, настраиваем эмиссию
                var instanced = new Material(flashMaterial);
                instanced.EnableKeyword("_EMISSION");
                instanced.SetColor("_EmissionColor", emissionColor);

                // Заполняем массив таким материалом
                var fm = new Material[rend.materials.Length];
                for (int i = 0; i < fm.Length; i++)
                    fm[i] = instanced;

                flashMaterialsList.Add(fm);
            }
            else
            {
                // Если шаблона нет, дублируем оригинал (мигание не произойдёт)
                flashMaterialsList.Add(rend.materials);
            }
        }

        // Запускаем бесконечное мигание (не прерывается после disableDelay)
        StartCoroutine(FlashMaterial());
        // Запускаем логику отключения физики/коллайдеров
        StartCoroutine(DisablePhysicsAfterDelay());
    }

    private IEnumerator FlashMaterial()
    {
        while (true)
        {
            // Ставим флеш-материалы
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                if (targetRenderers[i] != null)
                    targetRenderers[i].materials = flashMaterialsList[i];
            }
            yield return new WaitForSeconds(flashInterval);

            // Возвращаем оригинальные
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                if (targetRenderers[i] != null)
                    targetRenderers[i].materials = originalMaterialsList[i];
            }
            yield return new WaitForSeconds(flashInterval);
        }
    }

    private IEnumerator DisablePhysicsAfterDelay()
    {
        // Ждём перед отключением физики
        yield return new WaitForSeconds(disableDelay);

        // 1) Удаляем Rigidbody
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
            Destroy(rb);

        // 2) Удаляем все коллайдеры, кроме триггеров
        foreach (var col in GetComponents<Collider>())
        {
            if (!col.isTrigger)
                Destroy(col);
        }
    }
}
