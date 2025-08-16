using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK;

public class RCCPlayerInput : MonoBehaviour
{
    private IInputManager inputManager;
    private RCC_CarControllerV4 rcc;
    private RCC_Inputs _inputs = new RCC_Inputs();

    void Start()
    {
        // Получаем ссылку на менеджер ввода
        inputManager = InputManager.instance;

        // Получаем ссылки на компоненты для передачи ввода
        rcc = GetComponent<RCC_CarControllerV4>();

        // Сразу глушим двигатель до старта
        rcc.engineRunning = false;
    }

    void Update()
    {
        // Если RaceManager не готов или гонка ещё не началась — не даём ехать
        var rm = RaceManager.instance;
        if (rm == null || !rm.raceStarted)
        {
            if (rcc.engineRunning)
                rcc.engineRunning = false;
            return;
        }

        // Гонка началась — включаем двигатель один раз и обрабатываем ввод
        if (!rcc.engineRunning)
            rcc.engineRunning = true;

        HandleCarInput();
    }

    void HandleCarInput()
    {
        float throttle = Mathf.Clamp01(inputManager.GetAxis(0, InputAction.Throttle));
        float brake = Mathf.Clamp01(inputManager.GetAxis(0, InputAction.Brake));
        float steer = inputManager.GetAxis(0, InputAction.SteerLeft) - inputManager.GetAxis(0, InputAction.SteerRight);
        float handbrake = inputManager.GetAxis(0, InputAction.Handbrake);

        // Заполняем структуру и передаём в RCC
        _inputs.throttleInput = throttle;
        _inputs.brakeInput = brake;
        _inputs.steerInput = steer;
        _inputs.handbrakeInput = handbrake;
        rcc.OverrideInputs(_inputs, false);
    }
}
