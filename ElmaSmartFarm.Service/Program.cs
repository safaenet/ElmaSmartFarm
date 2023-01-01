using ElmaSmartFarm.DataLibraryCore.Config;
using ElmaSmartFarm.DataLibraryCore.Interfaces;
using ElmaSmartFarm.DataLibraryCore.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ElmaSmartFarm.Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        var folder = "log";
        var fileName = "Logfile.txt";
        var path = Path.Combine(folder, fileName);
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(path)
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            Log.Information("Starting up the Service...");
            var host = Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureServices(services =>
            {
                services.AddHostedService<Worker>()
                .AddSingleton<Config, Config>()
                .AddSingleton<IDataAccess, SqlDataAccess>()
                .AddSingleton<IDbProcessor, MsSqlDbProcessor>();
            })
            .UseSerilog()
            .Build();

            await host.RunAsync();
            return;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "There was a problem starting up the Service.");
            return;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}