using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System.ComponentModel;

namespace ElmaSmartFarm.UserControls.Models;

public class AllInOneSensorDataContext : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private ScalarSensorModel scalar;

    public ScalarSensorModel Scalar
    {
        get { return scalar; }
        set { scalar = value; OnPropertyChanged(nameof(Scalar)); }
    }
    protected void OnPropertyChanged(string info)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
    }
}