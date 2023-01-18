using Caliburn.Micro;
using ElmaSmartFarm.ApiClient.DataAccess;
using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using ElmaSmartFarm.UserControls.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace ElmaSmartFarm.ClientWpf.ViewModels;

public class LiveValuesViewModel : ViewAware
{
    public LiveValuesViewModel(int index)
    {
        //ScalarSensor = new();
        //ScalarSensor.Values = new();
        ScalarSensorPresenterViewModel = new();
        PoultryManager = new(index);
        PoultryManager.OnDataChanged += PoultryManager_OnDataChanged;
    }

    public ScalarSensorViewerViewModel ScalarSensorPresenterViewModel { get; set; }

    private void PoultryManager_OnDataChanged(object sender, EventArgs e)
    {
        Ticks = (DateTime.Now - PoultryManager.SystemStartupTime).ToString();
        NotifyOfPropertyChange(() => PoultryManager);
        //if (PoultryManager?.Poultry?.Farms[0]?.Scalars?.Sensors[0] != null)
            ScalarSensorPresenterViewModel.ScalarSensor = PoultryManager.Poultry.Farms[0].Scalars.Sensors[0];
        ScalarSensorPresenterViewModel.RefreshView();
        //MessageBox.Show(PoultryManager.Poultry.Farms[0].Scalars.Sensors[0].LastRead.Temperature.ToString());
    }

    private string statuss;

    public string Statuss
    {
        get { return statuss; }
        set { statuss = value; NotifyOfPropertyChange(() => Statuss); }
    }

    private PoultryManager poultryManager;
    private string ticks;

    public string Ticks
    {
        get { return ticks; }
        set { ticks = value; NotifyOfPropertyChange(() => Ticks); }
    }

    public PoultryManager PoultryManager
    {
        get => poultryManager;
        set { poultryManager = value; NotifyOfPropertyChange(() => PoultryManager); }
    }

    public async Task ConnectAsync()
    {
        if (PoultryManager != null && !PoultryManager.IsRunning) await PoultryManager.ConnectAsync();
        await PoultryManager.RequestPoultryOverHttp();
        if(ScalarSensorPresenterViewModel.ScalarSensor == null) ScalarSensorPresenterViewModel.ScalarSensor = PoultryManager.Poultry.Farms[0].Scalars.Sensors[0];
        //Ticks = (DateTime.Now - PoultryManager.SystemStartupTime).ToString();

        //NotifyOfPropertyChange(() => PoultryManager);
        //NotifyOfPropertyChange(() => scalar);

        //ScalarSensor.LastRead.Temperature = new Random().NextDouble();
        //if (PoultryManager.Poultry.Farms[0].Scalars.Sensors[0].Values == null) PoultryManager.Poultry.Farms[0].Scalars.Sensors[0].Values = new();
        //PoultryManager.Poultry.Farms[0].Scalars.Sensors[0].Values.Add(new() { Temperature = new Random().NextDouble(), ReadDate=DateTime.Now });
        //ScalarSensorPresenterViewModel.RefreshView();
    }

    public async Task DisconnectAsync()
    {
        await PoultryManager.DisconnectAsync();
    }
}