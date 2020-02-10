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
        public async Task<IActionResult> VersionLicenses(int id)
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
            var m = new VersionLicensesModel { ApplicationVersion = version, Licenses = version.Licences };
            return View(m);
        }

        [HttpGet]
        public async Task<IActionResult> New(int error = 0)
        {
            var apps = await _db.Applications
                .Include(a => a.ApplicationVersions)
                .ToListAsync();
            var clients = await _db.Clients.ToListAsync();
            return View(new NewLicenseGetModel { Applications = apps, Clients = clients, Error = error });
        }

        [HttpGet]
        public async Task<IActionResult> NewByVersion(int id)
        {
            var apps = await _db.Applications
                .Include(a => a.ApplicationVersions)
                .ToListAsync();
            var clients = await _db.Clients.ToListAsync();
            var selected = await _db.ApplicationsVersions.Where(v => v.Id == id).Include(v => v.Application).FirstOrDefaultAsync();
            return View("New", new NewLicenseGetModel { Applications = apps, Clients = clients, Selected = selected });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New([FromServices] Utils utils, NewLicensePostModel model)
        {
            var uLog = HttpContext.Items["User"] as User;
            if (ModelState.IsValid)
            {
                var app = await _db.Applications.Where(a => a.Id == model.Application).FirstOrDefaultAsync();
                var appv = await _db.ApplicationsVersions.Where(v => v.Id == model.Version).Include(v => v.Application).FirstOrDefaultAsync();
                var client = await _db.Clients.Include(c => c.Applications).Where(c => c.Id == model.Client).FirstOrDefaultAsync();
                if(app == null || appv == null || client == null)
                    RedirectToAction("New", new { error = 2 });
                if(appv.Application != app)
                    RedirectToAction("New", new { error = 2 });
                var productkey = utils.GenerateProdutKey(app, appv, client);
                var p = new ProductKey { GenerateDateTime = DateTime.Now, GenerateUser = uLog, Key = productkey };
                var l = new License
                {
                    Activate = false,
                    Client = client,
                    ApplicationVersion = appv,
                    ProductKey = p
                };
                if (!client.Applications.Contains(app))
                    client.Applications.Add(app);
                await _db.ProductKeys.AddAsync(p);
                await _db.Licenses.AddAsync(l);
                await _db.SaveChangesAsync();
                return RedirectToAction("VersionLicenses", new { id = appv.Id });
            }
            return RedirectToAction("New", new { error = 1 });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var lc = await _db.Licenses
                .Include(l => l.ProductKey)
                .Include(l => l.ApplicationVersion)
                    .ThenInclude(av => av.Application)
                .Include(l => l.Client)
                .Include(l => l.Hid)
                .FirstOrDefaultAsync();
            return View(new DetailsLicenseModel { License = lc });
        }

        [HttpGet]
        public async Task<IActionResult> GenerateConfirmCode(int id, int error = 0)
        {
            var l = await _db.Licenses.Include(ll => ll.ProductKey).Where(ll => ll.Id == id).FirstOrDefaultAsync();
            if (l == null)
                return RedirectToAction("Index");
            return View(new ConfirmCodeGetModel { License = l, Error = error });
        }

        [HttpPost]
        public async Task<IActionResult> GenerateConfirmCode([FromServices] Utils utils, int id, string hid)
        {
            var l = await _db.Licenses.Include(ll => ll.ProductKey).Include(ll => ll.Hid).Where(ll => ll.Id == id).FirstOrDefaultAsync();
            if(l == null)
                return RedirectToAction("Index");
            if (l.ProductKey == null)
                return RedirectToAction("Index");
            if (string.IsNullOrEmpty(hid))
                return RedirectToAction("GenerateConfirmCode", new { id = id, error = 1 });
            if (l.Hid.Value == hid)
                return RedirectToAction("GenerateConfirmCode", new { id = id, error = 2 });
            var cnfc = utils.GenerateConfirmCode(l.ProductKey, hid);
            var h = new HID
            {
                Value = hid
            };
            l.Hid = h;
            l.ConfirmCode = cnfc;
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new { id = id });
        }
    }
}