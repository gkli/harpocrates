using Harpocrates.SecretManagement.Contracts.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers.Azure
{
    internal class EventGridSecretManager : SecretManager
    {
        protected override Task<Key> OnRotateSecretAsync(Secret secret)
        {
            throw new NotImplementedException();
        }
    }
}
