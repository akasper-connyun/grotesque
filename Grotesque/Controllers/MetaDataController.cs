using System.Net.Http;
using System.Threading.Tasks;
using Grotesque.Models;
using Grotesque.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Grotesque.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class MetaDataController : Controller
    {
        private readonly ILogger _logger;
        private readonly TSApiClient tsApiClient = new TSApiClient();

        public MetaDataController(ILogger<MetaDataController> logger) => _logger = logger;

        [HttpGet("availability")]
        public async Task<IActionResult> GetAvailability()
        {
            HttpResponseMessage response = await tsApiClient.GetAvailability();
            if (response.IsSuccessStatusCode)
            {
                return Content(await response.Content.ReadAsStringAsync(), "application/json");
            }

            _logger.LogWarning(response.ReasonPhrase);
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> GetMetaData([FromBody] Metadata metadata)
        {
            if (metadata == null)
            {
                _logger.LogWarning("Could not retrieve metadata JSON from request");
                return BadRequest(error: new StringContent("Missing correct metadata body!"));
            }

            HttpResponseMessage response = await tsApiClient.GetMetadata(metadata);
            if (response.IsSuccessStatusCode)
            {
                return Content(await response.Content.ReadAsStringAsync(), "application/json");
            }

            _logger.LogWarning(response.ReasonPhrase);
            return BadRequest();
        }

        
    }
}