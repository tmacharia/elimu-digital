using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Contexts;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _appContext;
        private readonly LePadContext _lepadContext;

        public HomeController(AppDbContext appDbContext, LePadContext lePadContext)
        {
            _appContext = appDbContext;
            _lepadContext = lePadContext;
        }

        public IActionResult Index()
        {
            AppUser user = _appContext.Users.FirstOrDefault(x => x.Email == User.Identity.Name);

            if(user.AccountId < 1 || user.AccountType == AccountType.None)
            {
                ViewBag.IsNew = true;
            }
            else
            {
                ViewBag.IsNew = false;
            }

            ViewBag.Notifications = 8;

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
