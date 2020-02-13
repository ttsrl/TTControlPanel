using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTControlPanel.Services;
using TTLL.Models;

namespace TTControlPanel.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/GetLicense")]
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
                var pk = await _dB.Licenses
                .Include(l => l.ApplicationVersion)
                    .ThenInclude(l => l.Application)
                .Include(l => l.Hid)
                .Include(l => l.ProductKey)
                .Include(l => l.Client)
                .Where(l => l.ProductKey.Key == productKey && l.Hid.Value == hid)
                .FirstOrDefaultAsync();
                if (pk == null)
                    return NotFound(new { });
                var obj = new ApiLicenseStructure
                {
                    State = (int)pk.State,
                    ApplicationCode = pk.ApplicationVersion.Application.Code,
                    ApplicationVersion = pk.ApplicationVersion.Version,
                    ClientCode = pk.Client.Code,
                    ProductKey = pk.ProductKey.Key,
                    HID = pk.Hid.Value,
                    ConfirmCode = pk.ConfirmCode
                };
                return Ok(obj);
            }
            catch { return NotFound(new { }); }
        }
    }
}