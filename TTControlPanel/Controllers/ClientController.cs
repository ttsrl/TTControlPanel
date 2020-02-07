using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> Index()
        {
            var clients = await _db.Clients.Include(c => c.Applications).ToListAsync();
            return View(new IndexClientModel { Clients = clients });
        }

        [HttpGet]
        public async Task<IActionResult> New()
        {
            return View(new NewClientGetModel());
        }

        [HttpPost]
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
    }
}