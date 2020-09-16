using Harpocrates.SecretManagement.Contracts.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers
{
    public class SecretManagerFactory
    {
        private readonly DataAccess.ISecretMetadataDataAccessProvider _dataProvider;
        private readonly ILogger _logger;
        //private Runtime.Common.Configuration.IConfigurationManager _config;
        public SecretManagerFactory(DataAccess.ISecretMetadataDataAccessProvider dataProvider, ILogger logger)
        {
            _dataProvider = dataProvider;
            _logger = logger;
        }
        public async Task RotateSecretAsync(Secret secret, CancellationToken token)
        {
            if (null == secret) throw new ArgumentNullException(nameof(secret));
            if (null == secret.Configuration) throw new ArgumentNullException(nameof(secret.Configuration));
            if (null == secret.Configuration.Policy) throw new ArgumentNullException(nameof(secret.Configuration.Policy));

            ISecretManagemer provider = CreateSecretManagementProvider(secret.Configuration.Type);

            Key rotated = await provider.RotateSecretAsync(secret);

            //todo: need to write this into KV and update secret record...
            //interact w/ KV directly here?

            //update Key.Name into secret record?
            secret.CurrentKeyName = rotated.Name;
            await _dataProvider.SaveSecretAsync(secret, token); //Save secret

        }

        private ISecretManagemer CreateSecretManagementProvider(SecretConfigurationBase.SecretType type)
        {
            //todo: look at using external configuration for mapping, allowing for providers to be added from external aseemblies / services
            switch (type)
            {
                case SecretConfigurationBase.SecretType.StorageAccountKey:
                    return new Azure.StorageAccountSecretManager();
                case SecretConfigurationBase.SecretType.CosmosDbAccountKey:
                    return new Azure.CosmosDbSecretManager();
                case SecretConfigurationBase.SecretType.EventGrid:
                    return new Azure.EventGridSecretManager();
                case SecretConfigurationBase.SecretType.APIMManagement:
                    return new Azure.APIMManagementSecretManager();
                case SecretConfigurationBase.SecretType.SqlServerPassword:
                    return new Azure.SqlServerSecretManager();
            }

            throw new InvalidOperationException($"Unable to determine provider for type:{type}");
        }
    }
}
