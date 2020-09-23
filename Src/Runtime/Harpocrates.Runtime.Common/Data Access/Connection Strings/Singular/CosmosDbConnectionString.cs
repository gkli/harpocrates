using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.DataAccess.ConnectionStrings
{
    public class CosmosDbConnectionString : AzureResourceConnectionString
    {
        private class Keys
        {
            public const string AccountEndpoint = "AccountEndpoint";
            public const string AccountKey = "AccountKey";
            public const string ContainerName = "Container";
            public const string DatabaseName = "Database";
        }

        public CosmosDbConnectionString()
        {
        }


        public string AccountEndpoint
        {
            get
            {

                if (Builder.TryGetValue(Keys.AccountEndpoint, out object v)) return v as string;
                return string.Empty;
            }
            set
            {
                Builder.Add(Keys.AccountEndpoint, value);
            }
        }

        public string AccountKey
        {
            get
            {
                if (Builder.TryGetValue(Keys.AccountKey, out object v)) return v as string;
                return string.Empty;
            }
            set
            {
                Builder.Add(Keys.AccountKey, value);
            }
        }

        public string ContainerName
        {
            get
            {
                if (Builder.TryGetValue(Keys.ContainerName, out object v)) return v as string;
                return string.Empty;
            }
            set
            {
                Builder.Add(Keys.ContainerName, value);
            }
        }

        public string DatabaseName
        {
            get
            {
                if (Builder.TryGetValue(Keys.DatabaseName, out object v)) return v as string;
                return string.Empty;
            }
            set
            {
                Builder.Add(Keys.DatabaseName, value);
            }
        }

        public string AccountName { get { return GetAccountName(); } }

        private string GetAccountName()
        {
            Uri uri = new Uri(AccountEndpoint);
            int firstDotIdx = uri.Host.IndexOf(".");
            return uri.Host.Substring(0, firstDotIdx);
        }

    }
}
