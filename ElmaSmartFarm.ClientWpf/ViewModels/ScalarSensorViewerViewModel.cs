using Caliburn.Micro;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.ClientWpf.ViewModels;

public class ScalarSensorViewerViewModel : Screen
{
    public ScalarSensorViewerViewModel()
    {

    }

    private ScalarSensorModel scalarSensor;

    public ScalarSensorModel ScalarSensor
    {
        get => scalarSensor;
        set { scalarSensor = value; NotifyOfPropertyChange(() => ScalarSensor); }
    }

    public void RefreshView()
    {
        NotifyOfPropertyChange(() => ScalarSensor);
    }
}