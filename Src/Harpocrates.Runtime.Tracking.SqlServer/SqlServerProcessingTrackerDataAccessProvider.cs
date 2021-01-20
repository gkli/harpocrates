using Harpocrates.Runtime.Common.Contracts;
using Harpocrates.Runtime.Common.Contracts.Tracking;
using Harpocrates.Runtime.Tracking.SqlServer.Ef.Entities;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harpocrates.Runtime.Tracking.SqlServer
{
    public class SqlServerProcessingTrackerDataAccessProvider : Common.Tracking.ProcessingTrackerDataAccessProvider
    {
        private readonly Ef.TrackingDbContext _dbContext;
        public SqlServerProcessingTrackerDataAccessProvider(Ef.TrackingDbContext dbContext) : base(null)
        {
            _dbContext = dbContext;

            _dbContext.Database.EnsureCreated();
        }
        protected override async Task OnRecordTransactionEndAsync(Common.Tracking.TrackingContext context)
        {
            Ef.Entities.Transaction tx = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.TransactionId == context.Request.TransactionId);

            if (null == tx) return; //what happened here? where is the transaction record???

            //find last attempt
            Ef.Entities.Attempt attempt = null;

            if (context.TrackingState.ContainsKey(this))
            {
                object state = context.TrackingState[this];
                if (null != state && state is int)
                    attempt = tx.Attempts.FirstOrDefault(i => i.AttemptId == (int)state);
            }

            if (null == attempt) attempt = tx.Attempts.LastOrDefault(); // should this ever happen or be allowed? 

            if (null != attempt)
            {
                attempt.EndingStatus = context.Result.Status;
                attempt.EndTimeUtc = context.EndTime;

                await _dbContext.SaveChangesAsync();
            }


        }

        protected override async Task OnRecordTransactionStartAsync(Common.Tracking.TrackingContext context)
        {

            Ef.Entities.Transaction tx = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.TransactionId == context.Request.TransactionId);

            if (null == tx)
            {
                string key = string.Empty, uri = string.Empty;
                FormattedProcessRequest.SecretEvent evnt = FormattedProcessRequest.SecretEvent.Unknown;
                FormattedProcessRequest.RequestedAction action = FormattedProcessRequest.RequestedAction.Unknown;
                TransactionPurpose purpose = TransactionPurpose.Unknown;

                FormattedProcessRequest fpr = context.Request as FormattedProcessRequest;
                if (null == fpr)
                {
                    fpr = (context.Request as RawProcessRequest)?.FormatRequest();
                }

                if (null != fpr)
                {
                    uri = fpr.ObjectUri;
                    evnt = fpr.Event;
                    action = fpr.Action;
                    purpose = TransactionPurpose.ExecuteRotationProcess;
                }

                if (context.Request is RawProcessRequest)
                {
                    action = FormattedProcessRequest.RequestedAction.Unknown;
                    purpose = TransactionPurpose.ProcessKVEvent;
                }

                if (!string.IsNullOrWhiteSpace(uri))
                {
                    var s = SecretManagement.Contracts.Data.SecretBase.FromKeyvaultUri(uri);
                    if (null != s) key = s.Key;
                }

                tx = new Ef.Entities.Transaction()
                {
                    TransactionId = context.Request.TransactionId,
                    SecretKey = key,
                    SecretUri = uri,
                    Action = action,
                    Event = evnt,
                    Purpose = purpose
                };

                if (context.Request.ParentTransactionId != Guid.Empty)
                {
                    tx.ParentTransaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.TransactionId == context.Request.ParentTransactionId);
                }

                _dbContext.Transactions.Add(tx);
            }

            Ef.Entities.Attempt attempt = new Ef.Entities.Attempt()
            {
                StartingStatus = context.Result.Status,
                StartTimeUtc = context.StartTime,
                Transaction = tx
            };

            List<Ef.Entities.Attempt> attempts = tx.Attempts?.ToList();
            if (null == attempts) attempts = new List<Ef.Entities.Attempt>();
            attempts.Add(attempt);
            tx.Attempts = attempts;

            _dbContext.Attempts.Add(attempt);

            await _dbContext.SaveChangesAsync();

            context.TrackingState.Add(this, attempt.AttemptId);
        }

        protected override async Task<IEnumerable<Common.Contracts.Tracking.Transaction>> OnGetTransactionsAsync(DateTime fromUtc, DateTime toUtc)
        {
            var txs = await _dbContext.Transactions.Include(t => t.Attempts).Where(t => t.Attempts.Any(a => a.StartTimeUtc >= fromUtc && a.StartTimeUtc <= toUtc)).OrderBy(t => t.Id).ToListAsync();

            List<Common.Contracts.Tracking.Transaction> results = new List<Common.Contracts.Tracking.Transaction>();

            foreach (var tx in txs)
            {
                if (null == tx) continue;

                results.Add(tx.ToCommonTransaction());
            }

            return results;
        }

        protected override async Task<Common.Contracts.Tracking.Transaction> OnGetTransactionAsync(Guid id)
        {
            var tx = await _dbContext.Transactions.Include(t => t.ParentTransaction.Attempts).Include(t => t.Attempts).FirstOrDefaultAsync(t => t.TransactionId == id);
            if (null == tx) return null;

            return tx.ToCommonTransaction();
        }


    }
}
