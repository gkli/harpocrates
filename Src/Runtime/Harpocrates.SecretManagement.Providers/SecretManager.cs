using System;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers
{
    public abstract class SecretManager : ISecretManagemer
    {
        public async Task<Key> RotateSecretAsync(Contracts.Data.Secret secret, CancellationToken token)
        {
            if (null == secret) throw new ArgumentNullException(nameof(secret));
            if (null == secret.Configuration) throw new ArgumentNullException(nameof(secret.Configuration));
            if (null == secret.Configuration.Policy) throw new ArgumentNullException(nameof(secret.Configuration.Policy));

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
