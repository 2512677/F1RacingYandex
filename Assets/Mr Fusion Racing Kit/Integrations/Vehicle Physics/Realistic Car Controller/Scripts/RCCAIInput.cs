using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK;
using System;

public class RCCAIInput : MonoBehaviour, IAiInput
{
    private RCC_CarControllerV4 rcc;
    RCC_Inputs _inputs = new RCC_Inputs();

    void Start()
    {
        rcc = GetComponent<RCC_CarControllerV4>();
    }

    // Интерфейс требует четыре параметра, убираем boost
    public void SetInputValues(float throttle, float brake, float steer, float handbrake)
    {
        if (rcc != null)
        {
            // Проверяем, что гонка началась и обратный отсчёт завершён
            if (RaceManager.instance != null && RaceManager.instance.raceState == RaceState.Race)
            {
                // Включаем двигатель, если он ещё не запущен
                if (!rcc.engineRunning)
                {
                    rcc.engineRunning = true;
                }

                // Управляем машиной, если двигатель запущен
                if (rcc.engineRunning)
                {
                    _inputs.throttleInput = throttle;
                    _inputs.brakeInput = brake;
                    _inputs.steerInput = steer;
                    _inputs.handbrakeInput = handbrake;
                    rcc.OverrideInputs(_inputs, false);
                }
            }
        }
    }

    public void ApplyBoost(int boost)
    {
        if (rcc != null)
        {
            rcc.boostInput = boost == 1 ? 2.5f : 1.0f;
        }
    }
}
