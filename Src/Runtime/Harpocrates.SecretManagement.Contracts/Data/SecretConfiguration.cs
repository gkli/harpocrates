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
            SqlServerPassword
        }

        public string SecretUri { get; set; }

        public string OriginConnectionString { get; set; }
    }
}
