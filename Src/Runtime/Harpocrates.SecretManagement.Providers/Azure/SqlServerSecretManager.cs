using Harpocrates.Runtime.Common.Configuration;
using Harpocrates.SecretManagement.Contracts.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers.Azure
{
    internal class SqlServerSecretManager : AzureSecretManager
    {
        public SqlServerSecretManager(IConfigurationManager config) : base(config)
        {
        }

        protected override Task<Key> OnRotateSecretAsync(Secret secret,  CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
