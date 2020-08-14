using Harpocrates.Runtime.Common.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.Runtime
{
    internal class RequestProcessorFactory : Processors.IRequestProcessor<ProcessRequest>
    {
        private ILogger _logger;
        private Common.Configuration.IConfigurationManager _config;
        public RequestProcessorFactory(Common.Configuration.IConfigurationManager config, ILogger logger)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<ProcessResult> ProcessRequestAsync(ProcessRequest request, CancellationToken token)
        {
            if (request is RawProcessRequest)
            {
                Processors.IRequestProcessor<RawProcessRequest> processor = new Processors.RawRequestProcessor(_config, _logger);
                return await processor.ProcessRequestAsync(request as RawProcessRequest, token);
            }
            else if (request is FormattedProcessRequest)
            {
                Processors.IRequestProcessor<FormattedProcessRequest> processor = new Processors.SecretExpiringRequestProcessor(_config, _logger);
               
                //unknown request Action
                if (null == processor) throw new InvalidOperationException();

                return await processor.ProcessRequestAsync(request as FormattedProcessRequest, token);
            }
            //else if (request is SchedulingProcessRequest)
            //{
            //    Processors.IRequestProcessor<SchedulingProcessRequest> processor = new Processors.SchedulingRequestProcessor(_config, _logger);
            //    return await processor.ProcessRequestAsync(request as SchedulingProcessRequest, token);
            //}

            //unknown request Type
            throw new InvalidOperationException();
        }
    }
}
