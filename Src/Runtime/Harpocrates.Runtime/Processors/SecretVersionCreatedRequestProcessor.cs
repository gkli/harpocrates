using Harpocrates.Runtime.Common.Configuration;
using Harpocrates.Runtime.Common.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.Runtime.Processors
{
    internal class SecretVersionCreatedRequestProcessor : SecretEventRequestProcessor
    {
        public SecretVersionCreatedRequestProcessor(IConfigurationManager config, ILogger logger) : base(config, logger)
        {
        }


    }
}
