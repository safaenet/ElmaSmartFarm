using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ElmaSmartFarm.UserControls;
/// <summary>
/// Interaction logic for AllInOneSensorViewer.xaml
/// </summary>
public partial class AllInOneSensorViewer : UserControl, INotifyPropertyChanged
{
    public AllInOneSensorViewer()
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string prop = "")
    {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public ScalarSensorModel Sensor
    {
        get { return (ScalarSensorModel)GetValue(SensorProperty); }
        set { SetValue(SensorProperty, value); }
    }

    public static readonly DependencyProperty SensorProperty =
        DependencyProperty.Register(nameof(Sensor), typeof(ScalarSensorModel), typeof(AllInOneSensorViewer), new PropertyMetadata(null, (s, e) => { if (s is AllInOneSensorViewer c) c.OnSensorChanged(); }));

    protected virtual void OnSensorChanged()
    {
        Temperature = new Random().NextDouble();
    }

    private double? temperature = 20;
    public double? Temperature
    {
        get {
            return temperature; 
        }
        set { temperature = value; OnPropertyChanged(); }
    }

    public double? Temperature2
    {
        get { return Sensor.LastRead.Temperature; }
        set { Sensor.LastRead.Temperature = value; OnPropertyChanged(); }
    }
}