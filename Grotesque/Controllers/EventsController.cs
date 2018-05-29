﻿using System.Net.Http;
using System.Threading.Tasks;
using Grotesque.Models;
using Grotesque.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Grotesque.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class EventsController : Controller
    {
        private readonly ILogger _logger;
        private readonly TSApiClient tsApiClient = new TSApiClient();

        public EventsController(ILogger<EventsController> logger) => _logger = logger;

        [HttpPost]
        public async Task<IActionResult> GetEvents([FromBody] JObject query)
        {
            _logger.LogDebug("Getting events");
            if (query == null)
            {
                _logger.LogWarning("Could not retrieve query JSON from request");
                return BadRequest(error: new StringContent("Missing correct query body!"));
            }

            HttpResponseMessage response = await tsApiClient.GetEvents(query);
            if (response.IsSuccessStatusCode)
            {
                return Content(await response.Content.ReadAsStringAsync(), "application/json");
            }

            _logger.LogWarning(response.ReasonPhrase);
            return BadRequest();
        }

        [HttpPost("{deviceUrn}/{elementName}")]
        public async Task<IActionResult> GetEvents(string deviceUrn, string elementname, [FromBody]TopDataPointsQuery topDataPointsQuery)
        {
            HttpResponseMessage responseMessage =  await tsApiClient.GetLastDataPoints(topDataPointsQuery.from, topDataPointsQuery.to, deviceUrn, elementname, topDataPointsQuery.count);
            if (responseMessage.IsSuccessStatusCode)
            {
                return Content(await responseMessage.Content.ReadAsStringAsync(), "application/json");
            }

            _logger.LogWarning($"Reason: {responseMessage.ReasonPhrase} - {await responseMessage.Content.ReadAsStringAsync()}");
            return new BadRequestObjectResult(await responseMessage.Content.ReadAsStringAsync());
        }
    }
}
