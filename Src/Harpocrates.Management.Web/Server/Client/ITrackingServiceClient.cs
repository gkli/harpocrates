using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Harpocrates.Management.Web.Server.Client
{
    public interface ITrackingServiceClient
    {
        Task<IEnumerable<Runtime.Common.Contracts.Tracking.Transaction>> GetAsync(string url, DateTime? from, DateTime? to);
        Task<Runtime.Common.Contracts.Tracking.Transaction> GetAsync(string url, Guid id);

    }
}
