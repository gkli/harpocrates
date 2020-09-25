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
    internal class RedisCacheSecretManager : AzureSecretManager
    {
        private static class ValidKeyNames
        {
            public static readonly string Key1 = Microsoft.Azure.Management.Redis.Fluent.Models.RedisKeyType.Primary.ToString();
            public static readonly string Key2 = Microsoft.Azure.Management.Redis.Fluent.Models.RedisKeyType.Secondary.ToString();
        }

        public RedisCacheSecretManager(IConfigurationManager config, ILogger logger) : base(config, logger)
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
            Runtime.Common.DataAccess.ConnectionStrings.RedisCacheConnectionString sacs = new Runtime.Common.DataAccess.ConnectionStrings.RedisCacheConnectionString()
            {
                ConnectionString = secret.Configuration.SourceConnectionString
            };

            var azure = GetAzureEnvironment();

            string accountId = $"/subscriptions/{secret.Configuration.SubscriptionId}/resourceGroups/{sacs.ResourceGroup}/providers/Microsoft.Storage/storageAccounts/{sacs.AccountName}";

            var account = await azure.RedisCaches.GetByIdAsync(accountId, token);

            Microsoft.Azure.Management.Redis.Fluent.Models.RedisKeyType keyType = Microsoft.Azure.Management.Redis.Fluent.Models.RedisKeyType.Primary;

            if (Enum.TryParse<Microsoft.Azure.Management.Redis.Fluent.Models.RedisKeyType>(newKeyName, out Microsoft.Azure.Management.Redis.Fluent.Models.RedisKeyType key))
            {
                keyType = key;
            }

            var keys = account.RegenerateKey(keyType);

            switch (keyType)
            {
                case Microsoft.Azure.Management.Redis.Fluent.Models.RedisKeyType.Primary:
                    return keys.PrimaryKey;
                case Microsoft.Azure.Management.Redis.Fluent.Models.RedisKeyType.Secondary:
                    return keys.SecondaryKey;
            }

            return null;
        }
    }
}
