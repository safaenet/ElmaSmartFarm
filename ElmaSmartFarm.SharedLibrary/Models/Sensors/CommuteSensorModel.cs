using ElmaSmartFarm.SharedLibrary.Models.Sensors.ReadModels;

namespace ElmaSmartFarm.SharedLibrary.Models.Sensors
{
    public class CommuteSensorModel : SensorBaseModel
    {
        public int Section { get; set; }
        public List<CommuteReadModel> CommuteReads { get; set; }
        public DateTime? LastStepInDate => CommuteReads?.Where(c => c.StepType == CommuteSensorStepType.StepIn).MaxBy(r => r.ReadDate)?.ReadDate;
        public DateTime? LastStepOutDate => CommuteReads?.Where(c => c.StepType == CommuteSensorStepType.StepOut).MaxBy(r => r.ReadDate)?.ReadDate;
        public CommuteReadModel LastRead => CommuteReads?.MaxBy(r => r.ReadDate);
        public DateTime? LastReadDate => CommuteReads?.MaxBy(t => t.ReadDate)?.ReadDate;
    }
}