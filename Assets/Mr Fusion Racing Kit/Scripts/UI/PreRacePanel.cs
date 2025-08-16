using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RGSK
{
    public class PreRacePanel : MonoBehaviour
    {

        [Header("Buttons")]
        public Button startRaceButton;
        public Button exitButton;

        void Start()
        {
            AddButtonListeners();
        }


        void AddButtonListeners()
        {
            if (startRaceButton != null)
            {
                if(RaceManager.instance != null)
                    startRaceButton.onClick.AddListener(delegate { RaceManager.instance.BeginCountdown(); });
            }

            if (exitButton != null)
            {
                if (SceneController.instance != null)
                    exitButton.onClick.AddListener(delegate { SceneController.instance.ExitToMenu(); });
            }
        }
    }
}
