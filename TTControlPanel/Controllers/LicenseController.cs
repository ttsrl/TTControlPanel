using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TTControlPanel.Services;
using TTControlPanel.Models;
using System.Threading.Tasks;
using TTControlPanel.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using static TTControlPanel.Models.ProductKey;
using TTControlPanel.Filters;
using static TTLL.License;
using TTControlPanel.Utilities;

namespace TTControlPanel.Controllers
{
    public class LicenseController : Controller
    {
        private readonly Utils _utils;
        private readonly DBContext _db;

        public LicenseController(DBContext db, Utils utils)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _utils = utils ?? throw new ArgumentNullException(nameof(utils));
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Index()
        {
            var licenses = await _db.Licenses
                .Include(l => l.ApplicationVersion)
                    .ThenInclude(v => v.Application)
                .Include(l => l.Client)
                .Include(l => l.ProductKey)
                .OrderByDescending(l => l.Id)
                .ToListAsync();
            var m = new IndexLicenseModel { Licenses = licenses };
            return View(m);
        }

        [HttpGet]
        [Authentication]
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
        [Authentication]
        public async Task<IActionResult> New()
        {
            var apps = await _db.Applications
                .Include(a => a.ApplicationVersions)
                .ToListAsync();
            var clients = await _db.Clients.ToListAsync();
            return View(new NewLicenseGetModel { Applications = apps, Clients = clients });
        }

        [HttpPost]
        [Authentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(NewLicensePostModel model)
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
                var client = await _db.Clients.Where(c => c.Id == model.Client).FirstOrDefaultAsync();
                if (app == null || appv == null || client == null)
                    return View(new NewLicenseGetModel { Applications = apps, Clients = clients, Error = 2 });
                if (appv.Application != app)
                    return View(new NewLicenseGetModel { Applications = apps, Clients = clients, Error = 2 });
                if(model.Type < 0 || model.Type > 2)
                    return View(new NewLicenseGetModel { Applications = apps, Clients = clients, Error = 2 });
                var type = (PKType)model.Type;
                if(model.Days <= 0)
                    return View(new NewLicenseGetModel { Applications = apps, Clients = clients, Error = 2 });
                TimeSpan? time = null;
                if(type != PKType.Normal)
                    time = new TimeSpan(model.Days, 0, 0, 0);
                var productkey = await _utils.GenerateProdutKey(type, appv, client, time);
                var p = new ProductKey { GenerateDateTime = DateTimeCE.Now, GenerateUser = uLog, Key = productkey, Type = type };
                var l = new License
                {
                    Active = false,
                    Banned = false,
                    Client = client,
                    ApplicationVersion = appv,
                    ProductKey = p,
                    Notes = model.Notes
                };
                await _db.ProductKeys.AddAsync(p);
                await _db.Licenses.AddAsync(l);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(new NewLicenseGetModel { Applications = apps, Clients = clients, Error = 1 });
        }

