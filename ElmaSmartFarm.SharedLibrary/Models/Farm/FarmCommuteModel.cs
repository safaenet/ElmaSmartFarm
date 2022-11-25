using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class FarmCommuteModel
    {
        public List<CommuteSensorModel> CommuteSensors { get; set; }
        public bool HasCommuteSensors => CommuteSensors != null && CommuteSensors.Any(t => t.IsEnabled);
    }
}