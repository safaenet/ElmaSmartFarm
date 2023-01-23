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
        this.farmId = farmId;
        Farm = poultryManager?.Poultry?.Farms?.Where(f => f.Id == farmId).FirstOrDefault();
        poultryManager.OnDataChanged += PoultryManager_OnDataChanged;
    }

    private FarmModel farm;
    public FarmModel Farm
    {
        get { return farm; }
        set { farm = value; NotifyOfPropertyChange(() => Farm); }
    }

    private readonly PoultryManager poultryManager;
    private readonly int farmId;

    private void PoultryManager_OnDataChanged(object sender, EventArgs e)
    {
        Farm = poultryManager?.Poultry?.Farms?.Where(f => f.Id == farmId).FirstOrDefault();
        RefreshBindings();
    }

    private void RefreshBindings()
    {
        //NotifyOfPropertyChange(() => Farm);
        //NotifyOfPropertyChange(() => Farm.Scalars);
        //NotifyOfPropertyChange(() => Farm.Scalars.Sensors);
    }

    public void FormClosed()
    {
        poultryManager.OnDataChanged -= PoultryManager_OnDataChanged;
    }

    public bool ScalarSensorLeftEnabled => Farm?.Scalars?.ScalarSensorLeft?.IsEnabled ?? false;
    public bool ScalarSensorMiddleEnabled => Farm?.Scalars?.ScalarSensorMiddle?.IsEnabled ?? false;
    public bool ScalarSensorRightEnabled => Farm?.Scalars?.ScalarSensorRight?.IsEnabled ?? false;
    public string WindowTitleText => $"نمایش سالن شماره {Farm.FarmNumber}";
}