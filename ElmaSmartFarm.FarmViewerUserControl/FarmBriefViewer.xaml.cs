using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.UISettings;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ElmaSmartFarm.UserControls;

public partial class FarmBriefViewer : UserControl, INotifyPropertyChanged
{
    public FarmBriefViewer()
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

    public FarmModel Farm
    {
        get { return (FarmModel)GetValue(FarmProperty); }
        set { SetValue(FarmProperty, value); }
    }

    public static readonly DependencyProperty FarmProperty = DependencyProperty.Register(nameof(Farm), typeof(FarmModel), typeof(FarmBriefViewer), new PropertyMetadata(null, (s, e) => { if (s is FarmBriefViewer c) c.OnDataChanged(); }));

    public FarmSettings Settings
    {
        get { return (FarmSettings)GetValue(SettingsProperty); }
        set { SetValue(SettingsProperty, value); }
    }

    public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register(nameof(Settings), typeof(FarmSettings), typeof(FarmBriefViewer), new PropertyMetadata(new FarmSettings(), (s, e) => { if (s is FarmBriefViewer c) c.OnDataChanged(); }));

    protected virtual void OnDataChanged()
    {
        RefreshColors();
    }

    private void RefreshColors()
    {
        if (Farm == null || Farm.Scalars == null)
        {
            TemperatureColor = Settings.ScalarSettings.NormalValueColor.ToSolidBrush();
            HumidityColor = Settings.ScalarSettings.NormalValueColor.ToSolidBrush();
            LightColor = Settings.ScalarSettings.NormalValueColor.ToSolidBrush();
            AmmoniaColor = Settings.ScalarSettings.NormalValueColor.ToSolidBrush();
            Co2Color = Settings.ScalarSettings.NormalValueColor.ToSolidBrush();
        }
        else
        {
            if (Farm.Scalars.AverageTemperatureValue <= Settings.ScalarSettings.LowTemperatureThreshold) TemperatureColor = Settings.ScalarSettings.LowTemperatureColor.ToSolidBrush();
            else if (Farm.Scalars.AverageTemperatureValue >= Settings.ScalarSettings.HighTemperatureThreshold) TemperatureColor = Settings.ScalarSettings.HighTemperatureColor.ToSolidBrush();
            else TemperatureColor = Settings.ScalarSettings.NormalValueColor.ToSolidBrush();

            if (Farm.Scalars.AverageHumidityValue <= Settings.ScalarSettings.LowHumidityThreshold) HumidityColor = Settings.ScalarSettings.LowHumidityColor.ToSolidBrush();
            else if (Farm.Scalars.AverageHumidityValue >= Settings.ScalarSettings.HighHumidityThreshold) HumidityColor = Settings.ScalarSettings.HighHumidityColor.ToSolidBrush();
            else HumidityColor = Settings.ScalarSettings.NormalValueColor.ToSolidBrush();

            if (Farm.Scalars.AverageLightValue <= Settings.ScalarSettings.LightThreshold) LightColor = Settings.ScalarSettings.DarkLightColor.ToSolidBrush();
            else LightColor = Settings.ScalarSettings.BrightLightColor.ToSolidBrush();

            if (Farm.Scalars.AverageAmmoniaValue >= Settings.ScalarSettings.HighTemperatureThreshold) AmmoniaColor = Settings.ScalarSettings.HighAmmoniaColor.ToSolidBrush();
            else AmmoniaColor = Settings.ScalarSettings.NormalValueColor.ToSolidBrush();

            if (Farm.Scalars.AverageCo2Value >= Settings.ScalarSettings.HighCo2Threshold) Co2Color = Settings.ScalarSettings.HighCo2Color.ToSolidBrush();
            else Co2Color = Settings.ScalarSettings.NormalValueColor.ToSolidBrush();

            if (Farm.HasSensorError)
            {
                StatusTextColor = Settings.ScalarSettings.StatusTextErrorColor.ToSolidBrush();
                StatusIconColor = Settings.ScalarSettings.StatusIconErrorColor.ToSolidBrush();
            }
            else
            {
                StatusTextColor = Settings.ScalarSettings.StatusTextOkColor.ToSolidBrush();
                StatusIconColor = Settings.ScalarSettings.StatusIconOkColor.ToSolidBrush();
            }
        }
    }

