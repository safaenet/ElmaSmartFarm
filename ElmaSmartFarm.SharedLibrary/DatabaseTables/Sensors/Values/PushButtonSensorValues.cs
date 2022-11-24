namespace ElmaSmartFarm.SharedLibrary.DatabaseTables
{
    public static class PushButtonSensorValues
    {
        public const string Id = "Id";
        public const string SensorId = "SensorId";
        public const string ReadDate = "ReadDate";

        public const string pId = $"@{Id}";
        public const string pSensorId = $"@{SensorId}";
        public const string pReadDate = $"@{ReadDate}";
    }
}