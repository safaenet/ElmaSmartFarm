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
}