using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTControlPanel.Models;
using TTControlPanel.Services;

namespace TTControlPanel.Controllers
{
    public class LoginController : Controller
    {
        private readonly Cryptography _c;
        private readonly DBContext _db;

        public LoginController(DBContext db, Cryptography c)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _c = c ?? throw new ArgumentNullException(nameof(c));
        }

        [HttpGet]
        public IActionResult Index()
        {
            var user = (User)HttpContext.Items["User"];
            if (user is User)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string username, string password)
        {
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return View("Index", new LoginModel { Error = LoginError.UsernameEmail });

            password = await _c.Argon2HashAsync(password ?? "");
            var user = await _db.Users.FirstOrDefaultAsync(u => (u.Username.ToLower() == username.ToLower() || u.Email.ToLower() == username.ToLower()) && u.Password == password);
            if (user != null)
            {
                if(user.Ban)
                    return View("Index", new LoginModel { Error = LoginError.Banned });
                await _db.Entry(user).Reference(u => u.Role).LoadAsync();
                if (user.Role.GrantLogin && user.Role.GrantUseCPanel)
                {
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    return RedirectToAction("Index", "Home");
                }
                return View("Index", new LoginModel { Error = LoginError.Banned });
            }
            return View("Index", new LoginModel { Error = LoginError.UsernameEmail });
        }
    }
}