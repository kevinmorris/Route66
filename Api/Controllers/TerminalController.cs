using Api.Models;
using Api.State;
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

            return Ok();
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
        [HttpGet]
        public ActionResult<IEnumerable<FieldData>[]> Index()
        {
            var terminalStateKey =
                HttpContext.Session.GetString(KEY_TERMINAL_STATE);

            if (terminalStateKey == null)
            {
                return BadRequest("No state key set in session");
            }

            var terminalState = pool[terminalStateKey];
            return terminalState == null
                ? BadRequest("No terminal launched for that state key")
                : terminalState.FieldData;
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> SendKey([FromBody] FieldSubmission submission)
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
                await terminalState.SendFields(submission);
                return Ok();
            }
        }

    }
}
