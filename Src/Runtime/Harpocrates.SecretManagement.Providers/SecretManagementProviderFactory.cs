using Harpocrates.SecretManagement.Contracts.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers
{
    public class SecretManagementProviderFactory : ISecretManagementProvider
    {
        public async Task RotateSecretAsync(Secret secret)
        {
            if (null == secret) throw new ArgumentNullException(nameof(secret));
            if (null == secret.Configuration) throw new ArgumentNullException(nameof(secret.Configuration));
            if (null == secret.Configuration.Policy) throw new ArgumentNullException(nameof(secret.Configuration.Policy));

            ISecretManagementProvider provider = CreateSecretManagementProvider(secret.Configuration.Type);

            await provider.RotateSecretAsync(secret);
        }

        private ISecretManagementProvider CreateSecretManagementProvider(SecretConfigurationBase.SecretType type)
        {
            //todo: look at using external configuration for mapping, allowing for providers to be added from external aseemblies / services
            switch (type)
            {
                case SecretConfigurationBase.SecretType.StorageAccountKey:
                    return new Azure.StorageAccountSecretManagementProvider();
                case SecretConfigurationBase.SecretType.CosmosDbAccountKey:
                    return new Azure.CosmosDbSecretManagementProvider();
                case SecretConfigurationBase.SecretType.EventGrid:
                    return new Azure.EventGridSecretManagementProvider();
                case SecretConfigurationBase.SecretType.APIMManagement:
                    return new Azure.APIMManagementSecretManagementProvider();
                case SecretConfigurationBase.SecretType.SqlServerPassword:
                    return new Azure.SqlServerSecretManagementProvider();
            }

            throw new InvalidOperationException($"Unable to determine provider for type:{type}");
        }
    }
}
