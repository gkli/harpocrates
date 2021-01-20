using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.Contracts.Tracking
{
    public enum TransactionPurpose
    {
        Unknown = -1,
        ProcessKVEvent = 1,
        ExecuteRotationProcess
    }
    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public Transaction ParentTransaction { get; set; }

        public ProcessResult.ProcessingStatus Status { get; set; }
        public FormattedProcessRequest.RequestedAction Action { get; set; }
        public FormattedProcessRequest.SecretEvent Event { get; set; }
        public TransactionPurpose Purpose { get; set; }

        public string SecretUri { get; set; }
        public string SecretKey { get; set; }

        public IEnumerable<ProcessingAttempt> Attempts { get; set; }

        public DateTime StartedOnUtc { get; set; }
        public DateTime? EndedOnUtc { get; set; }
    }
}
