namespace ElmaSmartFarm.SharedLibrary
{
    public enum SensorType
    {
        FarmTemperature = 1,
        FarmHumidity,
        FarmAmbientLight,
        FarmFeed,
        FarmCheckup,
        FarmCommute,
        FarmElectricPower,
        PoultryMainElectricPower,
        PoultryBackupElectricPower,
        OutdoorTemperature,
        OutdoorHumidity
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
        InvalidValue
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
        FarmLeftTop = 11,
        FarmLeftMiddle = 12,
        FarmLeftBottom = 13,
        FarmMiddleTop = 21,
        FarmMiddleMiddle = 22,
        FarmMiddleBottom = 23,
        FarmRightTop = 31,
        FarmRightMiddle = 32,
        FarmRightBottom = 33,        
        Outdoor = 34
    }
}