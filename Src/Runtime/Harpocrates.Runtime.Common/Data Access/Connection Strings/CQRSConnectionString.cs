using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.DataAccess.ConnectionStrings
{
    public abstract class CQRSConnectionStringBase { }

    public class CQRSConnectionString<T> : CQRSConnectionStringBase
        where T : ConnectionStringBase
    {
        public CQRSConnectionString(T commandConnectionString, T queryConnectionString) //: base(null, null)
        {
            CommandConnectionString = commandConnectionString;
            QueryConnectionString = queryConnectionString;
        }

        public T CommandConnectionString { get; private set; }
        public T QueryConnectionString { get; private set; }
    }
}
