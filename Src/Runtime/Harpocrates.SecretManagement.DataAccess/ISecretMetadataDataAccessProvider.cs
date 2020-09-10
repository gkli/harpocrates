using System;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.DataAccess
{
    public interface ISecretMetadataDataAccessProvider
    {
        Task<Contracts.Data.Secret> GetSecretAsync(string key, CancellationToken token);
        Task<Contracts.Data.Secret> GetSecretAsync(Uri secretUri, CancellationToken token);

        Task<Contracts.Data.ConfiguredSecret> GetConfiguredSecretAsync(string key, CancellationToken token);
        Task<Contracts.Data.ConfiguredSecret> GetConfiguredSecretAsync(Uri secretUri, CancellationToken token);

        Task SaveSecretAsync(Contracts.Data.ConfiguredSecret secret, CancellationToken token);
        Task DeleteSecretAsync(string key, CancellationToken token);
        Task DeleteSecretAsync(Uri secretUri, CancellationToken token);

        Task<Contracts.Data.SecretPolicy> GetPolicyAsync(string policyId, CancellationToken token);
        Task DeletePolicyAsync(string policyId, CancellationToken token);
        Task SavePolicyAsync(Contracts.Data.SecretPolicy policy, CancellationToken token);

        Task<Contracts.Data.SecretConfiguration> GetConfigurationAsync(string configId, CancellationToken token);
        Task DeleteConfigurationAsync(string configId, CancellationToken token);
        Task SaveConfigurationAsync(Contracts.Data.SecretConfiguration config, CancellationToken token);

    }
}
