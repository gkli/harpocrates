using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Harpocrates.Management.Web.Controllers.Api
{
    public class TrackingController : SecureApiController
    {

        private readonly Server.Configuration.IConfigurationProvider _config;
        private readonly Server.Client.ITrackingServiceClient _client;
        public TrackingController(Server.Configuration.IConfigurationProvider config, Server.Client.ITrackingServiceClient client)
        {
            _config = config;
            _client = client;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Runtime.Common.Contracts.Tracking.Transaction>>> GetAsync([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            if (false == from.HasValue) from = DateTime.MinValue;
            if (false == to.HasValue) to = DateTime.UtcNow;

            var result = await _client.GetAsync(GetServiceUri(), from.Value, to.Value);

            if (result == null) return NotFound();

            return Ok(result);
        }


        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<Runtime.Common.Contracts.Tracking.Transaction>> GetAsync(Guid id)
        {
            if (id == Guid.Empty) return NotFound();

            var result = await _client.GetAsync(GetServiceUri(), id);

            if (result == null) return NotFound();

            return Ok(result);
        }


        private string GetServiceUri()
        {
            return _config.TrackingServiceBaseUri;
        }
    }
}
