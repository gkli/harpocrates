using Harpocrates.Runtime.Common.Configuration;
using Microsoft.Azure.Management.CosmosDB.Fluent;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers.Azure
{
    internal class CosmosDbReadOnlySecretManager : CosmosDbSecretManager
    {
        public CosmosDbReadOnlySecretManager(IConfigurationManager config, ILogger logger) : base(config, logger)
        {
        }


        protected override string GetPrimaryKeyName()
        {
            return Microsoft.Azure.Management.CosmosDB.Fluent.Models.KeyKind.PrimaryReadonly.Value;
        }

        protected override string GetSecondaryKeyName()
        {
            return Microsoft.Azure.Management.CosmosDB.Fluent.Models.KeyKind.SecondaryReadonly.Value;
        }
    }
}
