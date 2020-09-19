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

        public HostConfigurationManager(IServiceProvider serviceProvider, IConfiguration runtimeConfig)
        {
            _runtimeConfig = runtimeConfig;
        }

        protected override string OnGetDeadLetterMessagesQueueName()
        {
            return "deadletter";
        }

        protected override string OnGetFormattedMessagesQueueName()
        {
            return "formatted";
        }

        protected override int OnGetMaxNumberProcessingAttempts()
        {
            return 10;
        }

        protected override string OnGetRawMessagesQueueName()
        {
            return "events";
        }

        protected override StorageAccountConnectionString OnGetMonitoredQueueConnectionString()
        {
            /*
             * "DefaultEndpointsProtocol=https;AccountName=harpocrates;AccountKey=4PDtsssV6Gcl5zZmd9igruRgsU5qi5FvB1gvhV5h2Ax++Y7SymR4QES0EMlF9ftgjUB6mmnmQVfbIEI5YeFKtA==;EndpointSuffix=core.windows.net"
             * 
               public const string AccountEndpoint = "AccountEndpoint";
            public const string AccountKey = "AccountKey";
            public const string ContainerName = "Container";
            public const string KeyType = "KeyType";
             */


            return new StorageAccountConnectionString()
            {
                ConnectionString = "AccountEndpoint=https://harpocrates.core.windows.net;AccountKey=4PDtsssV6Gcl5zZmd9igruRgsU5qi5FvB1gvhV5h2Ax++Y7SymR4QES0EMlF9ftgjUB6mmnmQVfbIEI5YeFKtA==;KeyType=AccountKey;"

            };
        }
    }
}
