using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Harpocrates.Runtime.Tracking.SqlServer.Ef.Entities
{
    public class Transaction
    {
        public Transaction()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key]
        public Guid TransactionId { get; set; }
        public Transaction ParentTransaction { get; set; }

        // public Common.Contracts.ProcessResult.ProcessingStatus Status { get; set; }
        public Common.Contracts.FormattedProcessRequest.RequestedAction Action { get; set; }
        public Common.Contracts.FormattedProcessRequest.SecretEvent Event { get; set; }

        public string SecretUri { get; set; }
        public string SecretKey { get; set; }

        public IEnumerable<Attempt> Attempts { get; set; }

        public Common.Contracts.Tracking.Transaction ToCommonTransaction()
        {
            Common.Contracts.Tracking.Transaction tx = new Common.Contracts.Tracking.Transaction()
            {
                TransactionId = TransactionId,
                Action = Action,
                Event = Event,
                ParentTransaction = ParentTransaction?.ToCommonTransaction(),
                SecretKey = SecretKey,
                SecretUri = SecretUri
            };

            DateTime start = DateTime.UtcNow; DateTime? end = null;
            Common.Contracts.ProcessResult.ProcessingStatus startingStatus = Common.Contracts.ProcessResult.ProcessingStatus.Pending;
            Common.Contracts.ProcessResult.ProcessingStatus? endingStatus = null;

            List<Common.Contracts.Tracking.ProcessingAttempt> attempts = new List<Common.Contracts.Tracking.ProcessingAttempt>();

            if (null != Attempts)
            {
                foreach (var attempt in Attempts)
                {
                    if (attempt.StartTimeUtc <= start)
                    {
                        start = attempt.StartTimeUtc;
                        startingStatus = attempt.StartingStatus;
                    }

                    if (false == end.HasValue) end = attempt.EndTimeUtc;

                    if (attempt.EndTimeUtc.HasValue && attempt.EndTimeUtc.Value >= end.Value)
                    {
                        end = attempt.EndTimeUtc;
                        endingStatus = attempt.EndingStatus;
                    }

                    attempts.Add(attempt);
                }
            }

            tx.Attempts = attempts;
            tx.StartedOnUtc = start;
            tx.EndedOnUtc = end;
            tx.Status = endingStatus.HasValue ? endingStatus.Value : startingStatus;

            return tx;
        }

    }
}
