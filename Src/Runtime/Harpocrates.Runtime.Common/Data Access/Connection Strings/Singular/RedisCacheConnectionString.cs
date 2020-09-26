using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.DataAccess.ConnectionStrings
{
    public class RedisCacheConnectionString : AzureResourceConnectionString
    {
        private class Keys
        {
            public const string AccountEndpoint = "AccountEndpoint";
            public const string Password = "password";
            public const string SSL = "ssl";
            public const string AbortConnect = "abortConnect";
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

        public string Password
        {
            get
            {
                if (Builder.TryGetValue(Keys.Password, out object v)) return v as string;
                return string.Empty;
            }
            set
            {
                Builder.Add(Keys.Password, value);
            }
        }

        public bool Ssl
        {
            get
            {
                if (Builder.TryGetValue(Keys.SSL, out object v))
                {
                    if (bool.TryParse(v as string, out bool b)) return b;
                }
                return true;
            }
            set
            {
                Builder.Add(Keys.SSL, value);
            }
        }

        public string AccountName { get { return GetAccountName(); } }

        public bool AbortConnect
        {
            get
            {
                if (Builder.TryGetValue(Keys.AbortConnect, out object v))
                {
                    if (bool.TryParse(v as string, out bool b)) return b;
                };
                return false;
            }
            set
            {
                Builder.Add(Keys.AbortConnect, value);
            }
        }

        public string ToRedisFormat()
        {
            //harpocrates-redis.redis.cache.windows.net:6380,password=******,ssl=True,abortConnect=False

            return $"{AccountEndpoint},password={Password},ssl={Ssl},abortConnect={AbortConnect}";
        }

        private string GetAccountName()
        {
            Uri uri = new Uri(AccountEndpoint);

            string host = uri.Host;
            if (string.IsNullOrWhiteSpace(host)) host = uri.OriginalString;

            int firstDotIdx = host.IndexOf(".");
            return host.Substring(0, firstDotIdx);
        }

    }
}
