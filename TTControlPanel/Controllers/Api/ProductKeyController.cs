using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTControlPanel.Services;

namespace TTControlPanel.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/ProductKey")]
    [ApiController]
    public class ProductKeyController : ControllerBase
    {
        private readonly DBContext _dB;

        public ProductKeyController(DBContext dB)
        {
            _dB = dB ?? throw new ArgumentNullException(nameof(dB));
        }

        [HttpGet]
        public async Task<IActionResult> Get(string productKey)
        {
            try
            {
                if(string.IsNullOrEmpty(productKey))
                    return NotFound(new { });
                var lic = await _dB.Licenses
                    .Include(l => l.ProductKey)
                    .Where(l => l.ProductKey.Key == productKey)
                    .FirstOrDefaultAsync();
                if (lic == null)
                    return NotFound(new { });
                return Ok(new { });
            }
            catch { return NotFound(new { }); }
        }
    }
}