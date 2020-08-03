using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Host
{
    internal class HostConfigurationManager : Runtime.Common.Configuration.ConfigurationManager
    {
        private readonly IConfiguration _runtimeConfig;

        public HostConfigurationManager(IServiceProvider serviceProvider, IConfiguration runtimeConfig)
        {
            _runtimeConfig = runtimeConfig;
        }
    }
}
