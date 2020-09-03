using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.Contracts.Data
{
    public class ConfiguredSecret : Secret
    {
        public enum SecretType
        {
            StorageAccountKey,
            CosmosDbAccountKey,
            SqlServerPassword
        }

        public string SecretUri { get; set; }

        public string OriginConnectionString { get; set; }

        public SecretPolicy Policy { get; set; }
    }
}
