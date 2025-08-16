using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    [System.Serializable]
    public class FillImage
    {
        public Image image;
        public float minFill = 0;
        public float maxFill = 1;
    }

    [System.Serializable]
    public class AnalogDial
    {
        public Transform needle;
        public float minNeedleAngle, maxNeedleAngle;
        public float maxReading;
        public float needleOffset = 1;
    }

    public class VehicleHUD : MonoBehaviour
    {
        public RCC_CarControllerV3 vehicleController;
        public global::RCC_CarControllerV4 rcc;
        public Nitro vehicleNitro;
        public bool autoFindPlayer = true;
        public SpeedUnit speedUnit;

        [Header("Analog Tachometer")]
        public AnalogDial analogTachometer;

        [Header("Analog Speedometer")]
        public AnalogDial analogSpeedometer;

        [Header("Digital Tachometer")]
        public FillImage digitalTachometer;

        [Header("Nitro Bar")]
        public FillImage nitroBar;

        [Header("Text")]
        public Text speedText;
        public Text gearText;
        public Text rpmText;
        public Text speedUnitText;
        public bool rpmGearTextColor;

        [Header("Assists")]
        public RectTransform tcs;
        public RectTransform abs;

        [Header("Input Bars")]
        public Image throttleBar;
        public Image brakeBar;

        private void Start()
        {
            // Set the speed unit to that of the RaceManager
            if (RaceManager.instance != null)
            {
                speedUnit = RaceManager.instance.speedUnit;
            }

            // Update the speed unit text
            UpdateSpeedUnitText();
        }

        private void Update()
        {
            UpdateRCC();

            // If no vehicle has been assigned, return
            if (vehicleController == null)
                return;

            // Analog Tacho
            if (analogTachometer.needle != null)
            {
                float ratio = Mathf.InverseLerp(0, analogTachometer.maxReading, vehicleController.engineRPM);
                float angle = Mathf.Lerp(analogTachometer.minNeedleAngle, analogTachometer.maxNeedleAngle, ratio) * analogTachometer.needleOffset;
                Vector3 rotation = analogTachometer.needle.localEulerAngles;
                rotation.z = -angle;
                analogTachometer.needle.localEulerAngles = rotation;
            }

            // Analog Speedo
            if (analogSpeedometer.needle != null)
            {
                float ratio = Mathf.InverseLerp(0, analogSpeedometer.maxReading, vehicleController.currentSpeedKPH);
                float angle = Mathf.Lerp(analogSpeedometer.minNeedleAngle, analogSpeedometer.maxNeedleAngle, ratio) * analogSpeedometer.needleOffset;
                Vector3 rotation = analogSpeedometer.needle.localEulerAngles;
                rotation.z = -angle;
                analogSpeedometer.needle.localEulerAngles = rotation;
            }

            // Digital Tacho
            if (digitalTachometer.image != null)
            {
                digitalTachometer.image.fillAmount = (vehicleController.engineRPM / vehicleController.maxEngineRPM) * (digitalTachometer.maxFill - digitalTachometer.minFill) + digitalTachometer.minFill;
            }

            // Speed Text
            if (speedText != null)
            {
                speedText.text = Mathf.RoundToInt(speedUnit == SpeedUnit.KPH ? vehicleController.currentSpeedKPH : vehicleController.currentSpeedKPH * 0.62f).ToString();
            }

            // Gear Text
            if (gearText != null)
            {
                gearText.text = vehicleController.currentGear == 0 ? "R" : (vehicleController.currentGear == 1) ? "N" : (vehicleController.currentGear - 1).ToString();

                if (rpmGearTextColor)
                {
                    gearText.color = vehicleController.engineRPM > vehicleController.shiftUpRPM ? Color.red : Color.white;
                }
            }

            // RPM Text
            if (rpmText != null)
            {
                rpmText.text = Mathf.RoundToInt(vehicleController.engineRPM).ToString();
            }

            // Nitro
            if (vehicleNitro != null && nitroBar.image != null)
            {
                nitroBar.image.fillAmount = (vehicleNitro.capacity) * (nitroBar.maxFill - nitroBar.minFill) + nitroBar.minFill;
            }

            // Assists
            if (tcs != null)
            {
                tcs.gameObject.SetActive(vehicleController.isTcsActive);
            }

            if (abs != null)
            {
                abs.gameObject.SetActive(vehicleController.isAbsActive);
            }

            // Input bars
            if (throttleBar != null)
            {
                throttleBar.fillAmount = vehicleController.throttleInput;
            }

            if (brakeBar != null)
            {
                brakeBar.fillAmount = vehicleController.brakeInput;
            }
        }

        private void UpdateRCC()
        {
            if (rcc == null)
                return;

            // Analog Tacho
            if (analogTachometer.needle != null)
            {
                float ratio = Mathf.InverseLerp(0, analogTachometer.maxReading, rcc.engineRPM);
                float angle = Mathf.Lerp(analogTachometer.minNeedleAngle, analogTachometer.maxNeedleAngle, ratio) * analogTachometer.needleOffset;
                Vector3 rotation = analogTachometer.needle.localEulerAngles;
                rotation.z = -angle;
                analogTachometer.needle.localEulerAngles = rotation;
            }

            // Analog Speedo
            if (analogSpeedometer.needle != null)
            {
                float ratio = Mathf.InverseLerp(0, analogSpeedometer.maxReading, rcc.speed);
                float angle = Mathf.Lerp(analogSpeedometer.minNeedleAngle, analogSpeedometer.maxNeedleAngle, ratio) * analogSpeedometer.needleOffset;
                Vector3 rotation = analogSpeedometer.needle.localEulerAngles;
                rotation.z = -angle;
                analogSpeedometer.needle.localEulerAngles = rotation;
            }

            // Digital Tacho
            if (digitalTachometer.image != null)
            {
                if (rcc != null)
                {
                    digitalTachometer.image.fillAmount = (rcc.engineRPM / rcc.maxEngineRPM) * (digitalTachometer.maxFill - digitalTachometer.minFill) + digitalTachometer.minFill;
                }
            }

            // Speed Text
            if (speedText != null)
            {
                if (rcc != null)
                {
                    speedText.text = Mathf.RoundToInt(speedUnit == SpeedUnit.KPH ? rcc.speed : rcc.speed * 0.62f).ToString();
                }
            }

            // Gear Text
            if (gearText != null)
            {
                if (rcc != null)
                {
                    if (!rcc.NGear && !rcc.changingGear)
                    {
                        gearText.text = rcc.direction == 1 ? (rcc.currentGear + 1).ToString() : "R";
                    }
                    else
                    {
                        gearText.text = "N";
                    }

                    if (rpmGearTextColor)
                    {
                        gearText.color = rcc.engineRPM > rcc.maxEngineRPM * 0.95f ? Color.red : Color.white;
                    }
                }
            }

            // RPM Text
            if (rpmText != null)
            {
                rpmText.text = Mathf.RoundToInt(rcc.engineRPM).ToString();
            }

            // Nitro
            if (vehicleNitro != null && nitroBar.image != null)
            {
                nitroBar.image.fillAmount = (vehicleNitro.capacity) * (nitroBar.maxFill - nitroBar.minFill) + nitroBar.minFill;
            }

            // Assists
            if (tcs != null)
            {
                tcs.gameObject.SetActive(rcc.TCSAct);
            }

            if (abs != null)
            {
                abs.gameObject.SetActive(rcc.ABSAct);
            }

            // Input bars
            if (throttleBar != null)
            {
                throttleBar.fillAmount = rcc.throttleInput;
            }

            if (brakeBar != null)
            {
                brakeBar.fillAmount = rcc.brakeInput;
            }
        }

        public void UpdateSpeedUnitText()
        {
            if (speedUnitText != null)
            {
                speedUnitText.text = speedUnit == SpeedUnit.KPH ? "KPH" : "MPH";
            }
        }

        public void FindPlayer()
        {
            if (!autoFindPlayer)
                return;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                vehicleController = player.GetComponent<RCC_CarControllerV3>();
                rcc = player.GetComponent<global::RCC_CarControllerV4>();
                vehicleNitro = player.GetComponent<Nitro>();
            }
        }

        private void OnEnable()
        {
            // Find the player vehicle if null
            if (vehicleController == null)
            {
                FindPlayer();
            }
        }
    }
}
