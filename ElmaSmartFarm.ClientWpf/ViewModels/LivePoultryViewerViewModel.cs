using Caliburn.Micro;
using ElmaSmartFarm.ApiClient.DataAccess;
using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ElmaSmartFarm.ClientWpf.ViewModels;

public class LivePoultryViewerViewModel : ViewAware
{
    public LivePoultryViewerViewModel(int index)
    {
        PoultryManager = new(index);
        PoultryManager.OnDataChanged += PoultryManager_OnDataChanged;
        _ = ConnectAsync().ConfigureAwait(true);
    }

    private void PoultryManager_OnDataChanged(object sender, EventArgs e)
    {
        RefreshBindings();
    }

    private PoultryManager poultryManager;
    public PoultryManager PoultryManager
    {
        get => poultryManager;
        set { poultryManager = value; NotifyOfPropertyChange(() => PoultryManager); }
    }

    public async Task ConnectAsync()
    {
        if (PoultryManager != null && !PoultryManager.IsRunning) await PoultryManager.ConnectAsync();
        await PoultryManager.RequestPoultryOverHttp();
        RefreshBindings();
    }

    public async Task DisconnectAsync()
    {
        await PoultryManager.DisconnectAsync();
    }

    private void RefreshBindings()
    {
        NotifyOfPropertyChange(() => PoultryManager);
        //NotifyOfPropertyChange(() => PoultryManager.Poultry);
        //NotifyOfPropertyChange(() => PoultryManager.Poultry.Farms);
    }

    public async Task OpenFarmAsync()
    {
        LiveFarmViewerViewModel liveFarmViewModel = new(PoultryManager, PoultryManager.Poultry.Farms.FirstOrDefault().Id);
        WindowManager wm = new();
        await wm.ShowWindowAsync(liveFarmViewModel);
    }
}