using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Harpocrates.Management.Web.Server.Client
{
    public interface IMetadataServiceClient
    {
        Task<string> GetSingularJsonAsync(string url, string id, bool shallow);
        Task<string> GetAllJsonAsync(string url, bool shallow);
        Task<bool> DeleteAsync(string url, string id);
        Task<string> SaveJsonAsync(string url, string data);
    }
}
