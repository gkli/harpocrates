using Harpocrates.Runtime.Common.DataAccess.ConnectionStrings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Host.AzureFunction
{
    internal class FunctionConfigurationManager : Runtime.Common.Configuration.ConfigurationManager
    {
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
            throw new NotImplementedException();
        }

        protected override IServiceProvider OnGetServiceProvider()
        {
            throw new NotImplementedException();
        }
    }
}
