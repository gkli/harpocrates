
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Harpocrates.Runtime.Common.Configuration.KeyVault
{
    public class ClientSecretKeyVaultConfig : KeyVaultConfig
    {
        public ClientSecretKeyVaultConfig(string vaultName, string clientId, string clientSecret) : base(vaultName)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        private string ClientId { get; set; }
        private string ClientSecret { get; set; }

        protected override KeyVaultClient OnCreateKeyVaultClient()
        {
            return new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
                 async (string authority, string resource, string scope) =>
                 {
                     var authContext = new AuthenticationContext(authority);
                     var credential = new ClientCredential(ClientId, ClientSecret);
                     AuthenticationResult result = await authContext.AcquireTokenAsync(resource, credential);
                     return result.AccessToken;
                 }));
        }

    }
}
