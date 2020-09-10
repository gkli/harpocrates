using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.Contracts.Data
{
    public class ConfiguredSecret : Secret
    {
        public SecretConfiguration Configuration { get; set; }
    }
}
