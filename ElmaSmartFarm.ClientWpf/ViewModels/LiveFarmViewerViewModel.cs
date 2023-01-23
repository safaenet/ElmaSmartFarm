using Caliburn.Micro;
using ElmaSmartFarm.ApiClient.DataAccess;
using ElmaSmartFarm.SharedLibrary.Models;
using System;
using System.Linq;

namespace ElmaSmartFarm.ClientWpf.ViewModels;
public class LiveFarmViewerViewModel : ViewAware
{
    public LiveFarmViewerViewModel(PoultryManager _poultryManager, int farmId)
    {
        poultryManager = _poultryManager;
        this.farmId = farmId;
        Farm = poultryManager?.Poultry?.Farms?.Where(f => f.Id == farmId).FirstOrDefault();
        poultryManager.OnDataChanged += PoultryManager_OnDataChanged;
    }

    private FarmModel farm;
    public FarmModel Farm
    {
        get { return farm; }
        set { farm = value; NotifyOfPropertyChange(() => Farm); }
    }

    private readonly PoultryManager poultryManager;
    private readonly int farmId;

    private void PoultryManager_OnDataChanged(object sender, EventArgs e)
    {
        Farm = poultryManager?.Poultry?.Farms?.Where(f => f.Id == farmId).FirstOrDefault();
        RefreshBindings();
    }

    private void RefreshBindings()
    {
        //NotifyOfPropertyChange(() => Farm);
        //NotifyOfPropertyChange(() => Farm.Scalars);
        //NotifyOfPropertyChange(() => Farm.Scalars.Sensors);
    }

    public void FormClosed()
    {
        poultryManager.OnDataChanged -= PoultryManager_OnDataChanged;
    }

    public bool ScalarSensorLeftEnabled => Farm?.Scalars?.ScalarSensorLeft?.IsEnabled ?? false;
    public bool ScalarSensorMiddleEnabled => Farm?.Scalars?.ScalarSensorMiddle?.IsEnabled ?? false;
    public bool ScalarSensorRightEnabled => Farm?.Scalars?.ScalarSensorRight?.IsEnabled ?? false;
    public string WindowTitleText => $"نمایش سالن شماره {Farm.FarmNumber}";

    public int TemperatureSensorCount => Farm?.Scalars?.ActiveSensors?.Count(s => s.HasTemperature) ?? 0;
    public int HumiditySensorCount => Farm?.Scalars?.ActiveSensors?.Count(s => s.HasHumidity) ?? 0;
    public int LightSensorCount => Farm?.Scalars?.ActiveSensors?.Count(s => s.HasLight) ?? 0;
    public int AmmoniaSensorCount => Farm?.Scalars?.ActiveSensors?.Count(s => s.HasAmmonia) ?? 0;
    public int Co2SensorCount => Farm?.Scalars?.ActiveSensors?.Count(s => s.HasCo2) ?? 0;
    public int ScalarSensorCount => Farm?.Scalars?.ActiveSensors?.Count() ?? 0;
    public int CommuteSensorCount => Farm?.Commutes?.ActiveSensors?.Count() ?? 0;
    public int CheckupSensorCount => Farm?.Checkups?.ActiveSensors?.Count() ?? 0;
    public int FeedSensorCount => Farm?.Feeds?.ActiveSensors?.Count() ?? 0;
    public int ElectricPowerSensorCount => Farm?.ElectricPowers?.ActiveSensors?.Count() ?? 0;

    public bool HasTemperatureSensor => TemperatureSensorCount > 0;
    public bool HasHumiditySensor => HumiditySensorCount > 0;
    public bool HasLightSensor => LightSensorCount > 0;
    public bool HasAmmoniaSensor => AmmoniaSensorCount > 0;
    public bool HasCo2Sensor => Co2SensorCount > 0;
    public bool HasScalarSensor => ScalarSensorCount > 0;
    public bool HasCommuteSensor => CommuteSensorCount > 0;
    public bool HasCheckupSensor => CheckupSensorCount > 0;
    public bool HasFeedSensor => FeedSensorCount > 0;
    public bool HasElectricPowerSensor => ElectricPowerSensorCount > 0;

    public string TemperatureSensorText => $"{(HasTemperatureSensor ? "دارد" : "ندارد")}{(HasTemperatureSensor ? $" - {TemperatureSensorCount}" : "")}";
    public string HumiditySensorText => $"{(HasHumiditySensor ? "دارد" : "ندارد")}{(HasHumiditySensor ? $" - {HumiditySensorCount}" : "")}";
    public string LightSensorText => $"{(HasLightSensor ? "دارد" : "ندارد")}{(HasLightSensor ? $" - {LightSensorCount}" : "")}";
    public string AmmoniaSensorText => $"{(HasAmmoniaSensor ? "دارد" : "ندارد")}{(HasAmmoniaSensor ? $" - {AmmoniaSensorCount}" : "")}";
    public string Co2SensorText => $"{(HasCo2Sensor ? "دارد" : "ندارد")}{(HasCo2Sensor ? $" - {Co2SensorCount}" : "")}";
    public string ScalarSensorText => $"{(HasScalarSensor ? "دارد" : "ندارد")}{(HasScalarSensor ? $" - {ScalarSensorCount}" : "")}";
    public string CommuteSensorText => $"{(HasCommuteSensor ? "دارد" : "ندارد")}{(HasCommuteSensor ? $" - {CommuteSensorCount}" : "")}";
    public string CheckupSensorText => $"{(HasCheckupSensor ? "دارد" : "ندارد")}{(HasCheckupSensor ? $" - {CheckupSensorCount}" : "")}";
    public string FeedSensorText => $"{(HasFeedSensor ? "دارد" : "ندارد")}{(HasFeedSensor ? $" - {FeedSensorCount}" : "")}";
    public string ElectricPowerSensorText => $"{(HasElectricPowerSensor ? "دارد" : "ندارد")}{(HasElectricPowerSensor ? $" - {ElectricPowerSensorCount}" : "")}";
}