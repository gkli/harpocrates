
using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.Configuration
{
    public abstract class ConfigurationManager : IConfigurationManager
    {
        public int MaxNumberProcessingAttempts => OnGetMaxNumberProcessingAttempts();

        public string DeadLetterMessagesQueueName => OnGetDeadLetterMessagesQueueName();

        public string RawMessagesQueueName => OnGetRawMessagesQueueName();

        public string FormattedMessagesQueueName => OnGetFormattedMessagesQueueName();

        public DataAccess.ConnectionStrings.StorageAccountConnectionString MonitoredQueueConnectionString => OnGetMonitoredQueueConnectionString();

        protected abstract int OnGetMaxNumberProcessingAttempts();
        protected abstract string OnGetDeadLetterMessagesQueueName();
        protected abstract string OnGetRawMessagesQueueName();
        protected abstract string OnGetFormattedMessagesQueueName();

        protected abstract DataAccess.ConnectionStrings.StorageAccountConnectionString OnGetMonitoredQueueConnectionString();
    }
}
