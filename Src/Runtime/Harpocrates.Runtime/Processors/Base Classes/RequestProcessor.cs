using Harpocrates.Runtime.Common.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.Runtime.Processors
{
    public abstract class RequestProcessor<T> : IRequestProcessor<T>
         where T : ProcessRequest
    {
        protected ILogger Logger { get; private set; }
        protected Common.Configuration.IConfigurationManager Config { get; private set; }

        protected Common.Tracking.IProcessingTracker Tracker { get; private set; }

        public RequestProcessor(Common.Configuration.IConfigurationManager config, Common.Tracking.IProcessingTracker tracker, ILogger logger)
        {
            Logger = logger;
            Config = config;
            Tracker = tracker;
        }

        public async Task<ProcessResult> ProcessRequestAsync(T request, CancellationToken token)
        {
            if (null == request) throw new ArgumentNullException(nameof(request));
            //request.Validate();

            ProcessResult result = new ProcessResult() { Status = ProcessResult.ProcessingStatus.Pending };

            using (Common.Tracking.TrackingContext context = Tracker.GetTrackingContext(request, result))
            {
                try
                {
                    await OnProcessRequestAsync(request, result, token);
                }
                catch (Exception ex)
                {
                    result.Status = ProcessResult.ProcessingStatus.Failed;
                    result.Description = ex.Message;
                }

                //remove Pending...
                result.Status &= ~ProcessResult.ProcessingStatus.Pending;

                if ((result.Status & ProcessResult.ProcessingStatus.Success) > 0)
                {
                    result.Status |= ProcessResult.ProcessingStatus.DeleteMessage; //message is safe to remove...
                    return result;
                }
                else
                {
                    result.Status |= ProcessResult.ProcessingStatus.RetryRequested;

                    return result;
                }

            } //using TrackingContext
        }

        protected abstract Task OnProcessRequestAsync(T request, ProcessResult result, CancellationToken token);



    }
}
