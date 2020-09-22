using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Harpocrates.Runtime.Common.Configuration.KeyVault
{
    public class CertificateKeyVaultConfig : KeyVaultConfig
    {
        public CertificateKeyVaultConfig(string vaultName, string thumbPrint, string aadAppId) : base(vaultName)
        {
            CertificateThumbprint = thumbPrint;
            AzureADApplicationId = aadAppId;
        }

        private string CertificateThumbprint { get; set; }
        private string AzureADApplicationId { get; set; }

        protected override KeyVaultClient OnCreateKeyVaultClient()
        {
            KeyVaultClient client = null;
            using (var store = new X509Store(StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                var certs = store.Certificates
                    .Find(X509FindType.FindByThumbprint,
                        CertificateThumbprint, false);

                var cert = certs.OfType<X509Certificate2>().Single();

                client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(async (string authority, string resource, string scope) =>
                {
                    var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
                    var result = await context.AcquireTokenAsync(resource, new ClientAssertionCertificate(authority, cert)).ConfigureAwait(false);
                    return result.AccessToken;
                })
                    );

                store.Close();
            }


            return client;

        }
    }
}
