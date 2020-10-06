using Harpocrates.SecretManagement.Contracts.Data;
using Harpocrates.SecretManagement.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Harpocrates.Api.Host.Controllers.Metadata
{
    public class ServicesController : MetadataController<SecretConfiguration>
    {
        public ServicesController(ISecretMetadataDataAccessProvider dataProvider) : base(dataProvider)
        {
        }

        protected override async Task<bool> OnDeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        protected override async Task<IEnumerable<SecretConfiguration>> OnGetAllAsync()
        {
            return await DataAccessProvider.GetConfigurationsAsync(CancellationToken);
        }

        protected override async Task<SecretConfiguration> OnGetAsync(string id)
        {
            throw new NotImplementedException();
        }

        protected override async Task<SecretConfiguration> OnSaveAsync(SecretConfiguration data)
        {
            throw new NotImplementedException();
        }
    }
}
