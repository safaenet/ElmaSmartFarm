using Caliburn.Micro;
using ElmaSmartFarm.ApiClient.DataAccess;
using ElmaSmartFarm.SharedLibrary.Models;
using System;
using System.Linq;

namespace ElmaSmartFarm.ClientWpf.ViewModels;
public class LiveFarmViewerViewModel : ViewAware
{
    public LiveFarmViewerViewModel(PoultryManager _poultryManager, int farmId)
    {
        poultryManager = _poultryManager;
        Farm = poultryManager?.Poultry?.Farms?.Where(f => f.Id == farmId).FirstOrDefault();
        poultryManager.OnDataChanged += PoultryManager_OnDataChanged;
    }
    public LiveFarmViewerViewModel(PoultryManager _poultryManager, FarmModel _farm)
    {
        poultryManager = _poultryManager;
        Farm = _farm;
        poultryManager.OnDataChanged += PoultryManager_OnDataChanged;
    }

    private FarmModel farm;
    public FarmModel Farm
    {
        get { return farm; }
        set { farm = value; NotifyOfPropertyChange(() => Farm); }
    }

    private readonly PoultryManager poultryManager;

    private void PoultryManager_OnDataChanged(object sender, EventArgs e)
    {
        RefreshBindings();
    }

    private void RefreshBindings()
    {
        NotifyOfPropertyChange(() => Farm);
    }

    public void FormClosed()
    {
        poultryManager.OnDataChanged -= PoultryManager_OnDataChanged;
    }
}