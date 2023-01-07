﻿using Microsoft.Extensions.Configuration;
using System.IO;

namespace ElmaSmartFarm.DataLibraryCore.Config;

public static class SettingsDataAccess
{
    const string relativePath = @"../ElmaSmartFarm.DataLibraryCore/Config";
    public static IConfiguration AppConfiguration()
    {
        var absolutePath = Path.GetFullPath(relativePath);
        IConfiguration conf;
        var builder = new ConfigurationBuilder()
            .SetBasePath(absolutePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("syssettings.json", optional: false, reloadOnChange: true);
        conf = builder.Build();
        return conf;
    }

    public static IConfigurationSection GetSection(string section)
    {
        return AppConfiguration().GetSection(section);
    }
}