using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.DataAccess.StorageAccount.Contracts
{
    public class Policy : SecretManagement.Contracts.Data.SecretPolicy
    {
        public Guid SecretId { get; set; }
    }
}
