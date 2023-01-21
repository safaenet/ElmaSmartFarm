using Caliburn.Micro;
using ElmaSmartFarm.ApiClient.DataAccess;
using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace ElmaSmartFarm.ClientWpf.ViewModels;

public class LiveValuesViewModel : ViewAware
{
    public LiveValuesViewModel(int index)
    {
        ScalarSensor2 = new();
        ScalarSensor2.Values = new();
        //ScalarSensorPresenterViewModel = new();
        PoultryManager = new(index);
        PoultryManager.OnDataChanged += PoultryManager_OnDataChanged;
        Statuss = "ctor";
    }

    public ScalarSensorViewerViewModel ScalarSensorPresenterViewModel { get; set; }

    private void PoultryManager_OnDataChanged(object sender, EventArgs e)
    {
        NotifyOfPropertyChange(() => PoultryManager);
        //if (PoultryManager?.Poultry?.Farms[0]?.Scalars?.Sensors[0] != null)
        //ScalarSensorPresenterViewModel.ScalarSensor = PoultryManager.Poultry.Farms[0].Scalars.Sensors[0];
        //ScalarSensorPresenterViewModel.RefreshView();
        //MessageBox.Show(PoultryManager.Poultry.Farms[0].Scalars.Sensors[0].LastRead.Temperature.ToString());
    }

    private string statuss;

    public string Statuss
    {
        get { return statuss; }
        set { statuss = value; NotifyOfPropertyChange(() => Statuss); }
    }

    private PoultryManager poultryManager;

    public PoultryManager PoultryManager
    {
        get => poultryManager;
        set { poultryManager = value; NotifyOfPropertyChange(() => PoultryManager); }
    }

    private ScalarSensorModel scalarSensor2;

    public ScalarSensorModel ScalarSensor2
    {
        get { return scalarSensor2; }
        set { scalarSensor2 = value; NotifyOfPropertyChange(() => ScalarSensor2); }
    }

    private double? myVar;

    public double? Temperature
    {
        get { return myVar; }
        set { myVar = value; }
    }


    public async Task ConnectAsync()
    {
        Statuss = DateTime.Now.ToString();
        //if (PoultryManager != null && !PoultryManager.IsRunning) await PoultryManager.ConnectAsync();
        //await PoultryManager.RequestPoultryOverHttp();
        //if(ScalarSensorPresenterViewModel.ScalarSensor == null) ScalarSensorPresenterViewModel.ScalarSensor = PoultryManager.Poultry.Farms[0].Scalars.Sensors[0];

        //if (PoultryManager.Poultry.Farms[0].Scalars.Sensors[0].Values == null) PoultryManager.Poultry.Farms[0].Scalars.Sensors[0].Values = new();
        ////PoultryManager.Poultry.Farms[0].Scalars.Sensors[0].Values.Add(new() { Temperature = new Random().NextDouble(), ReadDate = DateTime.Now });
        ScalarSensor2.Values.Add(new() { Temperature = new Random().NextDouble() * 10, Humidity = new Random().Next(), ReadDate = DateTime.Now });
        NotifyOfPropertyChange(() => ScalarSensor2);
        var x = ScalarSensor2;
        ScalarSensor2 = null;
        ScalarSensor2 = x;

        //ScalarSensorPresenterViewModel.ScalarSensor = PoultryManager.Poultry.Farms[0].Scalars.Sensors[0];
    }

    public async Task DisconnectAsync()
    {
        await PoultryManager.DisconnectAsync();
    }
}