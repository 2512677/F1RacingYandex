using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RGSK
{
    [RequireComponent(typeof(RectTransform))]
    public class RaceProgressBarUI : MonoBehaviour
    {      
        public UIProgressBarDirection progressDirection;

        private List<ProgressIcon> racers = new List<ProgressIcon>();

        private RectTransform rectTransform;
        private float height;
        private float width;

        public RectTransform playerIcon;
        public RectTransform opponentIcon;
        public float offset;
        public bool autoRotateToMatchDirection = true;
        private List<int> ids = new List<int>();

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            height = rectTransform.rect.height;
            width = rectTransform.rect.width;
        }


        void Update()
        {
            UpdateProgress();
        }


        void GetRacerList()
        {
            for(int i = 0; i < RaceManager.instance.racerList.Count; i++)
            {
                if (ids.Contains(RaceManager.instance.racerList[i].GetInstanceID()))
                    continue;

                if (RaceManager.instance.racerList[i].IsPlayer())
                {
                    RectTransform icon = Instantiate(playerIcon, Vector3.zero, Quaternion.identity, 
                        rectTransform.transform) as RectTransform;

                    if (autoRotateToMatchDirection && progressDirection == UIProgressBarDirection.Horizontal)
                        icon.eulerAngles = new Vector3(0, 0, 90);

                    racers.Add(new ProgressIcon(RaceManager.instance.racerList[i], icon));
                }
                else
                {
                    RectTransform icon = Instantiate(opponentIcon, Vector3.zero, Quaternion.identity, 
                        rectTransform.transform) as RectTransform;

                    if (autoRotateToMatchDirection && progressDirection == UIProgressBarDirection.Horizontal)
                        icon.eulerAngles = new Vector3(0, 0, 90);

                    racers.Add(new ProgressIcon(RaceManager.instance.racerList[i], icon));
                }

                ids.Add(RaceManager.instance.racerList[i].GetInstanceID());
            }
        }


        void UpdateProgress()
        {
            if (racers.Count == 0)
                return;

            for (int i = 0; i < racers.Count; i++)
            {
                switch (progressDirection)
                {
                    case UIProgressBarDirection.Vertical:
                        float Vprogress = Mathf.Abs(racers[i].stats.raceCompletionPercentage / 100) * height;
                        racers[i].icon.localPosition = new Vector2(offset, Vprogress - (height / 2));
                        break;

                    case UIProgressBarDirection.Horizontal:
                        float Hprogress = Mathf.Abs(racers[i].stats.raceCompletionPercentage / 100) * width;
                        racers[i].icon.localPosition = new Vector2(Hprogress - (width / 2), offset);
                        break;
                }
            }
        }


        void OnEnable()
        {
            RaceManager.OnVehicleSpawn += GetRacerList;
        }


        void OnDisable()
        {
            RaceManager.OnVehicleSpawn -= GetRacerList;
        }


        [System.Serializable]
        public class ProgressIcon
        {
            public RacerStatistics stats;
            public RectTransform icon;

            public ProgressIcon(RacerStatistics rs, RectTransform rt)
            {
                stats = rs;
                icon = rt;
            }
        }
    }
}