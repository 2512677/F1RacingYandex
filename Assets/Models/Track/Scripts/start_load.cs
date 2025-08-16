using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class start_load : MonoBehaviour
{
    public Canvas loading_info;
    public Text tip_message;
    public Text loading_percentage;
    public Text versionText; // UI Text ��� ����������� ������ �����
    public string[] tips;

    private IEnumerator Start()
    {
        // ������������� ������� FPS
        Application.targetFrameRate = 60;

        // ������������� ����� � ������� �����, ���� ������ �� UI ������� ������
        if (versionText != null)
        {
            versionText.text = "������ �����: " + Application.version;
            // ���� ������ ������������ ������ �� RCC_Settings:
            // versionText.text = "������ �����: " + RCC_Settings.Instance.RCCVersion;
        }

        // ���������� ��������� ����� �� ������ tips
        if (tips.Length > 0)
        {
            tip_message.text = tips[Random.Range(0, tips.Length)];
        }

        // �������� �������� ����� ������� ��������
        yield return new WaitForSeconds(3f);

        // ���������� Canvas � ����������� � ��������
        loading_info.gameObject.SetActive(true);

        // �������� ����������� �������� �����
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Main Menu");
        asyncOperation.allowSceneActivation = false;

        // ��������� ����������� �������� ��������
        while (!asyncOperation.isDone)
        {
            // ��������� ������� �������� (����� ��������� 90%, ����� �� 100%)
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f) * 100;
            loading_percentage.text = $"{Mathf.RoundToInt(progress)}";

            // ����� �������� ��������� 90%, ��������� ��������
            if (asyncOperation.progress >= 0.9f)
            {
                loading_percentage.text = "100"; // ���������� 100%, ����� �������� ���������
                yield return new WaitForSeconds(1f); // �������� �������� ����� ���������� �����
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
