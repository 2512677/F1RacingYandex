using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RGSK
{
    public class RacerName : MonoBehaviour
    {
        [HideInInspector]
        public RacerStatistics racer;
        private Transform player;
        private Canvas canvas;

        public Vector3 positionOffset = new Vector3(0, 1.5f, 0);
        public float visibleDistance = 50;
        public bool onlyShowVehicleAhead = true;

        [Header("UI")]
        public Text positionText;
        public Text nameText;
        public Image nationality;

        void Start()
        {
            canvas = GetComponent<Canvas>();

            if(racer != null)
            {
                //Disable the gameObject if this is a player
                if(racer.isPlayer)
                {
                    gameObject.SetActive(false);
                }

                //Assign the name
                if (nameText != null)
                {
                    nameText.text = racer.racerInformation.racerName;
                }

                //Update the position
                if (positionText != null)
                {
                    positionText.text = racer.Position.ToString();
                }

                //Update the nationality image
                if (nationality != null)
                {
                    nationality.sprite = Helper.GetCountryFlag(racer.GetNationality());

                    if (nationality.sprite == null)
                        nationality.enabled = false;
                }
            }           
        }


        void LateUpdate()
        {
            UpdatePosition();
            UpdateVisibility();   
        }


        void UpdatePosition()
        {
            if (racer == null) return;

            //follow the target
            transform.position = racer.transform.position + positionOffset;
        }


        void UpdateVisibility()
        {
            if (racer == null)
                return;

            float distance = RaceManager.instance.GetDistanceBetween(racer);

            if (onlyShowVehicleAhead)
            {
                if (RaceManager.instance.playerStatistics != null && racer.Position == RaceManager.instance.playerStatistics.Position - 1)
                {
                    canvas.enabled = distance < visibleDistance;
                }
                else
                {
                    canvas.enabled = false;
                }
            }
            else
            {
                canvas.enabled = distance < visibleDistance;               
            }
        }


        void UpdatePositionText()
        {
            if (racer == null) return;

            if(positionText)
            {
                //Update the position text
                positionText.text = racer.Position.ToString();
            }
        }


        void DeActivate()
        {
            gameObject.SetActive(false);
        }


        void OnEnable()
        {
            RaceManager.OnRacePositionsChange += UpdatePositionText;
            RaceManager.OnPlayerFinish += DeActivate;
        }


        void OnDisable()
        {
            RaceManager.OnRacePositionsChange -= UpdatePositionText;
            RaceManager.OnPlayerFinish -= DeActivate;
        }
    }
}