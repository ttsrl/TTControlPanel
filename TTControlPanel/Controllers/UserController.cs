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
    public class UserController : Controller
    {
        private readonly DBContext _db;
        private readonly Cryptography _c;

        public UserController(DBContext db, Cryptography c)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _c = c ?? throw new ArgumentNullException(nameof(c));
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Index()
        {
            var users = await _db.Users.ToListAsync();
            return View(new IndexUserModel() { Users = users });
        }

        [HttpGet]
        [Authentication]
        public IActionResult New()
        {
            return View(new NewUserGetModel() { Error = 0 });
        }

        [HttpPost]
        [Authentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New([FromServices] Utils utils, NewUserPostModel model)
        {
            if (ModelState.IsValid)
            {
                var username = utils.GetUsername(model.Name, model.Surname);
                var usr = await _db.Users.Where(u => u.Email == model.Email.ToLower() || u.Username == username).FirstOrDefaultAsync();
                if(usr != null)
                    return View(new NewUserGetModel() { Error = 2 });
                if(model.Password != model.ConfPassword)
                    return View(new NewUserGetModel() { Error = 3 });
                var newuser = new User
                {
                    Username = username,
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    Password = await _c.Argon2HashAsync(model.Password),
                    Visible = true,
                    Role = await _db.Roles.Where(r => r.Id == 1).FirstOrDefaultAsync()
                };
                await _db.Users.AddAsync(newuser);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(new NewUserGetModel() { Error = 1 });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Details(int id)
        {
            return View();
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Edit(int id)
        {
            return View();
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Ban(int id)
        {
            var user = await _db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (user != null)
            {
                user.Visible = false;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Restore(int id)
        {
            var user = await _db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (user != null)
            {
                user.Visible = true;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (user != null)
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}