
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
        public SecretMetadataStorageAccountDataAccessProvider(Runtime.Common.DataAccess.ConnectionStrings.CQRSStorageAccountConnectionString connectionString, Runtime.Common.Configuration.IConfigurationManager config) : base(connectionString)
        {
            //determine container uri from connectionString...

            _rootContainer = BlobClientHelper.CreateBlobContainerClient(config, connectionString.CommandConnectionString.ContainerName);
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
            SecretBase sb = await GetSecretAsync(key, token);

            if (null != token && token.IsCancellationRequested) return null; //cancel

            Secret s = sb as Secret; //should never really happen give the code flow...
            if (null == s)
            {
                s = new Secret()
                {
                    ObjectName = sb.ObjectName,
                    ObjectType = sb.ObjectType,
                    VaultName = sb.VaultName,
                    Version = sb.Version
                };

            }

            Contracts.Secret cs = sb as Contracts.Secret;

            if (null != cs)
            {
                s.Configuration = await GetConfigurationAsync(cs.ConfigurationId.ToString(), token);

                if (null != token && token.IsCancellationRequested) return null; //cancel

                if (cs.PolicyId.HasValue && cs.PolicyId.Value != Guid.Empty) s.Policy = await GetPolicyAsync(cs.PolicyId.Value.ToString(), token);
            }

            return s;
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
                if (result.DefaultPolicy.PolicyId != Guid.Empty)
                {
                    result.DefaultPolicy = await GetPolicyAsync(result.DefaultPolicy.PolicyId.ToString(), token);
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

            await SaveObjectAsync(_rootContainer, FormatFileName(StorageFolders.Policy, sp.PolicyId.ToString()), GetObjectJson<Contracts.Policy>(sp), token);
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
                SubscriptionId = secret.SubscriptionId,
                FormatExpression = secret.FormatExpression
            };

            if (secret.Configuration != null)
            {
                ss.ConfigurationId = secret.Configuration.ConfigurationId;

                if (null != secret.Configuration.DefaultPolicy) ss.PolicyId = secret.Configuration.DefaultPolicy.PolicyId;
            }

            if (secret.Policy != null) ss.PolicyId = secret.Policy.PolicyId;

            await SaveObjectAsync(_rootContainer, FormatFileName(StorageFolders.Secret, ss.Key), GetObjectJson<Contracts.Secret>(ss), token);

            return ss.ConfigurationId.ToString();
        }

        protected async override Task<string> OnSaveConfigurationAsync(SecretConfiguration config, CancellationToken token)
        {
            Contracts.Config sc = Contracts.Config.FromSecretConfiguration(config);

            if (Guid.Empty == sc.ConfigurationId) sc.ConfigurationId = Guid.NewGuid();

            await SaveObjectAsync(_rootContainer, FormatFileName(StorageFolders.Config, sc.ConfigurationId.ToString()), GetObjectJson<Contracts.Config>(sc), token);

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



        private static string GetObjectJson<T>(T instance)
        {

            return Newtonsoft.Json.JsonConvert.SerializeObject(instance, new Newtonsoft.Json.JsonSerializerSettings()
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
        }

        private static async Task SaveObjectAsync(BlobContainerClient client, string fileName, string json, CancellationToken token)
        {
            try
            {

                using (System.IO.Stream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF32.GetBytes(json)))
                {
                    ms.Position = 0;
                    await client.GetBlobClient(fileName).UploadAsync(ms, overwrite: true, cancellationToken: token);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private static async Task<string> GetObjectAsync(BlobContainerClient client, string fileName, CancellationToken token)
        {
            try
            {
                BlobClient blobClient = client.GetBlobClient(fileName);
                if (await blobClient.ExistsAsync())
                {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        await blobClient.DownloadToAsync(ms, token);

                        ms.Position = 0;

                        using (System.IO.StreamReader sr = new System.IO.StreamReader(ms))
                            return await sr.ReadToEndAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return null;
        }
        private static async Task DeleteObjectAsync(BlobContainerClient client, string fileName, CancellationToken token)
        {
            try
            {
                await client.DeleteBlobIfExistsAsync(fileName, cancellationToken: token);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static string FormatFileName(string folder, string id)
        {
            return $"{folder}/{id.ToLower()}.json";
        }
    }
}
