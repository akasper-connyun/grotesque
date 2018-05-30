using Microsoft.AspNetCore.Mvc;

namespace Grotesque.Controllers
{
    [Route("")]
    public class RootController : Controller
    {
        [HttpGet]
        public IActionResult GetRoot()
        {
            // for K8s health probe
            return Ok();
        }
    }
}