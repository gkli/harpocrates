using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.DataAccess.ConnectionStrings
{
    public abstract class ConnectionStringBase
    {
        public ConnectionStringBase()
        {
            Builder = new System.Data.Common.DbConnectionStringBuilder();
        }

        public string ConnectionString
        {
            get { return Builder.ConnectionString; }
            set { Builder.ConnectionString = value; }
        }

        protected System.Data.Common.DbConnectionStringBuilder Builder { get; private set; }
    }
}
