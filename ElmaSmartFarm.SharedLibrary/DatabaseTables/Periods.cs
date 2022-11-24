namespace ElmaSmartFarm.SharedLibrary.DatabaseTables
{
    public static class Periods
    {
        public const string Id = "Id";
        public const string Name = "Name";
        public const string FarmId = "FarmId";
        public const string StartDate = "StartDate";
        public const string EndDate = "EndDate";
        public const string UserId = "UserId";
        public const string Descriptions = "Descriptions";

        public const string pId = $"@{Id}";
        public const string pName = $"@{Name}";
        public const string pFarmId = $"@{FarmId}";
        public const string pStartDate = $"@{StartDate}";
        public const string pEndDate = $"@{EndDate}";
        public const string pUserId = $"@{UserId}";
        public const string pDescriptions = $"@{Descriptions}";
    }
}