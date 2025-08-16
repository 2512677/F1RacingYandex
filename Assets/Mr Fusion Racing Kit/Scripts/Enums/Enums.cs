using System;
using System.ComponentModel;
using I2.Loc;

public enum RaceType
{
    Circuit,
    Sprint,
    LapKnockout,
    Checkpoint,
    SpeedTrap,
    Elimination,
    TimeTrial,
    TimeAttack,
    Endurance,
    Drift,
    Drag,
    Chase,
    Pursuit,
    Leader  // Новый режим «Лидер»


}

public static class RaceTypeExtensions
{
    /// <summary>
    /// Получает локализованную строку для RaceType с использованием I2 Localization.
    /// </summary>
    public static string GetLocalizedString(this RaceType raceType)
    {
        return LocalizationManager.GetTranslation($"RaceType/{raceType}");

    }
}


public static class SpeedUnitExtensions
{
    public static string GetLocalizedString(this SpeedUnit speedUnit)
    {
        return LocalizationManager.GetTranslation($"SpeedUnit/{speedUnit}");
    }
}












/// <summary>
/// Состояния гонки.
/// </summary>


public enum RaceState
{
    PreRace,          // Предгоночное состояние
    Race,             // Активная гонка
    Pause,            // Пауза в гонке
    PostRace,         // Постгонковое состояние
    Replay            // Реплей гонки
}

/// Режимы старта гонки.
/// 

public enum RaceStartMode
{
    StandingStart,    // Статический старт (все участники стартуют с места)
    RollingStart      // Скользящий старт (участники уже движутся при старте)
}

/// Классы автомобилей в гонке.

public enum RaceVehicleClass
{
    SingleClass,      // Одиночный класс (все автомобили одной категории)
    MultiClass,       // Многоуровневый класс (разные категории автомобилей)
    Custom            // Пользовательский класс (настраиваемые категории)
}

/// Типы триггеров гонки.

public enum RaceTriggerType
{
    FinishLine,       // Финишная линия
    Checkpoint,       // Чекпоинт
    SpeedTrap,        // Ловушка скорости
    Sector            // Сектор трассы
}

/// Логика запуска таймера окончания гонки. 
/// 

public enum RaceEndTimerStart
{
    After1st,         // После первого места
    AfterHalf         // После половины гонки
}

/// Режимы навигации ИИ.
public enum AINavMode
{
    Racing,           // Гонка
    RollingStart      // Скользящий старт   
}

/// Уровни сложности ИИ.
public enum AIDifficultyLevel
{
    
    Easy,

    
    Medium,

    
    Hard,

    
    Custom
}

public static class AIDifficultyLevelExtensions
{
    public static string GetLocalizedString(this AIDifficultyLevel difficultyLevel)
    {
        return LocalizationManager.GetTranslation($"DifficultyLevel/{difficultyLevel}");
    }
}

/// Режимы спавна игрока.
public enum PlayerSpawnMode
{
    Selected,         // Выбранный спавн (предопределенная позиция)
    RandomFromClass   // Случайный спавн из класса
}

/// Режимы спавна ИИ.
public enum AISpawnMode
{
    Random,           // Случайный спавн
    Order             // Спавн по порядку
}

/// Режимы позиционирования на стартовой решётке.
public enum GridPositioningMode
{
    Random,           // Случайное позиционирование
    First,            // Первое место на решётке
    Last,             // Последнее место на решётке
    Select            // Выбор позиции на решётке
}

/// Типы позиционирования.
public enum PositioningType
{
    PositionScore,    // Позиция по счёту
    Speed             // Позиция по скорости

}

/// Единицы измерения скорости.
public enum SpeedUnit
{
    KPH,              // Километры в час
    MPH               // Мили в час
}

/// Единицы измерения расстояния.
public enum DistanceUnit
{
    Meters,           // Метры
    Feet,             // Футы
    Yards             // Ярды
}

/// Форматы отображения времени.
public enum TimeFormat
{
    HrMinSec,         // Часы:Минуты:Секунды
    MinSecMs,         // Минуты:Секунды:Миллисекунды
    MinSec,           // Минуты:Секунды
    SecMs             // Секунды:Миллисекунды
}

/// Устройства ввода.
public enum InputDevices
{
    Keyboard,         // Клавиатура
    XboxController,   // Контроллер Xbox
    RacingWheel,      // Рули гонки
    Mobile            // Мобильные устройства
}

/// Типы управления рулём на мобильных устройствах.
public enum MobileSteerType
{
    Touch,            // Сенсорное управление
    Tilt              // Управление наклоном устройства
}

/// Режимы отображения позиции в UI.
public enum UIPositionDisplayMode
{
    Default,          // Стандартный режим
    OrdinalPosition,  // Порядковая позиция
    PositionOnly      // Только позиция
}

/// Режимы отображения круга в UI.
public enum UILapDisplayMode
{
    Default,          // Стандартный режим
    LapOnly           // Только круг
}

/// Режимы отображения зазора в UI.
public enum UIGapDisplayMode
{
    Time,             // Зазор по времени
    Distance          // Зазор по расстоянию
}

/// Элементы записи в таблице гонки.
public enum UIRaceEntryElement
{
    Position,         // Позиция
    Name,             // Имя
    Vehicle,          // Автомобиль
    BestLap,          // Лучший круг
    TotalTime,        // Общее время
    Points,           // Очки
    Gap,              // Зазор
    Nationality,      // Национальность
    TotalSpeed        // Общая скорость
}

