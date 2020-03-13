using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTControlPanel.Models;
using TTControlPanel.Services;

namespace TTControlPanel.Controllers
{
    public class HomeController : Controller
    {
        private readonly Cryptography _c;
        private readonly DBContext _db;

        public HomeController(DBContext db, Cryptography c)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _c = c ?? throw new ArgumentNullException(nameof(c));
        }

        public IActionResult Index()
        {
            var user = (User)HttpContext.Items["User"];
            if (user is User)
            {
                return View();
            }
            return RedirectToAction("Index", "Login");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int id = 500)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Code = id });
        }
    }
}
