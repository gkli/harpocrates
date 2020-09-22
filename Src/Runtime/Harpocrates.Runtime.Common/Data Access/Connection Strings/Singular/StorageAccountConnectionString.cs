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
            public const string ResourceGroup = "ResourceGroup";
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

        public string ResourceGroup
        {
            get
            {
                if (Builder.TryGetValue(Keys.ResourceGroup, out object v)) return v as string;
                return string.Empty;
            }
            set
            {
                Builder.Add(Keys.ResourceGroup, value);
            }
        }
        public string AccountName { get { return GetAccountName(); } }

        public AccountKeyType KeyType
        {
            get
            {
                if (Builder.TryGetValue(Keys.KeyType, out object v))
                {
                    if (Enum.TryParse<AccountKeyType>(v as string, out AccountKeyType result)) return result;
                    else return AccountKeyType.None;

                };
                return AccountKeyType.None;
            }
            set
            {
                Builder.Add(Keys.KeyType, value);
            }
        }

        public string ToStorageAccountFormat()
        {
            //"DefaultEndpointsProtocol=https;AccountName=harpocrates;AccountKey=4PDtsssV6Gcl5zZmd9igruRgsU5qi5FvB1gvhV5h2Ax++Y7SymR4QES0EMlF9ftgjUB6mmnmQVfbIEI5YeFKtA==;EndpointSuffix=core.windows.net"

            Uri uri = new Uri(AccountEndpoint);

            int firstDotIdx = uri.Host.IndexOf(".");
            string accountName = uri.Host.Substring(0, firstDotIdx);
            string suffix = uri.Host.Substring(firstDotIdx + 1, uri.Host.Length - firstDotIdx - 1);

            return $"DefaultEndpointsProtocol={uri.Scheme};AccountName={accountName};AccountKey={AccountKey};EndpointSuffix={suffix}";
        }

        private string GetAccountName()
        {
            Uri uri = new Uri(AccountEndpoint);
            int firstDotIdx = uri.Host.IndexOf(".");
            return uri.Host.Substring(0, firstDotIdx);
        }

    }
}
