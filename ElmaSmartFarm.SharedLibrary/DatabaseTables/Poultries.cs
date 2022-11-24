namespace ElmaSmartFarm.SharedLibrary.DatabaseTables
{
    public static class Poultries
    {
        public const string Id = "Id";
        public const string Name = "Name";
        public const string IsEnabled = "IsEnabled";
        public const string Descriptions = "Descriptions";

        public const string pId = $"@{Id}";
        public const string pName = $"@{Name}";
        public const string pIsEnabled = $"@{IsEnabled}";
        public const string pDescriptions = $"@{Descriptions}";
    }
}