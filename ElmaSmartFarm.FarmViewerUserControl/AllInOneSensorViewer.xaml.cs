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
        DependencyProperty.Register(nameof(ScalarSensor), typeof(ScalarSensorModel), typeof(AllInOneSensorViewer), new PropertyMetadata(null));

    public string StatusText
    {
        get { return (string)GetValue(StatusTextProperty); }
        set { SetValue(StatusTextProperty, value); }
    }

    public static readonly DependencyProperty StatusTextProperty =
        DependencyProperty.Register(nameof(StatusText), typeof(string), typeof(AllInOneSensorViewer), new PropertyMetadata("SafaSeed", OnStatusChangedCallBack));

    private static void OnStatusChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is AllInOneSensorViewer c)
        {
            c.OnStatusChanged();
        }
    }

    protected virtual void OnStatusChanged()
    {
        // Grab related data.
        // Raises INotifyPropertyChanged.PropertyChanged
        Temperature = new Random().NextDouble();
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