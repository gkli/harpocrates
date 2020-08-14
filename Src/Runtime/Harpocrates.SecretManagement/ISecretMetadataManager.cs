using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement
{
    public interface ISecretMetadataManager
    {
        Task ProcessExpiringSecretAsync(string secretUri, CancellationToken token);
        Task ProcessExpiredSecretAsync(string secretUri, CancellationToken token);

        //Task UpdateSecretAsync(string secretId, CancellationToken token);

        //Task SaveSecretAsync(Contracts.Data.Secret secret, Contracts.Data.SecretPolicy policy, CancellationToken token);

        //Task DeleteSecretAsync(string secretId, CancellationToken token);
    }
}
