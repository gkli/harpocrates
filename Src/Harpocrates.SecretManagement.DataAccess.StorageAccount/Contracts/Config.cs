using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.DataAccess.StorageAccount.Contracts
{
    public class Config : SecretManagement.Contracts.Data.SecretConfigurationBase
    {
        public Guid PolicyId { get; set; }


        public override SecretManagement.Contracts.Data.SecretConfiguration ToSecretConfiguration()
        {
            var config = base.ToSecretConfiguration();

            if (Guid.Empty != PolicyId)
            {
                config.Policy = new SecretManagement.Contracts.Data.SecretPolicy()
                {
                    PolicyId = PolicyId
                };
            }

            return config;
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
