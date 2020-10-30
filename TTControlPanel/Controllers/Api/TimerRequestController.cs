using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TTControlPanel.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/TimerRequest")]
    [ApiController]
    public class TimerRequestController : ControllerBase
    {
        [HttpGet]
        [EnableCors("AllowOrigin")]
        public async Task<IActionResult> Get(int returnCode = 200, int timer = 5000)
        {
            await Task.Delay(timer);
            if (returnCode == 200)
                return Ok(new { ok = true });
            else
                return NotFound();
        }
    }
}
