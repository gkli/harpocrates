using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.Contracts
{
    public class ProcessResult
    {
        [Flags]
        public enum ProcessingStatus
        {
            Pending = 1,
            Success = 2,
            Failed = 4,
            Aborted = 8,
            RetryRequested = 16,
            DeadLetter = 32,
            DeleteMessage = 64,
            Skipped = 128
        }

        public ProcessingStatus Status { get; set; }
        public string Description { get; set; }
    }
}
