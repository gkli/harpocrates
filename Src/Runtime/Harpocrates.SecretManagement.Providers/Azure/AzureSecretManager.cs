using Harpocrates.SecretManagement.Contracts.Data;
using Microsoft.Azure.KeyVault;
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
        protected IAzure GetAzureEnvironment()
        {
            //if (null != _azure) return _azure;

            //lock (_lock)
            //{
            //    if (null == _azure)
            //    {
            //todo: May not want to carry this arround as things could run in different subscriptions? -- should we be deployed to single subscription or
            //manage accross subscriptions?
            var credentials = SdkContext.AzureCredentialsFactory.FromFile(Environment.GetEnvironmentVariable("AZURE_AUTH_LOCATION"));

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

            TimeSpan expiresOn = secret.Configuration.Policy.RotationInterval;

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
