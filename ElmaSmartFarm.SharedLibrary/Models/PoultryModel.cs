using ElmaSmartFarm.SharedLibrary.Models.Alarm;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.SharedLibrary.Models;

public class PoultryModel
{
    public string Name { get; set; }
    public List<FarmModel> Farms { get; set; }
    public ScalarSensorModel Scalar { get; set; }
    public BinarySensorModel MainElectricPower { get; set; }
    public BinarySensorModel BackupElectricPower { get; set; }
    public int TotalPrimaryChickenCount => Farms != null ? Farms.Sum(c => c.Period != null && c.Period.ChickenStatistics != null ? c.Period.ChickenStatistics.ChickenPrimaryCount : 0) : 0;
    public int TotalAliveChickenCount => Farms != null ? Farms.Sum(c => c.Period != null && c.Period.ChickenStatistics != null ? c.Period.ChickenStatistics.TotalAliveCount : 0) : 0;
    public int TotalLostChickenCount => Farms != null ? Farms.Sum(c => c.Period != null && c.Period.ChickenStatistics != null ? c.Period.ChickenStatistics.TotalLossCount : 0) : 0;
    public bool IsInPeriod => Farms != null && Farms.Any(f => f.Period != null && f.Period.EndDate != null);
    public List<PoultryInPeriodErrorModel> InPeriodErrors { get; set; }
    public bool HasPeriodError => IsInPeriod && InPeriodErrors != null && InPeriodErrors.Any(e => e.DateErased == null);
    public List<AlarmModel> AlarmDevices { get; set; }
}