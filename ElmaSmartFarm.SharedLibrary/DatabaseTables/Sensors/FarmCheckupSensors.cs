﻿namespace ElmaSmartFarm.SharedLibrary.DatabaseTables
{
    public static class FarmCheckupSensors
    {
        public const string Id = "Id";
        public const string DeviceId = "DeviceId";
        public const string FarmId = "FarmId";
        public const string Name = "Name";
        public const string IsEnabled = "IsEnabled";
        public const string Descriptions = "Descriptions";

        public const string pId = $"@{Id}";
        public const string pDeviceId = $"@{DeviceId}";
        public const string pFarmId = $"@{FarmId}";
        public const string pName = $"@{Name}";
        public const string pIsEnabled = $"@{IsEnabled}";
        public const string pDescriptions = $"@{Descriptions}";
    }
}