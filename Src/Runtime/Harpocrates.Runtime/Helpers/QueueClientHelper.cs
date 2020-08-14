using Azure.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Helpers
{
    internal class QueueClientHelper
    {
        private static Dictionary<string, Azure.Storage.Queues.QueueClient> _clientCache = new Dictionary<string, Azure.Storage.Queues.QueueClient>();

        public static Azure.Storage.Queues.QueueClient CreateQueueClient(Common.Configuration.IConfigurationManager config, string queueName)
        {
            Common.DataAccess.ConnectionStrings.StorageAccountConnectionString sacs = config.MonitoredQueueConnectionString;

            string cacheKey = sacs.ConnectionString.ToLower();

            if (_clientCache.ContainsKey(cacheKey)) return _clientCache[cacheKey];

            Uri uri = GetMonitoredQueueUri(queueName, config);

            if (sacs.KeyType == Common.DataAccess.ConnectionStrings.StorageAccountConnectionString.AccountKeyType.None)
                return new Azure.Storage.Queues.QueueClient(uri, new DefaultAzureCredential());

            switch (sacs.KeyType)
            {
                case Common.DataAccess.ConnectionStrings.StorageAccountConnectionString.AccountKeyType.None:
                    return EnsureClientCache(cacheKey, new Azure.Storage.Queues.QueueClient(uri, new DefaultAzureCredential()));
                case Common.DataAccess.ConnectionStrings.StorageAccountConnectionString.AccountKeyType.SAS:
                    return EnsureClientCache(cacheKey, new Azure.Storage.Queues.QueueClient(uri));
            }

            return null;
        }

        public static Azure.Storage.Queues.QueueClient CreateQueueClient(Uri queueUri, Common.Configuration.IConfigurationManager config)
        {
            //todo: refactor to enable various crednetials...

            string cacheKey = queueUri.ToString().ToLower();

            if (_clientCache.ContainsKey(cacheKey)) return _clientCache[cacheKey];

            return EnsureClientCache(cacheKey, new Azure.Storage.Queues.QueueClient(queueUri, new DefaultAzureCredential()));
        }

        private static Azure.Storage.Queues.QueueClient EnsureClientCache(string key, Azure.Storage.Queues.QueueClient client)
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

        public static Uri GetMonitoredQueueUri(string queueName, Common.Configuration.IConfigurationManager config)
        {

            Common.DataAccess.ConnectionStrings.StorageAccountConnectionString sacs = config.MonitoredQueueConnectionString;

            //check if KeyType = SAS and change URL...

            Uri uri = new Uri(new Uri(sacs.AccountEndpoint), queueName);

            if (sacs.KeyType == Common.DataAccess.ConnectionStrings.StorageAccountConnectionString.AccountKeyType.SAS)
            {
                //todo: add sas query string
            }

            return uri;
        }
    }
}
