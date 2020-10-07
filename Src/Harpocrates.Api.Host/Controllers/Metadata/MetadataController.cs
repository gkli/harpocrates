using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.Api.Host.Controllers.Metadata
{
    [Route("api/metadata/[controller]")]
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
        public async Task<ActionResult<T>> GetAsync(string id, [FromQuery] bool shallow)
        {
            T result = await OnGetAsync(id, shallow);

            if (result == null) return NotFound();

            return Ok(result);
        }


        [HttpGet()]
        public async Task<ActionResult<IEnumerable<T>>> GetAllAsync([FromQuery] bool shallow)
        {
            IEnumerable<T> result = await OnGetAllAsync(shallow);

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

        protected abstract Task<T> OnGetAsync(string id, bool shallow);
        protected abstract Task<IEnumerable<T>> OnGetAllAsync(bool shallow);
        protected abstract Task<T> OnSaveAsync(T data);
        protected abstract Task<bool> OnDeleteAsync(string id);

    }
}
