using Caliburn.Micro;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.ClientWpf.ViewModels;

public class ScalarSensorPresenterViewModel : Screen
{
    public ScalarSensorPresenterViewModel(ScalarSensorModel _scalarSensor)
    {
        ScalarSensor = _scalarSensor;
    }

    private ScalarSensorModel scalarSensor;

    public ScalarSensorModel ScalarSensor
    {
        get { return scalarSensor; }
        set { scalarSensor = value; NotifyOfPropertyChange(() => ScalarSensor); }
    }

    public void RefreshView()
    {
        NotifyOfPropertyChange(() => ScalarSensor);
    }
}