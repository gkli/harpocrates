using System;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.DataAccess
{
    public interface ISecretMetadataDataAccessProvider
    {
        Task<Contracts.Data.Secret> GetSecretAsync(string secretId, CancellationToken token);
        Task<Contracts.Data.SecretConfiguration> GetSecretConfigurationAsync(string secretId, CancellationToken token);
        Task<Contracts.Data.SecretPolicy> GetSecretPolicy(string secretId, CancellationToken token);
        Task SaveSecretAsync(Contracts.Data.Secret secret, Contracts.Data.SecretPolicy policy, CancellationToken token);
        Task DeleteSecretAsync(string secreteId, CancellationToken token);

        Task<Contracts.Data.SecretPolicy> GetPolicyAsync(string policyId, CancellationToken token);
        Task SavePolicyAsync(Contracts.Data.SecretPolicy policy, CancellationToken token);
    }
}
