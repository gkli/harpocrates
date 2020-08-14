using Harpocrates.SecretManagement.Contracts.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement
{
    public class SecretMetadataManager : ISecretMetadataManager
    {
        private readonly DataAccess.ISecretMetadataDataAccessProvider _dataProvider;
        private Runtime.Common.Configuration.IConfigurationManager _config;
        private readonly ILogger _logger;
        public SecretMetadataManager(Runtime.Common.Configuration.IConfigurationManager config, ILogger logger)
        {
            //_dataProvider = dataProvider;
            _config = config;
            _logger = logger;

            //dataProvider = get from config

        }

        public Task ProcessExpiringSecretAsync(string secretUri, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task ProcessExpiredSecretAsync(string secretUri, CancellationToken token)
        {
            throw new NotImplementedException();
        }



        //public async Task UpdateSecretAsync(string secretId, CancellationToken token)
        //{
        //    //add hash value to secret record, this would tell us if we're the one's that changed the secret or if it was changed outside the system
        //    //add event for SecreteChanged --- does this buy us anything? what would happen if someone changes secret value at the origin? can we get synced up?
        //    //if changed outside the system, do we care? assume whoever changed KV value, synced up w/ origin?

        //    Secret secret = await _dataProvider.GetSecretAsync(secretId, token);
        //    SecretConfiguration cfg = await _dataProvider.GetSecretConfigurationAsync(secretId, token);
        //    SecretPolicy policy = await _dataProvider.GetSecretPolicy(secretId, token);

        //    //update Secret Provider (Cosmosb, Storage Account, etc)
        //    //update KV secret

        //    throw new NotImplementedException();
        //}

        //public async Task SaveSecretAsync(Secret secret, SecretPolicy policy, CancellationToken token)
        //{
        //    throw new NotImplementedException();
        //    //save secret metadata
        //}

        //public async Task DeleteSecretAsync(string secretId, CancellationToken token)
        //{
        //    throw new NotImplementedException();
        //}

      
    }
}
