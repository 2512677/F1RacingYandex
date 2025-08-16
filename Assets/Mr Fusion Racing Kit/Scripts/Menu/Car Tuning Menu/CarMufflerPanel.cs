using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarMufflerPanel : MonoBehaviour
{

    [Header("Назад")]
    public Button backButton;               // Кнопка возврата
    public GameObject previousPanel;        // Предыдущая панель

    public void Back()
    {
        if (previousPanel != null)
        {
            gameObject.SetActive(false);
            previousPanel.SetActive(true);
        }
    }
}