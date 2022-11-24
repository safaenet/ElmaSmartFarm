namespace ElmaSmartFarm.SharedLibrary.DatabaseTables
{
    public static class FoodStatistics
    {
        public const string Id = "Id";
        public const string PeriodId = "PeriodId";
        public const string FarmId = "FarmId";
        public const string FoodWeight = "FoodWeight";
        public const string FeedDate = "FeedDate";
        public const string DateRegistered = "DateRegistered";
        public const string Descriptions = "Descriptions";

        public const string pId = $"@{Id}";
        public const string pPeriodId = $"@{PeriodId}";
        public const string pFarmId = $"@{FarmId}";
        public const string pFoodWeight = $"@{FoodWeight}";
        public const string pFeedDate = $"@{FeedDate}";
        public const string pDateRegistered = $"@{DateRegistered}";
        public const string pDescriptions = $"@{Descriptions}";
    }
}