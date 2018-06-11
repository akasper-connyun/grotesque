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

        internal async Task<HttpResponseMessage> GetLastKpisForDevice(string deviceUrn, DateTime from, DateTime to, string size)
        {
            SearchSpan searchSpan = new SearchSpan(from, to);
            JObject query = new JObject(
                new JProperty("searchSpan", JObject.FromObject(searchSpan)),
                new JProperty("predicate", generateDevicePredicate(deviceUrn)),
                new JProperty("aggregates", generateLastValueAggregates("elementname", "String", 100, size))
            );

            return await MakeAsyncPostRequest("aggregates", JsonConvert.SerializeObject(query));
        }

        internal async Task<HttpResponseMessage> GetKpiForDevice(string deviceUrn, string kpi, DateTime from, DateTime to, string size)
        {
            SearchSpan searchSpan = new SearchSpan(from, to);
            JObject query = new JObject(
                new JProperty("searchSpan", JObject.FromObject(searchSpan)),
                new JProperty("predicate", generateDeviceElementPredicate(deviceUrn, kpi)),
                new JProperty("aggregates", generateAvgAggregates("elementname", 100, size))
            );

            return await MakeAsyncPostRequest("aggregates", JsonConvert.SerializeObject(query));
        }

        private JArray generateLastValueAggregates(string splitBy, string splitByType, int take, string bucketSize)
        {
            LastMeasure lastValueMeasure = new LastMeasure("value.Value", "Double", "iothubeventenqueuedutctime", "DateTime");
            return generateAggregates(splitBy, splitByType, take, bucketSize, JObject.FromObject(lastValueMeasure));
        }

        private JArray generateAvgAggregates(string kpi, int take, string bucketSize)
        {
            AvgMeasure avgMeasure = new AvgMeasure("value.Value", "Double");
            return generateAggregates(kpi, "String", take, bucketSize, JObject.FromObject(avgMeasure));
        }

        private JArray generateAggregates(string splitBy, string splitByType, int take, string bucketSize, params JObject[] measures)
        {
            AggregatesDimension dimension = new AggregatesDimension(splitBy, splitByType, take);
            DateDimension dateDimension = new DateDimension("$ts", bucketSize);

            LastMeasure lastValueMeasure = new LastMeasure("value.Value", "Double", "iothubeventenqueuedutctime", "DateTime");

            JArray aggregates = new JArray(
                new JObject(
                    new JProperty("dimension", JObject.FromObject(dimension)),
                    new JProperty("aggregate",
                        new JObject(
                            new JProperty("dimension", JObject.FromObject(dateDimension)),
                            new JProperty("measures", measures)
                        )
                    )
                )
            );

            return aggregates;
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
            return await MakeAsyncPostRequest("events", JsonConvert.SerializeObject(query));
        }

        public async Task<HttpResponseMessage> GetAggregates(JObject query)
        {
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
            SearchSpan searchSpan = new SearchSpan(from, to);
            JObject query = new JObject(
                new JProperty("searchSpan", JObject.FromObject(searchSpan)), 
                new JProperty("predicate", generateDeviceElementPredicate(deviceUrn, elementName)), 
                new JProperty("top", generateTop(number))
            );

            return await MakeAsyncPostRequest("events", JsonConvert.SerializeObject(query));
        }

        private JObject generateDeviceElementPredicate(string deviceUrn, string elementName)
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

        private JObject generateDevicePredicate(string deviceUrn)
        {
            return new JObject(new JProperty("eq", generateEqStringProperty("deviceurn", deviceUrn)));
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
