using RGSK;
using UnityEngine;
using UnityEngine.UI;

public class RaceSelector : MonoBehaviour
{
    [Header("Настройки гонки")]
    [Tooltip("Уникальный идентификатор гонки (должен совпадать с тем, что указан в CareerData)")]
    public string raceID;

    [Tooltip("Если true, гонка изначально заблокирована")]
    public bool lockedByDefault = false;

    [Tooltip("ID гонки, которую нужно пройти для разблокировки этой (если заблокирована)")]
    public string requiredRaceID;

    [Header("Требования к машине")]
    [Tooltip("Требуемый класс машины для участия в гонке (если не требуется, оставь None)")]
    public CarClass.VehicleClass requiredCarClass = CarClass.VehicleClass.None;

    [Header("UI элементы")]
    [Tooltip("Объект, показывающий, что гонка завершена (например, надпись 'ПРОЙДЕНО')")]
    public GameObject completedText;

    [Tooltip("Иконка замка, показывающая, что гонка недоступна")]
    public GameObject lockIcon;

    [Header("Параметры выбора")]
    [Tooltip("Ссылка на CareerData для запуска гонки")]
    public CareerData careerData;

    [Tooltip("Индекс раунда в CareerData, соответствующий этой гонке")]
    public int raceIndex;

    void Start()
    {
        UpdateRaceUI();
    }

    // Обновление UI: показываем текст "ПРОЙДЕНО" и/или иконку замка в зависимости от условий
    public void UpdateRaceUI()
    {
        // Отображаем текст "ПРОЙДЕНО", если эта гонка уже пройдена
        bool isCompleted = PlayerData.instance.playerData.completedRaces.Contains(raceID);
        if (completedText != null)
            completedText.SetActive(isCompleted);

        // Проверка разблокировки по условию прохождения предыдущей гонки
        bool isUnlockedByRace = !lockedByDefault ||
            (lockedByDefault && !string.IsNullOrEmpty(requiredRaceID) &&
             PlayerData.instance.playerData.completedRaces.Contains(requiredRaceID));

        // Проверка, соответствует ли выбранный класс машины требуемому
        bool isCarClassOk = CheckCarClass();

        // Гонка считается разблокированной, если выполнены оба условия
        bool isUnlocked = isUnlockedByRace && isCarClassOk;

        if (lockIcon != null)
            lockIcon.SetActive(!isUnlocked);

        Debug.Log($"Race {raceID}: isUnlockedByRace={isUnlockedByRace}, isCarClassOk={isCarClassOk} => isUnlocked={isUnlocked}");
    }

    // Проверка требуемого класса машины
    bool CheckCarClass()
    {
        // Если требуемый класс не задан (None) – условие считается выполненным
        if (requiredCarClass == CarClass.VehicleClass.None)
            return true;

        CarClass.VehicleClass playerCarClass = GetPlayerCarClass();
        return (playerCarClass == requiredCarClass);
    }

    // Получение класса выбранной машины игрока.
    // Замените этот метод на свою логику: например, через базу данных машин или через объект на сцене.
    CarClass.VehicleClass GetPlayerCarClass()
    {
        // Пример (если есть VehicleDatabase):
        // var vehicleData = VehicleDatabase.Instance.GetVehicleByID(PlayerData.instance.playerData.vehicleID);
        // return vehicleData.carClass;

        // Пример (если объект машины уже на сцене):
        // CarClass car = FindObjectOfType<PlayerCarController>()?.GetComponent<CarClass>();
        // if (car != null)
        //     return car.carClass;

        // Пока выводим предупреждение и возвращаем None – замените на свою логику!
        Debug.LogWarning("GetPlayerCarClass: реализуйте получение класса машины для игрока.");
        return CarClass.VehicleClass.None;
    }

    // Метод, вызываемый при выборе гонки (например, при нажатии кнопки)
    public void OnRaceSelected()
    {
        UpdateRaceUI(); // Обновляем состояние UI перед выбором

        bool isUnlockedByRace = !lockedByDefault ||
            (lockedByDefault && !string.IsNullOrEmpty(requiredRaceID) &&
             PlayerData.instance.playerData.completedRaces.Contains(requiredRaceID));
        bool isCarClassOk = CheckCarClass();
        bool isUnlocked = isUnlockedByRace && isCarClassOk;

        if (!isUnlocked)
        {
            // Если гонка заблокирована, выводим соответствующее сообщение в консоль
            if (!isUnlockedByRace)
                Debug.Log("Гонка заблокирована: необходимо пройти предыдущую гонку.");
            else if (!isCarClassOk)
                Debug.Log("Гонка недоступна: выбранный класс машины не соответствует требуемому.");
            return;
        }

        // Если все условия выполнены, запускаем гонку через CareerData (см. CareerData.cs)
        if (careerData != null)
        {
            Debug.Log($"Запуск гонки {raceID} (roundIndex: {raceIndex})");
            careerData.StartRace(raceIndex);
        }
        else
        {
            Debug.LogError("Ссылка на CareerData не установлена!");
        }
    }
}
