using Harpocrates.SecretManagement.Contracts.Data;

namespace Harpocrates.Management.Web.Controllers.Api
{
    public class ServicesController : MetadataController<SecretConfiguration>
    {
        public ServicesController(Server.Configuration.IConfigurationProvider config, Server.Client.IMetadataServiceClient client) : base(config, client)
        {
        }

        protected override string ServiceRelativePath => "/secrets";
    }
}
