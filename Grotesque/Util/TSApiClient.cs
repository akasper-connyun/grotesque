using Newtonsoft.Json;
using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Grotesque.Util
{
    public class TSApiClient
    {
        private HttpClient client = new HttpClient();
        private string TSEnvironmentFQDN = Environment.GetEnvironmentVariable("TSI_ENV_FQDN");

        private void SetClientHeaders()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-ms-client-application-name", "Grotesque");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Authentication.AcquireAccessTokenAsync().Result);
        }

        private Uri CreateUri(string path)
        {
            Uri uri = new UriBuilder("https", TSEnvironmentFQDN)
            {
                Path = path,
                Query = "api-version=2016-12-12",
            }.Uri;

            return uri;
        }

        private async Task<HttpResponseMessage> MakeAsyncPostRequest(string path, string content)
        {
            SetClientHeaders();
            HttpContent httpContent = new StringContent(content, encoding: Encoding.UTF8, mediaType: "application/json");

            return await client.PostAsync(CreateUri(path), httpContent);
        }

        public async Task<HttpResponseMessage> GetMetadata(JObject metadata)
        {
            return await MakeAsyncPostRequest("metadata", JsonConvert.SerializeObject(metadata));
        }

        public async Task<HttpResponseMessage> GetAvailability()
        {
            SetClientHeaders();
            return await client.GetAsync(CreateUri("availability"));
        }

        public async Task<HttpResponseMessage> GetEvents(JObject query)
        {
            SetClientHeaders();
            return await MakeAsyncPostRequest("events", JsonConvert.SerializeObject(query));
        }

        public async Task<HttpResponseMessage> GetAggregates(JObject query)
        {
            SetClientHeaders();
            return await MakeAsyncPostRequest("aggregates", JsonConvert.SerializeObject(query));
        }
    }
}
