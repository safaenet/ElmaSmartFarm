using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ElmaSmartFarm.ApiClient.Config;

public static class SettingsDataAccess
{
    public static IConfiguration AppConfiguration()
    {
        IConfiguration conf;
        var builder = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        conf = builder.Build();
        return conf;
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