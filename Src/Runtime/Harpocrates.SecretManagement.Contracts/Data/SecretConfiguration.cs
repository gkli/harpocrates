using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.Contracts.Data
{
    public class SecretConfiguration
    {
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

        public SecretPolicy Policy { get; set; }
    }
}
