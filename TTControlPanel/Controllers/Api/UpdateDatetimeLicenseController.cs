using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTControlPanel.Models;
using TTControlPanel.Services;
using TTControlPanel.Utilities;

namespace TTControlPanel.Controllers.Api
{
    [Route("api/UpdateDatetimeLicense")]
    [ApiController]
    public class UpdateDatetimeLicenseController : ControllerBase
    {
        private readonly DBContext _dB;

        public UpdateDatetimeLicenseController(DBContext dB)
        {
            _dB = dB ?? throw new ArgumentNullException(nameof(dB));
        }

        [HttpGet]
        public async Task<IActionResult> Get(string productKey, long date)
        {
            try
            {
                var dtUtc = date.FromUnixTime();
                //var localDt = dt.ToDateTimeCE(); //dt.ToLocalTime();
                var lic = await _dB.Licenses
                    .Include(l => l.ProductKey)
                    .Where(l => l.ProductKey.Key == productKey && l.Active)
                    .FirstOrDefaultAsync();
                if(lic != null)
                {
                    if(lic.ActivateDateTimeUtc != dtUtc)
                        lic.ActivateDateTimeUtc = dtUtc;

                    //last log update
                    var ll = await _dB.LastLogs.Include(l => l.License).Where(l => l.License.Id == lic.Id).FirstOrDefaultAsync();
                    if (ll == null)
                    {
                        var l = new LastLog
                        {
                            Api = Models.Api.UpdateDateTimeLicense,
                            License = lic,
                            DateTimeUtc = DateTime.Now.ToUniversalTime()
                        };
                        await _dB.LastLogs.AddAsync(l);
                    }
                    else
                    {
                        ll.Api = Models.Api.UpdateDateTimeLicense;
                        ll.DateTimeUtc = DateTime.Now.ToUniversalTime();
                    }
                    await _dB.SaveChangesAsync();
                    return Ok();
                }
                return NotFound();
            }
            catch { return NotFound(); }
        }
    }
}