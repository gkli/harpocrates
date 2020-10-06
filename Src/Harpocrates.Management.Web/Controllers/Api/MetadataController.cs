using Harpocrates.Management.Web.Controllers.Api;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Harpocrates.Management.Web.Controllers.Api
{
    public abstract class MetadataController<T> : SecureApiController
        where T : new()
    {

        public MetadataController(Server.Configuration.IConfigurationProvider config, Server.Client.IMetadataServiceClient client)
        {
            Configuration = config;
            Client = client;
        }

        protected Server.Configuration.IConfigurationProvider Configuration { get; private set; }
        protected Server.Client.IMetadataServiceClient Client { get; private set; }

        [HttpGet("{id}")]
        public async Task<ActionResult<T>> GetAsync(string id)
        {
            string json = await Client.GetSingularJsonAsync(GetServiceUri(), id);

            T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<T>>> GetAllAsync()
        {
            string json = await Client.GetAllJsonAsync(GetServiceUri());

            IEnumerable<T> result = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<T>>(json);

            if (result == null) return NotFound();

            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteAsync(string id)
        {
            return Ok(await Client.DeleteAsync(GetServiceUri(), id));
        }

        [HttpPost]
        public async Task<ActionResult<T>> SaveAsync(T data)
        {
            string json = await Client.SaveJsonAsync(GetServiceUri(), Newtonsoft.Json.JsonConvert.SerializeObject(data));

            T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);

            if (null == result) return NotFound();

            return Ok(result);
        }



        protected abstract string ServiceRelativePath { get; }


        //private async Task<TResult> GetFromServiceAsync<TResult>(string url)
        //{
        //    using (var response = await HttpClient.GetAsync(url))
        //    {
        //        response.EnsureSuccessStatusCode();

        //        string json = await response.Content.ReadAsStringAsync();

        //        return Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(json);
        //    }
        //}

        //private async Task<TResult> PostToServiceAsync<TResult, TData>(string url, TData data)
        //{

        //    using (var response = await HttpClient.PostAsync(url, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(data))))
        //    {
        //        response.EnsureSuccessStatusCode();

        //        string json = await response.Content.ReadAsStringAsync();

        //        return Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(json);
        //    }

        //}

        //private async Task<TResult> DeleteFromServiceAsync<TResult>(string url)
        //{
        //    using (var response = await HttpClient.DeleteAsync(url))
        //    {
        //        response.EnsureSuccessStatusCode();

        //        string json = await response.Content.ReadAsStringAsync();

        //        return Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(json);
        //    }
        //}

        private string GetServiceUri()
        {
            return $"{Configuration.MetadataServiceBaseUri}{ServiceRelativePath}";
        }
    }
}
