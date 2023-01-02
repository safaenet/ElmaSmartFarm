using Caliburn.Micro;
using ElmaSmartFarm.ApiClient.Config;
using System.Windows;

namespace ElmaSmartFarm.ClientWpf.ViewModels;

public class MainWindowViewModel : ViewAware
{
    public MainWindowViewModel()
    {
        var x = Config.GetPoultriesCount();
        MessageBox.Show(x.ToString());
    }
}