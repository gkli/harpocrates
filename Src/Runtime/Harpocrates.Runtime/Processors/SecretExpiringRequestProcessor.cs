using Harpocrates.Runtime.Common.Contracts;
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
    }
}
