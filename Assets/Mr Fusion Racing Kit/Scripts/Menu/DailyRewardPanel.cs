using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardPanel : MonoBehaviour
{

    [Header("�����")]
    public Button backButton;               // ������ ��������
    public GameObject previousPanel;        // ���������� ������

    public void Back()
    {
        if (previousPanel != null)
        {
            gameObject.SetActive(false);
            previousPanel.SetActive(true);
        }
    }
}