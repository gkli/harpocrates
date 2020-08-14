using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.Contracts.Data
{
    public class Secret
    {
        public string Uri { get; set; }
        public string VaultName { get; set; }
        public string ObjectType { get; set; }
        public string ObjectName { get; set; }
        public string Version { get; set; }

        public static Secret FromKeyvaultUri(string uri)
        {
            Uri u = new Uri(uri);

            return new Secret()
            {
                Uri = uri
            };
        }
    }
}
