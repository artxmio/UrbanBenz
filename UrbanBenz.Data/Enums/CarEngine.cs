namespace UrbanBenz.Data.Enums;

public enum CarEngine
{
    /// <summary>
    /// Неизвестный тип двигателя
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Бензиновый двигатель внутреннего сгорания
    /// </summary>
    Petrol = 1,

    /// <summary>
    /// Дизельный двигатель внутреннего сгорания
    /// </summary>
    Diesel = 2,

    /// <summary>
    /// Электрический двигатель (аккумуляторный)
    /// </summary>
    Electric = 3,

    /// <summary>
    /// Гибрид - бензин + электричество
    /// </summary>
    Hybrid = 4,

    /// <summary>
    /// Плагин-гибрид - подзаряжаемый от сети гибрид
    /// </summary>
    PlugInHybrid = 5,

    /// <summary>
    /// Газовое оборудование (пропан-бутан)
    /// </summary>
    LPG = 6,

    /// <summary>
    /// Газовое оборудование (метан)
    /// </summary>
    CNG = 7,

    /// <summary>
    /// Роторный двигатель
    /// </summary>
    Rotary = 8,

    /// <summary>
    /// Водородный двигатель (Fuel Cell)
    /// </summary>
    Hydrogen = 9
}