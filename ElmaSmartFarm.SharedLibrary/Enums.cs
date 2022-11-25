namespace ElmaSmartFarm.SharedLibrary
{
    public enum SensorType
    {
        FarmTemperature = 1,
        FarmHumidity,
        FarmAmbientLight,
        FarmFeed,
        FarmCommute,
        FarmCheckup,
        FarmElectricPower,
        PoultryMainElectricPower,
        PoultryBackupElectricPower
    }

    public enum EnvironmentType
    {
        Farm = 1,
        Outside,
        Warehouse
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
        NotOnline,
        InvalidValue
    }

    public enum AlarmDeviceType
    {
        Light = 1,
        SMS,
        Email
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
}