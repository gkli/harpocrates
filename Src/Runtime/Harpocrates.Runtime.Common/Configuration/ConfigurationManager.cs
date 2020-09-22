
using Harpocrates.Runtime.Common.DataAccess.ConnectionStrings;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Harpocrates.Runtime.Common.Configuration
{
    public abstract class ConfigurationManager : IConfigurationManager
    {
        public int MaxNumberProcessingAttempts => OnGetMaxNumberProcessingAttempts();

        public string DeadLetterMessagesQueueName => OnGetDeadLetterMessagesQueueName();

        public string RawMessagesQueueName => OnGetRawMessagesQueueName();

        public string FormattedMessagesQueueName => OnGetFormattedMessagesQueueName();

        public StorageAccountConnectionString MonitoredQueueConnectionString => OnGetMonitoredQueueConnectionString();

        public IServiceProvider ServiceProvider => OnGetServiceProvider();

        public CQRSStorageAccountConnectionString SecretManagementConnectionString => OnGetSecretManagementConnectionString();

        public ServicePrincipalConnectionString EnvironmentServicePrincipalConnectionString => OnGetEnvironmentServicePrincipalConnectionString();


        protected abstract int OnGetMaxNumberProcessingAttempts();
        protected abstract string OnGetDeadLetterMessagesQueueName();
        protected abstract string OnGetRawMessagesQueueName();
        protected abstract string OnGetFormattedMessagesQueueName();
        protected abstract IServiceProvider OnGetServiceProvider();
        protected abstract StorageAccountConnectionString OnGetMonitoredQueueConnectionString();
        protected abstract CQRSStorageAccountConnectionString OnGetSecretManagementConnectionString();
        protected abstract ServicePrincipalConnectionString OnGetEnvironmentServicePrincipalConnectionString();
    }
}
