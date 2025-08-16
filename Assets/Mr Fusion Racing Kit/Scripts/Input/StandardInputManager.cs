using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RGSK
{
    public class StandardInputManager : InputManager
    {
        private string playerAxisPrefix = "";
        private int maxPlayers = 1;
        private Dictionary<int, string>[] actionsDictionary;

        public string throttleAxis = "Throttle";
        public string brakeAxis = "Brake";
        public string steerAxis = "Steer";
        public string handbrakeAxis = "Handbrake";
        public string shiftUp = "ShiftUp";
        public string shiftDown = "ShiftDown";
        public string nitro = "Nitro";
        public string changeView = "ChangeView";
        public string lookLeft = "LookLeft";
        public string lookRight = "LookRight";
        public string lookBack = "LookBack";
        public string toggleHeadlights = "Headlights";
        public string respawn = "Respawn";
        public string pause = "Pause";
        public string focusNextVehicle = "NextVehicle";
        public string focusPreviousVehicle = "PreviousVehicle";

        protected override void Awake()
        {
            base.Awake();

            if (instance == null)
            {
                SetInstance(this);
            }
            else
            {
                Destroy(gameObject);
            }

            actionsDictionary = new Dictionary<int, string>[maxPlayers];
            for (int i = 0; i < maxPlayers; i++)
            {
                Dictionary<int, string> playerActions = new Dictionary<int, string>();
                actionsDictionary[i] = playerActions;
                string prefix = !string.IsNullOrEmpty(playerAxisPrefix) ? playerAxisPrefix + i : string.Empty;

                AddAction(InputAction.Throttle, prefix + throttleAxis, playerActions);
                AddAction(InputAction.Brake, prefix + brakeAxis, playerActions);
                AddAction(InputAction.SteerLeft, prefix + steerAxis, playerActions);
                AddAction(InputAction.SteerRight, prefix + steerAxis, playerActions);
                AddAction(InputAction.Handbrake, prefix + handbrakeAxis, playerActions);
                AddAction(InputAction.ShiftUp, prefix + shiftUp, playerActions);
                AddAction(InputAction.ShiftDown, prefix + shiftDown, playerActions);
                AddAction(InputAction.Nitro, prefix + nitro, playerActions);
                AddAction(InputAction.LookLeft, prefix + lookLeft, playerActions);
                AddAction(InputAction.LookRight, prefix + lookRight, playerActions);
                AddAction(InputAction.LookBack, prefix + lookBack, playerActions);
                AddAction(InputAction.ToggleHeadlights, prefix + toggleHeadlights, playerActions);
                AddAction(InputAction.ChangeView, prefix + changeView, playerActions);
                AddAction(InputAction.Respawn, prefix + respawn, playerActions);
                AddAction(InputAction.Pause, prefix + pause, playerActions);
                AddAction(InputAction.FocusNextVehicle, prefix + focusNextVehicle, playerActions);
                AddAction(InputAction.FocusPreviousVehicle, prefix + focusPreviousVehicle, playerActions);
            }
        }


        private static void AddAction(InputAction action, string actionName, Dictionary<int, string> actions)
        {
            if (string.IsNullOrEmpty(actionName))
                return;

            actions.Add((int)action, actionName);
        }


        public override bool GetButton(int playerID, InputAction action)
        {
            if (!actionsDictionary[playerID].ContainsKey((int)action))
                return false;

            bool value = Input.GetButton(actionsDictionary[playerID][(int)action]);
            if (useTouchInput)
            {
                value |= touchInputManager.GetButton(playerID, action);
            }

            return value;
        }


        public override bool GetButtonDown(int playerID, InputAction action)
        {
            if (!actionsDictionary[playerID].ContainsKey((int)action))
                return false;

            bool value = Input.GetButtonDown(actionsDictionary[playerID][(int)action]);
            if (useTouchInput)
            {
                value |= touchInputManager.GetButtonDown(playerID, action);
            }

            return value;
        }


        public override bool GetButtonUp(int playerID, InputAction action)
        {
            if (!actionsDictionary[playerID].ContainsKey((int)action))
                return false;

            bool value = Input.GetButtonUp(actionsDictionary[playerID][(int)action]);
            if (useTouchInput)
            {
                value |= touchInputManager.GetButtonUp(playerID, action);
            }

            return value;
        }


        public override float GetAxis(int playerID, InputAction action)
        {
            if (!actionsDictionary[playerID].ContainsKey((int)action))
                return 0;

            float value = Input.GetAxis(actionsDictionary[playerID][(int)action]);

            if (useTouchInput)
            {
                float touchValue = touchInputManager.GetAxis(playerID, action);
                if (Mathf.Abs(touchValue) > Mathf.Abs(value)) value = touchValue;
            }

            return value;
        }
    }
}
