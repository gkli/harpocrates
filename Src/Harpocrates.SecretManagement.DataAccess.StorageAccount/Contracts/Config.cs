using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.DataAccess.StorageAccount.Contracts
{
    public class Config : SecretManagement.Contracts.Data.SecretConfigurationBase
    {
        public Guid PolicyId { get; set; }

        public SecretManagement.Contracts.Data.SecretConfiguration ToSecretConfiguration()
        {
            return new SecretManagement.Contracts.Data.SecretConfiguration()
            {
                ConfigurationId = ConfigurationId,
                Name = Name,
                Description = Description,
                SubscriptionId = SubscriptionId,
                SourceConnectionString = SourceConnectionString,
                ServiceType = ServiceType,
                //SecretUri = SecretUri,
                Policy = new SecretManagement.Contracts.Data.SecretPolicy()
                {
                    PolicyId = PolicyId
                }
            };
        }

        public static Config FromSecretConfiguration(SecretManagement.Contracts.Data.SecretConfiguration config)
        {
            return new Config()
            {
                ConfigurationId = config.ConfigurationId,
                Name = config.Name,
                Description = config.Description,
                SubscriptionId = config.SubscriptionId,
                SourceConnectionString = config.SourceConnectionString,
                //SecretUri = config.SecretUri,
                ServiceType = config.ServiceType,
                PolicyId = config.Policy == null ? Guid.Empty : config.Policy.PolicyId // could this ever happen? prevent in data validation
            };
        }
    }
}
