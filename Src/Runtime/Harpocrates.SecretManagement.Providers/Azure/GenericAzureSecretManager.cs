using Harpocrates.Runtime.Common.Configuration;
using Harpocrates.SecretManagement.Contracts.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers.Azure
{
    internal class GenericAzureSecretManager : AzureSecretManager
    {
        public GenericAzureSecretManager(IConfigurationManager config, ILogger logger) : base(config, logger)
        {
        }

        protected override Task<Key> OnRotateSecretAsync(Secret secret, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
