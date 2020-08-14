using Harpocrates.Runtime.Common.DataAccess.ConnectionStrings;
using Harpocrates.SecretManagement.Contracts.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.DataAccess
{
    public abstract class SecretMetadataDataAccessProvider : Runtime.Common.DataAccess.DataAccessProvider, ISecretMetadataDataAccessProvider
    {
        protected SecretMetadataDataAccessProvider(CQRSConnectionStringBase connectionString) : base(connectionString)
        {
        }

        public async Task DeleteSecretAsync(string secreteId, CancellationToken token)
        {
            //TODO: Add parameter validation
            await OnDeleteSecretAsync(secreteId, token);
        }

        public async Task<SecretPolicy> GetPolicyAsync(string policyId, CancellationToken token)
        {
            //TODO: Add parameter validation
            return await OnGetPolicyAsync(policyId, token);
        }

        public async Task<Secret> GetSecretAsync(string secretId, CancellationToken token)
        {
            //TODO: Add parameter validation
            return await OnGetSecretAsync(secretId, token);
        }

        public async Task<SecretConfiguration> GetSecretConfigurationAsync(string secretId, CancellationToken token)
        {
            //TODO: Add parameter validation
            return await OnGetSecretConfigurationAsync(secretId, token);
        }

        public async Task<SecretPolicy> GetSecretPolicy(string secretId, CancellationToken token)
        {
            //TODO: Add parameter validation
            return await OnGetSecretPolicy(secretId, token);
        }

        public async Task SavePolicyAsync(SecretPolicy policy, CancellationToken token)
        {
            //TODO: Add parameter validation
            await OnSavePolicyAsync(policy, token);
        }

        public async Task SaveSecretAsync(Secret secret, SecretPolicy policy, CancellationToken token)
        {//TODO: Add parameter validation
            await OnSaveSecretAsync(secret, policy, token);
        }





        protected abstract Task<Contracts.Data.Secret> OnGetSecretAsync(string secretId, CancellationToken token);
        protected abstract Task<Contracts.Data.SecretConfiguration> OnGetSecretConfigurationAsync(string secretId, CancellationToken token);
        protected abstract Task<Contracts.Data.SecretPolicy> OnGetSecretPolicy(string secretId, CancellationToken token);
        protected abstract Task OnSaveSecretAsync(Contracts.Data.Secret secret, Contracts.Data.SecretPolicy policy, CancellationToken token);
        protected abstract Task OnDeleteSecretAsync(string secreteId, CancellationToken token);
        protected abstract Task<Contracts.Data.SecretPolicy> OnGetPolicyAsync(string policyId, CancellationToken token);
        protected abstract Task OnSavePolicyAsync(Contracts.Data.SecretPolicy policy, CancellationToken token);

    }
}
