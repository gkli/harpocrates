using Harpocrates.Runtime.Common.DataAccess.ConnectionStrings;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Harpocrates.Management.Web.Server
{
    internal class WebServerConfigurationProvider : Runtime.Common.Configuration.ConfigurationManager
    {
        private readonly IConfiguration _runtimeConfig;
        private readonly IServiceProvider _serviceProvider;
        public WebServerConfigurationProvider(IServiceProvider serviceProvider, IConfiguration runtimeConfig)
        {
            _runtimeConfig = runtimeConfig;
            _serviceProvider = serviceProvider;
        }

        protected override string OnGetDeadLetterMessagesQueueName()
        {
            throw new NotImplementedException();
        }

        protected override ServicePrincipalConnectionString OnGetEnvironmentServicePrincipalConnectionString()
        {
            throw new NotImplementedException();
        }

        protected override string OnGetFormattedMessagesQueueName()
        {
            throw new NotImplementedException();
        }

        protected override int OnGetMaxNumberProcessingAttempts()
        {
            throw new NotImplementedException();
        }

        protected override StorageAccountConnectionString OnGetMonitoredQueueConnectionString()
        {
            throw new NotImplementedException();
        }

        protected override string OnGetRawMessagesQueueName()
        {
            throw new NotImplementedException();
        }

        protected override CQRSStorageAccountConnectionString OnGetSecretManagementConnectionString()
        {
            string connectionString = _runtimeConfig["MetadataRepository:ConnectionString"];

            return new CQRSStorageAccountConnectionString(new StorageAccountConnectionString()
            {
                ConnectionString = connectionString
            }, new StorageAccountConnectionString()
            {
                ConnectionString = connectionString
            });
        }

        protected override IServiceProvider OnGetServiceProvider()
        {
            throw new NotImplementedException();
        }
    }
}
