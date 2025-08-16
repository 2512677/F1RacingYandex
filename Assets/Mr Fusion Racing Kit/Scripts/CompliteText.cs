using RGSK;
using UnityEngine;
using UnityEngine.UI;

public class CompliteText : MonoBehaviour
{
    [Header("Настройки гонки")]
    [Tooltip("Уникальный ID этой гонки (должен совпадать с тем, что сохраняется в PlayerData)")]
    public string raceID;

    [Tooltip("Если true, гонка изначально заблокирована и требует прохождения requiredRaceID")]
    public bool lockedByDefault = false;

    [Tooltip("ID гонки, которую нужно пройти для разблокировки этой (если lockedByDefault = true)")]
    public string requiredRaceID;

    [Header("UI")]
    [Tooltip("Текст, который показывает, что гонка завершена (\"ПРОЙДЕНО\")")]
    public GameObject completedText;

    [Tooltip("Иконка замка, если гонка заблокирована")]
    public GameObject lockIcon;

    [Tooltip("Кнопка запуска гонки (скрывается, если гонка заблокирована)")]
    public Button raceButton;

    [Header("Popup UI")]
    [Tooltip("Окно с сообщением о несоответствии класса автомобиля")]
    public GameObject popupWindow;
    [Tooltip("Текст сообщения в поп-апе")]
    public Text popupMessageText;
    [Tooltip("Кнопка OK в поп-апе")]
    public Button popupOkButton;

    [Header("Drift Popup UI")]
    [Tooltip("Окно с сообщением, что дрифт машина не подходит для этой гонки")]
    public GameObject driftPopupWindow;
    [Tooltip("Текст сообщения в дрифт поп-апе")]
    public Text driftPopupMessageText;
    [Tooltip("Кнопка OK в дрифт поп-апе")]
    public Button driftPopupOkButton;

    [Header("Career Data")]
    [Tooltip("Ссылка на данные карьеры")]
    public CareerData careerData;
    [Tooltip("Номер раунда карьеры")]
    public int roundIndex = 0;

    [Header("Physics Behavior Requirement (Optional)")]
    [Tooltip("Если true, для гонки требуется установить определённый тип физики")]
    public bool requireBehaviorType = false;
    [Tooltip("Требуемый тип физики для гонки")]
    public RCC_Settings.BehaviorType requiredBehaviorType; // Задавайте в инспекторе нужный тип (например, с именем "Racing")

    void Start()
    {
        // Если в инспекторе не указали кнопку вручную, пытаемся найти её на том же объекте
        if (!raceButton)
            raceButton = GetComponent<Button>();

        // ───────────────────────────────────────────────────────────────────────────────
        // УБРАНА автоматическая подписка на OnRaceButtonClicked, чтобы избежать двойной загрузки.
        // Теперь метод OnRaceButtonClicked() нужно привязать вручную в Inspector → Button → On Click().
        // ───────────────────────────────────────────────────────────────────────────────

        UpdateUI();
    }

    /// <summary>
    /// Основной метод проверки и обновления UI.
    /// Вызывается при старте и может вызываться повторно,
    /// если нужно обновить состояние после прохождения гонки.
    /// </summary>
    public void UpdateUI()
    {
        if (string.IsNullOrEmpty(raceID))
        {
            Debug.LogWarning($"[{name}] raceID не задан! Проверь настройки в инспекторе.");
            return;
        }

        // 1) Проверяем, пройдена ли текущая гонка
        bool isCompleted = PlayerData.instance.playerData.completedRaces.Contains(raceID);
        if (completedText)
            completedText.SetActive(isCompleted);

        // 2) Проверяем, разблокирована ли гонка:
        //    - Если lockedByDefault = false, она доступна сразу;
        //    - Если lockedByDefault = true, проверяем, пройдена ли requiredRaceID.
        bool isUnlocked = !lockedByDefault;
        if (lockedByDefault && !string.IsNullOrEmpty(requiredRaceID))
        {
            isUnlocked = PlayerData.instance.playerData.completedRaces.Contains(requiredRaceID);
        }

        // 3) Если гонка заблокирована, показываем замок и скрываем кнопку;
        //    Если разблокирована — скрываем замок, показываем кнопку.
        if (lockIcon)
            lockIcon.SetActive(!isUnlocked);

        if (raceButton)
            raceButton.gameObject.SetActive(isUnlocked);

        Debug.Log($"[{raceID}] isUnlocked = {isUnlocked}, isCompleted = {isCompleted}");
    }

