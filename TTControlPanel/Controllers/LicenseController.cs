using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TTControlPanel.Services;
using TTControlPanel.Models;
using System.Threading.Tasks;
using TTControlPanel.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace TTControlPanel.Controllers
{
    public class LicenseController : Controller
    {

        private readonly DBContext _db;

        public LicenseController(DBContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var licenses = await _db.Licenses
                .Include(l => l.ApplicationVersion)
                    .ThenInclude(v => v.Application)
                .Include(l => l.Client)
                .Include(l => l.ProductKey)
                .ToListAsync();
            var m = new IndexLicenseModel { Licenses = licenses };
            return View(m);
        }

        [HttpGet]
        public async Task<IActionResult> ApplicationVersion(int id)
        {
            var version = await _db.ApplicationsVersions.Where(v => v.Id == id)
                .Include(av => av.Application)
                .Include(av => av.Licences)
                    .ThenInclude(l => l.ProductKey)
                .Include(av => av.Licences)
                    .ThenInclude(l => l.Client)
                .FirstOrDefaultAsync();
            if (version == null)
                return RedirectToAction("Index");
            var m = new IndexLicenseModel { ApplicationVersion = version, Licenses = version.Licences };
            return View("Index", m);
        }
    }
}