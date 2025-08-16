using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RGSK
{
    public class StandardTouchInputManager : MonoBehaviour, ITouchInputManager
    {
        private int maxPlayers = 1;
        private Dictionary<int, MobileTouchButton>[] actionsDictionary;


        public void UpdateActionsDictionary()
        {
            MobileControlManager mobileControls = FindObjectOfType<MobileControlManager>();
            if(mobileControls != null)
            {
                MobileTouchButton[] buttons = mobileControls.GetComponentsInChildren<MobileTouchButton>();
                actionsDictionary = new Dictionary<int, MobileTouchButton>[maxPlayers];
                for (int i = 0; i < maxPlayers; i++)
                {
                    Dictionary<int, MobileTouchButton> playerActions = new Dictionary<int, MobileTouchButton>();
                    actionsDictionary[i] = playerActions;
                    for (int x = 0; x < buttons.Length; x++)
                    {
                        AddAction(buttons[x].inputAction, buttons[x], playerActions);
                    }
                }
            }
        }


        private static void AddAction(InputAction action, MobileTouchButton actionButton, Dictionary<int, MobileTouchButton> actions)
        {
            if (actions.ContainsKey((int)action))
                return;

            actions.Add((int)action, actionButton);
        }


        public virtual bool isEnabled
        {
            get
            {
                return isActiveAndEnabled;
            }

            set
            {
                this.enabled = value;
            }
        }


        public float GetAxis(int playerID, InputAction action)
        {
            if (actionsDictionary == null || !actionsDictionary[playerID].ContainsKey((int)action))
                return 0;

            float value = actionsDictionary[playerID][(int)action].InputValue;

            return value;
        }


        public bool GetButton(int playerID, InputAction action)
        {
            if (actionsDictionary == null || !actionsDictionary[playerID].ContainsKey((int)action))
                return false;

            return actionsDictionary[playerID][(int)action].held;
        }


        public bool GetButtonDown(int playerID, InputAction action)
        {
            if (actionsDictionary == null || !actionsDictionary[playerID].ContainsKey((int)action))
                return false;

            return actionsDictionary[playerID][(int)action].pressed;
        }


        public bool GetButtonUp(int playerID, InputAction action)
        {
            if (actionsDictionary == null || !actionsDictionary[playerID].ContainsKey((int)action))
                return false;

            return actionsDictionary[playerID][(int)action].released;
        }
    }
}