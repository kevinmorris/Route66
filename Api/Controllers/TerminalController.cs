using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Services;
using Services.Models;

namespace Api.Controllers
{
    [ApiController]
    public class TerminalController(TerminalState terminalState) : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
