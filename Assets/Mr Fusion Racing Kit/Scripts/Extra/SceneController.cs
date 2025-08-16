using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RGSK
{
    public class SceneController : MonoBehaviour
    {
        public static SceneController instance;
        public bool dontDestroyOnLoad = true;

        public string menuScene;

        [Header("Loading Screen")]
        public GameObject loadingPanel;
        public Text progressText;
        public Image progressBar;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            if (dontDestroyOnLoad)
            {
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }
        }


        public void LoadScene(string sceneName)
        {
            AudioListener.volume = 0;

            if (loadingPanel != null)
            {
                loadingPanel.SetActive(true);
            }

            StartCoroutine(LoadLevelAsync(sceneName));
        }


        public void ReloadScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);

            //Revert championship points
            if (ChampionshipManager.instance != null)
            {
                ChampionshipManager.instance.RevertChampionshipPoints();
            }
        }


        public void ExitToMenu()
        {
            LoadScene(menuScene);

            //Destroy any existing championships
            if (ChampionshipManager.instance != null)
            {
                Destroy(ChampionshipManager.instance.gameObject);
            }
        }


        IEnumerator LoadLevelAsync(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);

                if (progressBar != null)
                {
                    progressBar.fillAmount = progress;
                }

                if (progressText != null)
                {
                    progressText.text = (int)progress * 100 + "%";
                }

                yield return null;
            }
        }


        void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            //When a new scene is loaded, 

            //reset the timescale and audiolistener volume to 1
            Time.timeScale = 1;
            AudioListener.volume = 1;

            //Deactivate the loading panel
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(false);
            }
        }


        void OnEnable()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }


        void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }
    }
}
