using Harpocrates.Runtime.Common.Contracts.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Harpocrates.Management.Web.Server.Client
{
    internal class TrackingServiceClient : ITrackingServiceClient
    {
        private readonly HttpClient _client;
        public TrackingServiceClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<Transaction>> GetAsync(string url, DateTime? from, DateTime? to)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(url);
            if (from.HasValue || to.HasValue)
            {
                sb.Append("?");

                if (from.HasValue)
                {
                    sb.Append($"from={HttpUtility.UrlEncode(from.Value.ToString())}");
                }

                if (to.HasValue)
                {
                    if (from.HasValue) sb.Append("&");
                    sb.Append($"to={HttpUtility.UrlEncode(to.Value.ToString())}");
                }
            }

            using (var response = await _client.GetAsync(sb.ToString()))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                if (false == string.IsNullOrWhiteSpace(json))
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Transaction>>(json);
            }

            return null;
        }

        public async Task<Transaction> GetAsync(string url, Guid id)
        {
            using (var response = await _client.GetAsync($"{url}/{id}"))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                if (false == string.IsNullOrWhiteSpace(json))
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<Transaction>(json);
            }

            return null;
        }
    }
}
