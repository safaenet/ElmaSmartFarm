﻿namespace ElmaSmartFarm.SharedLibrary.DatabaseTables
{
    public static class FarmTemperatureSensors
    {
        public const string Id = "Id";
        public const string DeviceId = "DeviceId";
        public const string FarmId = "FarmId";
        public const string Name = "Name";
        public const string Section = "Section";
        public const string IsEnabled = "IsEnabled";
        public const string OffsetValue = "OffsetValue";
        public const string Descriptions = "Descriptions";

        public const string pId = $"@{Id}";
        public const string pDeviceId = $"@{DeviceId}";
        public const string pFarmId = $"@{FarmId}";
        public const string pName = $"@{Name}";
        public const string pSection = $"@{Section}";
        public const string pIsEnabled = $"@{IsEnabled}";
        public const string pOffsetValue = $"@{OffsetValue}";
        public const string pDescriptions = $"@{Descriptions}";
    }
}