using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RGSK
{
    public class ButtonSFX : MonoBehaviour
    {
        private Button button;
        public AudioClip audioClip;

        void Start()
        {
            button = GetComponent<Button>();

            if(button != null && audioClip != null)
            {
                if (AudioManager.instance != null)
                    button.onClick.AddListener(delegate { AudioManager.instance.PlayClip(audioClip); });
            }
        }
    }
}
