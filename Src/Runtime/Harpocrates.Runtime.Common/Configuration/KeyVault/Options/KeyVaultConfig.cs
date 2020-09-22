using Microsoft.Azure.KeyVault;

namespace Harpocrates.Runtime.Common.Configuration.KeyVault
{
    public abstract class KeyVaultConfig
    {
        private KeyVaultClient _client = null;
        public KeyVaultConfig(string vaultName)
        {
            KeyVaultName = vaultName;
        }

        public string KeyVaultName { get; private set; }

        public KeyVaultClient GetKeyVaultClient()
        {
            if (null == _client)
            {
                _client = OnCreateKeyVaultClient();
            }

            return _client;
        }

        protected abstract KeyVaultClient OnCreateKeyVaultClient();

    }
}
