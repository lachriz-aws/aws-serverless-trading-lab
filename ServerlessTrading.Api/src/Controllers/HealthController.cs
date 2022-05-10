using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;

namespace ServerlessTrading.Api.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        [HttpGet("")]
        public IActionResult GetHealth()
        {
            return Ok($"Hello from {RuntimeInformation.FrameworkDescription} running on {RuntimeInformation.RuntimeIdentifier}.");
        }
    }
}
