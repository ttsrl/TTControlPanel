using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TTControlPanel.Services;

namespace TTControlPanel.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/ActivateLicense")]
    public class ActivateLicenseController : ControllerBase
    {
        private readonly DBContext _dB;

        public ActivateLicenseController(DBContext dB)
        {
            _dB = dB ?? throw new ArgumentNullException(nameof(dB));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {

            return NotFound();
        }
    }
}