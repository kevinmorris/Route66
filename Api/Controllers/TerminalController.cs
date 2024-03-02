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
        private const string KEY_TERMINAL_STATE = "KEY_TERMINAL_STATE";

        [Route("connection")]
        [HttpPost]
        public IActionResult Connection([FromBody] ConnectionData connection)
        {
            var terminalStateKey =
                HttpContext.Session.GetString(KEY_TERMINAL_STATE) ??
                Guid.NewGuid().ToString();

            HttpContext.Session.SetString(KEY_TERMINAL_STATE, terminalStateKey);
            pool.Start(terminalStateKey, connection.Address, connection.Port);

            return RedirectToAction("Index");
        }

        [Route("poll")]
        public bool Poll()
        {
            var terminalStateKey =
                HttpContext.Session.GetString(KEY_TERMINAL_STATE);

            var newDataAvailable = (terminalStateKey != null && (pool[terminalStateKey]?.NewDataAvailable ?? false));
            return newDataAvailable;
        }

        [Route("")]
        public ActionResult<IEnumerable<FieldData>[]> Index()
        {
            var terminalStateKey =
                HttpContext.Session.GetString(KEY_TERMINAL_STATE);

            if (terminalStateKey == null)
            {
                return BadRequest("No state key set in session");
            }

            var terminalState = pool[terminalStateKey];
            if (terminalState == null)
            {
                return BadRequest("No terminal launched for that state key");
            }
            else
            {
                return terminalState.FieldData;
            }
        }
    }
}
