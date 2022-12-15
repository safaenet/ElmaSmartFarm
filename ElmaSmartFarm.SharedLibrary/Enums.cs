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

public enum FarmInPeriodErrorType
{
    HighTemperature = 1,
    LowTemperature,
    HighHumidity,
    LowHumidity,
    HighAmmonia,
    HighCo2,
    LongTimeBright,
    LongTimeDark,
    HighBrightness,
    LowBrightness,
    LongNoFeed,
    LongLeave,
    NoPower
}

public enum PoultryInPeriodErrorType
{
    NoMainPower = 1,
    NoBackupPower
}

public enum AlarmDeviceType
{
    FarmLight = 1,
    FarmSiren,
    PoultryLight,
    PoultrySiren
}

public enum AlarmLevel
{
    Low = 1,
    Warning,
    Critical,
    Fatal
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