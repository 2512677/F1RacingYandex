using UnityEngine;
using System.Collections.Generic;

namespace RGSK
{
    public class PlayerInput : MonoBehaviour
    {
        private IInputManager inputManager;
        private RCC_CarControllerV3 vehicle;
        private Nitro nitro;
        private RaceState raceState = RaceState.Race;

        void Start()
        {
            inputManager = InputManager.instance;
            vehicle = GetComponent<RCC_CarControllerV3>();
            nitro = GetComponent<Nitro>();

            //Load Transmission
            vehicle.transmissionType = (TransmissionType)PlayerPrefs.GetInt("Transmission");
        }


        void Update()
        {
            //Allow vehicle input only in the race state
            if (raceState != RaceState.Race)
                return;

            if (inputManager == null || vehicle == null)
                return;

            //Vehicle input values
            float throttleInput = Mathf.Clamp01(inputManager.GetAxis(0, InputAction.Throttle));
            float brakeInput = Mathf.Clamp01(inputManager.GetAxis(0, InputAction.Brake));
            float steerInput = Mathf.Clamp(inputManager.GetAxis(0, InputAction.SteerRight) + (inputManager.GetAxis(0, InputAction.SteerLeft)), -1, 1);
            float handbrakeInput = 0;

            if (RaceManager.instance != null)
            {
                handbrakeInput = RaceManager.instance.raceStarted ? inputManager.GetAxis(0, InputAction.Handbrake) : 1;
            }
            else
            {
                handbrakeInput = inputManager.GetAxis(0, InputAction.Handbrake);
            }

            //Shift up
            if (inputManager.GetButtonDown(0, InputAction.ShiftUp))
            {
                if (vehicle != null)
                    vehicle.ShiftUp();
            }

            //Shift down
            if (inputManager.GetButtonDown(0, InputAction.ShiftDown))
            {
                if (vehicle != null)
                    vehicle.ShiftDown();
            }

            //Toggle headlights
            if (inputManager.GetButtonDown(0, InputAction.ToggleHeadlights))
            {
                if (vehicle != null)
                    vehicle.ToggleHeadlights();
            }

            //Nitro
            if (nitro != null)
            {
                nitro.throttle = throttleInput;
                nitro.nitroEngaged = inputManager.GetButton(0, InputAction.Nitro);
            }

            //Respawn vehicle
            if (inputManager.GetButtonDown(0, InputAction.Respawn))
            {
                if (RaceManager.instance != null)
                {
                    RaceManager.instance.RespawnVehicle(transform);
                }
            }

            //Send input values to vehicle 
            vehicle.GetInput(throttleInput, brakeInput, steerInput, handbrakeInput);
        }


        void UpdateRaceState(RaceState state)
        {
            raceState = state;
        }


        void ResetInputValues()
        {
            if (vehicle != null)
            {
                vehicle.GetInput(0, 0, 0, 0);
            }

            if (nitro != null)
            {
                nitro.nitroEngaged = false;
            }
        }


        void OnEnable()
        {
            RaceManager.OnRaceStateChange += UpdateRaceState;
        }


        void OnDisable()
        {
            RaceManager.OnRaceStateChange -= UpdateRaceState;
            ResetInputValues();
        }
    }
}
