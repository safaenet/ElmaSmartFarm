namespace ElmaSmartFarm.SharedLibrary.DALModels
{
    public class PoultryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<FarmModel> Farms { get; set; }
        public TemperatureSensorModel OutdoorTemperature { get; set; }
        public HumiditySensorModel OutdoorHumidity { get; set; }
        public BinarySensorModel MainElectricPowerStatus { get; set; }
        public BinarySensorModel BackupElectricPowerStatus { get; set; }
        public int TotalPrimaryChickenCount => Farms != null ? Farms.Sum(c => c.ChickenStatistics != null ? c.ChickenStatistics.ChickenPrimaryCount : 0) : 0;
        public int TotalAliveChickenCount => Farms != null ? Farms.Sum(c => c.ChickenStatistics != null ? c.ChickenStatistics.TotalAliveCount : 0) : 0;
        public int TotalLostChickenCount => Farms != null ? Farms.Sum(c => c.ChickenStatistics != null ? c.ChickenStatistics.TotalLossCount : 0) : 0;
        public bool HasAlarm => Farms != null && Farms.Any(f => f.HasAlarm);
        public bool IsEnabled { get; set; }
    }
}