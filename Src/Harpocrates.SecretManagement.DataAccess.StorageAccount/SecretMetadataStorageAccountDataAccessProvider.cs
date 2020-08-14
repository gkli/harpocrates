
using Harpocrates.SecretManagement.Contracts.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.DataAccess.StorageAccount
{
    public class SecretMetadataStorageAccountDataAccessProvider : SecretMetadataDataAccessProvider
    {
        public SecretMetadataStorageAccountDataAccessProvider(Runtime.Common.DataAccess.ConnectionStrings.CQRSStorageAccountConnectionString connectionString) : base(connectionString)
        {
            
        }

        protected new Runtime.Common.DataAccess.ConnectionStrings.CQRSStorageAccountConnectionString ConnectionString
        {
            get
            {
                return base.ConnectionString as Runtime.Common.DataAccess.ConnectionStrings.CQRSStorageAccountConnectionString;
            }
        }

        protected override Task OnDeleteSecretAsync(string secreteId, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        protected override Task<SecretPolicy> OnGetPolicyAsync(string policyId, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        protected override Task<Secret> OnGetSecretAsync(string secretId, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        protected override Task<SecretConfiguration> OnGetSecretConfigurationAsync(string secretId, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        protected override Task<SecretPolicy> OnGetSecretPolicy(string secretId, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        protected override Task OnSavePolicyAsync(SecretPolicy policy, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        protected override Task OnSaveSecretAsync(Secret secret, SecretPolicy policy, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
