namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class CommuteSensorModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Section { get; set; }
        public List<CommuteReadModel> CommuteReads { get; set; }
        public DateTime? LastStepInDate => CommuteReads?.Where(c => c.StepType == CommuteSensorStepType.StepIn).MaxBy(r => r.ReadDate)?.ReadDate;
        public DateTime? LastStepOutDate => CommuteReads?.Where(c => c.StepType == CommuteSensorStepType.StepOut).MaxBy(r => r.ReadDate)?.ReadDate;
        public CommuteReadModel LastRead => CommuteReads?.MaxBy(r => r.ReadDate);
        public DateTime? LastReadDate => CommuteReads?.MaxBy(t => t.ReadDate)?.ReadDate;
        public bool IsEnabled { get; set; }
        public int BatteryLevel { get; set; }
        public string Descriptions { get; set; }
    }
}