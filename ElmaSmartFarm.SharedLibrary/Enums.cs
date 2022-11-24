namespace ElmaSmartFarm.SharedLibrary
{
    public enum SensorType
    {
        Temperature = 1,
        Humidity = 2,
        Light = 3,
        Feed = 4,
        PushButton = 5,
        ElectricPower = 6
    }

    public enum EnvironmentType
    {
        Farm = 1,
        Outside = 2,
        Warehouse = 3
    }

    public enum FarmAlarmType
    {
        Feed,
        HighTemperature,
        LowTemperature,
        HighHumidity,
        LowHumidity,
        LongBright,
        LongDark,
        LongLeave
    }

    public enum CommuteSensorStepType
    {
        StepIn = 1,
        StepOut = 2
    }

    public enum ButtonSensorType
    {
        FeedSensor = 1,
        CheckupSensor = 2
    }

    public enum BinarySensorType
    {
        PoultryMainElectricPower = 1,
        PoultryBackupElectricPower = 2,
        FarmElectricPower = 3
    }

    public enum BinarySensorStatus
    {
        Off = 0,
        On = 1
    }
}