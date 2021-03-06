﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Grotesque.Models;
using Grotesque.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Grotesque.Controllers
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class KpiController : Controller
    {
        private readonly ILogger _logger;
        private readonly TSApiClient tsApiClient = new TSApiClient();

        public KpiController(ILogger<KpiController> logger) => _logger = logger;

        [HttpGet("last/{deviceUrn}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<MetricValue>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(JObject))]
        public async Task<IActionResult> GetLastKpisForDevice(string deviceUrn, [FromQuery] FromToQueryParameters queryParameters)
        {
            if (ModelState.IsValid)
            {
                int diffMinutes = queryParameters.diffMinutes();
                if (diffMinutes > 0)
                {
                    HttpResponseMessage responseMessage = await tsApiClient.GetLastKpisForDevice(deviceUrn, queryParameters.from, queryParameters.to, $"{diffMinutes}m");
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        return await MapLatestValueQueryResult(responseMessage);
                    }

                    _logger.LogWarning($"Reason: {responseMessage.ReasonPhrase} - {await responseMessage.Content.ReadAsStringAsync()}");
                    return new BadRequestObjectResult(await responseMessage.Content.ReadAsStringAsync());
                }

                ModelState.AddModelError("to", "to value needs to be greater than from value");
                var errorMessages = JsonConvert.SerializeObject(ModelState.Values
                    .SelectMany(value => value.Errors)
                    .Select(error => error.Exception?.Message ?? error.ErrorMessage));

                _logger.LogWarning($"Input errors: {errorMessages}");
                return new BadRequestObjectResult(errorMessages);
            }

            return new BadRequestObjectResult(ModelState);
        }

        [HttpGet("avg/{deviceUrn}/{metric}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<TimestampValue>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(JObject))]
        public async Task<IActionResult> GetKpisForDeviceAndDimension(string deviceUrn, string metric, [FromQuery] FromToSizeQueryParameters queryParameters)
        {
            if (ModelState.IsValid)
            {
                int diffMinutes = queryParameters.diffMinutes();
                if (diffMinutes > 0)
                {
                    HttpResponseMessage responseMessage = await tsApiClient.GetKpiForDevice(deviceUrn, metric, queryParameters.from, queryParameters.to, queryParameters.size);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        return await MapKpiQueryResult(responseMessage);
                    }

                    _logger.LogWarning($"Reason: {responseMessage.ReasonPhrase} - {await responseMessage.Content.ReadAsStringAsync()}");
                    return new BadRequestObjectResult(await responseMessage.Content.ReadAsStringAsync());
                }

                ModelState.AddModelError("to", "to value needs to be greater than from value");
                var errorMessages = JsonConvert.SerializeObject(ModelState.Values
                    .SelectMany(value => value.Errors)
                    .Select(error => error.Exception?.Message ?? error.ErrorMessage));

                _logger.LogWarning($"Input errors: {errorMessages}");
                return new BadRequestObjectResult(errorMessages);
            }

            return new BadRequestObjectResult(ModelState);
        }

        private async Task<IActionResult> MapLatestValueQueryResult(HttpResponseMessage responseMessage)
        {
            string queryResultString = await responseMessage.Content.ReadAsStringAsync();
            JObject queryResultJson = JObject.Parse(queryResultString);

            _logger.LogInformation(JsonConvert.SerializeObject(queryResultJson));

            JObject aggregate = (JObject) queryResultJson.GetValue("aggregates")?.First;
            if (aggregate == null)
                return NotFound();

            JArray dimensions = (JArray)aggregate.GetValue("dimension");
            if (dimensions.Count == 0)
                return Content("[]", "application/json");

            JObject agg = (JObject)aggregate.GetValue("aggregate");
            JArray measures = (JArray)agg.GetValue("measures");
            var result = dimensions.Zip(measures, (d, m) => (d, m))
                .Select((entry) => {
                    JToken v = entry.m.Type == JTokenType.Null ? null : entry.m.Last;
                    return new MetricValue
                    {
                        metric = (string)entry.d,
                        value = v.Type == JTokenType.Null ? null : v.First
                    };
                }
            );

            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        private async Task<IActionResult> MapKpiQueryResult(HttpResponseMessage responseMessage)
        {
            string queryResultString = await responseMessage.Content.ReadAsStringAsync();
            JObject queryResultJson = JObject.Parse(queryResultString);

            _logger.LogInformation(JsonConvert.SerializeObject(queryResultJson));

            JObject aggregate = (JObject)queryResultJson.GetValue("aggregates")?.First;
            if (aggregate == null)
                return NotFound();

            JObject agg = (JObject)aggregate.GetValue("aggregate");
            JArray dimensions = (JArray)agg.GetValue("dimension");
            if (dimensions.Count == 0)
                return Content("[]", "application/json");

            JArray measures = (JArray)agg.GetValue("measures")?.First;
            var result = dimensions.Zip(measures, (d, m) => (d, m))
                .Select((entry) => new TimestampValue
                {
                    timestamp = (DateTime)entry.d,
                    value = entry.m?.First
                });

            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
    }

    public class FromToQueryParameters
    {
        [BindRequired]
        public DateTime from { get; set; }
        [BindRequired]
        public DateTime to { get; set; }

        public int diffMinutes()
        {
            return (int) to.Subtract(from).TotalMinutes;
        }
    }

    public class FromToSizeQueryParameters : FromToQueryParameters
    {
        [BindRequired]
        public string size { get; set; }
    }
}