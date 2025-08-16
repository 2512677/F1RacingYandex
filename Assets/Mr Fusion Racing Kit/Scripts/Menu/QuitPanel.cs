using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitPanel : MonoBehaviour
{
    public void QuitGame()
    {
        // Лог в консоль для проверки в редакторе
        Debug.Log("Игра закрыто.");

        // Закрыть приложение
        Application.Quit();

        // Для редактора Unity (только в режиме разработки)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
