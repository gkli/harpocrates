using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.DataAccess.StorageAccount.Contracts
{
    public class Config : SecretManagement.Contracts.Data.SecretConfiguration
    {
        public Guid ConfigurationId { get; set; }

        public Guid PolicyId { get; set; }
    }
}
