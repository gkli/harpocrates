using Harpocrates.Runtime.Common.Contracts;
using Harpocrates.Runtime.Common.Contracts.Tracking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Harpocrates.Runtime.Common.Tracking
{
    public class ConsoleProcessingTrackerDataAccessProvider : ProcessingTrackerDataAccessProvider
    {
        public ConsoleProcessingTrackerDataAccessProvider() : base(null) { }
        protected override async Task OnRecordTransactionEndAsync(TrackingContext context)
        {
            Console.WriteLine($"Ending. Tx: {context.Request.ParentTransactionId} - {context.Request.TransactionId}, Result:{context.Result.Status}, TimeStamp: {context.EndTime}");
        }

        protected override async Task OnRecordTransactionStartAsync(TrackingContext context)
        {
            Console.WriteLine($"Starting. Tx: {context.Request.ParentTransactionId} - {context.Request.TransactionId}, Result:{context.Result.Status}, TimeStamp: {context.StartTime}");
        }

        protected override async Task<IEnumerable<Transaction>> OnGetTransactionsAsync(DateTime fromUtc, DateTime toUtc)
        {
            return await Task.FromResult(new Transaction[] { }); //console provider does not return results
        }

        protected override Task<Transaction> OnGetTransactionAsync(Guid id)
        {
            return null;
        }
    }
}
