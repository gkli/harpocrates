
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace Harpocrates.Runtime.Common.Configuration.KeyVault
{
    public class ManagedIdentityKeyVaultConfig : KeyVaultConfig
    {
        public ManagedIdentityKeyVaultConfig(string vaultName) : base(vaultName)
        {
        }

        protected override KeyVaultClient OnCreateKeyVaultClient()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            return new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(
                    azureServiceTokenProvider.KeyVaultTokenCallback));
        }
    }
}
