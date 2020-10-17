using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Harpocrates.Runtime.Common.Tracking
{
    public interface IProcessingTrackerDataAccessProvider
    {
        Task RecordTransactionStartAsync(TrackingContext context);
        Task RecordTransactionEndAsync(TrackingContext context);

        Task<IEnumerable<Contracts.Tracking.Transaction>> GetTransactionsAsync(DateTime from, DateTime to);

        Task<Contracts.Tracking.Transaction> GetTransactionAsync(Guid id);
    }
}
