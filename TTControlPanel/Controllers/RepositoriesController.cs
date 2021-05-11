using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTControlPanel.Models.ViewModel;
using TTControlPanel.Services;

namespace TTControlPanel.Controllers
{
    public class RepositoriesController : Controller
    {
        private readonly DBContext _db;

        public RepositoriesController(DBContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var repos = await _db.Repositories.ToListAsync();
            return View(new RepositoriesModel { Repositories = repos });
        }

    }
}
