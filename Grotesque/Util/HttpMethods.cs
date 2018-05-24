using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http;

namespace Grotesque.Util
{
    public class HttpMethods
    {
        private static string TSEnvironmentFQDN = "078f5a28-fbff-4a2f-bc60-2fdab61a10c1.env.timeseries.azure.com";
        public static JToken MakeQuery(string method, string path, string[] queryArgs = null)
        {
            string accessToken = Authentication.AcquireAccessTokenAsync().Result;
            return GetResponse(CreateHttpsWebRequest(TSEnvironmentFQDN, method, path, accessToken, queryArgs));
        }

        public static JToken MakeBodyQuery(string path, string bodyContent)
        {
            string accessToken = Authentication.AcquireAccessTokenAsync().Result;
            return GetResponse(CreatePostWithBodyRequest(TSEnvironmentFQDN, path, accessToken, bodyContent));
        }

        private static HttpWebRequest CreatePostWithBodyRequest(string host, string path, string accessToken, string bodyContent)
        {
            Uri uri = new UriBuilder("https", host)
            {
                Path = path,
                Query = "api-version=2016-12-12"
            }.Uri;

            HttpWebRequest request = WebRequest.CreateHttp(uri);
            request.Method = "POST";
            request.Headers.Add("x-ms-client-application-name", "Grotesque");
            request.Headers.Add("Authorization", "Bearer " + accessToken);
            request.ContentType = "application/json";
            request.ContentLength = bodyContent.Length;
            using (Stream webStream = request.GetRequestStream())
            using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.UTF8))
            {
                requestWriter.Write(bodyContent);
            }

            return request;
        }

        private static HttpWebRequest CreateHttpsWebRequest(string host, string method, string path, string accessToken, string[] queryArgs = null)
        {
            string query = "api-version=2016-12-12";
            if (queryArgs != null && queryArgs.Any())
            {
                query += "&" + String.Join("&", queryArgs);
            }

            Uri uri = new UriBuilder("https", host)
            {
                Path = path,
                Query = query
            }.Uri;

            HttpWebRequest request = WebRequest.CreateHttp(uri);
            request.Method = method;
            request.Headers.Add("x-ms-client-application-name", "Grotesque");
            request.Headers.Add("Authorization", "Bearer " + accessToken);

            return request;
        }

        private static JToken GetResponse(HttpWebRequest request)
        {
            using (WebResponse webResponse = request.GetResponse())
            using (var sr = new StreamReader(webResponse.GetResponseStream()))
            {
                string result = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<JToken>(result);
            }
        }
    }
}
