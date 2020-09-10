using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.SecretManagement.Contracts.Data
{
    public class SecretPolicy
    {
        public TimeSpan RotationInterval { get; set; }

        // add things like auto-rotate? max-number-of-autorotations? etc... // if max number of auto-rotations is hit, notify user to manually rotate & reset the tool?
    }
}
