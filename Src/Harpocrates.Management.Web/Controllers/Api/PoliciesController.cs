using Harpocrates.SecretManagement.Contracts.Data;

namespace Harpocrates.Management.Web.Controllers.Api
{
    public class PoliciesController : MetadataController<SecretPolicy>
    {
        public PoliciesController(Server.Configuration.IConfigurationProvider config, Server.Client.IMetadataServiceClient client) : base(config, client)
        {
        }

        protected override string ServiceRelativePath => "/policies";

    }
}
