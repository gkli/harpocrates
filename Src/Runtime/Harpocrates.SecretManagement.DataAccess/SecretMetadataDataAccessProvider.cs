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


        public async Task<SecretBase> GetSecretAsync(string key, CancellationToken token)
        { //TODO: Validate input 

            if (token.IsCancellationRequested) return null;

            return await OnGetSecretAsync(key, token);
        }

        public async Task<SecretBase> GetSecretAsync(Uri secretUri, CancellationToken token)
        { //TODO: Validate input 

            if (token.IsCancellationRequested) return null;


            return await GetSecretAsync(GetSecretKeyFromUri(secretUri), token);
        }

        public async Task<Secret> GetConfiguredSecretAsync(string key, CancellationToken token)
        { //TODO: Validate input 

            if (token.IsCancellationRequested) return null;

            return await OnGetConfiguredSecretAsync(key, token);
        }

        public async Task<Secret> GetConfiguredSecretAsync(Uri secretUri, CancellationToken token)
        { //TODO: Validate input 

            if (token.IsCancellationRequested) return null;

            return await GetConfiguredSecretAsync(GetSecretKeyFromUri(secretUri), token);
        }

        public async Task<IEnumerable<Secret>> GetConfiguredSecretsAsync(CancellationToken token)
        {
            return await OnGetConfiguredSecretsAsync(token);
        }

        public async Task<string> SaveSecretAsync(Secret secret, CancellationToken token)
        { //TODO: Validate input 

            if (null == secret) throw new ArgumentNullException(nameof(secret));
            if (secret.SecretType == SecretType.Attached && null == secret.Configuration) throw new ArgumentNullException(nameof(secret.Configuration));

            return await OnSaveSecretAsync(secret, token);
        }

        public async Task DeleteSecretAsync(string key, CancellationToken token)
        { //TODO: Validate input 
            await OnDeleteSecretAsync(key, token);
        }

        public async Task DeleteSecretAsync(Uri secretUri, CancellationToken token)
        { //TODO: Validate input 
            await DeleteSecretAsync(GetSecretKeyFromUri(secretUri), token);
        }

        public async Task<IEnumerable<SecretPolicy>> GetPoliciesAsync(CancellationToken token)
        {
            return await OnGetPoliciesAsync(token);
        }

        public async Task<SecretPolicy> GetPolicyAsync(string policyId, CancellationToken token)
        {
            //TODO: Validate input 

            if (token.IsCancellationRequested) return null;

            return await OnGetPolicyAsync(policyId, token);
        }

        public async Task<string> SavePolicyAsync(SecretPolicy policy, CancellationToken token)
        {
            if (null == policy) throw new ArgumentNullException(nameof(policy));

            return await OnSavePolicyAsync(policy, token);
        }
        public async Task DeletePolicyAsync(string policyId, CancellationToken token)
        {
            //TODO: Validate input 
            await OnDeletePolicyAsync(policyId, token);
        }

        public async Task<SecretConfiguration> GetConfigurationAsync(string configId, CancellationToken token)
        {
            //TODO: Validate input 

            if (token.IsCancellationRequested) return null;

            return await OnGetConfigurationAsync(configId, token);
        }


        public async Task<IEnumerable<SecretConfiguration>> GetConfigurationsAsync(CancellationToken token)
        {
            return await OnGetConfigurationsAsync(token);
        }

        public async Task DeleteConfigurationAsync(string configId, CancellationToken token)
        {
            //TODO: Validate input 
            await OnDeleteConfigurationAsync(configId, token);
        }

        public async Task<string> SaveConfigurationAsync(SecretConfiguration config, CancellationToken token)
        {
            if (null == config) throw new ArgumentNullException(nameof(config));
            if (null == config.Policy) throw new ArgumentNullException(nameof(config.Policy));

            return await OnSaveConfigurationAsync(config, token);
        }

        public async Task AddSecretDependencyAsync(string dependsOnKey, string dependentKey, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(dependsOnKey)) throw new ArgumentException($"{nameof(dependsOnKey)} cannot be empty", nameof(dependsOnKey));
            if (string.IsNullOrWhiteSpace(dependentKey)) throw new ArgumentException($"{nameof(dependentKey)} cannot be empty", nameof(dependentKey));

            await OnAddSecretDependencyAsync(dependsOnKey, dependentKey, token);
        }

        public async Task RemoveSecretDependencyAsync(string dependsOnKey, string dependentKey, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(dependsOnKey)) throw new ArgumentException($"{nameof(dependsOnKey)} cannot be empty", nameof(dependsOnKey));
            if (string.IsNullOrWhiteSpace(dependentKey)) throw new ArgumentException($"{nameof(dependentKey)} cannot be empty", nameof(dependentKey));

            await OnRemoveSecretDependencyAsync(dependsOnKey, dependentKey, token);
        }

        public async Task<IEnumerable<Secret>> GetDependentSecretsAsync(string dependsOnKey, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(dependsOnKey)) throw new ArgumentException($"{nameof(dependsOnKey)} cannot be empty", nameof(dependsOnKey));

            return await OnGetDependentSecretsAsync(dependsOnKey, token);
        }



        private string GetSecretKeyFromUri(Uri secreturi)
        {
            return SecretBase.FromKeyvaultUri(secreturi.ToString()).Key;
        }
        protected abstract Task<SecretBase> OnGetSecretAsync(string key, CancellationToken token);
        protected abstract Task<Secret> OnGetConfiguredSecretAsync(string key, CancellationToken token);
        protected abstract Task<IEnumerable<Secret>> OnGetConfiguredSecretsAsync(CancellationToken token);

        protected abstract Task<string> OnSaveSecretAsync(Secret secret, CancellationToken token);
        protected abstract Task OnDeleteSecretAsync(string key, CancellationToken token);
        protected abstract Task<SecretPolicy> OnGetPolicyAsync(string policyId, CancellationToken token);
        protected abstract Task<IEnumerable<SecretPolicy>> OnGetPoliciesAsync(CancellationToken token);
        protected abstract Task<string> OnSavePolicyAsync(SecretPolicy policy, CancellationToken token);
        protected abstract Task OnDeletePolicyAsync(string policyId, CancellationToken token);

        protected abstract Task<SecretConfiguration> OnGetConfigurationAsync(string configId, CancellationToken token);
        protected abstract Task<IEnumerable<SecretConfiguration>> OnGetConfigurationsAsync(CancellationToken token);
        protected abstract Task OnDeleteConfigurationAsync(string configId, CancellationToken token);
        protected abstract Task<string> OnSaveConfigurationAsync(SecretConfiguration config, CancellationToken token);

        protected abstract Task OnAddSecretDependencyAsync(string dependsOnKey, string dependentKey, CancellationToken token);
        protected abstract Task OnRemoveSecretDependencyAsync(string dependsOnKey, string dependentKey, CancellationToken token);
        protected abstract Task<IEnumerable<Secret>> OnGetDependentSecretsAsync(string dependsOnKey, CancellationToken token);


    }
}
