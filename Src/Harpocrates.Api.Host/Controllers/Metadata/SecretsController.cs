using Harpocrates.SecretManagement.Contracts.Data;
using Harpocrates.SecretManagement.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Harpocrates.Api.Host.Controllers.Metadata
{
    public class SecretsController : MetadataController<Secret>
    {
        public SecretsController(ISecretMetadataDataAccessProvider dataProvider) : base(dataProvider)
        {
        }

        protected override Task<bool> OnDeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        protected override async Task<IEnumerable<Secret>> OnGetAllAsync()
        {
            return await DataAccessProvider.GetConfiguredSecretsAsync(CancellationToken);
        }

        protected override Task<Secret> OnGetAsync(string id)
        {
            throw new NotImplementedException();
        }

        protected override Task<Secret> OnSaveAsync(Secret data)
        {
            throw new NotImplementedException();
        }
    }
}
