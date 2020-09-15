using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.DataAccess.StorageAccount.Contracts
{
    public class Secret : SecretManagement.Contracts.Data.SecretBase
    {       
        public Guid ConfigurationId { get; set; }
    }
}
