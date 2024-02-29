﻿using Api.Models;
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
        [Route("poll")]
        public bool Poll()
        {
            var terminalStateKey =
                HttpContext.Session.GetString("KEY_TERMINAL_STATE");

            var newDataAvailable = (terminalStateKey != null && (pool[terminalStateKey]?.NewDataAvailable ?? false));
            return newDataAvailable;
        }

        [Route("")]
        public ActionResult<IEnumerable<FieldData>[]> Index()
        {
            var terminalStateKey =
                HttpContext.Session.GetString("KEY_TERMINAL_STATE") ?? 
                Guid.NewGuid().ToString();

            HttpContext.Session.SetString("KEY_TERMINAL_STATE", terminalStateKey);

            var terminalState = pool[terminalStateKey];
            if (terminalState == null)
            {
                pool.Start(terminalStateKey);
                return Ok();
            }
            else
            {
                return terminalState.FieldData;
            }
        }
    }
}
