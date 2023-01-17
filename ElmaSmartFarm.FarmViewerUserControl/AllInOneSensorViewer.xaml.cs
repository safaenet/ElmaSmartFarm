using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using ElmaSmartFarm.UserControls.Models;
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
        this.DataContext = this;
        InitializeComponent();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged(string prop)
    {
        PropertyChangedEventHandler handler = PropertyChanged;

        if (handler != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public ScalarSensorModel ScalarSensor
    {
        get { return (ScalarSensorModel)GetValue(ScalarSensorProperty); }
        set { SetValue(ScalarSensorProperty, value); OnPropertyChanged("ScalarSensor"); }
    }

    // Using a DependencyProperty as the backing store for Scalar.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ScalarSensorProperty =
        DependencyProperty.Register("ScalarSensor", typeof(ScalarSensorModel), typeof(AllInOneSensorViewer), new PropertyMetadata(null));

    //private ScalarSensorModel scalarSensor;

    //public ScalarSensorModel ScalarSensor
    //{
    //    get { return scalarSensor; }
    //    set { scalarSensor = value; OnPropertyChanged("ScalarSensor"); }
    //}


    public double? Temp
    {
        get { return ScalarSensor?.LastRead?.Temperature; }
        set { ScalarSensor.LastRead.Temperature = value; OnPropertyChanged("Temp"); }
    }

}