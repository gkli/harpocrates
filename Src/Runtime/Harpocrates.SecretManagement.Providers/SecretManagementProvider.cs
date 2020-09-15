using System;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers
{
    public abstract class SecretManagementProvider : ISecretManagementProvider
    {
        public async Task RotateSecretAsync(Contracts.Data.Secret secret)
        {
            if (null == secret) throw new ArgumentNullException(nameof(secret));
            if (null == secret.Configuration) throw new ArgumentNullException(nameof(secret.Configuration));
            if (null == secret.Configuration.Policy) throw new ArgumentNullException(nameof(secret.Configuration.Policy));

            await OnRotateSecretAsync(secret);
        }

        protected abstract Task OnRotateSecretAsync(Contracts.Data.Secret secret);
    }
}
