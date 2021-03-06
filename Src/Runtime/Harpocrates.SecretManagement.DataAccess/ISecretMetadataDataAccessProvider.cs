﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.DataAccess
{
    public interface ISecretMetadataDataAccessProvider
    {
        Task<Contracts.Data.SecretBase> GetSecretAsync(string key, CancellationToken token);
        Task<Contracts.Data.SecretBase> GetSecretAsync(Uri secretUri, CancellationToken token);

        Task<IEnumerable<Contracts.Data.SecretBase>> GetSecretsAsync(CancellationToken token);

        Task<Contracts.Data.Secret> GetConfiguredSecretAsync(string key, CancellationToken token);
        Task<Contracts.Data.Secret> GetConfiguredSecretAsync(Uri secretUri, CancellationToken token);
        Task<IEnumerable<Contracts.Data.Secret>> GetConfiguredSecretsAsync(CancellationToken token);

        Task<string> SaveSecretAsync(Contracts.Data.Secret secret, CancellationToken token);
        Task DeleteSecretAsync(string key, CancellationToken token);
        Task DeleteSecretAsync(Uri secretUri, CancellationToken token);

        Task<IEnumerable<Contracts.Data.SecretPolicy>> GetPoliciesAsync(CancellationToken token);
        Task<Contracts.Data.SecretPolicy> GetPolicyAsync(string policyId, CancellationToken token);
        Task DeletePolicyAsync(string policyId, CancellationToken token);
        Task<string> SavePolicyAsync(Contracts.Data.SecretPolicy policy, CancellationToken token);


        Task<Contracts.Data.SecretConfiguration> GetSecretConfigurationAsync(string configId, CancellationToken token);
        Task<IEnumerable<Contracts.Data.SecretConfiguration>> GetSecretConfigurationsAsync(CancellationToken token);

        Task<Contracts.Data.SecretConfigurationBase> GetConfigurationAsync(string configId, CancellationToken token);
        Task<IEnumerable<Contracts.Data.SecretConfigurationBase>> GetConfigurationsAsync(CancellationToken token);
        Task DeleteConfigurationAsync(string configId, CancellationToken token);
        Task<string> SaveConfigurationAsync(Contracts.Data.SecretConfiguration config, CancellationToken token);

        Task AddSecretDependencyAsync(string dependsOnKey, string dependentKey, CancellationToken token);
        Task RemoveSecretDependencyAsync(string dependsOnKey, string dependentKey, CancellationToken token);
        Task<IEnumerable<Contracts.Data.Secret>> GetDependentSecretsAsync(string dependsOnKey, CancellationToken token);

    }
}
