using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTControlPanel.Services;
using TTControlPanel.Utilities;

namespace TTControlPanel.Controllers.Api
{
    [Route("api/UpdateActivationDatetime")]
    [ApiController]
    public class UpdateActivationDatetimeController : ControllerBase
    {
        private readonly DBContext _dB;

        public UpdateActivationDatetimeController(DBContext dB)
        {
            _dB = dB ?? throw new ArgumentNullException(nameof(dB));
        }

        [HttpGet]
        public async Task<IActionResult> Get(string productKey, long date)
        {
            try
            {
                var dt = date.FromUnixTime();
                var localDt = dt.ToDateTimeCE(); //dt.ToLocalTime();
                var lic = await _dB.Licenses
                    .Include(l => l.ProductKey)
                    .Where(l => l.ProductKey.Key == productKey && l.Active)
                    .FirstOrDefaultAsync();
                if(lic != null)
                {
                    if(lic.ActivateDateTime != localDt)
                    {
                        lic.ActivateDateTime = localDt;
                        await _dB.SaveChangesAsync();
                    }
                    return Ok();
                }
                return NotFound();
            }
            catch { return NotFound(); }
        }
    }
}