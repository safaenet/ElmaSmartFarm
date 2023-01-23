using Caliburn.Micro;
using ElmaSmartFarm.ApiClient.DataAccess;
using System;
using System.Threading.Tasks;

namespace ElmaSmartFarm.ClientWpf.ViewModels;

public class LivePoultryViewerViewModel : ViewAware
{
    public LivePoultryViewerViewModel(int index)
    {
        poultryManager = new(index);
        poultryManager.OnDataChanged += PoultryManager_OnDataChanged;
        _ = ConnectAsync().ConfigureAwait(true);
    }

    private void PoultryManager_OnDataChanged(object sender, EventArgs e)
    {
        RefreshBindings();
    }

    private PoultryManager _poultryManager;
    public PoultryManager poultryManager
    {
        get => _poultryManager;
        set { _poultryManager = value; NotifyOfPropertyChange(() => poultryManager); }
    }

    public async Task ConnectAsync()
    {
        if (poultryManager != null && !poultryManager.IsRunning) await poultryManager.ConnectAsync();
        await poultryManager.RequestPoultryOverHttp();
        RefreshBindings();
    }

    public async Task DisconnectAsync()
    {
        await poultryManager.DisconnectAsync();
    }

    private void RefreshBindings()
    {
        NotifyOfPropertyChange(() => poultryManager);
        //NotifyOfPropertyChange(() => PoultryManager.Poultry);
        //NotifyOfPropertyChange(() => PoultryManager.Poultry.Farms);
    }

    public async Task OpenFarmAsync()
    {
        LiveFarmViewerViewModel liveFarmViewModel = new(poultryManager, poultryManager.Poultry.Farms[0].Id);
        WindowManager wm = new();
        await wm.ShowWindowAsync(liveFarmViewModel);
    }
}