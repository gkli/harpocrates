using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.Providers.Azure
{
    internal class StorageAccountSecretManagementProvider : SecretManagementProvider
    {
        protected override Task OnRotateSecretAsync(Secret secret)
        {
            throw new NotImplementedException();
        }
    }
}
