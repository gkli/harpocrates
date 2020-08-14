using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.Runtime.Processors
{
    public interface IRequestProcessor<T> where T : Common.Contracts.ProcessRequest
    {
        Task<Common.Contracts.ProcessResult> ProcessRequestAsync(T request, CancellationToken token);

    }
}
