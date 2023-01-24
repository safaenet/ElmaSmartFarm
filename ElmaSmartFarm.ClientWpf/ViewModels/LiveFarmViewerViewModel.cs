using Caliburn.Micro;
using ElmaSmartFarm.ApiClient.DataAccess;
using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.UISettings;
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
        if (Settings == null) Settings = new();
        if (Settings.ScalarSettings == null) Settings.ScalarSettings = new();
    }

    private FarmModel farm;
    public FarmModel Farm
    {
        get { return farm; }
        set { farm = value; NotifyOfPropertyChange(() => Farm); }
    }

    private FarmSettings settings;
    public FarmSettings Settings
    {
        get { return settings; }
        set { settings = value; NotifyOfPropertyChange(() => Settings); }
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
        var s = Farm.Commutes.ActiveSensors.SelectMany(s => s.Values).Where(r => r.ReadDate == DateTime.Now);
    }

    public string WindowTitleText => $"سالن شماره {Farm.FarmNumber}";

    public string TemperatureSensorText => $"{(Farm.HasTemperatureSensor ? "دارد" : "ندارد")}{(Farm.HasTemperatureSensor ? $" - {Farm.TemperatureSensorCount}" : "")}";
    public string HumiditySensorText => $"{(Farm.HasHumiditySensor ? "دارد" : "ندارد")}{(Farm.HasHumiditySensor ? $" - {Farm.HumiditySensorCount}" : "")}";
    public string LightSensorText => $"{(Farm.HasLightSensor ? "دارد" : "ندارد")}{(Farm.HasLightSensor ? $" - {Farm.LightSensorCount}" : "")}";
    public string AmmoniaSensorText => $"{(Farm.HasAmmoniaSensor ? "دارد" : "ندارد")}{(Farm.HasAmmoniaSensor ? $" - {Farm.AmmoniaSensorCount}" : "")}";
    public string Co2SensorText => $"{(Farm.HasCo2Sensor ? "دارد" : "ندارد")}{(Farm.HasCo2Sensor ? $" - {Farm.Co2SensorCount}" : "")}";
    public string ScalarSensorText => $"{(Farm.HasScalarSensor ? "دارد" : "ندارد")}{(Farm.HasScalarSensor ? $" - {Farm.ScalarSensorCount}" : "")}";
    public string CommuteSensorText => $"{(Farm.HasCommuteSensor ? "دارد" : "ندارد")}{(Farm.HasCommuteSensor ? $" - {Farm.CommuteSensorCount}" : "")}";
    public string CheckupSensorText => $"{(Farm.HasCheckupSensor ? "دارد" : "ندارد")}{(Farm.HasCheckupSensor ? $" - {Farm.CheckupSensorCount}" : "")}";
    public string FeedSensorText => $"{(Farm.HasFeedSensor ? "دارد" : "ندارد")}{(Farm.HasFeedSensor ? $" - {Farm.FeedSensorCount}" : "")}";
    public string ElectricPowerSensorText => $"{(Farm.HasElectricPowerSensor ? "دارد" : "ندارد")}{(Farm.HasElectricPowerSensor ? $" - {Farm.ElectricPowerSensorCount}" : "")}";

    public int TodayCommuteStepInCount => Farm.Commutes.Sensors.SelectMany(s => s.Values).Count(r => r.Value == SharedLibrary.CommuteSensorValueType.StepIn && DateOnly.FromDateTime(r.ReadDate) == DateOnly.FromDateTime(DateTime.Now));
    public int TodayCommuteStepOutCount => Farm.Commutes.Sensors.SelectMany(s => s.Values).Count(r => r.Value == SharedLibrary.CommuteSensorValueType.StepOut && DateOnly.FromDateTime(r.ReadDate) == DateOnly.FromDateTime(DateTime.Now));
    public int TodayCheckupCount => Farm.Checkups.Sensors.SelectMany(s => s.Values).Count(r => DateOnly.FromDateTime(r.ReadDate) == DateOnly.FromDateTime(DateTime.Now));
    public int TodayFeedCount => Farm.Feeds.Sensors.SelectMany(s => s.Values).Count(r => DateOnly.FromDateTime(r.ReadDate) == DateOnly.FromDateTime(DateTime.Now));
}