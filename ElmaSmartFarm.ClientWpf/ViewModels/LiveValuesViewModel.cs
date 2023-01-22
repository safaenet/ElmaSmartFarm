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

public class LiveValuesViewModel : ViewAware
{
    public LiveValuesViewModel(int index)
    {
        PoultryManager = new(index);
        PoultryManager.OnDataChanged += PoultryManager_OnDataChanged;
    }

    private void PoultryManager_OnDataChanged(object sender, EventArgs e)
    {
        NotifyOfPropertyChange(() => PoultryManager);
        RefreshBindings();
        //if (PoultryManager?.Poultry?.Farms[0]?.Scalars?.Sensors[0] != null)
        //ScalarSensorPresenterViewModel.ScalarSensor = PoultryManager.Poultry.Farms[0].Scalars.Sensors[0];
        //ScalarSensorPresenterViewModel.RefreshView();
        //MessageBox.Show(PoultryManager.Poultry.Farms[0].Scalars.Sensors[0].LastRead.Temperature.ToString());
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
        //if (PoultryManager.Poultry.Farms[0].Scalars.Sensors[0].Values == null) PoultryManager.Poultry.Farms[0].Scalars.Sensors[0].Values = new();
        ////PoultryManager.Poultry.Farms[0].Scalars.Sensors[0].Values.Add(new() { Temperature = new Random().NextDouble(), ReadDate = DateTime.Now });
        //ScalarSensor2.Values.Add(new() { Temperature = 80, Humidity = new Random().Next(), ReadDate = DateTime.Now });
        //NotifyOfPropertyChange(() => ScalarSensor2);
        RefreshBindings();
    }

    public async Task DisconnectAsync()
    {
        await PoultryManager.DisconnectAsync();
    }

    private void RefreshBindings()
    {
        NotifyOfPropertyChange(() => PoultryManager);
        NotifyOfPropertyChange(() => PoultryManager.Poultry);
        NotifyOfPropertyChange(() => PoultryManager.Poultry.Farms);
    }
}