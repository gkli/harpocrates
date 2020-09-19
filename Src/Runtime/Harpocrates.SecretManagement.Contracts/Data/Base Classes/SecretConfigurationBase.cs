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

        public enum SecretType
        {
            StorageAccountKey,
            CosmosDbAccountKey,
            SqlServerPassword,
            EventGrid,
            APIMManagement
        }

        public string SecretUri { get; set; }

        public string OriginConnectionString { get; set; }

        public string SubscriptionId { get; set; }

        public SecretType Type { get; set; }
    }
}
