using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RGSK;  // для доступа к RaceManager и RaceType

namespace RGSK
{
    public class MusicPlayer : MonoBehaviour
    {
        private AudioSource musicAudioSource;

        [Header("Обычные треки")]
        public AudioClip[] musicTracks;

        [Header("Треки для режима Chase")]
        public AudioClip[] chaseTracks;

        [Header("Настройки")]
        public bool randomize;
        public bool autoStartMusic;

        private AudioClip[] activeTracks; // текущий набор треков (обычные или chase)
        private int index = 0;
        private int lastIndex;

        void Start()
        {
            musicAudioSource = Helper.CreateAudioSource(gameObject, null, "Music", 0, 1,
                                                         (musicTracks.Length == 1 && chaseTracks.Length == 0),
                                                         false);

            // Если треков меньше либо одного, отключаем рандомайз
            if (musicTracks.Length <= 1 && chaseTracks.Length == 0)
                randomize = false;

            if (autoStartMusic)
                StartMusic();
        }

        /// <summary>
        /// Начать воспроизведение музыки. Если текущий режим гонки — Chase и заданы chaseTracks, 
        /// будет использоваться набор chaseTracks, иначе обычный musicTracks.
        /// </summary>
        public void StartMusic()
        {
            if (musicAudioSource == null || musicAudioSource.isPlaying)
                return;

            // Определяем, какие треки использовать
            if (RaceManager.instance != null &&
                RaceManager.instance.raceType == RaceType.Chase &&
                chaseTracks != null && chaseTracks.Length > 0)
            {
                activeTracks = chaseTracks;
            }
            else
            {
                activeTracks = musicTracks;
            }

            if (activeTracks == null || activeTracks.Length == 0)
                return;

            // Если у текущего набора только один трек, отключаем рандомайз
            if (activeTracks.Length <= 1)
                randomize = false;

            // Выбираем первый индекс
            index = !randomize ? 0 : Random.Range(0, activeTracks.Length);
            PlayTrack(index);
        }

        /// <summary>
        /// Воспроизвести следующий трек из текущего набора activeTracks.
        /// </summary>
        public void PlayNext()
        {
            if (activeTracks == null || activeTracks.Length == 0)
                return;

            index = (index + 1) % activeTracks.Length;
            PlayTrack(index);
        }

        /// <summary>
        /// Воспроизвести случайный трек (не совпадающий с последним) из activeTracks.
        /// </summary>
        public void PlayRandom()
        {
            if (activeTracks == null || activeTracks.Length == 0)
                return;

            int temp = lastIndex;
            if (activeTracks.Length == 1)
            {
                temp = 0;
            }
            else
            {
                while (temp == lastIndex)
                {
                    temp = Random.Range(0, activeTracks.Length);
                }
            }

            PlayTrack(temp);
        }

        /// <summary>
        /// Переключить текущий трек на заданный clip и начать его воспроизведение.
        /// </summary>
        public void OverrideMusicClip(AudioClip clip, bool loop)
        {
            if (musicAudioSource == null)
                return;

            musicAudioSource.clip = clip;
            musicAudioSource.loop = loop;
            musicAudioSource.Play();
        }

        /// <summary>
        /// Установить и запустить активный трек с индексом i из activeTracks.
        /// </summary>
        private void PlayTrack(int i)
        {
            if (activeTracks == null || activeTracks.Length == 0 || i < 0 || i >= activeTracks.Length)
                return;

            musicAudioSource.clip = activeTracks[i];
            musicAudioSource.Play();
            lastIndex = i;

            StartCoroutine(WaitForMusic(activeTracks[i].length));
        }

        IEnumerator WaitForMusic(float duration)
        {
            float time = 0;

            while (time < duration + 0.5f)
            {
                time += Time.deltaTime;
                yield return null;
            }

            TrackFinished();
        }

        private void TrackFinished()
        {
            if (musicAudioSource.isPlaying)
                return;

            if (!randomize)
            {
                PlayNext();
            }
            else
            {
                PlayRandom();
            }
        }

        public void Pause()
        {
            if (musicAudioSource != null)
            {
                musicAudioSource.Pause();
            }
        }

        public void UnPause()
        {
            if (musicAudioSource != null)
            {
                musicAudioSource.UnPause();
            }
        }
    }
}
