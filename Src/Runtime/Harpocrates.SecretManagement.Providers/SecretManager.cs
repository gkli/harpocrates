using System;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers
{
    public abstract class SecretManager : ISecretManagemer
    {
        public async Task<Key> RotateSecretAsync(Contracts.Data.Secret secret)
        {
            if (null == secret) throw new ArgumentNullException(nameof(secret));
            if (null == secret.Configuration) throw new ArgumentNullException(nameof(secret.Configuration));
            if (null == secret.Configuration.Policy) throw new ArgumentNullException(nameof(secret.Configuration.Policy));

            return await OnRotateSecretAsync(secret);
        }

        protected abstract Task<Key> OnRotateSecretAsync(Contracts.Data.Secret secret);
    }
}
