namespace ElmaSmartFarm.SharedLibrary.DatabaseTables
{
    public static class Farms
    {
        public const string Id = "Id";
        public const string Name = "Name";
        public const string PoultryId = "PoultryId";
        public const string FarmNumber = "FarmNumber";
        public const string MaxCapacity = "MaxCapacity";
        public const string IsEnabled = "IsEnabled";
        public const string Descriptions = "Descriptions";

        public const string pId = $"@{Id}";
        public const string pName = $"@{Name}";
        public const string pPoultryId = $"@{PoultryId}";
        public const string pFarmNumber = $"@{FarmNumber}";
        public const string pMaxCapacity = $"@{MaxCapacity}";
        public const string pIsEnabled = $"@{IsEnabled}";
        public const string pDescriptions = $"@{Descriptions}";
    }
}