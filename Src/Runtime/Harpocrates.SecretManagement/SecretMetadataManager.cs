using Harpocrates.SecretManagement.Contracts.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Harpocrates.SecretManagement
{
    public class SecretMetadataManager : ISecretMetadataManager
    {
        private readonly DataAccess.ISecretMetadataDataAccessProvider _dataProvider;
        private Runtime.Common.Configuration.IConfigurationManager _config;
        private readonly ILogger _logger;
        public SecretMetadataManager(Runtime.Common.Configuration.IConfigurationManager config, ILogger logger)
        {
            //_dataProvider = dataProvider;
            _config = config;
            _logger = logger;

            _dataProvider = _config.ServiceProvider.GetRequiredService<DataAccess.ISecretMetadataDataAccessProvider>();
            //we need a data provider in order to read/write secret data...
        }

        public async Task ProcessExpiringSecretAsync(string secretUri, CancellationToken token)
        {
            _logger?.LogInformation($"Processing expiring seceret {secretUri}.");

            await ProcessSecret(secretUri, token);

            _logger?.LogInformation($"Processed expiring seceret {secretUri}.");
        }

        public async Task ProcessExpiredSecretAsync(string secretUri, CancellationToken token)
        {
            _logger?.LogInformation($"Processing expired seceret {secretUri}.");

            await ProcessSecret(secretUri, token);

            _logger?.LogInformation($"Processed expired seceret {secretUri}.");
        }

        public async Task ProcessUpdatedSecretAsync(string secretUri, CancellationToken token)
        {
            _logger?.LogInformation($"Processing updated seceret {secretUri}.");

            await ProcessSecret(secretUri, token);

            _logger?.LogInformation($"Processed updated seceret {secretUri}.");
        }


        private async Task ProcessSecret(string secretUri, CancellationToken token)
        {

            var secret = await _dataProvider.GetConfiguredSecretAsync(Secret.FromKeyvaultUri(secretUri).Key, token);

            if (null == secret)
            {
                //what do we do we if can't find the secret?
                _logger?.LogWarning($"Unable to retrieve metadata for seceret {secretUri}");
                return;
            }

            if (secret.SecretType == SecretType.ManagedSystem && null == secret.Configuration)
            {
                //what do we do if secret doesn't have config
                _logger?.LogWarning($"Unable to retrieve configuration metadata for seceret {secretUri}");
                throw new InvalidOperationException("Unable to retreive seceret configuration metadata");
            }

            Providers.SecretManagerFactory factory = new Providers.SecretManagerFactory(_config, _dataProvider, _logger);
            await factory.RotateSecretAsync(secret, token);
        }
    }
}
