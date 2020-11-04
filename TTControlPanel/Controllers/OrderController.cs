﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTControlPanel.Filters;
using TTControlPanel.Models.ViewModel;
using TTControlPanel.Services;

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
            var ords = await _db.Orders
                .Include(o => o.Client)
                .Include(o => o.Invoice)
                .ToListAsync();
            return View(new IndexOrderGetModel { Orders = ords });
        }

    }
}
