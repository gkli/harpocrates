using Harpocrates.SecretManagement.Contracts.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers.Azure
{
    internal class StorageAccountSecretManager : SecretManager
    {
        private static class ValidKeyNames
        {
            public const string Key1 = "Key1";
            public const string Key2 = "Key2";
        }
        protected override async Task<Key> OnRotateSecretAsync(Secret secret)
        {
            Key result = new Key()
            {
                Name = ValidKeyNames.Key1,
                Value = new System.Security.SecureString()
            };

            string v = GenerateKey();

            foreach (char c in v.ToCharArray())
            {
                result.Value.AppendChar(c);
            }

            result.Value.MakeReadOnly();

            if (secret.CurrentKeyName == ValidKeyNames.Key1)
            {
                result.Name = ValidKeyNames.Key2;
            }

            //todo: update storage account to new value 
            //result.Name contains the name of the key to be udpated

            //string foo = result.Value;

            return result;

        }

        private static string GenerateKey()
        {
            return "123";
        }
    }
}
