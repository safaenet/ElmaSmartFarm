namespace ElmaSmartFarm.SharedLibrary.DatabaseTables
{
    public static class OutdoorHumiditySensors
    {
        public const string Id = "Id";
        public const string DeviceId = "DeviceId";
        public const string PoultryId = "PoultryId";
        public const string Name = "Name";
        public const string IsEnabled = "IsEnabled";
        public const string OffsetValue = "OffsetValue";
        public const string Descriptions = "Descriptions";

        public const string pId = $"@{Id}";
        public const string pDeviceId = $"@{DeviceId}";
        public const string pPoultryId = $"@{PoultryId}";
        public const string pName = $"@{Name}";
        public const string pIsEnabled = $"@{IsEnabled}";
        public const string pOffsetValue = $"@{OffsetValue}";
        public const string pDescriptions = $"@{Descriptions}";
    }
}