using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using TTControlPanel.Models;
using TTControlPanel.Models.ViewModel;
using TTControlPanel.Services;
using User = TTControlPanel.Models.User;

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

        public async Task<IActionResult> Index()
        {
            var user = (User)HttpContext.Items["User"];
            if (user is User)
            {
                var credential = new Credentials("ttsrl", "TTsrl092017");
                var client = new GitHubClient(new ProductHeaderValue("ttsrl")) { Credentials = credential };
                var commits = await client.Repository.Commit.GetAll("ttsrl", "TTControlPanel", new ApiOptions() { PageCount = 1, PageSize = 10 });
                var list = new List<GitCommit>();
                foreach(var c in commits)
                {
                    var tmpc = await client.Repository.Commit.Get("ttsrl", "TTControlPanel", c.Sha);
                    int adds = 0;
                    int dels = 0;
                    foreach (var cf in tmpc.Files)
                    {
                        adds += cf.Additions;
                        dels += cf.Deletions;
                    }
                    var obj = new GitCommit
                    {
                        AuthorUsername = c.Author.Login,
                        AuthorName = c.Commit.Author.Name,
                        Date = c.Commit.Author.Date,
                        Message = c.Commit.Message,
                        Files = tmpc.Files.Count,
                        Additions = adds,
                        Deletions = dels
                    };
                    list.Add(obj);
                }
                return View(new HomeGetModel() { Commits = list });
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
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier, Code = id });
        }
    }
}
