using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using System.Globalization;
using I2.Loc;

namespace RGSK
{
    public class Helper : MonoBehaviour
    {

        public static AudioSource CreateAudioSource(GameObject go, AudioClip clip, string audioMixerGroup, float spatialBlend, float volume, bool loop, bool playOnStartup)
        {
            AudioSource audioSource = new GameObject("audiosource " + (clip != null ? clip.name.ToString() : "")).AddComponent<AudioSource>();

            audioSource.transform.parent = go.transform;
            audioSource.transform.localPosition = Vector3.zero;
            audioSource.transform.localRotation = Quaternion.identity;

            audioSource.clip = clip != null ? clip : null;
            audioSource.volume = volume;
            audioSource.loop = loop;
            audioSource.spatialBlend = spatialBlend;
            audioSource.dopplerLevel = 0;

            if (GlobalSettings.Instance != null)
            {
                AudioMixer mixer = GlobalSettings.Instance.gameAudioMixer;
                if (mixer != null && audioMixerGroup != "")
                {
                    try
                    {
                        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups(audioMixerGroup)[0];
                    }
                    catch
                    {
                        Debug.LogWarning("The Audio Mixer Group '" + audioMixerGroup + "' was not found in the Audio Mixer");
                    }
                }
            }
            else
            {
                Debug.LogWarning("The AudioSource '" + audioSource.name + "' was not assigned an output mixer group because a Game Audio Mixer was not found.");
            }

            if (playOnStartup)
            {
                audioSource.Play();
            }

            return audioSource;
        }


        public static string FormatTime(float time)
        {
            var timeSpan = TimeSpan.FromSeconds(time);
            return string.Format("{0:00}:{1:00}.{2:000}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        }


        public static string FormatTime(float time, TimeFormat format)
        {
            var timeSpan = TimeSpan.FromSeconds(time);

            switch (format)
            {
                case TimeFormat.HrMinSec:
                    return string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

                case TimeFormat.MinSecMs:
                    return string.Format("{0:00}:{1:00}:{2:000}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);

                case TimeFormat.MinSec:
                    return string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);

                case TimeFormat.SecMs:
                    return string.Format("{0:0}.{1:000}", timeSpan.Seconds, timeSpan.Milliseconds);

                default:
                    return string.Format("{0:00}:{1:00}:{2:000}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
            }
        }


        public static void SetCurveLinear(AnimationCurve curve)
        {
            for (int i = 0; i < curve.keys.Length; ++i)
            {
                float intangent = 0;
                float outtangent = 0;
                bool intangent_set = false;
                bool outtangent_set = false;
                Vector2 point1;
                Vector2 point2;
                Vector2 deltapoint;
                Keyframe key = curve[i];

                if (i == 0)
                {
                    intangent = 0; intangent_set = true;
                }

                if (i == curve.keys.Length - 1)
                {
                    outtangent = 0; outtangent_set = true;
                }

                if (!intangent_set)
                {
                    point1.x = curve.keys[i - 1].time;
                    point1.y = curve.keys[i - 1].value;
                    point2.x = curve.keys[i].time;
                    point2.y = curve.keys[i].value;

                    deltapoint = point2 - point1;

                    intangent = deltapoint.y / deltapoint.x;
                }
                if (!outtangent_set)
                {
                    point1.x = curve.keys[i].time;
                    point1.y = curve.keys[i].value;
                    point2.x = curve.keys[i + 1].time;
                    point2.y = curve.keys[i + 1].value;

                    deltapoint = point2 - point1;

                    outtangent = deltapoint.y / deltapoint.x;
                }

                key.inTangent = intangent;
                key.outTangent = outtangent;
                curve.MoveKey(i, key);
            }
        }


        public static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            string suffix = "th"; // Суффикс по умолчанию
            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    suffix = LocalizationManager.GetTranslation("Ordinal/th");
                    return num + suffix;
            }

            switch (num % 10)
            {
                case 1:
                    suffix = LocalizationManager.GetTranslation("Ordinal/st");
                    break;
                case 2:
                    suffix = LocalizationManager.GetTranslation("Ordinal/nd");
                    break;
                case 3:
                    suffix = LocalizationManager.GetTranslation("Ordinal/rd");
                    break;
                default:
                    suffix = LocalizationManager.GetTranslation("Ordinal/th");
                    break;
            }

            return num + suffix;
        }



        public static Keyframe GetPeakPointInCurve(Keyframe[] keyframeArray)
        {
            Keyframe key = new Keyframe();
            float maxValue = keyframeArray[0].value;

            for (int i = 0; i < keyframeArray.Length; i++)
            {
                if (keyframeArray[i].value > maxValue)
                {
                    maxValue = keyframeArray[i].value;
                    key = keyframeArray[i];
                }
            }

            return key;
        }


        public static Sprite GetCountryFlag(Nationality nationality)
        {
            CountryFlagData flagData = CountryFlagData.Instance;

            if (flagData == null)
                return null;

            return flagData.GetFlag(nationality);
        }


        public static float RoundToNearest1000(float num)
        {
            return num % 1000 >= 500 ? num + 1000 - num % 1000 : num - num % 1000;
        }


        public static Bounds GetTotalMeshFilterBounds(Transform objectTransform)
        {
            var meshFilter = objectTransform.GetComponent<MeshFilter>();
            var result = meshFilter != null ? meshFilter.mesh.bounds : new Bounds();

            foreach (Transform transform in objectTransform)
            {
                var bounds = GetTotalMeshFilterBounds(transform);
                result.Encapsulate(bounds.min);
                result.Encapsulate(bounds.max);
            }

            var scaledMin = result.min;
            scaledMin.Scale(objectTransform.localScale);
            result.min = scaledMin;
            var scaledMax = result.max;
            scaledMax.Scale(objectTransform.localScale);
            result.max = scaledMax;
            return result;
        }


        public static float LoadBestLapTime(string key, string sceneName)
        {
            return PlayerPrefs.GetFloat(key + "@" + sceneName);
        }


        #region CONVERSIONS
        public static float KmToMiles(float value)
        {
            return value * 0.621371f;
        }


        public static float MilesToKm(float value)
        {
            return value * 1.61f;
        }


        public static float MeterToKm(float value)
        {
            return value * 0.001f;
        }


        public static float MeterToMiles(float value)
        {
            return value * 0.000621371f;
        }


        public static float MetersToFeet(float value)
        {
            return value * 3.281f;
        }


        public static float MetersToYards(float value)
        {
            return value * 1.1f;
        }


        public static float MpsToKph(float value)
        {
            return value * 3.6f;
        }


        public static float MpsToMph(float value)
        {
            return value * 2.237f;
        }


        public static float LinearToDecibel(float linear)
        {
            float dB;

            if (linear != 0)
                dB = 20.0f * Mathf.Log10(linear);
            else
                dB = -144.0f;

            return dB;
        }


        public static float DecibelToLinear(float dB)
        {
            float linear = Mathf.Pow(10.0f, dB / 20.0f);

            return linear;
        }


        public static float MinToSec(float value)
        {
            return value * 60;
        }


        public static float SecToMin(float value)
        {
            return value / 60;
        }
        #endregion
    }
}