using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.SecretManagement.Providers
{

    public class Key
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string SecretVersion { get; set; }
    }
    public interface ISecretManager
    {
        //todo: what do we want to return? we need to know new values so that we can store them into secret config...
        //should "Secret" be extended to carry these details back for saving to KV key as well as storage repo...

        Task<Key> RotateSecretAsync(Contracts.Data.Secret secret, CancellationToken token);
    }
}
