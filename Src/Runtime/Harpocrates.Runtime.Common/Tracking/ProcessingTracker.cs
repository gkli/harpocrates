using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.Tracking
{
    public class ProcessingTracker : IProcessingTracker
    {
        protected ILogger Logger { get; private set; }
        protected IProcessingTrackerDataAccessProvider TrackerDataProvider { get; private set; }

        public ProcessingTracker(IProcessingTrackerDataAccessProvider tracker, ILogger logger)
        {
            Logger = logger;
            TrackerDataProvider = tracker;
        }
        public ProcessingTracker(IProcessingTrackerDataAccessProvider tracker, ILogger<ProcessingTracker> logger) : this(tracker, logger as ILogger)
        {

        }

        public TrackingContext GetTrackingContext(Contracts.ProcessRequest request, Contracts.ProcessResult result)
        {
            var ctx = new TrackingContext(request, result, TrackerDataProvider);

            ctx.StartTrackingActivity();

            return ctx;
        }
    }
}
