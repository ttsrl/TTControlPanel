using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTControlPanel.Models;
using TTControlPanel.Services;
using static TTLL.License;

namespace TTControlPanel.Controllers.Api
{
    [Route("api/ActiveLicense")]
    public class ActiveLicenseController : ControllerBase
    {

        private readonly DBContext _dB;

        public ActiveLicenseController(DBContext dB)
        {
            _dB = dB ?? throw new ArgumentNullException(nameof(dB));
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromServices] Utils utils, string productKey, string appCode, string appVersion, string hid)
        {
            try
            {
                if(string.IsNullOrEmpty(productKey) || string.IsNullOrEmpty(appCode) || string.IsNullOrEmpty(appVersion) || string.IsNullOrEmpty(hid))
                    return NotFound();

                var l = await _dB.Licenses
                    .Include(p => p.ProductKey)
                    .Include(p => p.Hid)
                    .Include(p => p.Client)
                    .Include(p => p.ApplicationVersion)
                        .ThenInclude(v => v.Application)
                .Where(ll => ll.ProductKey.Key == productKey && !ll.Active && !ll.Banned && ll.ApplicationVersion.Application.Code == appCode && ll.ApplicationVersion.Version == appVersion)
                .FirstOrDefaultAsync();
                if (l == null)
                    return NotFound();
                var cnfc = GetConfirmCode(l.ProductKey.Key, hid);
                var h = new HID
                {
                    Value = hid
                };
                l.Hid = h;
                l.ConfirmCode = cnfc;
                l.Active = true;
                l.ActivateDateTime = DateTime.Now;
                await _dB.SaveChangesAsync();
                return Ok();
            }
            catch { return NotFound(); }
        }
    }
}