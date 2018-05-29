using Newtonsoft.Json;
using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Grotesque.Models;

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

        /**
         {
	        "searchSpan": {
		        "from": {
			        "dateTime": "2018-05-01T00:00:00.000Z"
		        },
		        "to": {
			        "dateTime": "2018-05-31T23:59:59.000Z"
		        }
	        },
	        "predicate": {
		        "and": [
			        {
				        "eq": {
					        "left": {
						        "property": "deviceurn",
						        "type": "String"
					        },
					        "right": "urn:cde:fairs:fairs:MachineRoller:003001"
				        }
			        },
			        {
				        "eq": {
					        "left": {
						        "property": "elementname",
						        "type": "String"
					        },
					        "right": "Motor.Power"
				        }
			        }
		        ]
	        },
	        "top": {
		        "sort": [{
			        "input": {
				        "builtInProperty": "$ts"
			        },
			        "order": "Asc"
		        }],
		        "count": 1000
	        }
        }
        */
        public async Task<HttpResponseMessage> GetLastDataPoints(DateTime from, DateTime to, string deviceUrn, string elementName, int number = 1000)
        {
            SetClientHeaders();
            SearchSpan searchSpan = new SearchSpan(from, to);
            JObject query = new JObject(
                new JProperty("searchSpan", JObject.FromObject(searchSpan)), 
                new JProperty("predicate", generatePredicate(deviceUrn, elementName)), 
                new JProperty("top", generateTop(number))
            );

            return await MakeAsyncPostRequest("events", JsonConvert.SerializeObject(query));
        }

        private JObject generatePredicate(string deviceUrn, string elementName)
        {
            return new JObject(
                new JProperty("and", 
                    new JObject(
                        new JProperty("eq", generateEqStringProperty("deviceurn", deviceUrn))
                    ),
                    new JObject(
                        new JProperty("eq", generateEqStringProperty("elementname", elementName))
                    )
                )
            );
        }

        private JObject generateEqStringProperty(string property, string value)
        {
            return new JObject(
                new JProperty("left", new JObject(
                    new JProperty("property", property), 
                    new JProperty("type", "String"))
                ),
                new JProperty("right", value)
            );
        }

        private JObject generateTop(int number)
        {
            return new JObject(
                new JProperty("sort", new JArray(
                    new JObject(
                        new JProperty("input", new JObject(
                            new JProperty("builtInProperty", "$ts"))),
                        new JProperty("order", "Desc")))),
                new JProperty("count", number)
            );
        }
    }
}
