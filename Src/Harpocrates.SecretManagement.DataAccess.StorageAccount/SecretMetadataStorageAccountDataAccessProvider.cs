
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Harpocrates.SecretManagement.Contracts.Data;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Security;
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
            public const string Associations = "associations";
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

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Contracts.Secret>(json).ToConfiguredSecret();
        }

        protected override async Task<IEnumerable<SecretBase>> OnGetSecretsAsync(CancellationToken token)
        {
            return await GetObjectsAsync<SecretBase>(_rootContainer, $"{StorageFolders.Secret}/", GetSecretAsync, token);
        }
        protected async override Task<Secret> OnGetConfiguredSecretAsync(string key, CancellationToken token)
        {
            SecretBase sb = await GetSecretAsync(key, token);

            if (null == sb) return null; //couldn't find base config, nothing to do here..

            if (null != token && token.IsCancellationRequested) return null; //cancel

            Secret s = sb as Secret; //should never really happen give the code flow...
            if (null == s)
            {
                s = sb.ToConfiguredSecret();
            }

            if (null == s.Configuration)
            {
                Contracts.Secret cs = sb as Contracts.Secret;

                if (null != cs)
                {
                    s.Configuration = await GetSecretConfigurationAsync(cs.ConfigurationId.ToString(), token);
                }
            }
            else if (Guid.Empty != s.Configuration.ConfigurationId &&
                    (string.IsNullOrWhiteSpace(s.Name) || (s.SecretType == SecretType.Attached && null == s.Configuration.Policy)))
            {
                s.Configuration = await GetSecretConfigurationAsync(s.Configuration.ConfigurationId.ToString(), token);
            }


            //Contracts.Secret cs = sb as Contracts.Secret;

            //if (null != cs)
            //{
            //    s.Configuration = await GetSecretConfigurationAsync(cs.ConfigurationId.ToString(), token);
            //}

            return s;
        }

        protected override async Task<IEnumerable<Secret>> OnGetConfiguredSecretsAsync(CancellationToken token)
        {
            return await GetObjectsAsync<Secret>(_rootContainer, $"{StorageFolders.Secret}/", GetConfiguredSecretAsync, token);
        }

        protected async override Task<SecretPolicy> OnGetPolicyAsync(string policyId, CancellationToken token)
        {
            string json = await GetObjectAsync(_rootContainer, FormatFileName(StorageFolders.Policy, policyId), token);
            if (string.IsNullOrWhiteSpace(json)) return null;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Contracts.Policy>(json);
        }

        protected override async Task<IEnumerable<SecretPolicy>> OnGetPoliciesAsync(CancellationToken token)
        {

            return await GetObjectsAsync<SecretPolicy>(_rootContainer, $"{StorageFolders.Policy}/", GetPolicyAsync, token);

            //List<Task<SecretPolicy>> workers = new List<Task<SecretPolicy>>();

            ////List<string> list = new List<string>();

            //string continuationToken = null;
            //List<SecretPolicy> policies = new List<SecretPolicy>();

            //do
            //{
            //    try
            //    {
            //        var resultSegment = _rootContainer.GetBlobs(prefix: $"{StorageFolders.Policy}/").AsPages(continuationToken, 50);

            //        foreach (Azure.Page<BlobItem> blobPage in resultSegment)
            //        {

            //            foreach (BlobItem blobItem in blobPage.Values)
            //            {
            //                string policyId = System.IO.Path.GetFileNameWithoutExtension(blobItem.Name);

            //                //schedule work to be done here
            //                workers.Add(GetPolicyAsync(policyId, token));
            //            }
            //            continuationToken = blobPage.ContinuationToken;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        throw;
            //    }
            //} while (false == string.IsNullOrWhiteSpace(continuationToken));

            //Task.WaitAll(workers.ToArray(), token);

            //foreach (var t in workers)
            //{
            //    policies.Add(t.Result);
            //}

            //return policies.AsReadOnly();
        }

        protected async override Task<SecretConfiguration> OnGetSecretConfigurationAsync(string configId, CancellationToken token)
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

        protected override async Task<IEnumerable<SecretConfiguration>> OnGetSecretConfigurationsAsync(CancellationToken token)
        {
            return await GetObjectsAsync<SecretConfiguration>(_rootContainer, $"{StorageFolders.Config}/", GetSecretConfigurationAsync, token);
        }

        protected async override Task<SecretConfigurationBase> OnGetConfigurationAsync(string configId, CancellationToken token)
        {
            string json = await GetObjectAsync(_rootContainer, FormatFileName(StorageFolders.Config, configId), token);
            if (string.IsNullOrWhiteSpace(json)) return null;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Contracts.Config>(json);
        }

        protected async override Task<IEnumerable<SecretConfigurationBase>> OnGetConfigurationsAsync(CancellationToken token)
        {
            return await GetObjectsAsync<SecretConfigurationBase>(_rootContainer, $"{StorageFolders.Config}/", GetConfigurationAsync, token);
        }

        protected async override Task<string> OnSavePolicyAsync(SecretPolicy policy, CancellationToken token)
        {
            Contracts.Policy sp = policy as Contracts.Policy;
            if (null == sp)
            {
                sp = new Contracts.Policy()
                {
                    RotationIntervalInSec = policy.RotationIntervalInSec,
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
                FormatExpression = secret.FormatExpression,
                SecretType = secret.SecretType,
                LastRotatedOn = secret.LastRotatedOn
            };

            if (secret.Configuration != null)
            {
                ss.ConfigurationId = secret.Configuration.ConfigurationId;
            }

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


        protected async override Task OnAddSecretDependencyAsync(string dependsOnKey, string dependentKey, CancellationToken token)
        {
            string fileName = FormatAssociationFileName(dependsOnKey, dependentKey);
            string json = $"{{\"depedendent\": \"{dependentKey}\", \"depedendsOn\": \"{dependsOnKey}\"}}";

            await SaveObjectAsync(_rootContainer, fileName, json, token);
        }

        protected async override Task<IEnumerable<Secret>> OnGetDependentSecretsAsync(string dependsOnKey, CancellationToken token)
        {
            List<Task<Secret>> workers = new List<Task<Secret>>();

            //List<string> list = new List<string>();

            string continuationToken = null;
            List<Secret> secrets = new List<Secret>();

            do
            {
                try
                {
                    var resultSegment = _rootContainer.GetBlobs(prefix: $"{StorageFolders.Associations}/{dependsOnKey.ToLower()}/").AsPages(continuationToken, 50);

                    foreach (Azure.Page<BlobItem> blobPage in resultSegment)
                    {

                        foreach (BlobItem blobItem in blobPage.Values)
                        {
                            string dependencyKey = System.IO.Path.GetFileNameWithoutExtension(blobItem.Name);

                            //schedule work to be done here
                            workers.Add(GetConfiguredSecretAsync(dependencyKey, token));
                        }
                        continuationToken = blobPage.ContinuationToken;
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            } while (false == string.IsNullOrWhiteSpace(continuationToken));

            Task.WaitAll(workers.ToArray(), token);

            foreach (var t in workers)
            {
                secrets.Add(t.Result);
            }

            return secrets.AsReadOnly();
        }

        protected async override Task OnRemoveSecretDependencyAsync(string dependsOnKey, string dependentKey, CancellationToken token)
        {
            string fileName = FormatAssociationFileName(dependsOnKey, dependentKey);
            await DeleteObjectAsync(_rootContainer, fileName, token);
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

                using (System.IO.Stream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
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

        private static string FormatAssociationFileName(string dependsOn, string dependent)
        {
            return FormatFileName($"{StorageFolders.Associations}/{dependsOn.ToLower()}", dependent);
        }
        private static string FormatFileName(string folder, string id)
        {
            return $"{folder}/{id.ToLower()}.json";
        }

        private delegate Task<T> GetSingularObject<T>(string id, CancellationToken token);
        private static async Task<IEnumerable<T>> GetObjectsAsync<T>(BlobContainerClient client, string prefix, GetSingularObject<T> action, CancellationToken token)
        {
            List<Task<T>> workers = new List<Task<T>>();

            string continuationToken = null;
            List<T> list = new List<T>();

            do
            {
                try
                {
                    var resultSegment = client.GetBlobs(prefix: prefix).AsPages(continuationToken, 50);

                    foreach (Azure.Page<BlobItem> blobPage in resultSegment)
                    {

                        foreach (BlobItem blobItem in blobPage.Values)
                        {
                            string policyId = System.IO.Path.GetFileNameWithoutExtension(blobItem.Name);

                            //schedule work to be done here
                            workers.Add(action(policyId, token));
                        }
                        continuationToken = blobPage.ContinuationToken;
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            } while (false == string.IsNullOrWhiteSpace(continuationToken));

            Task.WaitAll(workers.ToArray(), token);

            foreach (var t in workers)
            {
                list.Add(t.Result);
            }

            return list.AsReadOnly();


        }

    }
}
