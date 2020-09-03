using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Harpocrates.SecretManagement.Contracts.Data
{
    public class Secret
    {
        public string Uri => GetUri();
        public string VaultName { get; set; }
        public string ObjectType { get; set; }
        public string ObjectName { get; set; }
        public string Version { get; set; }

        public static Secret FromKeyvaultUri(string uri)
        {
            Uri u = new Uri(uri);

            Secret s = new Secret();

            //hostname vaultname.valut.azure.net
            string host = u.Host;
            if (!string.IsNullOrWhiteSpace(host) && host.IndexOf(".") > 0)
            {
                host = host.Split('.', StringSplitOptions.RemoveEmptyEntries)[0];
            }
            s.VaultName = host;

            //path = type/name/version

            string path = u.GetComponents(UriComponents.Path, UriFormat.Unescaped);
            if (false == string.IsNullOrWhiteSpace(path))
            {
                string[] parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 0)
                {
                    switch (parts[0].ToLower())
                    {
                        case "secrets":
                            s.ObjectType = "secret";
                            break;
                        case "keys":
                            s.ObjectType = "key";
                            break;
                        case "certificates":
                            s.ObjectType = "certificate";
                            break;
                        default:
                            s.ObjectType = parts[0];
                            break;
                    }

                    if (parts.Length > 1)
                    {
                        s.ObjectName = parts[1];

                        if (parts.Length > 2)
                        {
                            s.Version = parts[2];
                        }
                    }
                }
            }
            return s;

        }

        public string Key => GetKey(); //primary key for record

        private string GetKey()
        {
            string uri = Uri.ToString();

            //todo: make sure uri has value, if not, need to constrcut uri...


            uri = uri.ToLower();

            using (HashAlgorithm algorithm = SHA256.Create())
            {
                byte[] hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(uri));

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                    sb.Append(b.ToString("X2"));

                return sb.ToString();
            }
        }

        private string GetUri()
        {
            string type = "secrets";
            switch (ObjectType.ToLower())
            {
                case "key":
                    type = "keys";
                    break;
                case "certificate":
                    type = "certificates";
                    break;
            }
            string url = $"https://{VaultName}.vault.azure.net/{type}/{ObjectName}/{Version}".ToLower();

            return url;
        }
    }
}
