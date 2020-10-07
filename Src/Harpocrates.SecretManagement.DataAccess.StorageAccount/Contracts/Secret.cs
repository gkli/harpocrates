using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.DataAccess.StorageAccount.Contracts
{
    public class Secret : SecretManagement.Contracts.Data.SecretBase
    {
        public Guid ConfigurationId { get; set; }

        public override SecretManagement.Contracts.Data.Secret ToConfiguredSecret()
        {
            var secret = base.ToConfiguredSecret();
            if (Guid.Empty != ConfigurationId)
            {
                secret.Configuration = new SecretManagement.Contracts.Data.SecretConfiguration()
                {
                    ConfigurationId = ConfigurationId
                };
            }

            return secret;
        }

    }
}
