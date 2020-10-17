using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Harpocrates.Api.Host.Controllers.Tracking
{
    public class TrackingController : SecureApiController
    {
        private readonly Runtime.Common.Tracking.IProcessingTrackerDataAccessProvider _dataAccessProvider;
        public TrackingController(Runtime.Common.Tracking.IProcessingTrackerDataAccessProvider dataAccessProvider)
        {
            _dataAccessProvider = dataAccessProvider;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Runtime.Common.Contracts.Tracking.Transaction>>> GetAsync([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            if (false == from.HasValue) from = DateTime.MinValue;
            if (false == to.HasValue) to = DateTime.UtcNow;

            var result = await _dataAccessProvider.GetTransactionsAsync(from.Value, to.Value);

            if (result == null) return NotFound();

            return Ok(result);
        }


        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<Runtime.Common.Contracts.Tracking.Transaction>> GetAsync(Guid id)
        {
            var result = await _dataAccessProvider.GetTransactionAsync(id);

            if (result == null) return NotFound();

            return Ok(result);
        }
    }
}