    /// <summary>
    /// Обработчик нажатия на кнопку гонки.
    /// Если для данного раунда требуется определённый класс автомобиля, проверяет его,
    /// и если не соответствует, показывает popup сообщение с требованием.
    /// Если включено требование по физике, переключает режим физики.
    /// Если все проверки пройдены, запускает гонку через CareerData.
    /// </summary>
    public void OnRaceButtonClicked()
    {
        // Проверяем, назначены ли данные карьеры и корректен ли индекс раунда
        if (careerData == null || roundIndex < 0 || roundIndex >= careerData.careerRounds.Count)
        {
            Debug.LogWarning("CareerData не назначен или roundIndex выходит за пределы.");
            return;
        }

        var round = careerData.careerRounds[roundIndex];

        // Если для этого раунда требуется определённый класс автомобиля, выполняем проверку
        if (round.requireCarClass)
        {
            MenuVehicleInstantiator inst = FindObjectOfType<MenuVehicleInstantiator>();
            if (inst != null)
            {
                GameObject currentVehicle = inst.GetCurrentVehicle();
                if (currentVehicle != null)
                {
                    CarClass car = currentVehicle.GetComponent<CarClass>();
                    if (car == null || car.carClass != round.requiredCarClass)
                    {
                        if (popupWindow != null && popupMessageText != null && popupOkButton != null)
                        {
                            popupMessageText.text = "" + round.requiredCarClass.ToString();
                            popupWindow.SetActive(true);
                            popupOkButton.onClick.RemoveAllListeners();
                            popupOkButton.onClick.AddListener(() => popupWindow.SetActive(false));
                        }
                        else
                        {
                            Debug.LogWarning("Popup UI элементы не назначены в инспекторе.");
                        }
                        return; // Прерываем запуск гонки
                    }
                }
                else
                {
                    Debug.LogWarning("Текущий автомобиль не найден через MenuVehicleInstantiator.");
                    return;
                }
            }
            else
            {
                Debug.LogWarning("MenuVehicleInstantiator не найден в сцене.");
                return;
            }
        }

        // Новая проверка: если гонка не является дрифт-гонкой и не требует конкретного класса, 
        // то если выбран автомобиль с классом Drift, показываем новое окно с сообщением.
        if (!round.requireCarClass && round.raceType != RaceType.Drift)
        {
            MenuVehicleInstantiator inst = FindObjectOfType<MenuVehicleInstantiator>();
            if (inst != null)
            {
                GameObject currentVehicle = inst.GetCurrentVehicle();
                if (currentVehicle != null)
                {
                    CarClass car = currentVehicle.GetComponent<CarClass>();
                    if (car != null && car.carClass == CarClass.VehicleClass.Drift)
                    {
                        if (driftPopupWindow != null && driftPopupMessageText != null && driftPopupOkButton != null)
                        {
                            driftPopupMessageText.text = "";
                            driftPopupWindow.SetActive(true);
                            driftPopupOkButton.onClick.RemoveAllListeners();
                            driftPopupOkButton.onClick.AddListener(() => driftPopupWindow.SetActive(false));
                        }
                        else
                        {
                            Debug.LogWarning("Drift Popup UI элементы не назначены в инспекторе.");
                        }
                        return; // Прерываем запуск гонки
                    }
                }
                else
                {
                    Debug.LogWarning("Текущий автомобиль не найден через MenuVehicleInstantiator.");
                    return;
                }
            }
            else
            {
                Debug.LogWarning("MenuVehicleInstantiator не найден в сцене.");
                return;
            }
        }

        // Если для этого раунда требуется определённый тип физики, переключаем его
        if (requireBehaviorType && requiredBehaviorType != null)
        {
            // Ищем индекс требуемого типа физики в массиве behaviorTypes настроек RCC
            int index = -1;
            for (int i = 0; i < RCC_Settings.Instance.behaviorTypes.Length; i++)
            {
                if (RCC_Settings.Instance.behaviorTypes[i].behaviorName == requiredBehaviorType.behaviorName)
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                RCC_Settings.Instance.behaviorSelectedIndex = index;
                PlayerPrefs.SetInt("BehaviorType", index);
                Debug.Log($"CompliteText: Физика переключена на {requiredBehaviorType.behaviorName}");
            }
            else
            {
                Debug.LogWarning("Не найден требуемый тип физики в настройках.");
            }
        }

        // Если все проверки пройдены, запускаем гонку через CareerData
        careerData.StartRace(roundIndex);
    }
}
