using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTControlPanel.Filters;
using TTControlPanel.Models;
using TTControlPanel.Models.ViewModel;
using TTControlPanel.Services;
using TTControlPanel.Utilities;

namespace TTControlPanel.Controllers
{
    public class OrderController : Controller
    {
        private readonly Utils _utils;
        private readonly DBContext _db;

        public OrderController(DBContext db, Utils utils)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _utils = utils ?? throw new ArgumentNullException(nameof(utils));
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Index(string orderby)
        {
            var query = _db.Orders
                .Include(o => o.Client)
                .Include(o => o.Invoice)
                .Include(o => o.Working)
                .Where(o => o.Id > 0);
            if (!string.IsNullOrEmpty(orderby))
            {
                if (orderby == "-id")
                    query = query.OrderByDescending(q => q.Id);
                else if (orderby == "number")
                    query = query.OrderByDescending(q => q.Number);
                else if (orderby == "name")
                    query = query.OrderBy(q => q.Name);
                else if (orderby == "enddate")
                    query = query.OrderByDescending(q => q.DeliveryDateTimeUtc.TruncateSeconds());
            }
            var ords = await query.ToListAsync();
            return View(new IndexOrderGetModel { Orders = ords, OrderBy = orderby });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Client)
                .Include(o => o.Invoice)
                .Include(o => o.Working)
                .Where(o => o.Id == id).FirstOrDefaultAsync();
            if (order == null)
                return RedirectToAction("Index");
            return View(new DetailsOrderGetModel { Order = order });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> New()
        {
            var clients = await _db.Clients.ToListAsync();
            var numb = await _db.Orders.Select(o => o.Number).OrderByDescending(c => c).FirstOrDefaultAsync();
            var invs = await _db.Invoices.ToListAsync();
            return View(new NewOrderGetModel { Clients = clients, Number = numb + 1, Invoices = invs });
        }

        [HttpPost]
        [Authentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(NewOrderPostModel model)
        {
            var clients = await _db.Clients.ToListAsync();
            var numb = await _db.Orders.Select(o => o.Number).OrderByDescending(c => c).FirstOrDefaultAsync();
            var invs = await _db.Invoices.ToListAsync();
            if (ModelState.IsValid)
            {
                var searchNum = await _db.Orders.Where(o => o.Number == model.Number).FirstOrDefaultAsync();
                var searchNm = await _db.Orders.Where(o => o.Name == model.Name).FirstOrDefaultAsync();
                if (searchNum != null || searchNm != null)
                    return View(new NewOrderGetModel { Clients = clients, Number = numb + 1, Error = 2, Invoices = invs });
                var client = clients.Where(c => c.Id == model.Client).FirstOrDefault();
                if (client == null)
                    return View(new NewOrderGetModel { Clients = clients, Number = numb + 1, Error = 1, Invoices = invs });
                Invoice inv = null;
                if(!string.IsNullOrEmpty(model.Invoice))
                    inv = await _db.Invoices.Where(i => i.Code == model.Invoice).FirstOrDefaultAsync();
                var order = new Order
                {
                    Number = model.Number,
                    Name = model.Name,
                    Description = model.Description,
                    Invoice = string.IsNullOrEmpty(model.Invoice) ? null : (inv ?? new Invoice { Code = model.Invoice }),
                    Client = client,
                    DeliveryDateTimeUtc = model.DeliveryDate.ToUniversalTime()
                };
                await _db.Orders.AddAsync(order);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(new NewOrderGetModel { Clients = clients, Number = numb + 1, Error = 1, Invoices = invs });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Edit(int id)
        {
            var clients = await _db.Clients.ToListAsync();
            var invs = await _db.Invoices.ToListAsync();
            var order = await _db.Orders
                .Include(o => o.Client)
                .Include(o => o.Invoice)
                .Where(o => o.Id == id)
                .FirstOrDefaultAsync();
            if (order == null)
                return RedirectToAction("Index");
            return View(new EditOrderGetModel { Clients = clients, Invoices = invs, Order = order });
        }

        [HttpPost]
        [Authentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditOrderPostModel model)
        {
            var clients = await _db.Clients.ToListAsync();
            var invs = await _db.Invoices.ToListAsync();
            var order = await _db.Orders
                .Include(o => o.Client)
                .Include(o => o.Invoice)
                .Where(o => o.Id == id)
                .FirstOrDefaultAsync();
            if (order == null)
                return RedirectToAction("Index");
            if (ModelState.IsValid)
            {
                var searchNum = await _db.Orders.Where(o => o.Number == model.Number).FirstOrDefaultAsync();
                var searchNm = await _db.Orders.Where(o => o.Name == model.Name).FirstOrDefaultAsync();
                if (searchNum != null || searchNm != null)
                    return View(new EditOrderGetModel { Clients = clients, Invoices = invs, Order = order, Error = 2 });
                var client = clients.Where(c => c.Id == model.Client).FirstOrDefault();
                if (client == null)
                    return View(new EditOrderGetModel { Clients = clients, Invoices = invs, Order = order, Error = 1 });
                var inv = await _db.Invoices.Where(i => i.Code == model.Invoice).FirstOrDefaultAsync();
                order.Number = model.Number;
                order.Name = model.Name;
                order.Description = model.Description;
                order.Invoice = inv ?? new Invoice { Code = model.Invoice };
                order.Client = client;
                order.DeliveryDateTimeUtc = model.DeliveryDate.ToUniversalTime();
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(new EditOrderGetModel { Clients = clients, Invoices = invs, Order = order, Error = 1 });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Delete(int id)
        {
            var ords = await _db.Orders
               .Include(o => o.Client)
               .Include(o => o.Invoice)
               .Include(o => o.Working)
               .Where(o => o.Id > 0)
               .ToListAsync();
            var ord = await _db.Orders.Include(o => o.Working).Where(cc => cc.Id == id).FirstOrDefaultAsync();
            if (ord == null)
                return View("Index", new IndexOrderGetModel { Orders = ords, Error = 1 });
            if (ord.Working != null)
                return View("Index", new IndexOrderGetModel { Orders = ords, Error = 2 });
            _db.Orders.Remove(ord);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
