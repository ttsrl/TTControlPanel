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
    public class UserController : Controller
    {
        private readonly DBContext _db;
        private readonly Cryptography _c;
        private readonly Utils _utils;

        public UserController(DBContext db, Cryptography c, Utils u)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _c = c ?? throw new ArgumentNullException(nameof(c));
            _utils = u ?? throw new ArgumentNullException(nameof(u));
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
        public async Task<IActionResult> New(NewUserPostModel model)
        {
            if (ModelState.IsValid)
            {
                var username = _utils.GetUsername(model.Name, model.Surname);
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
                    Ban = false,
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
            var user = await _db.Users.Include(u => u.Role).Where(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
                return RedirectToAction("Index");
            var apps = await _db.ApplicationsVersions
                .Include(l =>l.AddedUser)
                .Where(l => l.AddedUser == user)
                .ToListAsync();
            var prods = await _db.ProductKeys
                .Include(p => p.GenerateUser)
                .Where(l => l.GenerateUser == user)
                .ToListAsync();
            var hids = await _db.Hids
                .Include(h => h.AddedUser)
                .Where(h => h.AddedUser == user)
                .ToListAsync();
            return View(new DetailsUserGetModel { User = user, ApplicationVersions = apps, ProductKeys = prods, Hids = hids });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Edit(int id)
        {
            var usr = await _db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (usr == null)
                return RedirectToAction("Index");
            return View(new EditUserGetModel { User = usr });
        }

        [HttpPost]
        [Authentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditUserPostModel model)
        {
            var usr = await _db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (usr == null)
                return RedirectToAction("Index");
            if (ModelState.IsValid)
            {
                var username = _utils.GetUsername(model.Name, model.Surname);
                var cusr = await _db.Users.Where(u => u.Email == model.Email.ToLower() || u.Username == username).FirstOrDefaultAsync();
                if (cusr != null)
                    return View(new EditUserGetModel() { User = usr, Error = 2 });
                if(!string.IsNullOrEmpty(model.Password) && !string.IsNullOrEmpty(model.ConfPassword))
                {
                    if (model.Password != model.ConfPassword)
                        return View(new EditUserGetModel() { User = usr, Error = 3 });
                    else
                        usr.Password = await _c.Argon2HashAsync(model.Password);
                }
                usr.Username = username;
                usr.Name = model.Name;
                usr.Surname = model.Surname;
                usr.Email = model.Email;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(new EditUserGetModel { User = usr, Error = 1 });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Ban(int id)
        {
            var user = await _db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (user != null)
            {
                user.Ban = true;
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
                user.Ban = false;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

    }
}