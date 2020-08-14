using Harpocrates.Runtime.Common.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.DataAccess.ConnectionStrings
{
    public class CQRSStorageAccountConnectionString : CQRSConnectionString<StorageAccountConnectionString>
    {
        public CQRSStorageAccountConnectionString(StorageAccountConnectionString commandConnectionString, StorageAccountConnectionString queryConnectionString) : base(commandConnectionString, queryConnectionString)
        {
        }
    }
}
