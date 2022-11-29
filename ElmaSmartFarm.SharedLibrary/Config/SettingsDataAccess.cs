using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmaSmartFarm.SharedLibrary.Config
{
    public static class SettingsDataAccess
    {
        const string relativePath = @"../ElmaSmartFarm.SharedLibrary/Config";
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
    }
}