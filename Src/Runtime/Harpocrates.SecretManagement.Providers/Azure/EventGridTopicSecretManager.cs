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
    internal class EventGridTopicSecretManager : AzureSecretManager
    {
        private static class ValidKeyNames
        {
            public const string Key1 = "key1";
            public const string Key2 = "key2";
        }

        public EventGridTopicSecretManager(IConfigurationManager config, ILogger logger) : base(config, logger)
        {
        }

        protected override async Task<Key> OnRotateSecretAsync(Secret secret, CancellationToken token)
        {
            Key result = new Key()
            {
                Name = ValidKeyNames.Key1,
                Value = string.Empty
            };

            if (string.Compare(secret.CurrentKeyName, ValidKeyNames.Key1, true) == 0)
            {
                result.Name = ValidKeyNames.Key2;
            }

            result.Value = await GenerateKey(secret, result.Name, token);


            return result;
        }

        private async Task<string> GenerateKey(Secret secret, string newKeyName, CancellationToken token)
        {
            Runtime.Common.DataAccess.ConnectionStrings.StorageAccountConnectionString sacs = new Runtime.Common.DataAccess.ConnectionStrings.StorageAccountConnectionString()
            {
                ConnectionString = secret.Configuration.SourceConnectionString
            };

            var azure = GetAzureEnvironment();

            string accountId = $"/subscriptions/{secret.Configuration.SubscriptionId}/resourceGroups/{sacs.ResourceGroup}/providers/Microsoft.Storage/storageAccounts/{sacs.AccountName}";

            //var account = await azure.RedisCaches.GetByIdAsync(accountId, token);

            ////if (string.IsNullOrWhiteSpace(secret.CurrentKeyName)) secret.CurrentKeyName = ValidKeyNames.Key1;

            //var keys = await account..(Microsoft.Azure.Management.ContainerRegistry.Fluent.AccessKeyType.Primary, token);

            //foreach (var key in keys)
            //{
            //    if (string.Compare(key.KeyName, newKeyName, true) == 0)
            //    {
            //        return key.Value;
            //    }
            //}

            return null;
        }
    }
}
