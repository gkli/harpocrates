using Harpocrates.SecretManagement.Contracts.Data;
using Harpocrates.SecretManagement.DataAccess;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        protected override async Task<IEnumerable<Secret>> OnGetAllAsync(bool shallow)
        {
            if (shallow)
            {
                var secrets = await DataAccessProvider.GetSecretsAsync(CancellationToken);
                if (null == secrets) return null;
                List<Secret> items = new List<Secret>();

                foreach (var secret in secrets)
                {
                    if (null != secret) {
                        if (secret is Secret) items.Add(secret as Secret);
                        else
                            items.Add(secret.ToConfiguredSecret());
                    } 
                }

                return items;
            }
            else
                return await DataAccessProvider.GetConfiguredSecretsAsync(CancellationToken);
        }

        protected override async Task<Secret> OnGetAsync(string id, bool shallow)
        {
            if (shallow)
            {
                var secret = await DataAccessProvider.GetSecretAsync(id, CancellationToken);
                if (null == secret) return null;

                return secret.ToConfiguredSecret();
            }
            else
                return await DataAccessProvider.GetConfiguredSecretAsync(id, CancellationToken);
        }

        protected override Task<Secret> OnSaveAsync(Secret data)
        {
            throw new NotImplementedException();
        }


    }
}
