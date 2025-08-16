using UnityEngine;
using UnityEngine.SceneManagement;

namespace RGSK
{
    public class PanelDeactivator : MonoBehaviour
    {
        public GameObject[] panelsToDeactivate;
        public GameObject firstSelectedPanel;

        void Start()
        {
            // First deactivate all the panels in the array
            for (int i = 0; i < panelsToDeactivate.Length; i++)
            {
                panelsToDeactivate[i].SetActive(false);
            }

            // Secondly, activate the first selected panel
            if (firstSelectedPanel != null)
            {
                firstSelectedPanel.SetActive(true);
            }
        }

        public void LoadTestScene()
        {
            SceneManager.LoadScene("Test");
        }
    }
}
