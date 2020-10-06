using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Harpocrates.SecretManagement.Contracts.Data
{
    public class SecretBase
    {
        public string Uri => GetUri(true);
        public string VaultName { get; set; }
        public string ObjectType { get; set; }
        public string ObjectName { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SubscriptionId { get; set; } //id of subscription that has the KeyVault instance
        public string CurrentKeyName { get; set; }
        public DateTime? LastRotatedOn { get; set; }
        public SecretType SecretType { get; set; }

        //todo: allow configuring complex Secrets: ie. ConnectionString: UserId="managed-secret-123";Password="managed-secret-124"

        /// <summary>
        /// Format string used to store secret value in ex: Database=db1;sever=server1;userId:John;Password:{managed-secret}
        /// </summary>
        public string FormatExpression { get; set; }

        public static SecretBase FromKeyvaultUri(string uri)
        {
            Uri u = new Uri(uri);

            SecretBase s = new SecretBase();

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
            string uri = GetUri(false);

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

        private string GetUri(bool includeVersion)
        {
            string type = "secrets";
            switch (ObjectType?.ToLower())
            {
                case "key":
                    type = "keys";
                    break;
                case "certificate":
                    type = "certificates";
                    break;
            }
            string url = includeVersion ? $"https://{VaultName}.vault.azure.net/{type}/{ObjectName}/{Version}" : $"https://{VaultName}.vault.azure.net/{type}/{ObjectName}";

            return url.ToLower();
        }
    }
}
