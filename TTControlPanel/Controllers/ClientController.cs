using System;
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
        {
            var clients = await _db.Clients.Include(c => c.Applications).ToListAsync();
            return View(new IndexClientModel { Clients = clients });
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
                    VAT = model.VAT
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
            var client = await _db.Clients.Include(c => c.Applications).Include(c => c.Address).Where(c => c.Id == id).FirstOrDefaultAsync();
            if (client == null)
                return RedirectToAction("Index");
            return View(new ClientDetailsGetMode { Client = client });
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
            var clients = await _db.Clients.Include(cc => cc.Applications).ToListAsync();
            var c = await _db.Clients.Include(cc => cc.Applications).Where(cc => cc.Id == id).FirstOrDefaultAsync();
            if (c == null)
                return View("Index", new IndexClientModel { Clients = clients, Error = 1 });
            if(c.Applications.Count > 0)
                return View("Index", new IndexClientModel { Clients = clients, Error = 2 });
            _db.Clients.Remove(c);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}