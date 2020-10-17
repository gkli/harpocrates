using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.Contracts.Tracking
{
    public class ProcessingAttempt
    {
        public ProcessResult.ProcessingStatus StartingStatus { get; set; }
        public ProcessResult.ProcessingStatus? EndingStatus { get; set; }

        public DateTime StartTimeUtc { get; set; }
        public DateTime? EndTimeUtc { get; set; }
    }
}
