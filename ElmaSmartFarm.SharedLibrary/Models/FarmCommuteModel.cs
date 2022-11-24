namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class FarmCommuteModel
    {
        public List<CommuteSensorModel> CommuteSensors { get; set; }
        public bool HasCommuteSensors => CommuteSensors != null && CommuteSensors.Any(t => t.IsEnabled);
    }
}