using Harpocrates.SecretManagement.Contracts.Data;
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
            public const string Key1 = "Key1";
            public const string Key2 = "Key2";
        }
        protected override async Task<Key> OnRotateSecretAsync(Secret secret, CancellationToken token)
        {

            //https://github.com/Azure-Samples/storage-dotnet-manage-storage-accounts/blob/master/Program.cs

            Key result = new Key()
            {
                Name = ValidKeyNames.Key1,
                Value = string.Empty
            };

            result.Value = await GenerateKey(secret, token);

            //foreach (char c in newKeyValue.ToCharArray())
            //{
            //    result.Value.AppendChar(c);
            //}

            //result.Value.MakeReadOnly();

            if (secret.CurrentKeyName == ValidKeyNames.Key1)
            {
                result.Name = ValidKeyNames.Key2;
            }

            return result;

        }

        private async Task<string> GenerateKey(Secret secret, CancellationToken token)
        {
            //todo: parse out connection string or use some other construct
            var account = await GetAzureEnvironment().StorageAccounts.GetByIdAsync(secret.Configuration.OriginConnectionString);
            //var keys = await account.GetKeysAsync(token);

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
