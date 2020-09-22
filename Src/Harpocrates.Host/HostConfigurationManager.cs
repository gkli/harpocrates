using Harpocrates.Runtime.Common.DataAccess.ConnectionStrings;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Host
{
    internal class HostConfigurationManager : Runtime.Common.Configuration.ConfigurationManager
    {
        private readonly IConfiguration _runtimeConfig;
        private readonly IServiceProvider _serviceProvider;
        public HostConfigurationManager(IServiceProvider serviceProvider, IConfiguration runtimeConfig)
        {
            _runtimeConfig = runtimeConfig;
            _serviceProvider = serviceProvider;
        }

        protected override string OnGetDeadLetterMessagesQueueName()
        {
            return _runtimeConfig["EventProcessing:DeadLetterQueueName"];
        }

        protected override string OnGetFormattedMessagesQueueName()
        {
            return _runtimeConfig["EventProcessing:FormattedMessageQueueName"];
        }

        protected override int OnGetMaxNumberProcessingAttempts()
        {
            if (int.TryParse(_runtimeConfig["EventProcessing:MaxNumberProcessingAttempts"], out int count)) return count;

            return 5; //default to 5 attempts
        }

        protected override string OnGetRawMessagesQueueName()
        {
            return _runtimeConfig["EventProcessing:RawMessageQueueName"];
        }

        protected override IServiceProvider OnGetServiceProvider()
        {
            return _serviceProvider;
        }

        protected override StorageAccountConnectionString OnGetMonitoredQueueConnectionString()
        {
            //todo: read from config

            return new StorageAccountConnectionString()
            {
                ConnectionString = _runtimeConfig["EventProcessing:QueueServiceConnectionString"]
            };
        }

        protected override CQRSStorageAccountConnectionString OnGetSecretManagementConnectionString()
        {
            //todo: read from config
            string connectionString = _runtimeConfig["MetadataRepository:ConnectionString"];

            return new CQRSStorageAccountConnectionString(new StorageAccountConnectionString()
            {
                ConnectionString = connectionString
            }, new StorageAccountConnectionString()
            {
                ConnectionString = connectionString
            });
        }

        protected override ServicePrincipalConnectionString OnGetEnvironmentServicePrincipalConnectionString()
        {
            return new ServicePrincipalConnectionString()
            {
                TenantId = _runtimeConfig.GetSection("Environment:TenantId").Value,
                ClientId = _runtimeConfig.GetSection("Environment:ClientId").Value,
                ClientSecret = _runtimeConfig.GetSection("Environment:ClientSecret").Value,
                EnvironmentName = _runtimeConfig.GetSection("Environment:Name").Value
            };
        }
    }
}
