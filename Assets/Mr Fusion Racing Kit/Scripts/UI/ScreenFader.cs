using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace RGSK
{
    [RequireComponent(typeof(Image))]
    public class ScreenFader : MonoBehaviour
    {
        public Image fadeImage;
        private bool isFading;
        public bool fadeOnStart = true;

        void Start()
        {
            if(fadeOnStart)
                DoFadeOut(2);
        }


        public void DoFadeOut(float duration)
        {
            if (fadeImage == null || isFading) return;

            //Focus on the gameObject
            transform.SetAsLastSibling();

            //Start the FadeOut coroutine
            StartCoroutine(FadeOut(duration));
        }


        public void DoFadeIn(float duration)
        {
            if (fadeImage == null || isFading) return;

            //Focus on the gameObject
            transform.SetAsLastSibling();

            //Start the FadeIn coroutine
            StartCoroutine(FadeIn(duration));
        }


        IEnumerator FadeOut(float duration)
        {
            float fadeTimer = 0;
            Color col = fadeImage.color;

            isFading = true;
            while (fadeTimer < duration)
            {
                fadeTimer += Time.time > 0 ? Time.unscaledDeltaTime : Time.deltaTime;
                col.a = Mathf.Lerp(1, 0, fadeTimer / duration);
                fadeImage.color = col;
                yield return null;
            }

            isFading = false;
        }


        IEnumerator FadeOut(float duration, Action callback)
        {
            float fadeTimer = 0;
            Color col = fadeImage.color;

            isFading = true;
            while (fadeTimer < duration)
            {
                fadeTimer += Time.time > 0 ? Time.unscaledDeltaTime : Time.deltaTime;
                col.a = Mathf.Lerp(1, 0, fadeTimer / duration);
                fadeImage.color = col;
                yield return null;
            }

            isFading = false;
            callback();
        }


        IEnumerator FadeIn(float duration)
        {
            float fadeTimer = 0;
            Color col = fadeImage.color;

            isFading = true;
            while (fadeTimer < duration)
            {
                fadeTimer += Time.time > 0 ? Time.unscaledDeltaTime : Time.deltaTime;
                col.a = Mathf.Lerp(0, 1, fadeTimer / duration);
                fadeImage.color = col;
                yield return null;
            }

            isFading = false;
        }
    }
}