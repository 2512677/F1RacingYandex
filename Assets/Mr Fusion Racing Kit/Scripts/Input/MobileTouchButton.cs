using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using RGSK;

public class MobileTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public InputAction inputAction;

    private float inputValue;
    public float InputValue
    {
        get { return inputValue; }
        set { inputValue = value; }
    }

    public bool pressed { get; private set; }
    public bool held { get; private set; }
    public bool released { get; private set; }

    // Если true, значит эта кнопка предназначена для респауна.
    // Для кнопок паузы или переключения камеры этот флаг должен быть выключен.
    public bool isRespawnButton = false;

    // Используется для получения входных данных извне (например, для TouchSteeringWheel)
    public bool externalInput;

    // Если true, значение InputValue будет отрицательным
    public bool invert;

    // Можно также оставить UnityEvent для дополнительных настроек через инспектор
    public UnityEvent onRespawn;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (externalInput)
            return;

        held = true;
        StartCoroutine(SetPressed());

        // Вызываем событие, если оно настроено
        if (onRespawn != null && onRespawn.GetPersistentEventCount() > 0)
        {
            onRespawn.Invoke();
        }
        else
        {
            // Если эта кнопка предназначена для респауна, вызываем метод респауна
            if (isRespawnButton)
            {
                RespawnCar();
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (externalInput)
            return;

        held = false;
        StartCoroutine(SetReleased());
    }

    IEnumerator SetPressed()
    {
        pressed = true;
        yield return new WaitForEndOfFrame();
        pressed = false;
    }

    IEnumerator SetReleased()
    {
        released = true;
        yield return new WaitForEndOfFrame();
        released = false;
    }

    void Update()
    {
        InputValue = held ? Mathf.MoveTowards(InputValue, invert ? -1 : 1, Time.deltaTime * 5f)
                          : Mathf.MoveTowards(InputValue, 0, Time.deltaTime * 5f);
    }

    void OnDisable()
    {
        InputValue = 0;
        pressed = false;
        held = false;
        released = false;
    }

    /// <summary>
    /// Метод для вызова респауна автомобиля.
    /// Вызывается только если isRespawnButton = true.
    /// </summary>
    public void RespawnCar()
    {
        if (RaceManager.instance != null && RaceManager.instance.playerStatistics != null)
        {
            // Вызываем респаун, передавая трансформ игрока
            RaceManager.instance.RespawnVehicle(RaceManager.instance.playerStatistics.transform);
        }
        else
        {
            Debug.LogWarning("RaceManager или playerStatistics не установлены. Респаун не может быть вызван.");
        }
    }
}
