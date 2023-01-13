using Caliburn.Micro;
using ElmaSmartFarm.ApiClient.DataAccess;
using System.Threading.Tasks;

namespace ElmaSmartFarm.ClientWpf.ViewModels;

public class MainWindowViewModel : ViewAware
{
    public MainWindowViewModel()
    {

    }

    public async Task ViewLiveValuesAsync()
    {
        LiveValuesViewModel liveValuesViewModel = new(0);
        WindowManager wm = new();
        await wm.ShowWindowAsync(liveValuesViewModel);
    }
}