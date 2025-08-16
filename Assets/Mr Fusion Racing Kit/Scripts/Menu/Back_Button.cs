using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Back_Button : MonoBehaviour
{

    [Header("Back")]
    public Button backButton; // Кнопка назад
    public GameObject previousPanel; // Предыдущая панель, к которой нужно вернуться
    // Start is called before the first frame update
    void Start()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(Back);
        }

    }

    // Update is called once per frame
    void Back()
    {
        if (previousPanel != null)
        {
            gameObject.SetActive(false);
            previousPanel.SetActive(true);
        }
    }
}