using Harpocrates.SecretManagement.Contracts.Data;
using Harpocrates.SecretManagement.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Harpocrates.Api.Host.Controllers.Metadata
{
    public class PoliciesController : MetadataController<SecretPolicy>
    {
        public PoliciesController(ISecretMetadataDataAccessProvider dataProvider) : base(dataProvider)
        {
        }

        protected override async Task<bool> OnDeleteAsync(string id)
        {
            await DataAccessProvider.DeletePolicyAsync(id, CancellationToken);
            return true;
        }

        protected override async Task<IEnumerable<SecretPolicy>> OnGetAllAsync()
        {
            return await DataAccessProvider.GetPoliciesAsync(CancellationToken);
        }

        protected override async Task<SecretPolicy> OnGetAsync(string id)
        {
            return await DataAccessProvider.GetPolicyAsync(id, CancellationToken);
        }

        protected override async Task<SecretPolicy> OnSaveAsync(SecretPolicy data)
        {
            string id = await DataAccessProvider.SavePolicyAsync(data, CancellationToken);

            if (string.IsNullOrWhiteSpace(id)) return null;

            if (Guid.TryParse(id, out Guid gid)) data.PolicyId = gid;
            else return null;

            return data;

        }
    }
}
