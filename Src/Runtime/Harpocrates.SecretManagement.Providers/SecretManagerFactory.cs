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

            if (secret.SecretType == SecretType.ManagedSystem)
            {
                if (null == secret.Configuration) throw new ArgumentNullException(nameof(secret.Configuration));
                if (null == secret.Configuration.Policy) throw new ArgumentException(nameof(secret.Configuration.Policy));
            }

            _logger?.LogInformation($"Attempting to rotate secret {secret.Uri}.");

            ISecretManager provider = null;

            try
            {
                ServiceType effectiveServiceType = (secret.Configuration == null) ? ServiceType.Unspecified : secret.Configuration.ServiceType;
                provider = CreateSecretManagementProvider(effectiveServiceType);
            }
            catch (InvalidOperationException)
            {
                _logger.LogWarning($"Unable to locate SecretManager for {secret.Configuration.ServiceType}");
                throw;
            }

            Key rotated = null;

            try
            {
                rotated = await provider.RotateSecretAsync(secret, token);
                _logger?.LogInformation($"Rotated secret {secret.Uri}");
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"Unable to rotate secret {secret.Uri}. Exception: {ex.Message}");
                throw;
            }

            if (null != rotated)
            {
                _logger?.LogInformation($"Updating secret metadata for {secret.Uri}");
                //update Key.Name into secret record?
                secret.CurrentKeyName = rotated.Name;
                secret.Version = rotated.SecretVersion;
                try
                {
                    await _dataProvider.SaveSecretAsync(secret, token); //Save secret

                    _logger?.LogInformation($"Updated secret metadata for {secret.Uri}");
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning($"Unable to update secret metadata {secret.Uri}. Exception: {ex.Message}");
                    throw;
                }
            }


        }

        private ISecretManager CreateSecretManagementProvider(ServiceType type)
        {
            //todo: look at using external configuration for mapping, allowing for providers to be added from external aseemblies / services
            switch (type)
            {
                case ServiceType.StorageAccountKey:
                    return new Azure.StorageAccountSecretManager(_config, _logger);
                case ServiceType.CosmosDbAccountKey:
                    return new Azure.CosmosDbSecretManager(_config, _logger);
                case ServiceType.EventGrid:
                    return new Azure.EventGridSecretManager(_config, _logger);
                case ServiceType.APIMManagement:
                    return new Azure.APIMManagementSecretManager(_config, _logger);
                case ServiceType.SqlServerPassword:
                    return new Azure.SqlServerSecretManager(_config, _logger);
                case ServiceType.Unspecified:
                    return new Azure.GenericAzureSecretManager(_config, _logger);
            }

            throw new InvalidOperationException($"Unable to determine provider for type:{type}");
        }
    }
}
