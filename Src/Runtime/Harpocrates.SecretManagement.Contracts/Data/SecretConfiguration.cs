using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.Contracts.Data
{
    public class SecretConfiguration:SecretConfigurationBase
    {
       

        public SecretPolicy Policy { get; set; }
    }
}
