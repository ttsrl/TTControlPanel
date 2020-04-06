using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTControlPanel.Models;
using TTControlPanel.Services;
using TTControlPanel.Utilities;
using static TTLL.License;

namespace TTControlPanel.Controllers.Api
{
    [Route("api/ActiveLicense")]
    [ApiController]
    public class ActiveLicenseController : ControllerBase
    {
        private readonly DBContext _dB;
        private readonly Utils _utils;
        public ActiveLicenseController(DBContext dB, Utils utils)
        {
            _dB = dB ?? throw new ArgumentNullException(nameof(dB));
            _utils = utils ?? throw new ArgumentNullException(nameof(utils));
        }

        [HttpGet]
        public async Task<IActionResult> Get(string productKey, string appCode, int appVersion, string hid)
        {
            try
            {
                if(string.IsNullOrEmpty(productKey) || string.IsNullOrEmpty(appCode) || appVersion == 0 || string.IsNullOrEmpty(hid))
                    return NotFound();
                var version = IntToVersion(appVersion);
                if(version == null)
                    return NotFound();
                string vstr = version.Major.ToString() + "." + version.Minor.ToString();
                var lic = await _dB.Licenses
                    .Include(p => p.ProductKey)
                    .Include(p => p.Hid)
                    .Include(p => p.Client)
                    .Include(p => p.ApplicationVersion)
                        .ThenInclude(v => v.Application)
                .Where(l => l.ProductKey.Key == productKey && !l.Banned && l.ApplicationVersion.Application.Code == appCode && l.ApplicationVersion.Version == vstr)
                .FirstOrDefaultAsync();
                if (lic == null)
                    return Ok(ActivationResult.InvalidData);
                if(lic.Banned)
                    return Ok(ActivationResult.Banned);
                if (lic.Active)
                    return Ok(ActivationResult.AlreadyActive);
                hid = hid.Replace("_", "-");
                var cnfc = GetConfirmCode(lic.ProductKey.Key, hid);
                var h = new HID
                {
                    Value = hid
                };
                lic.Hid = h;
                lic.ConfirmCode = cnfc;
                lic.Active = true;
                lic.ActivateDateTimeUtc = DateTime.Now.ToUniversalTime();

                //last log update
                var ll = await _dB.LastLogs.Include(l => l.License).Where(l => l.License.Id == lic.Id).FirstOrDefaultAsync();
                if (ll == null)
                {
                    var l = new LastLog
                    {
                        Api = Models.Api.ActiveLicense,
                        License = lic,
                        DateTimeUtc = DateTime.Now.ToUniversalTime()
                    };
                    await _dB.LastLogs.AddAsync(l);
                }
                else
                {
                    ll.Api = Models.Api.ActiveLicense;
                    ll.DateTimeUtc = DateTime.Now.ToUniversalTime();
                }
                await _dB.SaveChangesAsync();
                return Ok(ActivationResult.Activated);
            }
            catch { return NotFound(); }
        }
    }
}