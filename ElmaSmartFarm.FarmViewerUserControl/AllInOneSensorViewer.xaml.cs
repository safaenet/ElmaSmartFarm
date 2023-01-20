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

    public ScalarSensorModel ScalarSensor
    {
        get { return (ScalarSensorModel)GetValue(ScalarSensorProperty); }
        set { SetValue(ScalarSensorProperty, value); }
    }

    public static readonly DependencyProperty ScalarSensorProperty =
        DependencyProperty.Register(nameof(ScalarSensor), typeof(ScalarSensorModel), typeof(AllInOneSensorViewer), new PropertyMetadata(null, (s, e) => { if (s is AllInOneSensorViewer c) { c.OnStatusChanged(); } }));

    protected virtual void OnStatusChanged()
    {
        // Grab related data.
        // Raises INotifyPropertyChanged.PropertyChanged
        Temperature = new Random().NextDouble()*10;
        //OnPropertyChanged("Temperature");
    }

    private double? temperature = 20;
    public double? Temperature
    {
        get {
            return temperature; 
        }
        set { temperature = value; OnPropertyChanged(); }
    }
}