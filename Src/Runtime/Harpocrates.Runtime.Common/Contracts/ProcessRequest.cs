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
        }


        public string OriginalMessageJson { get; protected set; }


        public string Serialize()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
