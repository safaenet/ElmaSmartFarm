namespace ElmaSmartFarm.SharedLibrary.DatabaseTables
{
    public static class ChickenLosses
    {
        public const string Id = "Id";
        public const string PeriodId = "PeriodId";
        public const string FarmId = "FarmId";
        public const string LossCount = "LossCount";
        public const string DateHappened = "DateHappened";
        public const string DateRegistered = "DateRegistered";
        public const string Descriptions = "Descriptions";

        public const string pId = $"@{Id}";
        public const string pPeriodId = $"@{PeriodId}";
        public const string pFarmId = $"@{FarmId}";
        public const string pLossCount = $"@{LossCount}";
        public const string pDateHappened = $"@{DateHappened}";
        public const string pDateRegistered = $"@{DateRegistered}";
        public const string pDescriptions = $"@{Descriptions}";
    }
}