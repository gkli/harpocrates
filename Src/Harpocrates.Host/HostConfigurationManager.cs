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

        protected override string OnGetRawMessagesQueueName()
        {
            throw new NotImplementedException();
        }

        protected override StorageAccountConnectionString OnGetMonitoredQueueConnectionString()
        {
            throw new NotImplementedException();
        }
    }
}
