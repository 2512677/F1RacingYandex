//______________________________________________//
//___________Realistic Engine Sounds____________//
//______________________________________________//
//_______Copyright © 2025 Skril Studio__________//
//______________________________________________//
//__________ http://skrilstudio.com/ ___________//
//______________________________________________//
//________ http://fb.com/yugelmobile/ __________//
//______________________________________________//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK;  // для RaceCamera и CameraMode

namespace SkrilStudio
{
    public class RCC_RES2 : MonoBehaviour
    {
        private RealisticEngineSound[] res2;
        private RCC_CarControllerV4 rcc;

        // Было: RCC_Camera, теперь RaceCamera
        private RaceCamera raceCam;
        private GameObject car;

        // Чувствительность газа
        private float gasPedalSensity = 0.01f;

        // 0 = exterior, 1 = interior, 2 = scene start (ещё не выбрано)
        private int currentActivePrefab = 2;

        [Tooltip("Имя объекта с RaceCamera. Если пусто/не найдено — возьмём первую RaceCamera в сцене.")]
        public string raceCameraName = "RaceCamera";

        void Start()
        {
            // Берём оба префаба RES (в т.ч. неактивные)
            res2 = GetComponentsInChildren<RealisticEngineSound>(true);

            // Находим машину (как в исходнике, плюс фоллбек на стандартный метод)
            car = gameObject.GetFirstParentWithComponent<RCC_CarControllerV4>();
            if (!car && GetComponentInParent<RCC_CarControllerV4>() != null)
                car = GetComponentInParent<RCC_CarControllerV4>().gameObject;

            if (car)
                rcc = car.GetComponent<RCC_CarControllerV4>();

            // Ищем RaceCamera по имени, иначе любую в сцене
            GameObject camGo = null;
            if (!string.IsNullOrEmpty(raceCameraName))
                camGo = GameObject.Find(raceCameraName);
            if (camGo)
                raceCam = camGo.GetComponent<RaceCamera>();
            if (!raceCam)
                raceCam = FindObjectOfType<RaceCamera>();

            // Подготовка параметров RES из RCC
            if (rcc != null && res2 != null)
            {
                for (int i = 0; i < res2.Length; i++)
                {
                    res2[i].maxRPMLimit = rcc.maxEngineRPM;
                    res2[i].carMaxSpeed = rcc.maxspeed;
                }
                // Отключаем встроенный звук RCC
                rcc.audioType = RCC_CarControllerV4.AudioType.Off;
            }
        }

        void Update()
        {
            if (rcc == null || res2 == null || res2.Length == 0)
                return;

            if (currentActivePrefab != 2) // когда уже выбран внешний/внутренний набор
            {
                var inst = res2[currentActivePrefab];
                if (inst.enabled)
                {
                    // RPM / скорость / переключение
                    if (rcc.engineRunning)
                        inst.engineCurrentRPM = rcc.engineRPM;
                    inst.carCurrentSpeed = rcc.speed;
                    inst.isShifting = rcc.changingGear;

                    // Газ вперёд
                    if (rcc.direction != -1)
                    {
                        if (rcc.throttleInput >= gasPedalSensity)
                            inst.gasPedalPressing = !rcc.changingGear;
                        if (rcc.throttleInput > -gasPedalSensity && rcc.throttleInput < gasPedalSensity)
                            inst.gasPedalPressing = false;
                        inst.isReversing = false;
                    }
                    else // Задний ход
                    {
                        if (rcc.throttleInput <= -gasPedalSensity)
                            inst.gasPedalPressing = true;
                        if (rcc.changingGear)
                            inst.gasPedalPressing = false;
                        if (inst.enableReverseGear)
                            inst.isReversing = true;
                    }
                }

                // Двигатель заглушён — глушим RPM
                if (!rcc.engineRunning)
                    res2[currentActivePrefab].engineCurrentRPM = 0;
            }
        }

        void LateUpdate()
        {
            if (res2 == null || res2.Length == 0) return;

            if (currentActivePrefab != 2)
            {
                if (res2[currentActivePrefab].enabled && res2.Length > 1)
                    CameraUpdate();
            }
            else
            {
                // На старте сцены сразу выбрать набор звуков по режиму камеры
                if (res2.Length > 1)
                    CameraUpdate();
                else
                    currentActivePrefab = 0;
            }
        }

        private void CameraUpdate()
        {
            // Интерьер — только когда камера в Cockpit
            bool interior = raceCam && raceCam.cameraMode == CameraMode.Cockpit;

            int wanted = interior ? 1 : 0;
            if (currentActivePrefab == wanted) return;

            if (res2.Length > 1)
            {
                // 0 — exterior, 1 — interior
                res2[0].gameObject.SetActive(!interior);
                res2[1].gameObject.SetActive(interior);
            }
            currentActivePrefab = wanted;
        }
    }
}
