namespace ElmaSmartFarm.SharedLibrary.DatabaseTables
{
    public static class BinarySensorValues
    {
        public const string Id = "Id";
        public const string SensorId = "SensorId";
        public const string Status = "Status";
        public const string ReadDate = "ReadDate";

        public const string pId = $"@{Id}";
        public const string pSensorId = $"@{SensorId}";
        public const string pStatus = $"@{Status}";
        public const string pReadDate = $"@{ReadDate}";
    }
}