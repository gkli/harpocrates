using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.Contracts
{
    public abstract class ProcessRequest
    {
        public ProcessRequest(string json)
        {
            OriginalMessageJson = json;
            TransactionId = Guid.NewGuid();
            ParentTransactionId = Guid.Empty;
        }


        public string OriginalMessageJson { get; protected set; }

        public Guid TransactionId { get; set; }
        public Guid ParentTransactionId { get; set; }


        public string Serialize()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
