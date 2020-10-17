using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.Tracking
{
    public interface IProcessingTracker
    {
        TrackingContext GetTrackingContext(Contracts.ProcessRequest request, Contracts.ProcessResult result);
    }
}
