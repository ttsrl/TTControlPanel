using Microsoft.AspNetCore.Mvc;
using TTControlPanel.Models;

namespace TTControlPanel.Controllers
{
    public class LogoutController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var user = (User)HttpContext.Items["User"];
            if(user is User)
            {
                HttpContext.Session.Remove("UserId");
            }
            return RedirectToAction("Index", "Login");
        }
    }
}