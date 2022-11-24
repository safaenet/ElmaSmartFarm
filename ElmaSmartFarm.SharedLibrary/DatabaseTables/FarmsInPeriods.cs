namespace ElmaSmartFarm.SharedLibrary.DatabaseTables
{
    public static class FarmsInPeriods
    {
        public const string FarmId = "FarmId";
        public const string PeriodId = "PeriodId";
        public const string ChickenCount = "ChickenCount";

        public const string pFarmId = $"@{FarmId}";
        public const string pPeriodId = $"@{PeriodId}";
        public const string pChickenCount = $"@{ChickenCount}";
    }
}