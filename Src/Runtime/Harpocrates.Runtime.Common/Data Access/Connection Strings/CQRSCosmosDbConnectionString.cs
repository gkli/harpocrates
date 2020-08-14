using Harpocrates.Runtime.Common.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.DataAccess.ConnectionStrings
{
    public class CQRSCosmosDbConnectionString : CQRSConnectionString<CosmosDbConnectionString>
    {
        public CQRSCosmosDbConnectionString(CosmosDbConnectionString commandConnectionString, CosmosDbConnectionString queryConnectionString) : base(commandConnectionString, queryConnectionString)
        {
        }
    }
}
