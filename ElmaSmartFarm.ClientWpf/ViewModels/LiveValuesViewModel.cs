using Caliburn.Micro;
using ElmaSmartFarm.ApiClient.DataAccess;
using System.Threading.Tasks;

namespace ElmaSmartFarm.ClientWpf.ViewModels;

public class LiveValuesViewModel : ViewAware
{
    public LiveValuesViewModel(int index)
    {
        PoultryManager = new(index);
    }

    private PoultryManager poultryManager;

    public PoultryManager PoultryManager
    {
        get => poultryManager;
        set { poultryManager = value; NotifyOfPropertyChange(() => PoultryManager); }
    }

    public async Task ConnectAsync()
    {
        await PoultryManager.ConnectAsync();
    }

    public async Task DisconnectAsync()
    {
        await PoultryManager.DisconnectAsync();
    }
}