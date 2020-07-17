using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTControlPanel.Filters;
using TTControlPanel.Models;
using TTControlPanel.Models.ViewModel;
using TTControlPanel.Services;

namespace TTControlPanel.Controllers
{
    public class ClientController : Controller
    {
        private readonly DBContext _db;

        public ClientController(DBContext db, Cryptography c)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [Authentication]
        public async Task<IActionResult> Index()
        {            //var apps = await _db.ApplicationsVersions
            //    .Include(a => a.Licences)
            //        .ThenInclude(l => l.Client)
            //    .Select(a => new { a.Application.Name, a.Licences })
            //    .ToDictionaryAsync(a => a.Name, a => a.Licences.Select(l => l.Client).ToList());
            var clients = await _db.Clients.ToListAsync();
            List<IndexClientModel.ClientApps> list = new List<IndexClientModel.ClientApps>();
            foreach (var c in clients)
            {
                var apps = await _db.Licenses
                    .Include(l => l.Client)
                    .Include(l => l.ApplicationVersion)
                        .ThenInclude(av => av.Application)
                    .Where(l => l.Client.Id == c.Id)
                    .ToListAsync();
                var ca = new IndexClientModel.ClientApps()
                {
                    Client = c,
                    Licenses = apps
                };
                list.Add(ca);
            }
            return View(new IndexClientModel { Clients = list /*Clients = clients, AppsClient = apps*/ });
        }

        [HttpGet]
        [Authentication]
        public IActionResult New()
        {
            return View(new NewClientGetModel());
        }

        [HttpPost]
        [Authentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New([FromServices] Utils utils, NewClientPostModel model)
        {
            if (ModelState.IsValid)
            {
                var uLog = HttpContext.Items["User"] as User;
                //validate code
                var code = "";
                if (model.AutomaticCode)
                    code = await utils.GenerateClientCode();
                else
                    code = (await utils.ValidateClientCode(model.Code)) ? model.Code : "";
                //validate vat
                var vat = await utils.ValidateVAT(model.VAT) ? model.VAT : "";
                if (string.IsNullOrEmpty(code))
                    return View(new NewClientGetModel { Error = 2 });
                if (string.IsNullOrEmpty(vat))
                    return View(new NewClientGetModel { Error = 3 });
                var addr = new Address
                {
                    Street = model.Street,
                    CAP = model.Cap,
                    City = model.City,
                    Province = model.Province,
                    Country = model.Country
                };
                await _db.Addresses.AddAsync(addr);
                var client = new Client
                {
                    Address = addr,
                    Code = code,
                    Name = model.Name,
                    VAT = model.VAT,
                    AddedUser = uLog
                };
                await _db.Clients.AddAsync(client);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(new NewClientGetModel { Error = 1 });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Details(int id)
        {
            var client = await _db.Clients.Include(c => c.Address).Where(c => c.Id == id).FirstOrDefaultAsync();
            if (client == null)
                return RedirectToAction("Index");
            var apps = await _db.ApplicationsVersions
                .Include(a => a.Licences)
                    .ThenInclude(l => l.Client)
                .Include(a => a.Application)
                .Where(a => a.Licences.Select(l => l.Client).Contains(client))
                .ToListAsync();
            return View(new ClientDetailsGetMode { Client = client, Applications = apps });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Edit(int id)
        {
            var client = await _db.Clients.Include(c => c.Address).Where(c => c.Id == id).FirstOrDefaultAsync();
            if (client == null)
                return RedirectToAction("Index");
            return View(new EditClientGetModel { Client = client });
        }

        [HttpPost]
        [Authentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromServices] Utils utils, int id, EditClientPostModel model)
        {
            var client = await _db.Clients.Include(c => c.Address).Where(c => c.Id == id).FirstOrDefaultAsync();
            if (client == null)
                return RedirectToAction("Index");
            if (ModelState.IsValid)
            {
                var code = (model.Code == client.Code) ? model.Code : ((await utils.ValidateClientCode(model.Code)) ? model.Code : "");
                var vat = (model.VAT == client.VAT) ? model.VAT : ((await utils.ValidateVAT(model.VAT)) ? model.VAT : "");
                if (string.IsNullOrEmpty(code))
                    return View(new EditClientGetModel { Client = client, Error = 2 });
                if (string.IsNullOrEmpty(vat))
                    return View(new EditClientGetModel { Client = client, Error = 3 });
                client.Name = model.Name;
                client.VAT = vat;
                client.Code = code;
                client.Address.CAP = model.Cap;
                client.Address.City = model.City;
                client.Address.Country = model.Country;
                client.Address.Province = model.Province;
                client.Address.Street = model.Street;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(new EditClientGetModel { Client = client, Error = 1 });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.Clients.Where(cc => cc.Id == id).FirstOrDefaultAsync();
            var clients = await _db.Clients.ToListAsync();
            List<IndexClientModel.ClientApps> list = new List<IndexClientModel.ClientApps>();
            foreach (var cc in clients)
            {
                var apps = await _db.Licenses
                    .Include(l => l.Client)
                    .Include(l => l.ApplicationVersion)
                        .ThenInclude(av => av.Application)
                    .Where(l => l.Client.Id == cc.Id)
                    .ToListAsync();
                var ca = new IndexClientModel.ClientApps()
                {
                    Client = cc,
                    Licenses = apps
                };
                list.Add(ca);
            }
            if (c == null)
                return View("Index", new IndexClientModel { Clients = list, Error = 1 });
            var appsc = list.Where(cccc => cccc.Client == c).FirstOrDefault();
            if (appsc.Licenses.Count > 0)
                return View("Index", new IndexClientModel { Clients = list, Error = 2 });
            _db.Addresses.Remove(c.Address);
            _db.Clients.Remove(c);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}