
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


        protected async override Task<SecretBase> OnGetSecretAsync(string key, CancellationToken token)
        {
            string json = await GetObjectAsync(_rootContainer, FormatFileName(StorageFolders.Secret, key), token);
            if (string.IsNullOrWhiteSpace(json)) return null;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Contracts.Secret>(json);
        }

        protected async override Task<Secret> OnGetConfiguredSecretAsync(string key, CancellationToken token)
        {
            SecretBase s = await GetSecretAsync(key, token);

            if (null != token && token.IsCancellationRequested) return null; //cancel

            Secret cs = s as Secret; //should never really happen give the code flow...
            if (null == cs)
            {
                cs = new Secret()
                {
                    ObjectName = s.ObjectName,
                    ObjectType = s.ObjectType,
                    VaultName = s.VaultName,
                    Version = s.Version
                };

            }
            if (s is Contracts.Secret)
            {
                cs.Configuration = await GetConfigurationAsync((s as Contracts.Secret).ConfigurationId.ToString(), token);
            }

            return cs;
        }

        protected async override Task<SecretPolicy> OnGetPolicyAsync(string policyId, CancellationToken token)
        {
            string json = await GetObjectAsync(_rootContainer, FormatFileName(StorageFolders.Policy, policyId), token);
            if (string.IsNullOrWhiteSpace(json)) return null;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Contracts.Policy>(json);
        }

        protected async override Task<SecretConfiguration> OnGetConfigurationAsync(string configId, CancellationToken token)
        {
            string json = await GetObjectAsync(_rootContainer, FormatFileName(StorageFolders.Config, configId), token);
            if (string.IsNullOrWhiteSpace(json)) return null;

            var cfg = Newtonsoft.Json.JsonConvert.DeserializeObject<Contracts.Config>(json);

            SecretConfiguration result = null;

            if (null != cfg)
            {
                result = cfg.ToSecretConfiguration();
                if (result.Policy.PolicyId != Guid.Empty)
                {
                    result.Policy = await GetPolicyAsync(result.Policy.PolicyId.ToString(), token);
                }
            }

            return result;
        }


        protected async override Task<string> OnSavePolicyAsync(SecretPolicy policy, CancellationToken token)
        {
            Contracts.Policy sp = policy as Contracts.Policy;
            if (null == sp)
            {
                sp = new Contracts.Policy()
                {
                    RotationInterval = policy.RotationInterval,
                    Name = policy.Name,
                    Description = policy.Description,
                    PolicyId = policy.PolicyId
                };
            }

            if (Guid.Empty == sp.PolicyId) sp.PolicyId = Guid.NewGuid();

            await SaveObjectAsync(_rootContainer, FormatFileName(StorageFolders.Policy, sp.PolicyId.ToString()), Newtonsoft.Json.JsonConvert.SerializeObject(sp), token);
            return sp.PolicyId.ToString();
        }

        protected async override Task<string> OnSaveSecretAsync(Secret secret, CancellationToken token)
        {
            Contracts.Secret ss = new Contracts.Secret()
            {
                Name = secret.Name,
                Description = secret.Description,
                CurrentKeyName = secret.CurrentKeyName,
                ObjectName = secret.ObjectName,
                ObjectType = secret.ObjectType,
                VaultName = secret.VaultName,
                Version = secret.Version,
                ConfigurationId = secret.Configuration == null ? Guid.Empty : secret.Configuration.ConfigurationId //should never happen!!!
                //todo: catch above in data validation
            };

            //if (ss.ConfigurationId != Guid.Empty)
            //{

            //}

            //if (Guid.Empty == ss.ConfigurationId) ss.ConfigurationId = Guid.NewGuid();

            ////todo: what to we do w/ Policy here?

            //if (Guid.Empty == ss.PolicyId && null != ss.Policy)
            //{
            //    if (Guid.TryParse(await SavePolicyAsync(ss.Policy, token), out Guid id))
            //    {
            //        ss.PolicyId = id;
            //    }
            //}

            await SaveObjectAsync(_rootContainer, FormatFileName(StorageFolders.Config, ss.Key), Newtonsoft.Json.JsonConvert.SerializeObject(ss), token);

            return ss.ConfigurationId.ToString();
        }

        protected async override Task<string> OnSaveConfigurationAsync(SecretConfiguration config, CancellationToken token)
        {
            Contracts.Config sc = Contracts.Config.FromSecretConfiguration(config);

            if (Guid.Empty == sc.ConfigurationId) sc.ConfigurationId = Guid.NewGuid();

            //todo: what to we do w/ Policy here?

            //if (Guid.Empty == sc.PolicyId && null != sc.Policy)
            //{
            //    if (Guid.TryParse(await SavePolicyAsync(sc.Policy, token), out Guid id))
            //    {
            //        sc.PolicyId = id;
            //    }
            //}

            await SaveObjectAsync(_rootContainer, FormatFileName(StorageFolders.Config, sc.ConfigurationId.ToString()), Newtonsoft.Json.JsonConvert.SerializeObject(sc), token);

            return sc.ConfigurationId.ToString();
        }

        protected async override Task OnDeleteSecretAsync(string key, CancellationToken token)
        {
            throw new NotImplementedException();
        }
        protected async override Task OnDeleteConfigurationAsync(string configId, CancellationToken token)
        {
            throw new NotImplementedException();
        }
        protected async override Task OnDeletePolicyAsync(string policyId, CancellationToken token)
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

        private static string FormatFileName(string folder, string id)
        {
            return $"{folder}/{id.ToLower()}.json";
        }
    }
}
