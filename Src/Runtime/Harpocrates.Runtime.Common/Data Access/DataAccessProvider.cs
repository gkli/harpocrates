using Harpocrates.Runtime.Common.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.DataAccess
{
    public abstract class DataAccessProvider
    {
        public DataAccessProvider(ConnectionStrings.CQRSConnectionStringBase connectionString)
        {
            ConnectionString = connectionString;
        }

        protected ConnectionStrings.CQRSConnectionStringBase ConnectionString { get; private set; }
    }
}
