using System;
using System.Configuration;
using System.Linq;

namespace Rabbit.Components.Data.EntityFramework
{
    internal sealed class GlobalConfig
    {
        public static bool AutomaticMigrationsEnabled = false;

        static GlobalConfig()
        {
            const string key = "Data.EntityFramework.AutomaticMigrationsEnabled";
            var settings = ConfigurationManager.AppSettings;
            if (settings.AllKeys.Contains(key) && "true".Equals(settings[key], StringComparison.OrdinalIgnoreCase))
            {
                AutomaticMigrationsEnabled = true;
            }
        }
    }
}