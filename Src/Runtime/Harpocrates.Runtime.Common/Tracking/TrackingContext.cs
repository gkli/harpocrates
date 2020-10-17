using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.Tracking
{
    public class TrackingContext : IDisposable
    {
        private readonly IProcessingTrackerDataAccessProvider _dataProvider;

        private bool _isDisposed;

        internal TrackingContext(Contracts.ProcessRequest request, Contracts.ProcessResult result, IProcessingTrackerDataAccessProvider dataProvider)
        {
            Request = request;
            Result = result;
            _dataProvider = dataProvider;
            TrackingState = new Dictionary<IProcessingTrackerDataAccessProvider, object>();
        }

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        public Contracts.ProcessRequest Request { get; private set; }
        public Contracts.ProcessResult Result { get; private set; }

        public Dictionary<IProcessingTrackerDataAccessProvider, object> TrackingState { get; private set; }

        internal void StartTrackingActivity()
        {
            StartTime = DateTime.UtcNow;

            try
            {
                //TODO: write record to dataprovider
                //Console.WriteLine($"Starting: Tx {_request.TransactionId}, ParentTx: {_request.ParentTransactionId}, Result:{_result.Status}, Time: {_startTime}");
                if (null != _dataProvider) 
                    _dataProvider.RecordTransactionStartAsync(this).Wait();
            }
            catch { }

        }
        private void EndTrackingActivity()
        {
            EndTime = DateTime.UtcNow;

            try
            {
                //TODO: write record to dataprovider
                //Console.WriteLine($"Ending. Tx {_request.TransactionId}, ParentTx: {_request.ParentTransactionId}, Result:{_result.Status}, Duration: {endTime.Subtract(_startTime).TotalMilliseconds}");
                if (null != _dataProvider) 
                    _dataProvider.RecordTransactionEndAsync(this).Wait();
            }
            catch { }

        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                // free managed resources
                EndTrackingActivity();
            }

            _isDisposed = true;
        }

    }
}
