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

        protected override async Task<IEnumerable<SecretConfiguration>> OnGetAllAsync(bool shallow)
        {
            if (shallow)
            {
                var services = await DataAccessProvider.GetConfigurationsAsync(CancellationToken);
                if (null == services) return null;

                List<SecretConfiguration> items = new List<SecretConfiguration>();

                foreach (var service in services)
                {
                    items.Add(service.ToSecretConfiguration());
                }

                return items;
            }
            else
                return await DataAccessProvider.GetSecretConfigurationsAsync(CancellationToken);
        }

        protected override async Task<SecretConfiguration> OnGetAsync(string id, bool shallow)
        {
            if (shallow)
            {
                var cfg = await DataAccessProvider.GetConfigurationAsync(id, CancellationToken);
                if (null == cfg) return null;
                return cfg.ToSecretConfiguration();
            }
            else
                return await DataAccessProvider.GetSecretConfigurationAsync(id, CancellationToken);
        }

        protected override async Task<SecretConfiguration> OnSaveAsync(SecretConfiguration data)
        {
            throw new NotImplementedException();
        }
    }
}
