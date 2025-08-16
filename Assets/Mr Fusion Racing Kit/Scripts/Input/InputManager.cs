using UnityEngine;
using System.Collections;
using System;

namespace RGSK
{
    public abstract class InputManager : MonoBehaviour, IInputManager
    {
        private static IInputManager _instance;
        public static IInputManager instance { get { return _instance; } }
        private ITouchInputManager _touchInputManager;
        public virtual ITouchInputManager touchInputManager
        {
            get
            {
                return _touchInputManager;
            }
            set
            {
                _touchInputManager = value;
            }
        }

        public bool dontDestroyOnLoad = true;


        public static void SetInstance(IInputManager instance)
        {
            _instance = instance;
        }


        protected virtual void Awake()
        {
            if (dontDestroyOnLoad)
            {
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }

            _touchInputManager = GetComponent<ITouchInputManager>();
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
        

        protected virtual bool useTouchInput
        {
            get
            {
                return _touchInputManager != null ? _touchInputManager.isEnabled : false;
            }
        }


        public abstract bool GetButton(int playerID, InputAction action);

        public abstract bool GetButtonDown(int playerID, InputAction action);

        public abstract bool GetButtonUp(int playerID, InputAction action);

        public abstract float GetAxis(int playerID, InputAction action);
    }
}