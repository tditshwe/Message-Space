using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Api.T2.Tests.Tools
{
    public static class AppConfig
    {
        private static IConfigurationBuilder _builder = new ConfigurationBuilder().AddJsonFile(Directory.GetCurrentDirectory() + "\\appsettings.json");
        private static IConfigurationRoot _config = _builder.Build();

        public static IConfigurationSection GetSection(string name)
        {
            return _config.GetSection(name);
        }

        public static IConfigurationRoot GetConfig()
        { 
            return _config;
        }
    }
}
