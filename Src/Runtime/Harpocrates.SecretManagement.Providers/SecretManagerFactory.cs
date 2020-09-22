using Harpocrates.SecretManagement.Contracts.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers
{
    public class SecretManagerFactory
    {
        private readonly DataAccess.ISecretMetadataDataAccessProvider _dataProvider;
        private readonly Runtime.Common.Configuration.IConfigurationManager _config;
        private readonly ILogger _logger;
        //private Runtime.Common.Configuration.IConfigurationManager _config;
        public SecretManagerFactory(Runtime.Common.Configuration.IConfigurationManager config, DataAccess.ISecretMetadataDataAccessProvider dataProvider, ILogger logger)
        {
            _config = config;
            _dataProvider = dataProvider;
            _logger = logger;
        }
        public async Task RotateSecretAsync(Secret secret, CancellationToken token)
        {
            if (null == secret) throw new ArgumentNullException(nameof(secret));
            if (null == secret.Configuration) throw new ArgumentNullException(nameof(secret.Configuration));
            if (null == secret.Policy && null == secret.Configuration.DefaultPolicy) throw new ArgumentException("A secret.Policy or secret.Configuration.DefaultPolicy must be specified");

            _logger?.LogInformation($"Attempting to rotate secret {secret.Key}");

            ISecretManager provider = null;

            try
            {
                provider = CreateSecretManagementProvider(secret.Configuration.Type);
            }
            catch (InvalidOperationException)
            {
                _logger.LogWarning($"Unable to locate SecretManager for {secret.Configuration.Type}");
                throw;
            }

            Key rotated = null;

            try
            {
                rotated = await provider.RotateSecretAsync(secret, token);
                _logger?.LogInformation($"Rotated secret {secret.Key}");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"Unable to rotate secret {secret.Key}. Exception: {ex.Message}");
                throw;
            }

            if (null != rotated)
            {
                _logger?.LogInformation($"Updating secret metadata for {secret.Key}");
                //update Key.Name into secret record?
                secret.CurrentKeyName = rotated.Name;

                try
                {
                    await _dataProvider.SaveSecretAsync(secret, token); //Save secret

                    _logger?.LogInformation($"Updated secret metadata for {secret.Key}");
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning($"Unable to update secret metadata {secret.Key}. Exception: {ex.Message}");
                    throw;
                }
            }


        }

        private ISecretManager CreateSecretManagementProvider(SecretConfigurationBase.SecretType type)
        {
            //todo: look at using external configuration for mapping, allowing for providers to be added from external aseemblies / services
            switch (type)
            {
                case SecretConfigurationBase.SecretType.StorageAccountKey:
                    return new Azure.StorageAccountSecretManager(_config);
                case SecretConfigurationBase.SecretType.CosmosDbAccountKey:
                    return new Azure.CosmosDbSecretManager(_config);
                case SecretConfigurationBase.SecretType.EventGrid:
                    return new Azure.EventGridSecretManager(_config);
                case SecretConfigurationBase.SecretType.APIMManagement:
                    return new Azure.APIMManagementSecretManager(_config);
                case SecretConfigurationBase.SecretType.SqlServerPassword:
                    return new Azure.SqlServerSecretManager(_config);
            }

            throw new InvalidOperationException($"Unable to determine provider for type:{type}");
        }
    }
}
