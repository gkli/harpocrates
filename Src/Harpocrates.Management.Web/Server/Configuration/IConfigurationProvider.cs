using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Harpocrates.Management.Web.Server.Configuration
{
    public interface IConfigurationProvider
    {
        string MetadataServiceBaseUri { get; }
    }
}
