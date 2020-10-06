using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Harpocrates.Management.Web.Server.Configuration
{
    internal class ServerConfigurationProvider : IConfigurationProvider
    {
        private readonly IConfiguration _runtimeConfig;
        private readonly IServiceProvider _serviceProvider;
        public ServerConfigurationProvider(IServiceProvider serviceProvider, IConfiguration runtimeConfig)
        {
            _runtimeConfig = runtimeConfig;
            _serviceProvider = serviceProvider;
        }

        public string MetadataServiceBaseUri => GetMetadataServiceBaseUri();

        private string GetMetadataServiceBaseUri()
        {
            //todo: add env variable probing
            return _runtimeConfig["Harpocrates:Backend:MetadataServiceBaseUri"];
        }
    }
}
