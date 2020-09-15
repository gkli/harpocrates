using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.Contracts.Data
{
    public class Secret : SecretBase
    {
        public SecretConfiguration Configuration { get; set; }
    }
}
