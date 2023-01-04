using Microsoft.Extensions.Configuration;
using Serilog;
using System;

namespace ElmaSmartFarm.ApiClient.Config;

public static class SettingsDataAccess
{
    public static IConfiguration AppConfiguration()
    {
        try
        {
            IConfiguration conf;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            conf = builder.Build();
            return conf;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error in {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType}");
        }
        return null;
    }

    public static IConfigurationSection GetSection(string section)
    {
        return AppConfiguration().GetSection(section);
    }

    public static T GetValue<T>(string section)
    {
        return AppConfiguration().GetValue<T>(section);
    }
}