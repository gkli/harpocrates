using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.Contracts.Data
{
    public class SecretConfigurationBase
    {
        public Guid ConfigurationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // public string SecretUri { get; set; }

        public string SourceConnectionString { get; set; }

        public string SubscriptionId { get; set; }

        public ServiceType ServiceType { get; set; }

        public SecretConfiguration ToSecretConfig()
        {
            return new SecretConfiguration()
            {
                ConfigurationId = ConfigurationId,
                Description = Description,
                Name = Name,
                ServiceType = ServiceType,
                SourceConnectionString = SourceConnectionString,
                SubscriptionId = SubscriptionId
            };
        }
    }
}
