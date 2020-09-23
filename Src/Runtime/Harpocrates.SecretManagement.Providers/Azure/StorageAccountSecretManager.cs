using Harpocrates.Runtime.Common.Configuration;
using Harpocrates.SecretManagement.Contracts.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers.Azure
{
    internal class StorageAccountSecretManager : AzureSecretManager
    {
        private static class ValidKeyNames
        {
            public const string Key1 = "key1";
            public const string Key2 = "key2";
        }

        public StorageAccountSecretManager(Runtime.Common.Configuration.IConfigurationManager config, ILogger logger) : base(config, logger) { }

        protected override async Task<Key> OnRotateSecretAsync(Secret secret, CancellationToken token)
        {
            Key result = new Key()
            {
                Name = ValidKeyNames.Key1,
                Value = string.Empty
            };

            result.Value = await GenerateKey(secret, token);

            if (secret.CurrentKeyName == ValidKeyNames.Key1)
            {
                result.Name = ValidKeyNames.Key2;
            }

            return result;

        }

        private async Task<string> GenerateKey(Secret secret, CancellationToken token)
        {
            Runtime.Common.DataAccess.ConnectionStrings.StorageAccountConnectionString sacs = new Runtime.Common.DataAccess.ConnectionStrings.StorageAccountConnectionString()
            {
                ConnectionString = secret.Configuration.SourceConnectionString
            };

            var azure = GetAzureEnvironment();

            string accountId = $"/subscriptions/{secret.Configuration.SubscriptionId}/resourceGroups/{sacs.ResourceGroup}/providers/Microsoft.Storage/storageAccounts/{sacs.AccountName}";

            var account = await azure.StorageAccounts.GetByIdAsync(accountId);

            if (string.IsNullOrWhiteSpace(secret.CurrentKeyName)) secret.CurrentKeyName = ValidKeyNames.Key1;

            var keys = await account.RegenerateKeyAsync(secret.CurrentKeyName, token);

            foreach (var key in keys)
            {
                if (string.Compare(key.KeyName, secret.CurrentKeyName, true) == 0)
                {
                    return key.Value;
                }
            }

            return null;
        }
    }
}
