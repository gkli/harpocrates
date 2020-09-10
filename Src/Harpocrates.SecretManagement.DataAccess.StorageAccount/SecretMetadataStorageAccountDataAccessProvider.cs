
using Azure.Storage.Blobs;
using Harpocrates.SecretManagement.Contracts.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.DataAccess.StorageAccount
{
    public class SecretMetadataStorageAccountDataAccessProvider : SecretMetadataDataAccessProvider
    {
        private static class StorageFolders
        {
            public const string Secret = "secrets";
            public const string Policy = "policies";
            public const string Config = "configurations";
        }

        private readonly BlobContainerClient _rootContainer;
        public SecretMetadataStorageAccountDataAccessProvider(Runtime.Common.DataAccess.ConnectionStrings.CQRSStorageAccountConnectionString connectionString) : base(connectionString)
        {
            //determine container uri from connectionString...

            //_rootContainer = BlobClientHelper.CreateBlobContainerClient(containerUri, Config);
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
            //return await OnGetConfiguredSecretAsync(key, token);
            throw new NotImplementedException();
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

        protected async override Task OnDeletePolicyAsync(string policyId, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        protected async override Task<SecretConfiguration> OnGetConfigurationAsync(string configId, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        protected async override Task OnDeleteConfigurationAsync(string configId, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        protected async override Task OnSaveConfigurationAsync(SecretConfiguration config, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        private static async Task SaveObjectAsync(BlobContainerClient client, string fileName, string json, CancellationToken token)
        {
            using (System.IO.Stream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF32.GetBytes(json)))
            {
                await client.UploadBlobAsync(fileName, ms, token);
            }
        }
        private static async Task<string> GetObjectAsync(BlobContainerClient client, string fileName, CancellationToken token)
        {
            BlobClient blobClient = client.GetBlobClient(fileName);

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                await blobClient.DownloadToAsync(ms, token);

                using (System.IO.StreamReader sr = new System.IO.StreamReader(ms))
                    return await sr.ReadToEndAsync();
            }
        }
        private static async Task DeleteObjectAsync(BlobContainerClient client, string fileName, CancellationToken token)
        {
            await client.DeleteBlobIfExistsAsync(fileName, cancellationToken: token);
        }
    }
}
