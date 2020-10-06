using Harpocrates.SecretManagement.Contracts.Data;

namespace Harpocrates.Management.Web.Controllers.Api
{
    public class SecretsController : MetadataController<Secret>
    {
        public SecretsController(Server.Configuration.IConfigurationProvider config, Server.Client.IMetadataServiceClient client) : base(config, client)
        {
        }

        protected override string ServiceRelativePath => "/services";

    }
}
