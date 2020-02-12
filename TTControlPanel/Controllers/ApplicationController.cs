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
    public class ApplicationController : Controller
    {

        private readonly Cryptography _c;
        private readonly DBContext _db;

        public ApplicationController(DBContext db, Cryptography c)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _c = c ?? throw new ArgumentNullException(nameof(c));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var apps = await _db.Applications
                .Include(a => a.ApplicationVersions)
                .ToListAsync();
            var m = new IndexApplicationModel
            {
                Applications = apps
            };
            return View(m);
        }

        [HttpGet]
        public async Task<IActionResult> New()
        {
            return View(new NewApplicationGetModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New([FromServices] Utils utils, NewApplicationPostModel model)
        {
            if (ModelState.IsValid)
            {
                //validate code
                var code = "";
                if (model.AutomaticCode)
                    code = await utils.GenerateApplicationCode();
                else
                    code = (await utils.ValidateApplicationCode(model.Code)) ? model.Code : "";
                //validate version
                Version v = null;
                var resV = Version.TryParse(model.Major + "." + model.Minor, out v);
                var version = (resV) ? v.ToString() : "";
                //validate name
                var name = await utils.ValidateApplicationName(model.Name) ? model.Name : "";
                if(string.IsNullOrEmpty(code))
                    return View(new NewApplicationGetModel { Error = 2 });
                if (string.IsNullOrEmpty(name))
                    return View(new NewApplicationGetModel { Error = 3 });
                if (string.IsNullOrEmpty(version))
                    return View(new NewApplicationGetModel { Error = 4 });
                var appV = new ApplicationVersion
                {
                    ReleaseDate = model.Release,
                    Version = version
                };
                await _db.ApplicationsVersions.AddAsync(appV);
                var app = new Application
                {
                    Code = code,
                    Name = name,
                    ApplicationVersions = new List<ApplicationVersion>() { appV }
                };
                await _db.Applications.AddAsync(app);
                appV.Application = app;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(new NewApplicationGetModel { Error = 1 });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id )
        {
            var app = await _db.Applications.Include(a => a.ApplicationVersions).Where(a => a.Id == id).FirstOrDefaultAsync();
            if(app == null)
                return RedirectToAction("Index");
            if(app.ApplicationVersions.Count == 0)
                return RedirectToAction("Index");
            return View(new EditApplicationGetModel { Application = app, InitialVersion = app.ApplicationVersions[0] });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromServices] Utils utils, int id, EditApplicationPostModel model)
        {
            var app = await _db.Applications.Include(a => a.ApplicationVersions).Where(a => a.Id == id).FirstOrDefaultAsync();
            if (app == null)
                return RedirectToAction("Index");
            if (app.ApplicationVersions.Count == 0)
                return RedirectToAction("Index");
            if (ModelState.IsValid)
            {
                var code = (model.Code == app.Code) ? model.Code : ((await utils.ValidateApplicationCode(model.Code)) ? model.Code : "");
                var name = (model.Name == app.Name) ? model.Name : ((await utils.ValidateApplicationName(model.Name)) ? model.Name : "");
                if(string.IsNullOrEmpty(code))
                    return View(new EditApplicationGetModel { Application = app, InitialVersion = app.ApplicationVersions[0], Error = 2 });
                if (string.IsNullOrEmpty(name))
                    return View(new EditApplicationGetModel { Application = app, InitialVersion = app.ApplicationVersions[0], Error = 3 });
                app.Code = code;
                app.Name = name;
                app.ApplicationVersions[0].ReleaseDate = model.Release;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(new EditApplicationGetModel { Application = app, InitialVersion = app.ApplicationVersions[0], Error = 1 });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var app = await _db.Applications.Where(a => a.Id == id)
                .Include(a => a.ApplicationVersions)
                    .ThenInclude(v => v.Licences)
                        .ThenInclude(v => v.ProductKey)
                .Include(a => a.ApplicationVersions)
                    .ThenInclude(v => v.Licences)
                        .ThenInclude(v => v.Hid)
                .FirstOrDefaultAsync();
            if(app == null)
                return RedirectToAction("Index");
            foreach(var v  in app.ApplicationVersions)
            {
                foreach(var l in v.Licences)
                {
                    if(l.Hid != null)
                        _db.Hids.Remove(l.Hid);
                }
                _db.Licenses.RemoveRange(v.Licences);
            }
            _db.ApplicationsVersions.RemoveRange(app.ApplicationVersions);
            _db.Applications.Remove(app);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Versions(int id)
        {
            var app = await _db.Applications.Where(a => a.Id == id)
                .Include(a => a.ApplicationVersions)
                    .ThenInclude(v => v.Licences)
                        .ThenInclude(l => l.ProductKey)
                .Include(a => a.ApplicationVersions)
                    .ThenInclude(v => v.Licences)
                        .ThenInclude(l => l.Client)
                .FirstOrDefaultAsync();
            if (app == null)
                return RedirectToAction("Index");
            var m = new VersionsApplicationGetModel
            {
                Application = app
            };
            return View(m);
        }

        [HttpGet]
        public async Task<IActionResult> NewVersion(int id)
        {
            var app = await _db.Applications.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (app == null)
                return RedirectToAction("Index");
            return View(new NewVersionApplicationGetModel { Application = app });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewVersion(int id, NewVersionApplicationPostModel model)
        {
            //validate application
            var app = await _db.Applications.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (app == null)
                return View(new NewVersionApplicationGetModel { Application = app, Error = 2 });
            if (ModelState.IsValid)
            {
                //validate version
                Version v = null;
                var resV = Version.TryParse(model.Major + "." + model.Minor, out v);
                var strV = (resV) ? v.ToString() : "";
                if (string.IsNullOrEmpty(strV))
                    return View(new NewVersionApplicationGetModel { Application = app, Error = 3 });
                var vers = await _db.ApplicationsVersions.Where(av => av.Application == app && av.Version == strV).FirstOrDefaultAsync();
                if (vers != null)
                    return View(new NewVersionApplicationGetModel { Application = app, Error = 4 });
                var appV = new ApplicationVersion
                {
                    ReleaseDate = model.Release,
                    Version = strV,
                    Application = app,
                    Licences = new List<License>(),
                    Notes = model.Notes
                };
                await _db.ApplicationsVersions.AddAsync(appV);
                await _db.SaveChangesAsync();
                return RedirectToAction("Versions", new { id = id });
            }
            return View(new NewVersionApplicationGetModel { Application = app, Error = 1 });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteVersion(int id)
        {
            var ve = await _db.ApplicationsVersions
                .Include(v => v.Application)
                .Include(v => v.Licences)
                    .ThenInclude(l => l.Hid)
                .Where(v => v.Id == id)
                .FirstOrDefaultAsync();
            if(ve == null)
                return RedirectToAction("Index");
            foreach (var l in ve.Licences)
            {
                if(l.Hid != null)
                    _db.Hids.Remove(l.Hid);
                _db.Licenses.Remove(l);
            }
            _db.ApplicationsVersions.Remove(ve);
            await _db.SaveChangesAsync();
            return RedirectToAction("Versions", new { id = ve.Application.Id });
        }

    }
}