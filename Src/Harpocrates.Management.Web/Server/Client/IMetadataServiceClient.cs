using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Harpocrates.Management.Web.Server.Client
{
    public interface IMetadataServiceClient
    {
        Task<string> GetSingularAsync(string url, string id);
        Task<string> GetAllAsync(string url);
        Task<bool> DeleteAsync(string url, string id);
        Task<string> SaveAsync(string url, string data);
    }
}
