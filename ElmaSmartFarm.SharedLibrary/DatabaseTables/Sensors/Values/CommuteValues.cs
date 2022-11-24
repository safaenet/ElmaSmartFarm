namespace ElmaSmartFarm.SharedLibrary.DatabaseTables
{
    public static class CommuteValues
    {
        public const string Id = "Id";
        public const string SensorId = "SensorId";
        public const string IsStepIn = "IsStepIn";
        public const string StepDate = "StepDate";

        public const string pId = $"@{Id}";
        public const string pSensorId = $"@{SensorId}";
        public const string pIsStepIn = $"@{IsStepIn}";
        public const string pStepDate = $"@{StepDate}";
    }
}