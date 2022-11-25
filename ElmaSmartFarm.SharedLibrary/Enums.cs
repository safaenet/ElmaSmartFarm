namespace ElmaSmartFarm.SharedLibrary
{
    public enum SensorType
    {
        Temperature = 1,
        Humidity,
        Light,
        Feed,
        PushButton,
        ElectricPower
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

    public enum CommuteSensorStepType
    {
        StepIn = 1,
        StepOut
    }

    public enum ButtonSensorType
    {
        FeedSensor = 1,
        CheckupSensor
    }

    public enum BinarySensorType
    {
        PoultryMainElectricPower = 1,
        PoultryBackupElectricPower,
        FarmElectricPower
    }

    public enum BinarySensorStatus
    {
        Off = 0,
        On = 1
    }
}