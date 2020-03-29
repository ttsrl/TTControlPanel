using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TTControlPanel.Filters;
using TTControlPanel.Models;
using TTControlPanel.Models.ViewModel;
using TTControlPanel.Services;

namespace TTControlPanel.Controllers
{
    public class HomeController : Controller
    {
        private readonly Cryptography _c;
        private readonly DBContext _db;
        private readonly GitHub _git;

        public HomeController(DBContext db, Cryptography c, GitHub git)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _c = c ?? throw new ArgumentNullException(nameof(c));
            _git = git ?? throw new ArgumentNullException(nameof(git));
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Index()
        {
            //var user = (User)HttpContext.Items["User"];
            //if (user is User)
            //{
            //    return View(new HomeGetModel() { Commits = await _git.GetCommits() });
            //}
            //return RedirectToAction("Index", "Login");
            return View(new HomeGetModel() { Commits = await _git.GetCommits() });
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int id = 500)
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier, Code = id });
        }
    }
}
