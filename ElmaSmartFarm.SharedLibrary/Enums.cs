namespace ElmaSmartFarm.SharedLibrary;

public enum SensorBaseType
{
    Scalar = 1,
    PushButton,
    Commute,
    Binary
}

public enum SensorType
{
    FarmScalar = 1,
    FarmFeed,
    FarmCheckup,
    FarmCommute,
    FarmElectricPower,
    PoultryMainElectricPower,
    PoultryBackupElectricPower,
    OutdoorScalar
}

public enum LocationType
{
    Poultry = 1,
    Farm
}

public enum FarmInPeriodErrorType
{
    LongTimeNoFeed = 1,
    HighTemperature,
    LowTemperature,
    HighHumidity,
    LowHumidity,
    LongTimeBright,
    LongTimeDark,
    HighBrightness,
    LowBrightness,
    LongTimeLeave,
    NoElectricPower
}

public enum PoultryInPeriodErrorType
{
    NoMainElectricPower = 1,
    NoBackupPower
}

public enum SensorErrorType
{
    LowBattery = 1,
    NotAlive,
    InvalidData,
    InvalidValue,

    InvalidTemperatureData,
    InvalidTemperatureValue,
    InvalidHumidityData,
    InvalidHumidityValue,
    InvalidLightData,
    InvalidLightValue,
    InvalidAmmoniaData,
    InvalidAmmoniaValue,
    InvalidCo2Data,
    InvalidCo2Value
}

public enum AlarmDeviceType
{
    Light = 1,
    SMS,
    Email
}

public enum AlarmLevel
{
    Low = 1,
    Medium,
    High
}

public enum CommuteSensorValueType
{
    StepIn = 1,
    StepOut
}

public enum BinarySensorValueType
{
    Off = 0,
    On = 1
}

public enum SensorSection
{
    FarmLeftTop = 1,
    FarmLeftMiddle = 2,
    FarmLeftBottom = 3,

    FarmMiddleTop = 4,
    FarmMiddleMiddle = 5,
    FarmMiddleBottom = 6,

    FarmRightTop = 7,
    FarmRightMiddle = 8,
    FarmRightBottom = 9,

    Outdoor = 10
}