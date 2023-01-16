using Caliburn.Micro;
using ElmaSmartFarm.ApiClient.DataAccess;
using ElmaSmartFarm.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElmaSmartFarm.ClientWpf.ViewModels;

public class LiveValuesViewModel : ViewAware
{
    public LiveValuesViewModel(int index)
    {
        PoultryManager = new(index);
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

    public async Task ConnectAsync()
    {
        if (PoultryManager != null && !PoultryManager.IsRunning) await PoultryManager.ConnectAsync();
        await PoultryManager.RequestPoultryOverHttp();
        Ticks = (DateTime.Now - PoultryManager.SystemStartupTime).ToString();
    }

    public async Task DisconnectAsync()
    {
        await PoultryManager.DisconnectAsync();
    }
}