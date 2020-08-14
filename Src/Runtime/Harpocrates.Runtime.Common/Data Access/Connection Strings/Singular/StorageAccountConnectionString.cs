using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.DataAccess.ConnectionStrings
{
    public class StorageAccountConnectionString : ConnectionStringBase
    {
        public enum AccountKeyType
        {
            None = 0,
            AccountKey,
            SAS
        }

        private class Keys
        {
            public const string AccountEndpoint = "AccountEndpoint";
            public const string AccountKey = "AccountKey";
            public const string ContainerName = "Container";
            public const string KeyType = "KeyType";
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

        public AccountKeyType KeyType
        {
            get
            {
                if (Builder.TryGetValue(Keys.ContainerName, out object v))
                {
                    if (Enum.TryParse<AccountKeyType>(v as string, out AccountKeyType result)) return result;
                    else return AccountKeyType.None;

                };
                return AccountKeyType.None;
            }
            set
            {
                Builder.Add(Keys.ContainerName, value);
            }
        }
    }
}
