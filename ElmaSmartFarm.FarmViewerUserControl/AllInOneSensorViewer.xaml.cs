using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System.Windows;
using System.Windows.Controls;

namespace ElmaSmartFarm.UserControls;
/// <summary>
/// Interaction logic for AllInOneSensorViewer.xaml
/// </summary>
public partial class AllInOneSensorViewer : UserControl
{
    public AllInOneSensorViewer()
    {
        InitializeComponent();
    }

    public ScalarSensorModel ScalarSensor
    {
        get { return (ScalarSensorModel)GetValue(ScalarSensorProperty); }
        set { SetValue(ScalarSensorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ScalarSensorProperty =
        DependencyProperty.Register("ScalarSensor", typeof(ScalarSensorModel), typeof(AllInOneSensorViewer), new PropertyMetadata(null));


}