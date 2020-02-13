using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TTControlPanel.Services;
using TTControlPanel.Models;
using System.Threading.Tasks;
using TTControlPanel.Models.ViewModel;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> New()
        {
            var apps = await _db.Applications
                .Include(a => a.ApplicationVersions)
                .ToListAsync();
            var clients = await _db.Clients.ToListAsync();
            return View(new NewLicenseGetModel { Applications = apps, Clients = clients });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New([FromServices] Utils utils, NewLicensePostModel model)
        {
            var uLog = HttpContext.Items["User"] as User;
            var apps = await _db.Applications
                .Include(a => a.ApplicationVersions)
                .ToListAsync();
            var clients = await _db.Clients.ToListAsync();
            if (ModelState.IsValid)
            {
                var app = await _db.Applications.Where(a => a.Id == model.Application).FirstOrDefaultAsync();
                var appv = await _db.ApplicationsVersions.Include(v => v.Application).Where(v => v.Id == model.Version).FirstOrDefaultAsync();
                var client = await _db.Clients.Include(c => c.Applications).Where(c => c.Id == model.Client).FirstOrDefaultAsync();
                if (app == null || appv == null || client == null)
                    return View(new NewLicenseGetModel { Applications = apps, Clients = clients, Error = 2 });
                if (appv.Application != app)
                    return View(new NewLicenseGetModel { Applications = apps, Clients = clients, Error = 2 });
                var productkey = await utils.GenerateProdutKey(app, appv, client);
                var p = new ProductKey { GenerateDateTime = DateTime.Now, GenerateUser = uLog, Key = productkey };
                var l = new License
                {
                    State = License.LicenseState.Inactive,
                    Client = client,
                    ApplicationVersion = appv,
                    ProductKey = p,
                    Notes = model.Notes
                };
                if (!client.Applications.Contains(app))
                    client.Applications.Add(app);
                await _db.ProductKeys.AddAsync(p);
                await _db.Licenses.AddAsync(l);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(new NewLicenseGetModel { Applications = apps, Clients = clients, Error = 1 });
        }

        [HttpGet]
        public async Task<IActionResult> PrecompiledNew(int id)
        {
            var apps = await _db.Applications
                .Include(a => a.ApplicationVersions)
                .ToListAsync();
            var clients = await _db.Clients.ToListAsync();
            var av = await _db.ApplicationsVersions.Include(v => v.Application).Where(v => v.Id == id).FirstOrDefaultAsync();
            if (av == null)
                return RedirectToAction("Index");
            return View(new PrecompiledNewLicenseGetModel { Clients = clients, ApplicationVersion = av });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrecompiledNew([FromServices] Utils utils, int id, PrecompiledNewLicensePostModel model)
        {
            var uLog = HttpContext.Items["User"] as User;
            var clients = await _db.Clients.ToListAsync();
            var av = await _db.ApplicationsVersions.Include(v => v.Licences).Include(v => v.Application).Where(v => v.Id == id).FirstOrDefaultAsync();
            if (av == null)
                return RedirectToAction("Index");
            if (ModelState.IsValid)
            {
                var client = await _db.Clients.Include(c => c.Applications).Where(c => c.Id == model.Client).FirstOrDefaultAsync();
                if(client == null)
                    return View(new PrecompiledNewLicenseGetModel { Clients = clients, ApplicationVersion = av, Error = 2 });
                var productkey = await utils.GenerateProdutKey(av.Application, av, client);
                var p = new ProductKey { GenerateDateTime = DateTime.Now, GenerateUser = uLog, Key = productkey };
                var l = new License
                {
                    State = License.LicenseState.Inactive,
                    Client = client,
                    ApplicationVersion = av,
                    ProductKey = p,
                    Notes = model.Notes
                };
                if (!client.Applications.Contains(av.Application))
                    client.Applications.Add(av.Application);
                await _db.ProductKeys.AddAsync(p);
                await _db.Licenses.AddAsync(l);
                await _db.SaveChangesAsync();
                return RedirectToAction("VersionLicenses", new { id = id });
            }
            return View(new PrecompiledNewLicenseGetModel { Clients = clients, ApplicationVersion = av, Error = 1 });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int mod )
        {
            var l = await _db.Licenses.Include(ll => ll.ProductKey).Where(ll => ll.Id == id).FirstOrDefaultAsync();
            var cl = await _db.Clients.ToListAsync();
            if (l == null)
                return RedirectToAction("Index");
            return View(new EditLicenseGetModel { License = l, Clients = cl, Mode = mod });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int mod, EditLicensePostModel model)
        {
            var l = await _db.Licenses.Include(ll => ll.ApplicationVersion).Include(ll => ll.ProductKey).Where(ll => ll.Id == id).FirstOrDefaultAsync();
            var cl = await _db.Clients.ToListAsync();
            if(l == null)
                return RedirectToAction("Index");
            if (ModelState.IsValid)
            {
                var client = await _db.Clients.Where(c => c.Id == model.Client).FirstOrDefaultAsync();
                if(client == null)
                    return View(new EditLicenseGetModel { License = l, Clients = cl, Error = 2, Mode = mod });
                l.Client = client;
                await _db.SaveChangesAsync();
                if(mod == 0)
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("VersionLicenses", new { id = l.ApplicationVersion.Id });
            }
            return View(new EditLicenseGetModel { License = l, Clients = cl, Error = 1, Mode = mod });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, int mod)
        {
            var l = await _db.Licenses.Include(ll => ll.Hid).Where(ll => ll.Id == id).FirstOrDefaultAsync();
            if(l == null)
                return RedirectToAction("Index");
            if(l.Hid != null)
                _db.Hids.Remove(l.Hid);
            _db.Licenses.Remove(l);
            await _db.SaveChangesAsync();
            if (mod == 0)
                return RedirectToAction("Index");
            else
                return RedirectToAction("VersionLicenses", new { id = l.ApplicationVersion.Id });
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
        public async Task<IActionResult> GenerateConfirmCode(int id)
        {
            var l = await _db.Licenses.Include(ll => ll.ProductKey).Include(ll => ll.Hid).Where(ll => ll.Id == id).FirstOrDefaultAsync();
            if (l == null)
                return RedirectToAction("Index");
            if (l.ProductKey == null)
                return RedirectToAction("Index");
            return View(new ConfirmCodeGetModel { License = l });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateConfirmCode([FromServices] Utils utils, int id, string hid)
        {
            var l = await _db.Licenses.Include(ll => ll.ProductKey).Include(ll => ll.Hid).Where(ll => ll.Id == id).FirstOrDefaultAsync();
            if(l == null)
                return RedirectToAction("Index");
            if (l.ProductKey == null)
                return RedirectToAction("Index");
            if (string.IsNullOrEmpty(hid))
                return View(new ConfirmCodeGetModel { License = l, Error = 1 });
            if (l.Hid.Value == hid)
                return View(new ConfirmCodeGetModel { License = l, Error = 2 });
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