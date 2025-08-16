//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2025 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The primary input management system for RCC, now using the legacy input system.
/// This class checks standard Unity axes/keys and translates them into RCC-friendly
/// variables and events for other systems to access (e.g., RCC_CarControllerV4).
/// </summary>
public class RCC_InputManager : RCC_Singleton<RCC_InputManager> {

    /// <summary>
    /// An instance of RCC_Inputs, which aggregates all relevant input values (throttle, brake, etc.).
    /// </summary>
    public RCC_Inputs inputs = new RCC_Inputs();

    /// <summary>
    /// Indicates whether gyroscopic (mobile) steering is in use.
    /// </summary>
    public bool gyroUsed = false;

    #region Events and Delegates

    public delegate void onStartStopEngine();
    public static event onStartStopEngine OnStartStopEngine;

    public delegate void onLowBeamHeadlights();
    public static event onLowBeamHeadlights OnLowBeamHeadlights;

    public delegate void onHighBeamHeadlights();
    public static event onHighBeamHeadlights OnHighBeamHeadlights;

    public delegate void onChangeCamera();
    public static event onChangeCamera OnChangeCamera;

    public delegate void onIndicatorLeft();
    public static event onIndicatorLeft OnIndicatorLeft;

    public delegate void onIndicatorRight();
    public static event onIndicatorRight OnIndicatorRight;

    public delegate void onIndicatorHazard();
    public static event onIndicatorHazard OnIndicatorHazard;

    public delegate void onGearShiftUp();
    public static event onGearShiftUp OnGearShiftUp;

    public delegate void onGearShiftDown();
    public static event onGearShiftDown OnGearShiftDown;

    public delegate void onNGear(bool state);
    public static event onNGear OnNGear;

    public delegate void onSlowMotion(bool state);
    public static event onSlowMotion OnSlowMotion;

    public delegate void onRecord();
    public static event onRecord OnRecord;

    public delegate void onReplay();
    public static event onReplay OnReplay;

    public delegate void onLookBack(bool state);
    public static event onLookBack OnLookBack;

    public delegate void onTrailerDetach();
    public static event onTrailerDetach OnTrailerDetach;

    #endregion

    private void Awake() {
        // Hide this GameObject from the scene hierarchy for cleanliness.
        gameObject.hideFlags = HideFlags.HideInHierarchy;

        // Instantiate the inputs container.
        inputs = new RCC_Inputs();
    }

    private void Update() {
        // Create or re-initialize the inputs structure if null.
        if (inputs == null)
            inputs = new RCC_Inputs();

        // Collect current frame inputs from the old Input Manager (or mobile, if enabled).
        GetInputs();
        CheckInputEvents();
    }

    /// <summary>
    /// Gathers axis data for throttle, brake, steering, etc., using old Input.GetAxis.
    /// If mobile controls are enabled, it reads from RCC_MobileButtons instead.
    /// </summary>
    public void GetInputs() {

        if (!Settings.mobileControllerEnabled) {
            // Example axis names: "Vertical" for throttle/brake, "Horizontal" for steering.
            // Feel free to rename these to match your project’s axes.
            inputs.throttleInput = Mathf.Clamp01(Input.GetAxis("Vertical"));     // Or use a custom axis name.
            inputs.brakeInput = Mathf.Clamp(-Input.GetAxis("Vertical"), 0f, 1f); // Example approach if you want negative vertical to be brake.
            inputs.steerInput = Input.GetAxis("Horizontal");
            inputs.handbrakeInput = Input.GetButton("Handbrake") ? 1f : 0f;           // Example: treat spacebar (“Jump”) as handbrake.
            inputs.boostInput = Input.GetKey(KeyCode.F) ? 1f : 0f;              // Example: left ctrl or mouse button.
            inputs.clutchInput = 0f; // If needed, use another axis or key, e.g., Input.GetAxis("Clutch")
            inputs.orbitX = Input.GetAxis("Mouse X");
            inputs.orbitY = Input.GetAxis("Mouse Y");

            // Camera orbit/zoom inputs (if you’re using these):
            inputs.orbitX = 0f;
            inputs.orbitY = 0f;
            inputs.scroll = Vector2.zero;
            // In the old input system, you’d typically read these as mouse movements or separate axes.
            // E.g.: orbitX = Input.GetAxis("Mouse X"), orbitY = Input.GetAxis("Mouse Y"), etc.
        } else {
            // If using mobile controls, read from RCC_MobileButtons.
            inputs.throttleInput = RCC_MobileButtons.mobileInputs.throttleInput;
            inputs.brakeInput = RCC_MobileButtons.mobileInputs.brakeInput;
            inputs.steerInput = RCC_MobileButtons.mobileInputs.steerInput;
            inputs.handbrakeInput = RCC_MobileButtons.mobileInputs.handbrakeInput;
            inputs.boostInput = RCC_MobileButtons.mobileInputs.boostInput;
        }
    }

    /// <summary>
    /// Checks for key/button presses for events such as engine start/stop, headlights, shifting, etc.
    /// </summary>
    private void CheckInputEvents() {

        // Sample key to start/stop engine (replace KeyCode.E if you prefer something else)
        if (Input.GetKeyDown(KeyCode.I)) {
            OnStartStopEngine?.Invoke();
        }

        // Sample key for trailer detach
        if (Input.GetKeyDown(KeyCode.T)) {
            OnTrailerDetach?.Invoke();
        }

        // Sample key for look-back: hold to look behind, release to revert
        if (Input.GetKeyDown(KeyCode.B)) {
            OnLookBack?.Invoke(true);
        }
        if (Input.GetKeyUp(KeyCode.B)) {
            OnLookBack?.Invoke(false);
        }

        // Sample keys for record & replay
        if (Input.GetKeyDown(KeyCode.R)) {
            OnRecord?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            OnReplay?.Invoke();
        }

        // Sample key for slow-motion (hold or toggle):
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            OnSlowMotion?.Invoke(true);
        }
        if (Input.GetKeyUp(KeyCode.Alpha1)) {
            OnSlowMotion?.Invoke(false);
        }

        // Sample key for N gear: hold or toggle
        if (Input.GetKeyDown(KeyCode.N)) {
            OnNGear?.Invoke(true);
        }
        if (Input.GetKeyUp(KeyCode.N)) {
            OnNGear?.Invoke(false);
        }

        // Sample keys for shifting up/down
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            OnGearShiftUp?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            OnGearShiftDown?.Invoke();
        }

        // Sample keys for indicators (left, right, hazard)
        if (Input.GetKeyDown(KeyCode.Q)) {
            OnIndicatorLeft?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            OnIndicatorRight?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Z)) {
            OnIndicatorHazard?.Invoke();
        }

        // Sample key for toggling camera
        if (Input.GetKeyDown(KeyCode.C)) {
            OnChangeCamera?.Invoke();
        }

        // Sample keys for headlights
        if (Input.GetKeyDown(KeyCode.L)) {
            OnLowBeamHeadlights?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            OnHighBeamHeadlights?.Invoke();
        }

    }
}
