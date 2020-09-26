using Harpocrates.Runtime.Common.Contracts;
using Harpocrates.SecretManagement;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.Runtime.Processors
{
    internal class SecretExpiringRequestProcessor : SecretEventRequestProcessor
    {
        public SecretExpiringRequestProcessor(Common.Configuration.IConfigurationManager config, ILogger logger) : base(config, logger)
        {
        }

        protected override async Task InvokeSecretMetadataManagerMethodAsync(ISecretMetadataManager manager, FormattedProcessRequest request, CancellationToken token)
        {
            await manager.ProcessExpiringSecretAsync(request.ObjectUri, token);
        }
    }
}
