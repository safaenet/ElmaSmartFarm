using MQTTnet;
using MQTTnet.Client;
using Serilog;
using Serilog.Events;
using System.Text;

namespace ElmaSmartFarm.Service
{

    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(@"log\LogFile.txt")
                .CreateLogger();

            try
            {
                Log.Information("Starting up the Service...");
                IHost host = Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
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
}