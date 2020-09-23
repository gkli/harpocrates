
using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.DataAccess.ConnectionStrings
{
    public abstract class AzureResourceConnectionString : ConnectionStringBase
    {

        private class Keys
        {
            public const string ResourceGroup = "ResourceGroup";
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
    }
}