        [HttpGet]
        [Authentication]
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
        [Authentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrecompiledNew(int id, PrecompiledNewLicensePostModel model)
        {
            var uLog = HttpContext.Items["User"] as User;
            var clients = await _db.Clients.ToListAsync();
            var av = await _db.ApplicationsVersions.Include(v => v.Licences).Include(v => v.Application).Where(v => v.Id == id).FirstOrDefaultAsync();
            if (av == null)
                return RedirectToAction("Index");
            if (ModelState.IsValid)
            {
                var client = await _db.Clients.Where(c => c.Id == model.Client).FirstOrDefaultAsync();
                if(client == null)
                    return View(new PrecompiledNewLicenseGetModel { Clients = clients, ApplicationVersion = av, Error = 2 });
                if (model.Type < 0 || model.Type > 2)
                    return View(new PrecompiledNewLicenseGetModel { Clients = clients, ApplicationVersion = av, Error = 2 });
                var type = (PKType)model.Type;
                if (model.Days <= 0)
                    return View(new PrecompiledNewLicenseGetModel { Clients = clients, ApplicationVersion = av, Error = 2 });
                TimeSpan? time = null;
                if (type != PKType.Normal)
                    time = new TimeSpan(model.Days, 0, 0, 0);
                var productkey = await _utils.GenerateProdutKey(type, av, client, time);
                var p = new ProductKey { GenerateDateTime = DateTimeCE.Now, GenerateUser = uLog, Key = productkey, Type = type };
                var l = new License
                {
                    Active = false,
                    Banned = false,
                    Client = client,
                    ApplicationVersion = av,
                    ProductKey = p,
                    Notes = model.Notes
                };
                await _db.ProductKeys.AddAsync(p);
                await _db.Licenses.AddAsync(l);
                await _db.SaveChangesAsync();
                return RedirectToAction("VersionLicenses", new { id = id });
            }
            return View(new PrecompiledNewLicenseGetModel { Clients = clients, ApplicationVersion = av, Error = 1 });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Delete(int id, int mod)
        {
            var l = await _db.Licenses
                .Include(ll => ll.ApplicationVersion)
                    .ThenInclude(av => av.Application)
                .Include(ll => ll.ApplicationVersion)
                    .ThenInclude(av => av.Licences)
                .Include(ll => ll.ApplicationVersion)
                    .ThenInclude(av => av.Licences)
                        .ThenInclude(av => av.ProductKey)
                .Include(ll => ll.ApplicationVersion)
                    .ThenInclude(av => av.Licences)
                        .ThenInclude(ll => ll.Client)
                .Include(ll => ll.Hid)
                .Where(ll => ll.Id == id)
                .FirstOrDefaultAsync();

            if (l == null)
            {
                if (mod == 0)
                    return View("Index", new IndexLicenseModel { Licenses = l.ApplicationVersion.Licences, Error = 1 });
                else
                    return View("VersionLicenses", new VersionLicensesModel { ApplicationVersion = l.ApplicationVersion, Licenses = l.ApplicationVersion.Licences, Error = 1 });
            }
            if(l.Active == true)
            {
                if (mod == 0)
                    return View("Index", new IndexLicenseModel { Licenses = l.ApplicationVersion.Licences, Error = 2 });
                else
                    return View("VersionLicenses", new VersionLicensesModel { ApplicationVersion = l.ApplicationVersion, Licenses = l.ApplicationVersion.Licences, Error = 2 });
            }

            if (l.Hid != null)
                _db.Hids.Remove(l.Hid);

            _db.Licenses.Remove(l);
            await _db.SaveChangesAsync();
            if (mod == 0)
                return View("Index", new IndexLicenseModel { Licenses = l.ApplicationVersion.Licences });
            else
                return View("VersionLicenses", new VersionLicensesModel { ApplicationVersion = l.ApplicationVersion, Licenses = l.ApplicationVersion.Licences });
        }


        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Details(int id)
        {
            var lc = await _db.Licenses
                .Include(l => l.ProductKey)
                    .ThenInclude(pk => pk.GenerateUser)
                .Include(l => l.ApplicationVersion)
                    .ThenInclude(av => av.Application)
                .Include(l => l.Client)
                .Include(l => l.Hid)
                    .ThenInclude(h => h.AddedUser)
                .Where(l => l.Id == id)
                .FirstOrDefaultAsync();
            return View(new DetailsLicenseModel { License = lc });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> AddRequestCode(int id)
        {
            var l = await _db.Licenses
                .Include(ll => ll.ProductKey)
                .Include(ll => ll.ApplicationVersion)
                    .ThenInclude(av => av.Application)
                .Include(ll => ll.Client)
                .Include(ll => ll.Hid)
                .Where(ll => ll.Id == id).FirstOrDefaultAsync();
            if (l == null)
                return RedirectToAction("Index");
            if (l.ProductKey == null)
                return View("Details", new DetailsLicenseModel { License = l, Error = 1 });
            if (l.Hid != null )
                return View("Details", new DetailsLicenseModel { License = l, Error = 2 });
            return View(new RequestCodeGetModel { License = l });
        }


        [HttpGet]
        [Authentication]
        public async Task<IActionResult> EditRequestCode(int id)
        {
            var l = await _db.Licenses
               .Include(ll => ll.ProductKey)
               .Include(ll => ll.ApplicationVersion)
                   .ThenInclude(av => av.Application)
               .Include(ll => ll.Client)
               .Include(ll => ll.Hid)
               .Where(ll => ll.Id == id).FirstOrDefaultAsync();
            if (l == null)
                return RedirectToAction("Index");
            if (l.ProductKey == null)
                return View("Details", new DetailsLicenseModel { License = l, Error = 1 });
            if (l.Hid == null)
                return View("Details", new DetailsLicenseModel { License = l, Error = 3 });
            return View("AddRequestCode", new RequestCodeGetModel { License = l });
        }


        [HttpPost]
        [Authentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRequestCode(int id, string hid)
        {
            var uLog = HttpContext.Items["User"] as User;
            var l = await _db.Licenses
                .Include(ll => ll.ProductKey)
                .Include(ll => ll.Hid)
                .Where(ll => ll.Id == id).FirstOrDefaultAsync();
            if(l == null)
                return RedirectToAction("Index");
            if (l.ProductKey == null)
                return RedirectToAction("Index");
            if (string.IsNullOrEmpty(hid))
                return View("AddRequestCode", new RequestCodeGetModel { License = l, Error = 1 });
            if (l.Hid != null && l.Hid.Value == hid)
                return View("AddRequestCode", new RequestCodeGetModel { License = l, Error = 2 });
            var cnfc = GetConfirmCode(l.ProductKey.Key, hid);
            var h = new HID { Value = hid, AddedUser = uLog };
            l.Hid = h;
            l.ConfirmCode = cnfc;
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new { id = id });
        }


        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Ban(int id, int mod)
        {
            var l = await _db.Licenses.Include(ll => ll.ApplicationVersion).Where(ll => ll.Id == id).FirstOrDefaultAsync();
            if (l == null)
                return RedirectToAction("Index");
            l.Banned = true;
            await _db.SaveChangesAsync();
            if (mod == 0)
                return RedirectToAction("Index");
            else
                return RedirectToAction("VersionLicenses", new { id = l.ApplicationVersion.Id });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Restore(int id, int mod)
        {
            var l = await _db.Licenses.Include(ll => ll.ApplicationVersion).Where(ll => ll.Id == id).FirstOrDefaultAsync();
            if (l == null)
                return RedirectToAction("Index");
            l.Banned = false;
            await _db.SaveChangesAsync();
            if (mod == 0)
                return RedirectToAction("Index");
            else
                return RedirectToAction("VersionLicenses", new { id = l.ApplicationVersion.Id });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> OfflineActivation(int id)
        {
            var l = await _db.Licenses
                .Include(ll => ll.ProductKey)
                .Include(ll => ll.ApplicationVersion)
                    .ThenInclude(av => av.Application)
                .Include(ll => ll.Client)
                .Include(ll => ll.Hid)
                .Where(ll => ll.Id == id && !ll.Banned && !ll.Active).FirstOrDefaultAsync();
            if (l == null)
                return RedirectToAction("Index");
            if (l.Hid == null)
                return View("Details", new DetailsLicenseModel { License = l, Error = 2 });
            if (string.IsNullOrEmpty(l.ConfirmCode))
                return View("Details", new DetailsLicenseModel { License = l, Error = 3 });
            l.Active = true;
            l.ActivateDateTime = DateTimeCE.Now;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}