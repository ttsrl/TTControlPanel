using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTControlPanel.Models;
using TTControlPanel.Services;
using TTControlPanel.Utilities;
using TTLL.Models;

namespace TTControlPanel.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/GetLicense")]
    [ApiController]
    public class GetLicenseController : ControllerBase
    {
        private readonly DBContext _dB;

        public GetLicenseController(DBContext dB)
        {
            _dB = dB ?? throw new ArgumentNullException(nameof(dB));
        }

        [HttpGet]
        public async Task<IActionResult> Get(string productKey, string hid)
        {
            try
            {
                if(string.IsNullOrEmpty(productKey) || string.IsNullOrEmpty(hid))
                    return NotFound(new { });
                hid = hid.Replace("_", "-");
                var lic = await _dB.Licenses
                .Include(l => l.ApplicationVersion)
                    .ThenInclude(l => l.Application)
                .Include(l => l.Hid)
                .Include(l => l.ProductKey)
                .Include(l => l.Client)
                .Where(l => l.ProductKey.Key == productKey && l.Hid.Value == hid)
                .FirstOrDefaultAsync();
                if (lic == null)
                    return NotFound(new { });
                var obj = new ApiLicenseStructure
                {
                    Active = lic.Active,
                    Banned = lic.Banned,
                    ApplicationCode = lic.ApplicationVersion.Application.Code,
                    ApplicationVersion = lic.ApplicationVersion.Version,
                    ClientCode = lic.Client.Code,
                    ProductKey = lic.ProductKey.Key,
                    HID = lic.Hid.Value,
                    ConfirmCode = lic.ConfirmCode,
                    ActivationDateTimeUtc = lic.ActivateDateTimeUtc == null ? 0 : ((DateTime)lic.ActivateDateTimeUtc).ToUnixTime()
                };

                //last log update
                var ll = await _dB.LastLogs.Include(l => l.License).Where(l => l.License.Id == lic.Id).FirstOrDefaultAsync();
                if (ll == null)
                {
                    var l = new LastLog
                    {
                        Api = Models.Api.GetLicense,
                        License = lic,
                        DateTimeUtc = DateTime.Now.ToUniversalTime()
                    };
                    await _dB.LastLogs.AddAsync(l);
                }
                else
                {
                    ll.Api = Models.Api.GetLicense;
                    ll.DateTimeUtc = DateTime.Now.ToUniversalTime();
                }
                await _dB.SaveChangesAsync();
                return Ok(obj);
            }
            catch { return NotFound(new { }); }
        }
    }
}