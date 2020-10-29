using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TTControlPanel.Controllers
{
    public class OrdersController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
