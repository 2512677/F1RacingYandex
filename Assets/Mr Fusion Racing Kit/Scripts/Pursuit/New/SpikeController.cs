using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class SpikeController : MonoBehaviour
{
    [Tooltip("����� ������� ������ ����� ������ ������ ������ � ���������� (����� ���������)")]
    public float disableDelay = 0.2f;

    [Tooltip("�������� ������� ��������� � ��������")]
    public float flashInterval = 0.2f;

    [Tooltip("��������-������, ������� ����� ������ (����� �������������)")]
    public Material flashMaterial;

    [Tooltip("���� ������� ��� �������")]
    public Color emissionColor = Color.red;

    [Tooltip("������ Renderer'��, ������� ������ ������. ���� ����, �������� MeshRenderer ����� �������")]
    public Renderer[] targetRenderers;

    // ��������� ������������ ������� ���������� ��� ������� Renderer
    private List<Material[]> originalMaterialsList = new List<Material[]>();
    // �������������� ������� ����-���������� ��� ������� Renderer
    private List<Material[]> flashMaterialsList = new List<Material[]>();

    private void Start()
    {
        // ���� �� ������� ������� � ���� ������������ Renderer �� ���� �������
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            var rend = GetComponent<Renderer>();
            if (rend != null)
                targetRenderers = new[] { rend };
        }

        // �������� ������������ ��������� � ��������� ����-�������
        foreach (var rend in targetRenderers)
        {
            if (rend == null) continue;

            // ��������� ��������
            originalMaterialsList.Add(rend.materials);

            // ��������� ����-������
            if (flashMaterial != null)
            {
                // ��������� ��������� ��������, ����������� �������
                var instanced = new Material(flashMaterial);
                instanced.EnableKeyword("_EMISSION");
                instanced.SetColor("_EmissionColor", emissionColor);

                // ��������� ������ ����� ����������
                var fm = new Material[rend.materials.Length];
                for (int i = 0; i < fm.Length; i++)
                    fm[i] = instanced;

                flashMaterialsList.Add(fm);
            }
            else
            {
                // ���� ������� ���, ��������� �������� (������� �� ���������)
                flashMaterialsList.Add(rend.materials);
            }
        }

        // ��������� ����������� ������� (�� ����������� ����� disableDelay)
        StartCoroutine(FlashMaterial());
        // ��������� ������ ���������� ������/�����������
        StartCoroutine(DisablePhysicsAfterDelay());
    }

    private IEnumerator FlashMaterial()
    {
        while (true)
        {
            // ������ ����-���������
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                if (targetRenderers[i] != null)
                    targetRenderers[i].materials = flashMaterialsList[i];
            }
            yield return new WaitForSeconds(flashInterval);

            // ���������� ������������
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
        // ��� ����� ����������� ������
        yield return new WaitForSeconds(disableDelay);

        // 1) ������� Rigidbody
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
            Destroy(rb);

        // 2) ������� ��� ����������, ����� ���������
        foreach (var col in GetComponents<Collider>())
        {
            if (!col.isTrigger)
                Destroy(col);
        }
    }
}
