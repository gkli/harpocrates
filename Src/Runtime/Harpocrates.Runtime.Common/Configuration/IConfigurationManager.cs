using System;

namespace Harpocrates.Runtime.Common.Configuration
{
    public interface IConfigurationManager
    {
        int MaxNumberProcessingAttempts { get; }
        string DeadLetterMessagesQueueName { get; }
        string RawMessagesQueueName { get; }
        string FormattedMessagesQueueName { get; }

        IServiceProvider ServiceProvider { get; }

        DataAccess.ConnectionStrings.StorageAccountConnectionString MonitoredQueueConnectionString { get; }

        DataAccess.ConnectionStrings.CQRSStorageAccountConnectionString SecretManagementConnectionString { get; }

        DataAccess.ConnectionStrings.ServicePrincipalConnectionString EnvironmentServicePrincipalConnectionString { get; }
    }
}
