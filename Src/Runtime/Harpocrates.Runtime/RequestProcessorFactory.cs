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
        private Common.Tracking.IProcessingTracker _tracker;
        public RequestProcessorFactory(Common.Configuration.IConfigurationManager config, Common.Tracking.IProcessingTracker tracker, ILogger logger)
        {
            _logger = logger;
            _config = config;
            _tracker = tracker;
        }

        public async Task<ProcessResult> ProcessRequestAsync(ProcessRequest request, CancellationToken token)
        {
            if (request is RawProcessRequest)
            {
                Processors.IRequestProcessor<RawProcessRequest> processor = new Processors.RawRequestProcessor(_config, _tracker, _logger);
                return await processor.ProcessRequestAsync(request as RawProcessRequest, token);
            }
            else if (request is FormattedProcessRequest)
            {
                //TODO: refactor Action vs Event -- current implementation a little confusing
                FormattedProcessRequest formattedRequest = request as FormattedProcessRequest;

                Processors.IRequestProcessor<FormattedProcessRequest> processor = null;
                switch (formattedRequest.Action)
                {
                    case FormattedProcessRequest.RequestedAction.Rotate:
                        processor = new Processors.SecretExpiringRequestProcessor(_config, _tracker, _logger);
                        break;
                    case FormattedProcessRequest.RequestedAction.Cleanup:
                        processor = new Processors.SecretExpiredRequestProcessor(_config, _tracker, _logger);
                        break;
                    case FormattedProcessRequest.RequestedAction.ScheduleDependencyUpdates:
                        processor = new Processors.ScheduleDependencyUpdatesRequestProcessor(_config, _tracker, _logger);
                        break;
                    case FormattedProcessRequest.RequestedAction.PerformDependencyUpdate:
                        processor = new Processors.SecretVersionCreatedRequestProcessor(_config, _tracker, _logger);
                        break;
                }

                //unknown request Action
                if (null == processor) throw new InvalidOperationException($"No processor has been identified for operation of type: {formattedRequest.Action}");

                return await processor.ProcessRequestAsync(formattedRequest, token);
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
