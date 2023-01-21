using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using ElmaSmartFarm.SharedLibrary.Models.UISettings;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ElmaSmartFarm.UserControls;

public partial class FarmAverageViewer : UserControl, INotifyPropertyChanged
{
    public FarmAverageViewer()
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

    public ScalarSensorModel Sensor
    {
        get { return (ScalarSensorModel)GetValue(SensorProperty); }
        set { SetValue(SensorProperty, value); }
    }

    public static readonly DependencyProperty SensorProperty = DependencyProperty.Register(nameof(Sensor), typeof(ScalarSensorModel), typeof(FarmAverageViewer), new PropertyMetadata(null, (s, e) => { if (s is FarmAverageViewer c) c.OnSensorChanged(); }));

    public ScalarSensorSettings Settings
    {
        get { return (ScalarSensorSettings)GetValue(SettingsProperty); }
        set { SetValue(SettingsProperty, value); }
    }

    public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register(nameof(Settings), typeof(ScalarSensorSettings), typeof(FarmAverageViewer), new PropertyMetadata(new ScalarSensorSettings(), (s, e) => { if (s is FarmAverageViewer c) c.OnSensorChanged(); }));

    protected virtual void OnSensorChanged()
    {
        RefreshColors();
        if (Sensor == null) Status = "عدم اتصال";
        else
        {
            if (Sensor.HasError) Status = "خطا وجود دارد";
            else Status = "وضعیت پایدار";
        }
    }

    private void RefreshColors()
    {
        if (Sensor == null || Sensor.LastRead == null)
        {
            TemperatureColor = Settings.NormalValueColor.ToSolidBrush();
            HumidityColor = Settings.NormalValueColor.ToSolidBrush();
            LightColor = Settings.NormalValueColor.ToSolidBrush();
            AmmoniaColor = Settings.NormalValueColor.ToSolidBrush();
            Co2Color = Settings.NormalValueColor.ToSolidBrush();
        }
        else
        {
            if (Sensor.LastRead.Temperature <= Settings.LowTemperatureThreshold) TemperatureColor = Settings.LowTemperatureColor.ToSolidBrush();
            else if (Sensor.LastRead.Temperature >= Settings.HighTemperatureThreshold) TemperatureColor = Settings.HighTemperatureColor.ToSolidBrush();
            else TemperatureColor = Settings.NormalValueColor.ToSolidBrush();

            if (Sensor.LastRead.Humidity <= Settings.LowHumidityThreshold) HumidityColor = Settings.LowHumidityColor.ToSolidBrush();
            else if (Sensor.LastRead.Humidity >= Settings.HighHumidityThreshold) HumidityColor = Settings.HighHumidityColor.ToSolidBrush();
            else HumidityColor = Settings.NormalValueColor.ToSolidBrush();

            if (Sensor.LastRead.Light <= Settings.LightThreshold) LightColor = Settings.DarkLightColor.ToSolidBrush();
            else LightColor = Settings.BrightLightColor.ToSolidBrush();

            if (Sensor == null || Sensor.LastRead == null) AmmoniaColor = Settings.NormalValueColor.ToSolidBrush();
            else if (Sensor.LastRead.Ammonia >= Settings.HighTemperatureThreshold) AmmoniaColor = Settings.HighAmmoniaColor.ToSolidBrush();
            else AmmoniaColor = Settings.NormalValueColor.ToSolidBrush();

            if (Sensor.LastRead.Co2 >= Settings.HighCo2Threshold) Co2Color = Settings.HighCo2Color.ToSolidBrush();
            else Co2Color = Settings.NormalValueColor.ToSolidBrush();

            if (Sensor.HasError)
            {
                StatusTextColor = Settings.StatusTextErrorColor.ToSolidBrush();
                StatusIconColor = Settings.StatusIconErrorColor.ToSolidBrush();
            }
            else
            {
                StatusTextColor = Settings.StatusTextOkColor.ToSolidBrush();
                StatusIconColor = Settings.StatusIconOkColor.ToSolidBrush();
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

    private string status = "Ok";
    public string Status
    {
        get { return status; }
        set { status = value; OnPropertyChanged(); }
    }
}