using Harpocrates.Runtime.Common.Contracts;
using Harpocrates.Runtime.Common.Contracts.Tracking;
using Harpocrates.Runtime.Common.DataAccess.ConnectionStrings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Harpocrates.Runtime.Common.Tracking
{
    public abstract class ProcessingTrackerDataAccessProvider : DataAccess.DataAccessProvider, IProcessingTrackerDataAccessProvider
    {
        protected ProcessingTrackerDataAccessProvider(CQRSConnectionStringBase connectionString) : base(connectionString)
        {
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync(DateTime fromUtc, DateTime toUtc)
        {
            if (fromUtc > DateTime.UtcNow) return new Transaction[] { };

            if (toUtc <= fromUtc) return new Transaction[] { };

            return await OnGetTransactionsAsync(fromUtc, toUtc);
        }

        public async Task<Transaction> GetTransactionAsync(Guid id)
        {
            if (Guid.Empty == id) return null;

            return await OnGetTransactionAsync(id);
        }

        public async Task RecordTransactionEndAsync(TrackingContext context)
        {
            if (null == context) throw new ArgumentNullException(nameof(context));
            if (null == context.Request) throw new ArgumentNullException(nameof(context.Request)); ;
            if (null == context.Result) throw new ArgumentNullException(nameof(context.Result)); ;

            await OnRecordTransactionEndAsync(context);

            if (context.TrackingState.ContainsKey(this)) context.TrackingState.Remove(this); //ensures instance is de-referenced once END is called...
        }

        public async Task RecordTransactionStartAsync(TrackingContext context)
        {
            if (null == context) throw new ArgumentNullException(nameof(context));
            if (null == context.Request) throw new ArgumentNullException(nameof(context.Request)); ;
            if (null == context.Result) throw new ArgumentNullException(nameof(context.Result)); ;

            await OnRecordTransactionStartAsync(context);
        }

        protected abstract Task OnRecordTransactionEndAsync(TrackingContext context);
        protected abstract Task OnRecordTransactionStartAsync(TrackingContext context);
        protected abstract Task<IEnumerable<Transaction>> OnGetTransactionsAsync(DateTime fromUtc, DateTime toUtc);
        protected abstract Task<Transaction> OnGetTransactionAsync(Guid id);
    }
}
