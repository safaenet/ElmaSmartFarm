using Caliburn.Micro;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;

namespace ElmaSmartFarm.ClientWpf.ViewModels;

public class ScalarSensorPresenterViewModel : Screen
{
    public ScalarSensorPresenterViewModel()
    {
        //ScalarSensor = _scalarSensor;
        
        //Status = "Safa";
    }

    //private ScalarSensorModel scalarSensor;

    //public ScalarSensorModel ScalarSensor
    //{
    //    get { return scalarSensor; }
    //    set { scalarSensor = value; NotifyOfPropertyChange(() => ScalarSensor); }
    //}

    private string statuss;

    public string Statuss
    {
        get { return statuss; }
        set { statuss = value; NotifyOfPropertyChange(() => Statuss); }
    }
}