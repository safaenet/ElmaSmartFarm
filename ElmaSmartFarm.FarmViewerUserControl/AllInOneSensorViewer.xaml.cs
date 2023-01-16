using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using ElmaSmartFarm.UserControls.Models;
using System.ComponentModel;
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
        DataContext = MyDataContext;
    }

    public ScalarSensorModel ScalarSensor
    {
        get { return MyDataContext.Scalar; }
        set { MyDataContext.Scalar = value; }
    }

    public static readonly DependencyProperty ScalarSensorProperty =
        DependencyProperty.Register("ScalarSensor", typeof(ScalarSensorModel), typeof(AllInOneSensorViewer), new PropertyMetadata(null));

    //public string Status
    //{
    //    get { return (string)GetValue(StatusProperty); }
    //    set { SetValue(StatusProperty, value); }
    //}

    //public static readonly DependencyProperty StatusProperty =
    //    DependencyProperty.Register("Status", typeof(string), typeof(AllInOneSensorViewer), new PropertyMetadata("Safa"));

    private AllInOneSensorDataContext myDataContext;

    public AllInOneSensorDataContext MyDataContext
    {
        get { return myDataContext; }
        set { myDataContext = value; }
    }
}