using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Harpocrates.Management.Web.Server.Client
{
    internal class MetadataServiceClient : IMetadataServiceClient
    {
        private readonly HttpClient _client;
        public MetadataServiceClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<bool> DeleteAsync(string url, string id)
        {
            using (var response = await _client.DeleteAsync(url))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                return Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(json);
            }
        }

        public async Task<string> GetAllAsync(string url)
        {
            using (var response = await _client.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> GetSingularAsync(string url, string id)
        {
            using (var response = await _client.GetAsync($"{url}/id"))
            {
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> SaveAsync(string url, string data)
        {
            using (var response = await _client.PostAsync(url, new StringContent(data)))
            {
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }




    }
}
