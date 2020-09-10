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


        public async Task<Secret> GetSecretAsync(string key, CancellationToken token)
        { //TODO: Validate input 
            return await OnGetSecretAsync(key, token);
        }

        public async Task<Secret> GetSecretAsync(Uri secretUri, CancellationToken token)
        { //TODO: Validate input 
            return await GetSecretAsync(GetSecretKeyFromUri(secretUri), token);
        }

        public async Task<ConfiguredSecret> GetConfiguredSecretAsync(string key, CancellationToken token)
        { //TODO: Validate input 
            return await OnGetConfiguredSecretAsync(key, token);
        }

        public async Task<ConfiguredSecret> GetConfiguredSecretAsync(Uri secretUri, CancellationToken token)
        { //TODO: Validate input 
            return await GetConfiguredSecretAsync(GetSecretKeyFromUri(secretUri), token);
        }

        public async Task SaveSecretAsync(ConfiguredSecret secret, CancellationToken token)
        { //TODO: Validate input 
            await OnSaveSecretAsync(secret, token);
        }

        public async Task DeleteSecretAsync(string key, CancellationToken token)
        { //TODO: Validate input 
            await OnDeleteSecretAsync(key, token);
        }

        public async Task DeleteSecretAsync(Uri secretUri, CancellationToken token)
        { //TODO: Validate input 
            await DeleteSecretAsync(GetSecretKeyFromUri(secretUri), token);
        }

        public async Task<SecretPolicy> GetPolicyAsync(string policyId, CancellationToken token)
        {
            //TODO: Validate input 

            return await OnGetPolicyAsync(policyId, token);
        }

        public async Task SavePolicyAsync(SecretPolicy policy, CancellationToken token)
        {
            //TODO: Validate input 

            await OnSavePolicyAsync(policy, token);
        }

        public async Task DeletePolicyAsync(string policyId, CancellationToken token)
        {
            //TODO: Validate input 
            await OnDeletePolicyAsync(policyId, token);
        }



        public async Task<SecretConfiguration> GetConfigurationAsync(string configId, CancellationToken token)
        {
            //TODO: Validate input 
            return await OnGetConfigurationAsync(configId, token);
        }

        public async Task DeleteConfigurationAsync(string configId, CancellationToken token)
        {
            //TODO: Validate input 
            await OnDeleteConfigurationAsync(configId, token);
        }

        public async Task SaveConfigurationAsync(SecretConfiguration config, CancellationToken token)
        {
            //TODO: Validate input 
            await OnSaveConfigurationAsync(config, token);
        }

        private string GetSecretKeyFromUri(Uri secreturi)
        {
            return Secret.FromKeyvaultUri(secreturi.ToString()).Key;
        }
        protected abstract Task<Secret> OnGetSecretAsync(string key, CancellationToken token);
        protected abstract Task<ConfiguredSecret> OnGetConfiguredSecretAsync(string key, CancellationToken token);
        protected abstract Task OnSaveSecretAsync(ConfiguredSecret secret, CancellationToken token);
        protected abstract Task OnDeleteSecretAsync(string key, CancellationToken token);
        protected abstract Task<SecretPolicy> OnGetPolicyAsync(string policyId, CancellationToken token);
        protected abstract Task OnSavePolicyAsync(SecretPolicy policy, CancellationToken token);
        protected abstract Task OnDeletePolicyAsync(string policyId, CancellationToken token);

        protected abstract Task<SecretConfiguration> OnGetConfigurationAsync(string configId, CancellationToken token);
        protected abstract Task OnDeleteConfigurationAsync(string configId, CancellationToken token);
        protected abstract Task OnSaveConfigurationAsync(SecretConfiguration config, CancellationToken token);
    }
}
