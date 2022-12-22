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

/// <summary>
/// Used for "Action" in database, Table: SensorWatchLogs.
/// </summary>
public enum SensorWatchAction
{
    Unwatch = 0,
    Watch
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

/// <summary>
/// Used for "Action" in database, Table: AlarmTriggerLogs.
/// </summary>
public enum AlarmTriggerType
{
    Off = 0,
    On
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