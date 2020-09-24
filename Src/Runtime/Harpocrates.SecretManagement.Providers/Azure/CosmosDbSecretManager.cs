using Harpocrates.Runtime.Common.Configuration;
using Harpocrates.SecretManagement.Contracts.Data;
using Microsoft.Azure.Management.CosmosDB.Fluent;
using Microsoft.Azure.Management.CosmosDB.Fluent.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers.Azure
{
    internal class CosmosDbSecretManager : AzureSecretManager
    {

        public CosmosDbSecretManager(IConfigurationManager config, ILogger logger) : base(config, logger)
        {
        }

        protected override async Task<Key> OnRotateSecretAsync(Secret secret, CancellationToken token)
        {
            Key result = new Key()
            {
                Name = GetPrimaryKeyName(),
                Value = string.Empty
            };

            if (string.Compare(secret.CurrentKeyName, GetPrimaryKeyName(), true) == 0)
            {
                result.Name = GetSecondaryKeyName();
            }

            result.Value = await GenerateKey(secret, result.Name, token);

            return result;
        }


        private async Task<string> GenerateKey(Secret secret, string newKeyName, CancellationToken token)
        {
            Runtime.Common.DataAccess.ConnectionStrings.CosmosDbConnectionString sacs = new Runtime.Common.DataAccess.ConnectionStrings.CosmosDbConnectionString()
            {
                ConnectionString = secret.Configuration.SourceConnectionString
            };

            var azure = GetAzureEnvironment();

            string accountId = $"/subscriptions/{secret.Configuration.SubscriptionId}/resourceGroups/{sacs.ResourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts/{sacs.AccountName}";

            var account = await azure.CosmosDBAccounts.GetByIdAsync(accountId, token);

            return await GenerateDbKeyAsync(newKeyName, account, token);
        }

        private async Task<string> GenerateDbKeyAsync(string key, ICosmosDBAccount account, CancellationToken token)
        {
            await account.RegenerateKeyAsync(key, token);

            var keys = await account.ListKeysAsync();

            if (string.Compare(key, KeyKind.Primary.Value, true) == 0) return keys.PrimaryMasterKey;
            if (string.Compare(key, KeyKind.PrimaryReadonly.Value, true) == 0) return keys.PrimaryReadonlyMasterKey;
            if (string.Compare(key, KeyKind.Secondary.Value, true) == 0) return keys.SecondaryMasterKey;
            if (string.Compare(key, KeyKind.SecondaryReadonly.Value, true) == 0) return keys.SecondaryReadonlyMasterKey;

            return null;
        }

        protected virtual string GetPrimaryKeyName()
        {
            return KeyKind.Primary.Value;
        }

        protected virtual string GetSecondaryKeyName()
        {
            return KeyKind.Secondary.Value;
        }
    }
}
