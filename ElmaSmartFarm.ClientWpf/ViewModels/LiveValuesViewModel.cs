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
        PoultryManager = new(index);
        PoultryManager.OnDataChanged += PoultryManager_OnDataChanged;
    }

    private void PoultryManager_OnDataChanged(object sender, EventArgs e)
    {
        Ticks = (DateTime.Now - PoultryManager.SystemStartupTime).ToString();
        NotifyOfPropertyChange(() => PoultryManager);
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

    public List<MqttMessageModel> UnknownMqttMessages
    {
        get => PoultryManager.UnknownMqttMessages; 
        set
        {
            PoultryManager.UnknownMqttMessages = value;
            NotifyOfPropertyChange(() => PoultryManager);
            NotifyOfPropertyChange(() => UnknownMqttMessages);
        }
    }

    public ScalarSensorModel scalar
    {
        get => PoultryManager.Poultry.Farms[0].Scalars.Sensors[0];
        set { PoultryManager.Poultry.Farms[0].Scalars.Sensors[0] = value; NotifyOfPropertyChange(() => scalar); NotifyOfPropertyChange(() => PoultryManager); }
    }

    public async Task ConnectAsync()
    {
        //if (PoultryManager != null && !PoultryManager.IsRunning) await PoultryManager.ConnectAsync();
        //await PoultryManager.RequestPoultryOverHttp();
        //Ticks = (DateTime.Now - PoultryManager.SystemStartupTime).ToString();

        //NotifyOfPropertyChange(() => PoultryManager);
        //NotifyOfPropertyChange(() => scalar);
        Statuss = DateTime.Now.ToString();
    }

    public async Task DisconnectAsync()
    {
        await PoultryManager.DisconnectAsync();
    }
}