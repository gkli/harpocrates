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
            if (null == secret.Configuration) throw new ArgumentNullException(nameof(secret.Configuration));
            if (null == secret.Policy && null == secret.Configuration.DefaultPolicy) throw new ArgumentException("A secret.Policy or secret.Configuration.DefaultPolicy must be specified");

            Key result = await OnRotateSecretAsync(secret, token);
            if (null != result)
            {
                bool success = await PersistSecretToVaultAsync(secret, result.Value, token);
                result.Value = "VALUE HAS BEEN MASKED";
                if (false == success)
                {
                    //todo: change exception type to something more meaningful
                    throw new InvalidOperationException("Unable to store newly generated value in vault.");
                }
            }

            return result;
        }

        protected async Task<bool> PersistSecretToVaultAsync(Contracts.Data.Secret secret, string value, CancellationToken token)
        {
            return await OnPersistSecretToVaultAsync(secret, value, token);
        }

        protected abstract Task<Key> OnRotateSecretAsync(Contracts.Data.Secret secret, CancellationToken token);

        protected abstract Task<bool> OnPersistSecretToVaultAsync(Contracts.Data.Secret secret, string value, CancellationToken token);
    }
}
