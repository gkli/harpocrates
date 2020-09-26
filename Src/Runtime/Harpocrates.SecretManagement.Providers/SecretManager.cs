using System;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers
{
    public abstract class SecretManager : ISecretManager
    {

        public SecretManager(Runtime.Common.Configuration.IConfigurationManager config)
        {
            Config = config;
        }

        protected Runtime.Common.Configuration.IConfigurationManager Config { get; private set; }

        public async Task<Key> RotateSecretAsync(Contracts.Data.Secret secret, CancellationToken token)
        {
            if (null == secret) throw new ArgumentNullException(nameof(secret));
            if (secret.SecretType == Contracts.Data.SecretType.Attached)
            {
                if (null == secret.Configuration) throw new ArgumentNullException(nameof(secret.Configuration));
                if (null == secret.Configuration.Policy) throw new ArgumentException(nameof(secret.Configuration.Policy));
            }
            Key result = null;
            if (secret.SecretType == Contracts.Data.SecretType.Attached)
                result = await OnRotateSecretAsync(secret, token);
            else
                result = new Key() { };

            if (null != result)
            {
                result.SecretVersion = await PersistSecretToVaultAsync(secret, result.Value, token);
                result.Value = "********************************";
                if (string.IsNullOrWhiteSpace(result.SecretVersion))
                {
                    //todo: change exception type to something more meaningful
                    throw new InvalidOperationException("Unable to store newly generated value in vault.");
                }
            }

            return result;
        }

        protected async Task<string> PersistSecretToVaultAsync(Contracts.Data.Secret secret, string value, CancellationToken token)
        {
            return await OnPersistSecretToVaultAsync(secret, value, token);
        }

        protected abstract Task<Key> OnRotateSecretAsync(Contracts.Data.Secret secret, CancellationToken token);

        protected abstract Task<string> OnPersistSecretToVaultAsync(Contracts.Data.Secret secret, string value, CancellationToken token);
    }
}
