using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Services;
using Services.Models;

namespace Api.Controllers
{
    [ApiController]
    public class TerminalController(TerminalStatePool pool) : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            var terminalStateKey =
                HttpContext.Session.GetString("KEY_TERMINAL_STATE") ?? 
                Guid.NewGuid().ToString();

            HttpContext.Session.SetString("KEY_TERMINAL_STATE", terminalStateKey);

            var terminalState = pool[terminalStateKey];
            if (terminalState == null)
            {
                pool.Start(terminalStateKey);
            }

            return Ok();
        }
    }
}
