
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

        protected async override Task OnDeleteSecretAsync(string key, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        protected async override Task<Secret> OnGetSecretAsync(string key, CancellationToken token)
        {
            //TODO: Should we prune data?
            return await OnGetConfiguredSecretAsync(key, token);
        }

        protected async override Task<ConfiguredSecret> OnGetConfiguredSecretAsync(string key, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        protected async override Task<SecretPolicy> OnGetPolicyAsync(string policyId, CancellationToken token)
        {
            throw new NotImplementedException();
        }



        protected async override Task OnSavePolicyAsync(SecretPolicy policy, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        protected async override Task OnSaveSecretAsync(ConfiguredSecret secret, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
