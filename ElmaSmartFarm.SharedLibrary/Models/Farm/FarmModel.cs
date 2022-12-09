namespace ElmaSmartFarm.SharedLibrary.Models;

public class FarmModel
{
    public int Id { get; set; }
    public int PoultryId { get; set; }
    public string Name { get; set; }
    public int FarmNumber { get; set; }
    public int MaxCapacity { get; set; }
    public ScalarSensorSetModel Scalars { get; set; } = new();
    public CommuteSensorSetModel Commutes { get; set; } = new();
    public PushButtonSensorSetModel Checkups { get; set; } = new();
    public PushButtonSensorSetModel Feeds { get; set; } = new();
    public BinarySensorSetModel ElectricPowers { get; set; } = new();
    public PeriodModel Period { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsInPeriod => IsEnabled && Period != null && Period.EndDate != null;
    public List<FarmInPeriodErrorModel> InPeriodErrors { get; set; }
    public bool HasSensorError => IsEnabled && ((Scalars != null && Scalars.HasError) || (Commutes != null && Commutes.HasError) || (Checkups != null && Checkups.HasError) || (Feeds != null && Feeds.HasError) || (ElectricPowers != null && ElectricPowers.HasError));
    public string Descriptions { get; set; }
}