/// Направления прогресс-бара в UI.
public enum UIProgressBarDirection
{
    Vertical,         // Вертикальное направление
    Horizontal        // Горизонтальное направление
}

/// Типы триггеров (пустое перечисление, возможно, для будущего использования).
public enum TriggerType
{
    // Возможно, добавить типы триггеров в будущем
}

/// Режимы камеры.
public enum CameraMode
{
    Chase,            // Камера преследования
    Cockpit,          // Камера кабины
    Fixed             // Фиксированная камера
}

/// Национальности участников гонки.
public enum Nationality
{
    Other, // Другое
    Afghanistan,
    Albania,
    Algeria,
    Andorra,
    Angola,
    AntiguaAndDeps,
    Argentina,
    Armenia,
    Australia,
    Austria,
    Azerbaijan,
    Bahamas,
    Bahrain,
    Bangladesh,
    Barbados,
    Belarus,
    Belgium,
    Belize,
    Benin,
    Bhutan,
    Bolivia,
    BosniaHerzegovina,
    Botswana,
    Brazil,
    Brunei,
    Bulgaria,
    Burkina,
    Burundi,
    Cambodia,
    Cameroon,
    Canada,
    CapeVerde,
    CentralAfricanRep,
    Chad,
    Chile,
    China,
    Colombia,
    Comoros,
    Congo,
    CongoDemocraticRep,
    CostaRica,
    Croatia,
    Cuba,
    Cyprus,
    CzechRepublic,
    Denmark,
    Djibouti,
    Dominica,
    DominicanRepublic,
    EastTimor,
    Ecuador,
    Egypt,
    ElSalvador,
    EquatorialGuinea,
    Eritrea,
    Estonia,
    Ethiopia,
    Fiji,
    Finland,
    France,
    Gabon,
    Gambia,
    Georgia,
    Germany,
    Ghana,
    Greece,
    Grenada,
    Guatemala,
    Guinea,
    GuineaBissau,
    Guyana,
    Haiti,
    Honduras,
    Hungary,
    Iceland,
    India,
    Indonesia,
    Iran,
    Iraq,
    Ireland,
    Israel,
    Italy,
    IvoryCoast,
    Jamaica,
    Japan,
    Jordan,
    Kazakhstan,
    Kenya,
    Kiribati,
    KoreaNorth,
    KoreaSouth,
    Kosovo,
    Kuwait,
    Kyrgyzstan,
    Laos,
    Latvia,
    Lebanon,
    Lesotho,
    Liberia,
    Libya,
    Liechtenstein,
    Lithuania,
    Luxembourg,
    Macedonia,
    Madagascar,
    Malawi,
    Malaysia,
    Maldives,
    Mali,
    Malta,
    MarshallIslands,
    Mauritania,
    Mauritius,
    Mexico,
    Micronesia,
    Moldova,
    Monaco,
    Mongolia,
    Montenegro,
    Morocco,
    Mozambique,
    Myanmar,
    Namibia,
    Nauru,
    Nepal,
    Netherlands,
    NewZealand,
    Nicaragua,
    Niger,
    Nigeria,
    Norway,
    Oman,
    Pakistan,
    Palau,
    Panama,
    PapuaNewGuinea,
    Paraguay,
    Peru,
    Philippines,
    Poland,
    Portugal,
    Qatar,
    Romania,
    RussianFederation,
    Rwanda,
    StKittsAndNevis,
    StLucia,
    SaintVincentAndTheGrenadines,
    Samoa,
    SanMarino,
    SaoTomeAndPrincipe,
    SaudiArabia,
    Senegal,
    Serbia,
    Seychelles,
    SierraLeone,
    Singapore,
    Slovakia,
    Slovenia,
    SolomonIslands,
    Somalia,
    SouthAfrica,
    SouthSudan,
    Spain,
    SriLanka,
    Sudan,
    Suriname,
    Swaziland,
    Sweden,
    Switzerland,
    Syria,
    Taiwan,
    Tajikistan,
    Tanzania,
    Thailand,
    Togo,
    Tonga,
    TrinidadAndTobago,
    Tunisia,
    Turkey,
    Turkmenistan,
    Tuvalu,
    Uganda,
    Ukraine,
    UnitedArabEmirates,
    UnitedKingdom,
    UnitedStates,
    Uruguay,
    Uzbekistan,
    Vanuatu,
    VaticanCity,
    Venezuela,
    Vietnam,
    Yemen,
    Zambia,
    Zimbabwe
}

/// Кнопки контроллера Xbox.
public enum XboxInputButtons
{
    A,                   // Кнопка A
    X,                   // Кнопка X
    Y,                   // Кнопка Y
    B,                   // Кнопка B
    LeftBumper,          // Левая верхняя кнопка (bumper)
    RightBumper,         // Правая верхняя кнопка (bumper)
    RightTrigger,        // Правая кнопка-триггер
    LeftTrigger,         // Левая кнопка-триггер
    DpadVertical,        // Вертикальное направление D-pad
    DpadHorizontal,      // Горизонтальное направление D-pad
    RightAnalogHorizontal, // Горизонтальное движение правого аналогового стика
    RightAnalogVertical,   // Вертикальное движение правого аналогового стика
    LeftAnalogHorizontal,  // Горизонтальное движение левого аналогового стика
    LeftAnalogVertical,    // Вертикальное движение левого аналогового стика
    RightAnalogClick,      // Нажатие правого аналогового стика
    LeftAnalogClick,       // Нажатие левого аналогового стика
    Start,                 // Кнопка Start
    Back                   // Кнопка Back
}