using ElmaSmartFarm.SharedLibrary.Models.Sensors;
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
        DataContext = this;
    }

    //public string? Status => ScalarSensor.HasError ? "Has Error" : "No Error";

    public ScalarSensorModel ScalarSensor
    {
        get { return (ScalarSensorModel)GetValue(ScalarSensorProperty); }
        set { SetValue(ScalarSensorProperty, value); OnPropertyChanged(nameof(ScalarSensor)); }
    }

    // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ScalarSensorProperty =
        DependencyProperty.Register("ScalarSensor", typeof(ScalarSensorModel), typeof(AllInOneSensorViewer), new PropertyMetadata(null));

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}