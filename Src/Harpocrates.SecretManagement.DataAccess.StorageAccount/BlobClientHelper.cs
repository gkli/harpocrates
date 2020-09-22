using System;
using Azure.Identity;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.DataAccess.StorageAccount
{
    internal class BlobClientHelper
    {
        public static Azure.Storage.Blobs.BlobContainerClient CreateBlobContainerClient(Uri containerUri)
        {
            //todo: refactor to enable various crednetials...

            return new Azure.Storage.Blobs.BlobContainerClient(containerUri, new DefaultAzureCredential());
        }

        public static Azure.Storage.Blobs.BlobContainerClient CreateBlobContainerClient(string connectionString, string containerName)
        {
            //todo: refactor to enable various crednetials...

            return new Azure.Storage.Blobs.BlobContainerClient(connectionString, containerName);
        }

        private static Dictionary<string, Azure.Storage.Blobs.BlobContainerClient> _clientCache = new Dictionary<string, Azure.Storage.Blobs.BlobContainerClient>();
        public static Azure.Storage.Blobs.BlobContainerClient CreateBlobContainerClient(Runtime.Common.Configuration.IConfigurationManager config, string containerName)
        {
            Runtime.Common.DataAccess.ConnectionStrings.StorageAccountConnectionString sacs = config.SecretManagementConnectionString.CommandConnectionString;

            string cacheKey = $"connection={sacs.ConnectionString.ToLower()};queue={containerName}";

            if (_clientCache.ContainsKey(cacheKey)) return _clientCache[cacheKey];

            Uri uri = GetStorageBlobContainerUri(containerName, sacs);

            switch (sacs.KeyType)
            {
                case Runtime.Common.DataAccess.ConnectionStrings.StorageAccountConnectionString.AccountKeyType.None:
                    return EnsureClientCache(cacheKey, new Azure.Storage.Blobs.BlobContainerClient(uri, new DefaultAzureCredential()));
                case Runtime.Common.DataAccess.ConnectionStrings.StorageAccountConnectionString.AccountKeyType.SAS:
                    return EnsureClientCache(cacheKey, new Azure.Storage.Blobs.BlobContainerClient(uri));
                case Runtime.Common.DataAccess.ConnectionStrings.StorageAccountConnectionString.AccountKeyType.AccountKey:
                    return EnsureClientCache(cacheKey, new Azure.Storage.Blobs.BlobContainerClient(sacs.ToStorageAccountFormat(), containerName));
            }

            return null;
        }



        public static Azure.Storage.Blobs.BlobContainerClient CreateBlobContainerClient(Uri containerUri, Runtime.Common.Configuration.IConfigurationManager config)
        {
            //todo: refactor to enable various crednetials...

            string cacheKey = containerUri.ToString().ToLower();

            if (_clientCache.ContainsKey(cacheKey)) return _clientCache[cacheKey];

            return EnsureClientCache(cacheKey, new Azure.Storage.Blobs.BlobContainerClient(containerUri, new DefaultAzureCredential()));
        }

        private static Azure.Storage.Blobs.BlobContainerClient EnsureClientCache(string key, Azure.Storage.Blobs.BlobContainerClient client)
        {

            lock (_clientCache)
            {
                if (false == _clientCache.ContainsKey(key))
                {
                    _clientCache.Add(key, client);
                }
            }

            return client;
        }

        public static Uri GetStorageBlobContainerUri(string containerName, Runtime.Common.DataAccess.ConnectionStrings.StorageAccountConnectionString sacs)
        {
            //check if KeyType = SAS and change URL...

            Uri uri = new Uri(new Uri(sacs.AccountEndpoint), containerName);

            if (sacs.KeyType == Runtime.Common.DataAccess.ConnectionStrings.StorageAccountConnectionString.AccountKeyType.SAS)
            {
                //todo: add sas query string
            }

            return uri;
        }


    }
}
