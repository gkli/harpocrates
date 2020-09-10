using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.DataAccess.StorageAccount.Contracts
{
    public class Secret : SecretManagement.Contracts.Data.Secret
    {
        public Guid SecretId { get; set; }
        public Guid ConfigurationId { get; set; }
    }
}
