using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RGSK
{
    public class GhostVehicleManager : Recorder
    {
        private RecordableObject ghostVehicle;
        private GameObject ghostVehicleGameObject;
        private bool ghostActive;

        public void Initialize()
        {
            record = true;
        }


        public void CreateGhostVehicle()
        {
            if (RaceManager.instance.playerStatistics == null)
                return;

            //Получить транспортное средство игрока от менеджера гонки
            GameObject vehicle = RaceManager.instance.playerStatistics.gameObject;

            //Создать новый записываемый объект на основе транспортного средства игрока
            RecordableObject targetVehicle = new RecordableObject();
            targetVehicle.transform = vehicle.transform;
            targetVehicle.rigidbody = vehicle.GetComponent<Rigidbody>();
			targetVehicle.rgskVehicle = vehicle.GetComponent<RCC_CarControllerV3>();
            recordableObjects.Add(targetVehicle);

            //Создайте точку возрождения подальше от транспортного средства игрока, просто чтобы быть в безопасности
            Vector3 spawnPos = vehicle.transform.position + Vector3.one * 100;

            //Создаем призрака и выполняем другие важные задачи
            ghostVehicleGameObject = Instantiate(vehicle.gameObject, spawnPos, Quaternion.identity) as GameObject;
            ghostVehicleGameObject.SetColliderLayer("IgnoreCollision");

            //Используйте шейдер, если он назначен
            if (RaceManager.instance.ghostVehicleShader != null)
                ghostVehicleGameObject.ChangeRendererMaterials(RaceManager.instance.ghostVehicleShader);

            //Используйте материал, если он назначен
            if (RaceManager.instance.ghostVehicleMaterial != null)
                ghostVehicleGameObject.ChangeRendererMaterials(RaceManager.instance.ghostVehicleMaterial);

            ghostVehicleGameObject.tag = "Untagged";
            ghostVehicleGameObject.name += "_Ghost";

            //Создаем новый записываемый объект для призрачного транспортного средства
            ghostVehicle = new RecordableObject();
            ghostVehicle.transform = ghostVehicleGameObject.transform;
            ghostVehicle.rigidbody = ghostVehicleGameObject.GetComponent<Rigidbody>();
			ghostVehicle.rgskVehicle = ghostVehicleGameObject.GetComponent<RCC_CarControllerV3>();

            //Удаляем ненужные компоненты из призрачного транспортного средства
            //Подождать 0,5 с для генерации звука во время выполнения, частиц и т. д. перед удалением компонентов
            Invoke("RemoveComponents", 0.5f);
        }


        public override void Playback()
        {
            base.Playback();

            if (currentFrame < totalFrames)
            {
                PlaybackReplayFrame(ghostVehicle, ghostVehicle.replayFrameData[currentFrame]);
            }
            else
            {
                ResetGhost();
            }
        }


        public void StartGhost()
        {
            currentFrame = 0;
            playbackSpeed = 1;
            playback = true;
            ghostVehicleGameObject.SetActive(true);
            recordableObjects[0].replayFrameData.Clear();
            ghostActive = true;
        }


        void ResetGhost()
        {
            currentFrame = 0;
            playback = false;
            ghostVehicleGameObject.SetActive(false);
            ghostActive = false;
        }


        public void CacheValues()
        {
            ghostVehicle.replayFrameData.Clear();
            totalFrames = GetTotalFrames();

            for (int i = 0; i < recordableObjects[0].replayFrameData.Count; i++)
            {
                ghostVehicle.replayFrameData.Add(recordableObjects[0].replayFrameData[i]);
            }
        }


        void RemoveComponents()
        {
            Destroy(ghostVehicleGameObject.GetComponent<TimeTrialInitialize>());
            Destroy(ghostVehicleGameObject.GetComponent<RacerStatistics>());
            Destroy(ghostVehicleGameObject.GetComponent<Nitro>());
            Destroy(ghostVehicleGameObject.GetComponent<Slipstream>());
            Destroy(ghostVehicleGameObject.GetComponent<RCCPlayerInput>());
            Destroy(ghostVehicleGameObject.GetComponent<RCCAIInput>());
            Destroy(ghostVehicleGameObject.GetComponent<AiLogic>());

            foreach (Transform obj in ghostVehicleGameObject.GetComponentsInChildren<Transform>())
            {
                if (obj.GetComponent<Camera>())
                {
                    Destroy(obj.gameObject);
                }

                if (obj.GetComponent<AudioSource>())
                {
                    obj.GetComponent<AudioSource>().mute = true;
                }

                if (obj.GetComponent<RCC_WheelCollider>())
                {
                    obj.GetComponent<RCC_WheelCollider>().enabled = false;
                }

                if (obj.GetComponent<ParticleSystem>())
                {
                    obj.GetComponent<ParticleSystem>().gameObject.SetActive(false);
                }
            }

            ResetGhost();
        }


        void UpdateGhostState(RaceState state)
        {
            if (!RaceManager.instance.raceStarted)
                return;

            if (state == RaceState.Race)
                ResumeGhost();
            else
                PauseGhost();
        }


        void PauseGhost()
        {
            playback = false;

            record = false;

            ghostVehicleGameObject.SetActive(false);
        }


        void ResumeGhost()
        {
            playback = ghostActive;

            record = true;

            ghostVehicleGameObject.SetActive(ghostActive);
        }


        void OnEnable()
        {
            RaceManager.OnRaceStart += Initialize;
            RaceManager.OnRaceStateChange += UpdateGhostState;
        }


        void OnDisable()
        {
            RaceManager.OnRaceStart -= Initialize;
            RaceManager.OnRaceStateChange -= UpdateGhostState;
        }
    }
}