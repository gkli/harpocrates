using Harpocrates.SecretManagement.Contracts.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers.Azure
{
    internal class APIMManagementSecretManagementProvider : SecretManagementProvider
    {
        protected override Task OnRotateSecretAsync(Secret secret)
        {
            throw new NotImplementedException();
        }
    }
}
