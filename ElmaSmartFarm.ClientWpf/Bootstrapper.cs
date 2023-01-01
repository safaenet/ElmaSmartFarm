using Caliburn.Micro;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ElmaSmartFarm.ClientWpf;

public class Bootstrapper : BootstrapperBase
{
    public SimpleContainer Container = new();
    protected override object GetInstance(Type serviceType, string key)
    {
        return Container.GetInstance(serviceType, key);
    }

    protected override void Configure()
    {
        Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(@"log\LogFile.txt")
                .CreateLogger();

        try
        {
            Container.Instance(Container)
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>();
            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(ViewModelType => Container.RegisterPerRequest(
                    ViewModelType, ViewModelType.ToString(), ViewModelType));

            //Container
            //    .Singleton<IApiProcessor, ApiProcessor>();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in Bootstrapper");
        }
    }

    protected override IEnumerable<object> GetAllInstances(Type serviceType)
    {
        return Container.GetAllInstances(serviceType);
    }

    protected override void BuildUp(object instance)
    {
        Container.BuildUp(instance);
    }
    public Bootstrapper()
    {
        Initialize();
    }

    protected override void OnStartup(object sender, StartupEventArgs e)
    {
        //_ = DisplayRootViewFor<LoginViewModel>();
    }
}