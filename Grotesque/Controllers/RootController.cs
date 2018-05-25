using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Grotesque.Controllers
{
    [Route("/")]
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