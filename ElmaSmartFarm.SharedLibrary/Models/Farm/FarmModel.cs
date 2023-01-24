using System.Collections.Generic;
using System.Linq;

namespace ElmaSmartFarm.SharedLibrary.Models;

public class FarmModel
{
    public int Id { get; set; }
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
    public bool IsInPeriod => IsEnabled && Period != null && Period.EndDate is not null;
    public List<FarmInPeriodErrorModel> InPeriodErrors { get; set; } = new();
    public bool HasSensorError => IsEnabled && ((Scalars != null && Scalars.HasError) || (Commutes != null && Commutes.HasError) || (Checkups != null && Checkups.HasError) || (Feeds != null && Feeds.HasError) || (ElectricPowers != null && ElectricPowers.HasError));
    public bool HasPeriodError => IsInPeriod && InPeriodErrors != null && InPeriodErrors.Any(e => e.DateErased is null);
    public string Descriptions { get; set; }

    #region Other readonly properties:
    public int ScalarSensorCount => Scalars?.ActiveSensors?.Count() ?? 0;
    public int TemperatureSensorCount => Scalars?.ActiveSensors?.Count(s => s.HasTemperature) ?? 0;
    public int HumiditySensorCount => Scalars?.ActiveSensors?.Count(s => s.HasHumidity) ?? 0;
    public int LightSensorCount => Scalars?.ActiveSensors?.Count(s => s.HasLight) ?? 0;
    public int AmmoniaSensorCount => Scalars?.ActiveSensors?.Count(s => s.HasAmmonia) ?? 0;
    public int Co2SensorCount => Scalars?.ActiveSensors?.Count(s => s.HasCo2) ?? 0;
    public int CommuteSensorCount => Commutes?.ActiveSensors?.Count() ?? 0;
    public int CheckupSensorCount => Commutes?.ActiveSensors?.Count() ?? 0;
    public int FeedSensorCount => Commutes?.ActiveSensors?.Count() ?? 0;
    public int ElectricPowerSensorCount => Commutes?.ActiveSensors?.Count() ?? 0;
    public bool HasScalarSensor => ScalarSensorCount > 0;
    public bool HasTemperatureSensor => TemperatureSensorCount > 0;
    public bool HasHumiditySensor => HumiditySensorCount > 0;
    public bool HasLightSensor => LightSensorCount > 0;
    public bool HasAmmoniaSensor => AmmoniaSensorCount > 0;
    public bool HasCo2Sensor => Co2SensorCount > 0;
    public bool HasCommuteSensor => CommuteSensorCount > 0;
    public bool HasCheckupSensor => CheckupSensorCount > 0;
    public bool HasFeedSensor => FeedSensorCount > 0;
    public bool HasElectricPowerSensor => ElectricPowerSensorCount > 0;
    #endregion
}