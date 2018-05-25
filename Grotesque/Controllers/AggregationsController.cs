using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Grotesque.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Grotesque.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class AggregationsController : Controller
    {
        private readonly ILogger _logger;
        private readonly TSApiClient tsApiClient = new TSApiClient();

        public AggregationsController(ILogger<AggregationsController> logger) => _logger = logger;

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
    }
}