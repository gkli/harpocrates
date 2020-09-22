using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.DataAccess.ConnectionStrings
{
    public class ServicePrincipalConnectionString : ConnectionStringBase
    {
        private class Keys
        {
            public const string TenantId = "TenantId";
            public const string ClientId = "ClientId";
            public const string ClientSecret = "ClientSecret";
            public const string EnvironmentName = "EnvironmentName";
        }

        public string TenantId
        {
            get
            {
                if (Builder.TryGetValue(Keys.TenantId, out object v)) return v as string;
                return string.Empty;
            }
            set
            {
                Builder.Add(Keys.TenantId, value);
            }
        }


        public string ClientId
        {
            get
            {
                if (Builder.TryGetValue(Keys.ClientId, out object v)) return v as string;
                return string.Empty;
            }
            set
            {
                Builder.Add(Keys.ClientId, value);
            }
        }


        public string ClientSecret
        {
            get
            {
                if (Builder.TryGetValue(Keys.ClientSecret, out object v)) return v as string;
                return string.Empty;
            }
            set
            {
                Builder.Add(Keys.ClientSecret, value);
            }
        }

        public string EnvironmentName
        {
            get
            {
                if (Builder.TryGetValue(Keys.EnvironmentName, out object v)) return v as string;
                return string.Empty;
            }
            set
            {
                Builder.Add(Keys.EnvironmentName, value);
            }
        }

    }
}
