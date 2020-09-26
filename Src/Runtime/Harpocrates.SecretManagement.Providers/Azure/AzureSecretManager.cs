using Harpocrates.SecretManagement.Contracts.Data;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.Azure.Management.Fluent.Azure;

namespace Harpocrates.SecretManagement.Providers.Azure
{
    public abstract class AzureSecretManager : SecretManager
    {
        private class SecretExpressionParser
        {
            private readonly KeyVaultClient _client;
            private readonly DataAccess.ISecretMetadataDataAccessProvider _secretDataProvider;
            private readonly ILogger _logger;
            private readonly Dictionary<string, string> _tokenCache = new Dictionary<string, string>();
            private readonly CancellationToken _cancelToken;
            public SecretExpressionParser(DataAccess.ISecretMetadataDataAccessProvider secretDataProvider, KeyVaultClient client, ILogger logger, CancellationToken token)
            {
                _client = client;
                _secretDataProvider = secretDataProvider;
                _cancelToken = token;
                _logger = logger;
            }

            public string Process(string expression)
            {
                string pattern = @"\{{(.*?)\}}";

                MatchEvaluator evaluator = new MatchEvaluator(LookupTokenValue);

                return Regex.Replace(expression, pattern, evaluator, RegexOptions.IgnorePatternWhitespace);
            }

            private string LookupTokenValue(Match match)
            {
                string key = match.Value;
                if (_tokenCache.ContainsKey(key)) return _tokenCache[key];

                //key is in {{xasaasasadfa}} format, so we skip first 2 and last 2 chars
                string token = key.Substring(2, key.Length - 4);
                string value = LookupTokenValueAsync(token, _client).Result;

                //still not in cache? though shouldn't be possible since we're the only thread operating on this...
                if (false == _tokenCache.ContainsKey(key)) //add to to cache
                {
                    _tokenCache.Add(key, value);
                }

                return value;
            }

            private async Task<string> LookupTokenValueAsync(string token, KeyVaultClient client)
            {
                var tokenSecret = await _secretDataProvider.GetSecretAsync(token, _cancelToken);

                if (null == tokenSecret) return string.Empty; //secret not found...

                //secret.VaultName is from SECRET record that TOKEN points to...
                try
                {
                    var kvSecret = await _client.GetSecretAsync($"https://{tokenSecret.VaultName}.vault.azure.net", tokenSecret.ObjectName);

                    if (kvSecret != null) return kvSecret.Value;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Unable to lookup key vault value for token secret {tokenSecret.Uri}. Error: {ex.Message}");
                }

                return string.Empty;
            }
        }


        public AzureSecretManager(Runtime.Common.Configuration.IConfigurationManager config, ILogger logger) : base(config)
        {
            Logger = logger;
        }

        protected ILogger Logger { get; private set; }

        //protected DataAccess.ISecretMetadataDataAccessProvider MetadataDataAccessProvider { get; private set; }

        protected IAzure GetAzureEnvironment()
        {
            var azure = GetAzureAuthenticated().WithDefaultSubscription();


            return azure;
        }

        protected IAuthenticated GetAzureAuthenticated()
        {

            var envSp = Config.EnvironmentServicePrincipalConnectionString;

            AzureEnvironment azureEnvironment = AzureEnvironment.AzureGlobalCloud;
            if (false == string.IsNullOrEmpty(envSp.EnvironmentName))
            {
                foreach (var env in AzureEnvironment.KnownEnvironments)
                {
                    if (string.Compare(env.Name, envSp.EnvironmentName, false) == 0)
                    {
                        azureEnvironment = env;
                        break;
                    }
                }
            }

            //this sp needs to be "Storage Account Key Operator Service Role" for storage accounts...
            var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(envSp.ClientId, envSp.ClientSecret, envSp.TenantId, azureEnvironment);

            //var credentials = SdkContext.AzureCredentialsFactory.FromSystemAssignedManagedServiceIdentity(Microsoft.Azure.Management.ResourceManager.Fluent.Authentication.MSIResourceType.VirtualMachine, null);


            var authenticated = Microsoft.Azure.Management.Fluent.Azure.Configure()
                            .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                            .Authenticate(credentials);

            return authenticated;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="value"></param>
        /// <param name="token"></param>
        /// <returns>Key Vault Secret Version Identifier</returns>
        protected override async Task<string> OnPersistSecretToVaultAsync(Secret secret, string value, CancellationToken token)
        {
            //todo: store new secret value
            //todo: vary credential providers based on need (MI/Keys/etc)
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            KeyVaultClient kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

            SecretPolicy effectivePolicy = secret?.Configuration?.Policy;

            switch (secret.ObjectType)
            {
                case "secret":

                    var attributes = new SecretAttributes() { Enabled = true };
                    if (null != effectivePolicy) attributes.Expires = DateTime.UtcNow.Add(effectivePolicy.RotationInterval);

                    value = await ProcessSecretExpressionAsync(secret, value, kvClient, token);

                    var result = await kvClient.SetSecretAsync($"https://{secret.VaultName}.vault.azure.net", secret.ObjectName, value
                        , secretAttributes: attributes, cancellationToken: token);

                    if (string.Compare(result.SecretIdentifier.Version, secret.Version, true) != 0) //disable previous version...
                        await kvClient.UpdateSecretAsync(secret.Uri, secretAttributes: new SecretAttributes() { Enabled = false });

                    return result.SecretIdentifier.Version;
                case "key":
                    break;
                case "certificate":
                    break;

            }

            return null;
        }

        private async Task<string> ProcessSecretExpressionAsync(Secret secret, string value, KeyVaultClient kvClient, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(secret.FormatExpression)) return value;

            return await Task.Run<string>(() =>
            {
                DataAccess.ISecretMetadataDataAccessProvider dataProvider = Config.ServiceProvider.GetRequiredService<DataAccess.ISecretMetadataDataAccessProvider>();

                SecretExpressionParser parser = new SecretExpressionParser(dataProvider, kvClient, Logger, token);

                return parser.Process(secret.FormatExpression);
            });
        }
    }
}
