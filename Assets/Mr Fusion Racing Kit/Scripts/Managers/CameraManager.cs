using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RGSK
{
    public class CameraManager : MonoBehaviour
    {
        public RaceCamera raceCamera;
        public CinematicCamera cinematicCamera;
        public MinimapCamera miniMapCamera;
        public Camera introCamera;
        private Camera activeCamera;

        private List<VehicleCameraInfo> vehicleList = new List<VehicleCameraInfo>();
        private int vehicleIndex;
        private int vehicleCameraIndex;
        private int previousVehicleIndex;
        private int previousVehicleCameraIndex;
        private bool replayMode;
        public bool saveCameraIndex;

        private IInputManager inputManager;
        private RaceState raceState = RaceState.Race;

        void Start()
        {
            //Enable the race camera
            if(raceCamera != null)
            {
                EnableCamera(raceCamera.GetComponent<Camera>());
            }
          
            //Disable the cinematic camera 
            if (cinematicCamera != null)
            {
                DisableCamera(cinematicCamera.GetComponent<Camera>());
            }

            //Enable the intro camera on pre race
            if (introCamera != null && !RaceManager.instance.autoStartRace)
            {
                DisableCamera(raceCamera.GetComponent<Camera>());
                EnableCamera(introCamera);
            }

            ResetRaceCamera();

            //Find input manager instance
            inputManager = InputManager.instance;
        }


        void Update()
        {
            if (inputManager != null)
            {
                //Handle camera input
                if (inputManager.GetButtonDown(0, InputAction.ChangeView))
                {
                    //Allow camera switching only in race state
                    if (raceState == RaceState.Race)
                    {
                        SwitchVehicleCamera();
                    }
                }

                if (raceState == RaceState.Race || raceState == RaceState.Replay)
                {
                    //Alow for vehicle retargetting only in race & replay states
                    if (inputManager.GetButtonDown(0, InputAction.FocusNextVehicle))
                    {
                        SwitchTargetVehicle(1);
                    }


                    if (inputManager.GetButtonDown(0, InputAction.FocusPreviousVehicle))
                    {
                        SwitchTargetVehicle(-1);
                    }
                }
            }
        }


        public void SetTarget(Transform target)
        {
            if (raceCamera != null)
            {
                raceCamera.SetTarget(target);
            }

            if (cinematicCamera != null)
            {
                cinematicCamera.SetTarget(target);
            }

            if (miniMapCamera != null)
            {
                miniMapCamera.SetTarget(target);
            }
        }


        public void SwitchTargetVehicle(int next)
        {
            if (vehicleList.Count <= 1)
                return;

            //Switch target index
            vehicleIndex += next;

            //Keep the vehicle index within the range of available targets
            if (vehicleIndex > vehicleList.Count - 1)
                vehicleIndex = 0;

            if (vehicleIndex < 0)
                vehicleIndex = vehicleList.Count - 1;

            //Set the new target to focus on
			SetTarget(vehicleList[vehicleIndex].vehicle);

            //Set the camera index appropriatley incase the new vehice doesn't have the same number of camera mounts
            if (vehicleCameraIndex != -1)
            {
                if (vehicleList[vehicleIndex].attachedCameras.Length > vehicleCameraIndex)
                {
                    SetCameraIndex();
                }
                else
                {
                    ResetRaceCamera();
                }
            }
        }


        public void SwitchVehicleCamera()
        {
            if (vehicleList.Count == 0)
                return;

            //Move to the next camera mount
            vehicleCameraIndex++;

            //If we have cycled through all the camera mounts of this vehicle, reset the race camera
            if (vehicleCameraIndex > vehicleList[vehicleIndex].attachedCameras.Length - 1)
            {
                //No need to reset the race camera if the vehicle has no camera mounts
                if(vehicleList[vehicleIndex].attachedCameras.Length > 0)
                    ResetRaceCamera();

                //In replay mode, enable the cinematic camera
                if (replayMode)
                {
                    EnableCinematicCamera();
                }
            }

            SetCameraIndex();
        }


        public void SwitchReplayCameras()
        {
            //In replay mode we use this function to switch cameras
            if (cinematicCamera.GetComponent<Camera>().enabled)
            {
                DisableCamera(cinematicCamera.GetComponent<Camera>());
                EnableCamera(raceCamera.GetComponent<Camera>());
                SetCameraIndex();
            }
            else
            {
                SwitchVehicleCamera();
            }
        }


        void SetCameraIndex()
        {
            for (int i = 0; i < vehicleList[vehicleIndex].attachedCameras.Length; i++)
            {
                if (i == vehicleCameraIndex)
                {
                    //Set the camera mode to match the current camera mount position
                    raceCamera.SwitchCameraMode(vehicleList[vehicleIndex].attachedCameras[i].mode);

                    //Set the mount target to the current camera mount transform
                    raceCamera.SetMountTarget(vehicleList[vehicleIndex].attachedCameras[i].transform);
					
                    //Toggle UI mirror visiblity
                    if (RacePanel.instance != null)
                    {
                        RacePanel.instance.ShowRearViewMirror(vehicleList[vehicleIndex].attachedCameras[i].showRearViewMirror);
                    }
                }
            }
           
            //Save this index
            if (saveCameraIndex && !replayMode)
            {
                PlayerPrefs.SetInt("LastCameraIndex", vehicleCameraIndex);
            }
        }


        public void EnableCinematicCamera()
        {
            //If the cinematic camera has not been assigned, return
            if (cinematicCamera == null)
                return;

            //Disable the race camera
            DisableCamera(raceCamera.GetComponent<Camera>());

            //Enable the cinematic camera
            EnableCamera(cinematicCamera.GetComponent<Camera>());
        }


        public void EnableCamera(Camera cam)
        {
            //Enable the camera component
            cam.enabled = true;

            //Enable the AudioListener component
            if (cam.GetComponent<AudioListener>())
                cam.GetComponent<AudioListener>().enabled = true;

            //Set this camera as the active camera
            activeCamera = cam;
        }


        public void DisableCamera(Camera cam)
        {
            //Disable the camera component
            cam.enabled = false;

            //Disable the AudioListener component
            if (cam.GetComponent<AudioListener>())
                cam.GetComponent<AudioListener>().enabled = false;
        }


        public void AssignVehicles(RacerStatistics[] vehicles)
        {
            //Add the vehicles and their camera info to the list
            foreach(RacerStatistics vehicle in vehicles)
            {
                VehicleCameraInfo info = new VehicleCameraInfo(vehicle.transform, GetAttachedCameras(vehicle.transform));
                vehicleList.Add(info);
            }

            //Loop through the vehicle list and find the player
            foreach (VehicleCameraInfo _target in vehicleList)
            {
                if (_target.vehicle.CompareTag("Player"))
                    vehicleIndex = vehicleList.IndexOf(_target);
            }

            //Set the target vehicle
			SetTarget(vehicleList[vehicleIndex].vehicle);

            //Load the last camera index that the player used
            if (saveCameraIndex)
            {
                if (PlayerPrefs.HasKey("LastCameraIndex"))
                {
                    vehicleCameraIndex = PlayerPrefs.GetInt("LastCameraIndex");
                    SetCameraIndex();
                }
            }
        }


		public void AddVehicle(Transform vehicle)
		{
			//Add the vehicles and their camera info to the list
			VehicleCameraInfo info = new VehicleCameraInfo(vehicle, GetAttachedCameras(vehicle));
			vehicleList.Add(info);

			//Loop through the vehicle list and find the player
			foreach (VehicleCameraInfo _target in vehicleList)
			{
				if (_target.vehicle.CompareTag("Player"))
					vehicleIndex = vehicleList.IndexOf(_target);
			}

			//Set the target vehicle
			SetTarget(vehicleList[vehicleIndex].vehicle);

			//Load the last camera index that the player used
			if (saveCameraIndex)
			{
				if (PlayerPrefs.HasKey("LastCameraIndex"))
				{
					vehicleCameraIndex = PlayerPrefs.GetInt("LastCameraIndex");
					SetCameraIndex();
				}
			}
		}


        void ResetRaceCamera()
        {
            vehicleCameraIndex = -1;

            if(raceCamera != null)
            {
                //Reset the camera index
                vehicleCameraIndex = -1;

                //Set the camera mode back to chase
                raceCamera.GetComponent<RaceCamera>().SwitchCameraMode(CameraMode.Chase);

                //Check whether the rear view mirror should be visible
                if (RacePanel.instance != null)
                {
                    RacePanel.instance.ShowRearViewMirror(raceCamera.chaseCameraSettings.showRearMirrorUI);
                }
            }
        }


        void UpdateCameraBasedOnRaceState(RaceState state)
        {
            raceState = state;

            switch (state)
            {
                case RaceState.PreRace:
                    if(introCamera != null)
                    {
                        EnableCamera(introCamera);
                    }

                    if (raceCamera != null && introCamera != null)
                    {
                        DisableCamera(raceCamera.GetComponent<Camera>());
                    }
                    break;

                case RaceState.Race:
                    if (introCamera != null)
                    {
                        DisableCamera(introCamera);
                        //The race intro camera is only needed for pre race state so 
                        //also de-activate the object.
                        introCamera.gameObject.SetActive(false);
                    }

                    if (raceCamera != null)
                    {
                        EnableCamera(raceCamera.GetComponent<Camera>());
                    }

                    if(replayMode)
                    {
                        ExitReplaMode();
                    }
                    break;

                case RaceState.Pause:
                    if (replayMode)
                    {
                        ExitReplaMode();
                    }
                    break;

                case RaceState.Replay:
                    EnterReplayMode();
                    break;
            }
        }


        public void EnterReplayMode()
        {
            //Store the current camera indexes
            previousVehicleCameraIndex = vehicleCameraIndex;
            previousVehicleIndex = vehicleIndex;

            //Reset the race camera
            ResetRaceCamera();

            //Enable the cinematic camera
            EnableCinematicCamera();

            //Enter replay mode
            replayMode = true;
        }


        public void ExitReplaMode()
        {
            //Revert camera index values
            vehicleCameraIndex = previousVehicleCameraIndex;
            vehicleIndex = previousVehicleIndex;

            //Retarget the previously selected vehicle
			SetTarget(vehicleList[vehicleIndex].vehicle);

            //Revert camera position
            if (vehicleCameraIndex == -1)
            {
                ResetRaceCamera();
            }
            else
            {
                SetCameraIndex();
            }

            //Disable the cinematic camera and enable the race camera
            DisableCamera(cinematicCamera.GetComponent<Camera>());
            EnableCamera(raceCamera.GetComponent<Camera>());

            //Exit replay mode
            replayMode = false;
        }
			

        public Camera GetActiveCamera()
        {
            return activeCamera;
        }


        public CameraMount[] GetAttachedCameras(Transform target)
        {
            return target.GetComponentsInChildren<CameraMount>();
        }


        void OnEnable()
        {
            RaceManager.OnRaceStateChange += UpdateCameraBasedOnRaceState;
        }


        void OnDisable()
        {
            RaceManager.OnRaceStateChange -= UpdateCameraBasedOnRaceState;
        }


        [System.Serializable]
        public class VehicleCameraInfo
        {
            public Transform vehicle;
            public CameraMount[] attachedCameras;

            public VehicleCameraInfo(Transform _vehicle, CameraMount[] _cameraMount)
            {
                vehicle = _vehicle;
                attachedCameras = _cameraMount;
            }
        }
    }
}