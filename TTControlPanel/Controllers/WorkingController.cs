using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTControlPanel.Filters;
using TTControlPanel.Models.ViewModel;
using TTControlPanel.Services;
using TTControlPanel.Models;
using Microsoft.AspNetCore.Http;

namespace TTControlPanel.Controllers
{
    public class WorkingController : Controller
    {
        private readonly Utils _utils;
        private readonly DBContext _db;

        public WorkingController(DBContext db, Utils utils)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _utils = utils ?? throw new ArgumentNullException(nameof(utils));
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Index(string orderby)
        {
            var query = _db.Workings
                .Include(w => w.FinalClient)
                .Where(w => w.Id > 0);
            var order = "-id";
            if (!string.IsNullOrEmpty(orderby))
                order = orderby;
            else
            {
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("WorkingOrderBy")))
                    order = HttpContext.Session.GetString("WorkingOrderBy");
            }

            if (string.IsNullOrEmpty(order))
            {
                if (order == "id")
                    query = query.OrderBy(i => i.Id);
                else if (order == "enddate")
                    query = query.OrderBy(i => i.EndDateTimeUtc);
                else if (order == "-enddate")
                    query = query.OrderByDescending(i => i.EndDateTimeUtc);
                HttpContext.Session.SetString("WorkingOrderBy", orderby);
            }
            else
                query = query.OrderByDescending(i => i.Id);
            var works = await query.ToListAsync();
            var ordes = await _db.Orders.Include(o => o.Working).ToListAsync();
            return View( new IndexWorkingGetModel { Workings = works, OrderBy = order, Orders = ordes });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> New(int order = 0)
        {
            var ords = await _db.Orders
                .Include(o => o.Working)
                .Where(o => o.Working == null)
                .ToListAsync();
            var clients = await _db.Clients.ToListAsync();
            Order sel = null;
            if (order != 0)
                sel = await _db.Orders.Include(o => o.Client).Where(o => o.Id == order).FirstOrDefaultAsync();
            return View(new NewWorkingGetModel { Orders = ords, Clients = clients, Selected = sel });
        }

        [HttpPost]
        [Authentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(NewWorkingPostModel model)
        {
            var ords = await _db.Orders
                .Include(o => o.Working)
                .Where(o => o.Working == null)
                .ToListAsync();
            var clients = await _db.Clients.ToListAsync();
            if (ModelState.IsValid)
            {
                var client = await _db.Clients.Where(c => c.Id == model.Client).FirstOrDefaultAsync();
                var order = await _db.Orders.Include(o => o.Working).Where(o => o.Id == model.Order).FirstOrDefaultAsync();
                if(client == null || order == null)
                    return View(new NewWorkingGetModel { Orders = ords, Clients = clients, Error = 1 });
                if(order.Working != null)
                    return View(new NewWorkingGetModel { Orders = ords, Clients = clients, Error = 2 });
                order.Working = new Working
                {
                    Code = await _utils.GenerateWorkingCode(),
                    FinalClient = client
                };
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(new NewWorkingGetModel { Orders = ords, Clients = clients, Error = 1 });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Delete()
        {
            return View();
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Details()
        {
            return View();
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Start()
        {
            return View();
        }

    }
}