    private SolidColorBrush temperatureColor = new(Colors.Black);
    public SolidColorBrush TemperatureColor
    {
        get { return temperatureColor; }
        set { temperatureColor = value; OnPropertyChanged(); }
    }

    private SolidColorBrush humidityColor = new(Colors.Black);
    public SolidColorBrush HumidityColor
    {
        get { return humidityColor; }
        set { humidityColor = value; OnPropertyChanged(); }
    }

    private SolidColorBrush lightColor = new(Colors.Black);
    public SolidColorBrush LightColor
    {
        get { return lightColor; }
        set { lightColor = value; OnPropertyChanged(); }
    }

    private SolidColorBrush ammoniaColor = new(Colors.Black);
    public SolidColorBrush AmmoniaColor
    {
        get { return ammoniaColor; }
        set { ammoniaColor = value; OnPropertyChanged(); }
    }

    private SolidColorBrush co2Color = new(Colors.Black);
    public SolidColorBrush Co2Color
    {
        get { return co2Color; }
        set { co2Color = value; OnPropertyChanged(); }
    }

    private SolidColorBrush statusTextColor = new(Colors.Black);
    public SolidColorBrush StatusTextColor
    {
        get { return statusTextColor; }
        set { statusTextColor = value; OnPropertyChanged(); }
    }

    private SolidColorBrush statusIconColor = new(Colors.Black);
    public SolidColorBrush StatusIconColor
    {
        get { return statusIconColor; }
        set { statusIconColor = value; OnPropertyChanged(); }
    }
    public string ScalarSensorSetStatus => Farm == null || Farm.Scalars == null || Farm.Scalars.HasActiveSensors == false ? "سنسور یافت نشد" : Farm.Scalars.HasError ? Farm.Scalars.ActiveSensors.MaxBy(s => s.LastError.DateHappened).LastError.ErrorType.ToString() : "وضعیت پایدار";

    public string LastCommuteIn => Farm == null || Farm.Commutes == null || Farm.Commutes.HasActiveSensors == false ? "سنسور یافت نشد" : Farm.Commutes.ActiveSensors.Max(s => s.LastStepInDate).ToString();
    public string LastCommuteOut => Farm == null || Farm.Commutes == null || Farm.Commutes.HasActiveSensors == false ? "سنسور یافت نشد" : Farm.Commutes.ActiveSensors.Max(s => s.LastStepOutDate).ToString();
    public string CommuteSensorSetStatus => Farm == null || Farm.Commutes == null || Farm.Commutes.HasActiveSensors == false ? "سنسور یافت نشد" : Farm.Commutes.HasError ? Farm.Commutes.ActiveSensors.MaxBy(s => s.LastError.DateHappened).LastError.ErrorType.ToString() : "وضعیت پایدار";
    
    public string LastCheckup => Farm == null || Farm.Checkups == null || Farm.Checkups.HasActiveSensors == false ? "سنسور یافت نشد" : Farm.Checkups.ActiveSensors.Max(s => s.LastRead.ReadDate).ToString();
    public string CheckupSensorSetStatus => Farm == null || Farm.Checkups == null || Farm.Checkups.HasActiveSensors == false ? "سنسور یافت نشد" : Farm.Checkups.HasError ? Farm.Checkups.ActiveSensors.MaxBy(s => s.LastError.DateHappened).LastError.ErrorType.ToString() : "وضعیت پایدار";

    public string LastFeed => Farm == null || Farm.Feeds == null || Farm.Feeds.HasActiveSensors == false ? "سنسور یافت نشد" : Farm.Feeds.ActiveSensors.Max(s => s.LastRead.ReadDate).ToString();
    public string FeedSensorSetStatus => Farm == null || Farm.Feeds == null || Farm.Feeds.HasActiveSensors == false ? "سنسور یافت نشد" : Farm.Feeds.HasError ? Farm.Feeds.ActiveSensors.MaxBy(s => s.LastError.DateHappened).LastError.ErrorType.ToString() : "وضعیت پایدار";

}