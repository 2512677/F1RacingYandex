using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PRIVACY : MonoBehaviour
{
    // Start is called before the first frame update
   public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://sevenwolf.uz/Privacy.html");
    }

    public void Level()
    {
        SceneManager.LoadScene("startup_screen");
    }
}
