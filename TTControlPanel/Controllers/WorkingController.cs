using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TTControlPanel.Filters;
using TTControlPanel.Services;

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
        public IActionResult Index()
        {
            return View();
        }
    }
}
