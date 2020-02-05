﻿using System;
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
                    code = utils.GenerateApplicationCode();
                else
                    code = (utils.ValidateApplicationCode(model.Code)) ? model.Code : "";
                //validate version
                Version v = null;
                var resV = Version.TryParse(model.Major + "." + model.Minor, out v);
                var version = (resV) ? v.ToString() : "";
                //validate name
                var name = utils.ValidateApplicationName(model.Name) ? model.Name : "";
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
        public async Task<IActionResult> Edit(int id)
        {
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var app = await _db.Applications.Where(a => a.Id == id)
                .Include(a => a.ApplicationVersions)
                    .ThenInclude(v => v.Licences)
                        .ThenInclude(v => v.ProductKey)
                .FirstOrDefaultAsync();
            if(app == null)
                return RedirectToAction("Index");
            foreach(var v  in app.ApplicationVersions)
            {
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
        public async Task<IActionResult> DeleteVersion(int id)
        {
            return RedirectToAction("Index");
        }

    }
}