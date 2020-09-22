using Harpocrates.SecretManagement.Contracts.Data;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers.Azure
{
    public abstract class AzureSecretManager : SecretManager
    {
        public AzureSecretManager(Runtime.Common.Configuration.IConfigurationManager config) : base(config)
        {

        }

        protected IAzure GetAzureEnvironment()
        {
            //if (null != _azure) return _azure;

            //lock (_lock)
            //{
            //    if (null == _azure)
            //    {
            //todo: May not want to carry this arround as things could run in different subscriptions? -- should we be deployed to single subscription or
            //manage accross subscriptions?


            //todo: this may differ depending on hosting scenario, if so, would need to move to Configuration...
            //var credentials = SdkContext.AzureCredentialsFactory.FromFile(Environment.GetEnvironmentVariable("AZURE_AUTH_LOCATION"));



            //ADD: environment details to Config
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


            var azure = Microsoft.Azure.Management.Fluent.Azure.Configure()
                            .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                            .Authenticate(credentials)
                            .WithDefaultSubscription();
            //    }
            //}

            return azure;
        }

        protected override async Task<bool> OnPersistSecretToVaultAsync(Secret secret, string value, CancellationToken token)
        {
            //todo: store new secret value
            //todo: vary credential providers based on need (MI/Keys/etc)
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            KeyVaultClient kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

            SecretPolicy effectivePolicy = secret.Policy;
            if (null == effectivePolicy) effectivePolicy = secret.Configuration.DefaultPolicy;

            TimeSpan expiresOn = effectivePolicy.RotationInterval;

            //todo: look at secret.FormatString to determine actual value of the secret to be written to KV

            switch (secret.ObjectType)
            {
                case "secret":
                    var result = await kvClient.SetSecretAsync($"https://{secret.VaultName}.vault.azure.net", secret.ObjectName, value
                        , secretAttributes: new Microsoft.Azure.KeyVault.Models.SecretAttributes()
                        {
                            Enabled = true,
                            Expires = DateTime.UtcNow.Add(expiresOn)
                        }
                        , cancellationToken: token); ;
                    return true;
                case "key":
                    break;
                case "certificate":
                    break;

            }

            return false;
        }

        //private HttpClient GetHttpClient()
        //{
        //    //todo: change to use caches instance or use HttpClientFactory from IService
        //    return new HttpClient();
        //}
    }
}
