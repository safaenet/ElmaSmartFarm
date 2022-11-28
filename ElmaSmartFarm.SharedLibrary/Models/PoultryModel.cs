using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class PoultryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<FarmModel> Farms { get; set; }
        public TemperatureSensorModel OutdoorTemperature { get; set; }
        public HumiditySensorModel OutdoorHumidity { get; set; }
        public BinarySensorModel MainElectricPower { get; set; }
        public BinarySensorModel BackupElectricPower { get; set; }
        public int TotalPrimaryChickenCount => Farms != null ? Farms.Sum(c => c.Period != null && c.Period.ChickenStatistics != null ? c.Period.ChickenStatistics.ChickenPrimaryCount : 0) : 0;
        public int TotalAliveChickenCount => Farms != null ? Farms.Sum(c => c.Period != null && c.Period.ChickenStatistics != null ? c.Period.ChickenStatistics.TotalAliveCount : 0) : 0;
        public int TotalLostChickenCount => Farms != null ? Farms.Sum(c => c.Period != null && c.Period.ChickenStatistics != null ? c.Period.ChickenStatistics.TotalLossCount : 0) : 0;
        public bool IsInPeriod => Farms != null && Farms.Any(f => f.Period != null && f.Period.EndDate != null);
        public IEnumerable<PoultryInPeriodErrorModel> InPeriodErrors { get; set; }
        public bool IsEnabled { get; set; }
    }
}