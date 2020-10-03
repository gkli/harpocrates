using Harpocrates.Management.Web.Controllers.Api;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.Management.Web.Controllers.Api
{
    public abstract class MetadataController<T> : SecureApiController
        where T : new()
    {

        public MetadataController(SecretManagement.DataAccess.ISecretMetadataDataAccessProvider dataProvider)
        {
            DataAccessProvider = dataProvider;
            CancellationToken = CancellationToken.None;
        }

        protected SecretManagement.DataAccess.ISecretMetadataDataAccessProvider DataAccessProvider { get; private set; }
        protected CancellationToken CancellationToken { get; private set; }

        [HttpGet("{id}")]
        public async Task<ActionResult<T>> GetAsync(string id)
        {
            T result = await OnGetAsync(id);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<T>>> GetAllAsync()
        {
            IEnumerable<T> result = await OnGetAllAsync();

            if (result == null) return NotFound();

            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteAsync(string id)
        {
            return Ok(await OnDeleteAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<T>> SaveAsync(T data)
        {
            T result = await OnSaveAsync(data);
            if (null == result) return NotFound();

            return Ok(result);
        }

        protected abstract Task<T> OnGetAsync(string id);
        protected abstract Task<IEnumerable<T>> OnGetAllAsync();
        protected abstract Task<T> OnSaveAsync(T data);
        protected abstract Task<bool> OnDeleteAsync(string id);

    }
}
