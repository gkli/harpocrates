using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers
{
    public interface ISecretManagementProvider
    {
        //todo: what do we want to return? we need to know new values so that we can store them into secret config...
        //should "Secret" be extended to carry these details back for saving to KV key as well as storage repo...

        Task RotateSecretAsync(Contracts.Data.Secret secret);
    }
}
