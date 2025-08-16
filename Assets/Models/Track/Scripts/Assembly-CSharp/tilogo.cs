using UnityEngine;
using UnityEngine.SceneManagement;

public class tilogo : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("tilogo Start() → загружаем startup_screen через 3 секунды");
        Invoke("LoadStartup", 7f);
    }

    private void LoadStartup()
    {
        Debug.Log("→ SceneManager.LoadScene(\"startup_screen\")");
        SceneManager.LoadScene("startup_screen");
    }
}
