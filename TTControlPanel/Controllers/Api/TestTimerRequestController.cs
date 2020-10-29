using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TTControlPanel.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/TestTimerRequest")]
    [ApiController]
    public class TestTimerRequestController : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> Get(int returnValue)
        {
            await Task.Delay(10000);
            if (returnValue == 200)
                return Ok();
            else
                return NotFound();
        }
    }
}
