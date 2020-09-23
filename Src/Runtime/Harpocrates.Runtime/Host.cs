
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.Runtime
{
    public class Host
    {
        private readonly ILogger _logger;
        private readonly Common.Configuration.IConfigurationManager _config;

        private QueueMonitor<Common.Contracts.RawProcessRequest> _rawMessageQueueMonitor;
        private QueueMonitor<Common.Contracts.FormattedProcessRequest> _formattedMessageQueueMonitor;

        public Host(Common.Configuration.IConfigurationManager config, ILogger<Host> logger)
        {
            _logger = logger;
            _config = config;
        }

        public async Task StartAsync(CancellationToken token)
        {
            _logger.LogInformation($"Monitoring starting: {DateTime.Now}");

            _rawMessageQueueMonitor = new QueueMonitor<Common.Contracts.RawProcessRequest>(_config.RawMessagesQueueName, TimeSpan.FromSeconds(120), _config, _logger);
            _formattedMessageQueueMonitor = new QueueMonitor<Common.Contracts.FormattedProcessRequest>(_config.FormattedMessagesQueueName, TimeSpan.FromSeconds(120), _config, _logger);

            while (!token.IsCancellationRequested)
            {
                List<Task> pendingTasks = new List<Task>();

                // do not await, this can run in the background
                pendingTasks.Add(_rawMessageQueueMonitor.ProcessPendingMessagesAsync(token));

                await _formattedMessageQueueMonitor.ProcessPendingMessagesAsync(token);

                //if we're done, but pending tasks are still going, wait for that 
                Task.WaitAll(pendingTasks.ToArray(), token);

                await Task.Delay(2500);
            }

            _logger.LogInformation($"Monitoring stopping: {DateTime.Now}");
        }

        public async Task CreateSampleDataSetAsync(CancellationToken token)
        {
            string subscriptionId = "e4e151a2-0cd9-4598-aa8d-cb8d5f72eeef";

            SecretManagement.DataAccess.ISecretMetadataDataAccessProvider dataProvider =
                _config.ServiceProvider.GetRequiredService<SecretManagement.DataAccess.ISecretMetadataDataAccessProvider>();

            Dictionary<Guid, SecretManagement.Contracts.Data.SecretPolicy> policies = new Dictionary<Guid, SecretManagement.Contracts.Data.SecretPolicy>();
            #region Create Policies

            //Near expiry is raised 30 days before expiration, so baseline all durations w/ 30 days

            Guid id = Guid.Parse("86705a80-4f30-466d-900a-25e80c0e15e4");
            policies.Add(id, new SecretManagement.Contracts.Data.SecretPolicy()
            {
                PolicyId = id,
                Name = "15-Day Rotation",
                Description = "Rotates secrets every 15 days",
                RotationInterval = TimeSpan.FromDays(45)
            });

            id = Guid.Parse("d5bfb8a3-bc76-4a1E-bb46-50904ebb9273");
            policies.Add(id, new SecretManagement.Contracts.Data.SecretPolicy()
            {
                PolicyId = id,
                Name = "30-Day Rotation",
                Description = "Rotates secrets every 30 days",
                RotationInterval = TimeSpan.FromDays(60)
            });

            id = Guid.Parse("6CADA23D-76B1-4B50-A773-E4D0822D6821");
            policies.Add(id, new SecretManagement.Contracts.Data.SecretPolicy()
            {
                PolicyId = id,
                Name = "24-Hour Rotation",
                Description = "Rotates secrets every 24 hours",
                RotationInterval = TimeSpan.FromDays(31)
            });

            id = Guid.Parse("520FD7E3-04EF-48F6-B163-99B7DC74B216");
            policies.Add(id, new SecretManagement.Contracts.Data.SecretPolicy()
            {
                PolicyId = id,
                Name = "1-Hour Rotation",
                Description = "Rotates secrets every 1 hour",
                RotationInterval = TimeSpan.FromDays(30) + TimeSpan.FromHours(1)
            }); ;

            foreach (var policy in policies.Values)
            {
                await dataProvider.SavePolicyAsync(policy, token);
            }
            #endregion

            Dictionary<Guid, SecretManagement.Contracts.Data.SecretConfiguration> configs = new Dictionary<Guid, SecretManagement.Contracts.Data.SecretConfiguration>();
            #region Create Configurations

            id = Guid.Parse("5c09346e-bd0a-4a6b-b26d-c89b5111cae3");
            configs.Add(id, new SecretManagement.Contracts.Data.SecretConfiguration()
            {
                ConfigurationId = id,
                Name = "harpocratestest1",
                Description = "Harpocratestest1 storage account",
                ServiceType = SecretManagement.Contracts.Data.ServiceType.StorageAccountKey,
                SourceConnectionString = "AccountEndpoint=https://harpocratestest1.core.windows.net;ResourceGroup=harpocrates;",
                SubscriptionId = subscriptionId,
                Policy = policies[Guid.Parse("86705a80-4f30-466d-900a-25e80c0e15e4")]
            });

            id = Guid.Parse("5f72a920-040a-4750-8974-d29629bbe20f");
            configs.Add(id, new SecretManagement.Contracts.Data.SecretConfiguration()
            {
                ConfigurationId = id,
                Name = "harpocratestest2",
                Description = "Harpocratestest2 storage account",
                ServiceType = SecretManagement.Contracts.Data.ServiceType.StorageAccountKey,
                SourceConnectionString = "AccountEndpoint=https://harpocratestest2.core.windows.net;ResourceGroup=harpocrates;",
                SubscriptionId = subscriptionId,
                Policy = policies[Guid.Parse("d5bfb8a3-bc76-4a1E-bb46-50904ebb9273")]
            });

            foreach (var config in configs.Values)
            {
                await dataProvider.SaveConfigurationAsync(config, token);
            }
            #endregion

            string[] urls = new string[]
                     {"https://harpocrates-test2.vault.azure.net/secrets/harpocratestest2-key/d239cec181a24ce1b382dd2cb514c0ee",
                         "https://harpocrates-test1.vault.azure.net/secrets/harpocratestest1-key/b349f2ecea8b4306af2fb0b1b5aff7e9",
                         "https://harpocrates-test2.vault.azure.net/secrets/App2-Connection-String/0d91baac50b746f9af3d4fdce8c93cb7",
                         "https://harpocrates-test2.vault.azure.net/secrets/App1-Connection-String/50ddd0d3e6d248cfa1ebb56145848189",
                         "https://harpocrates-test1.vault.azure.net/secrets/Custom-app-composite-string/cb0fe84f326540309c5d151283206fa7"};

            List<SecretManagement.Contracts.Data.Secret> secrets = new List<SecretManagement.Contracts.Data.Secret>();

            #region Create Secrets
            SecretManagement.Contracts.Data.SecretBase sb = SecretManagement.Contracts.Data.Secret.FromKeyvaultUri(urls[0]);
            secrets.Add(new SecretManagement.Contracts.Data.Secret()
            {
                ObjectName = sb.ObjectName,
                ObjectType = sb.ObjectType,
                VaultName = sb.VaultName,
                Version = sb.Version,
                SubscriptionId = subscriptionId,

                CurrentKeyName = "Key1",
                Name = "harpocratestest2-key",
                Description = "Harpocratestest2 storage account access key",
                FormatExpression = null,
                SecretType = SecretManagement.Contracts.Data.SecretType.ManagedSystem,
                Configuration = configs[Guid.Parse("5f72a920-040a-4750-8974-d29629bbe20f")]
            });

            sb = SecretManagement.Contracts.Data.Secret.FromKeyvaultUri(urls[1]);
            secrets.Add(new SecretManagement.Contracts.Data.Secret()
            {
                ObjectName = sb.ObjectName,
                ObjectType = sb.ObjectType,
                VaultName = sb.VaultName,
                Version = sb.Version,
                SubscriptionId = subscriptionId,

                CurrentKeyName = "Key1",
                Name = "harpocratestest1-key",
                Description = "Harpocratestest1 storage account access key",
                FormatExpression = null,
                SecretType = SecretManagement.Contracts.Data.SecretType.ManagedSystem,
                Configuration = configs[Guid.Parse("5c09346e-bd0a-4a6b-b26d-c89b5111cae3")]
            });

            sb = SecretManagement.Contracts.Data.Secret.FromKeyvaultUri(urls[2]);
            secrets.Add(new SecretManagement.Contracts.Data.Secret()
            {
                ObjectName = sb.ObjectName,
                ObjectType = sb.ObjectType,
                VaultName = sb.VaultName,
                Version = sb.Version,
                SubscriptionId = subscriptionId,

                CurrentKeyName = "Key1",
                Name = "App1 Connection String",
                Description = "Storage account connection string used by App1",
                FormatExpression = $"DefaultEndpointsProtocol=https;AccountName=harpocrates;AccountKey={{{{{secrets[1].Key}}}}};EndpointSuffix=core.windows.net",
                SecretType = SecretManagement.Contracts.Data.SecretType.Dependency
            });

            sb = SecretManagement.Contracts.Data.Secret.FromKeyvaultUri(urls[3]);
            secrets.Add(new SecretManagement.Contracts.Data.Secret()
            {
                ObjectName = sb.ObjectName,
                ObjectType = sb.ObjectType,
                VaultName = sb.VaultName,
                Version = sb.Version,
                SubscriptionId = subscriptionId,
                Name = "App2 Connection String",
                Description = "Storage account connection string used by App2",
                FormatExpression = $"DefaultEndpointsProtocol=https;AccountName=harpocrates;AccountKey={{{{{secrets[0].Key}}}}};EndpointSuffix=core.windows.net",
                SecretType = SecretManagement.Contracts.Data.SecretType.Dependency
            });

            sb = SecretManagement.Contracts.Data.Secret.FromKeyvaultUri(urls[4]);
            secrets.Add(new SecretManagement.Contracts.Data.Secret()
            {
                ObjectName = sb.ObjectName,
                ObjectType = sb.ObjectType,
                VaultName = sb.VaultName,
                Version = sb.Version,
                SubscriptionId = subscriptionId,
                Name = "Custom app composite string",
                Description = "Storage account connection string used by App2",
                FormatExpression = $"AccountKey1={{{{{secrets[0].Key}}}}};AccountKey2={{{{{secrets[1].Key}}}}};",
                SecretType = SecretManagement.Contracts.Data.SecretType.Dependency
            });
            #endregion

            foreach (var secret in secrets)
            {
                await dataProvider.SaveSecretAsync(secret, token);
            }

            await dataProvider.AddSecretDependencyAsync(secrets[1].Key, secrets[2].Key, token);
            await dataProvider.AddSecretDependencyAsync(secrets[0].Key, secrets[3].Key, token);
            await dataProvider.AddSecretDependencyAsync(secrets[0].Key, secrets[4].Key, token);
            await dataProvider.AddSecretDependencyAsync(secrets[1].Key, secrets[4].Key, token);

            var children = await dataProvider.GetDependentSecretsAsync(secrets[0].Key, token);
        }

    }
}
