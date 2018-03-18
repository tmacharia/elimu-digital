using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.ViewModels;
using DAL.Contexts;
using DAL.Models;
using DAL.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Microsoft.AspNetCore.Identity;

namespace web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IDataManager _dashManager;
        private readonly IRepositoryFactory _repos;

        public HomeController(UserManager<AppUser> userManager, 
            IDataManager dashboardManager,
            IRepositoryFactory factory)
        {
            _userManager = userManager;
            _dashManager = dashboardManager;
            _repos = factory;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Action = "Dashboard";

            AppUser user = await _userManager.GetUserAsync(User);

            if(user.AccountId < 1 || user.AccountType == AccountType.None)
                ViewBag.IsNew = true;
            else
                ViewBag.IsNew = false;

            if (User.Role() == "Administrator")
            {
                ViewBag.summary = _dashManager.GetSummary();
                ViewBag.lecturers = _repos.Lecturers.ListWith("Profile").Take(10).ToList();
                ViewBag.students = _repos.Students.ListWith("Profile").Take(10).ToList();
            }
            else if(User.Role() == "Lecturer" && user.AccountId > 0)
            {
                ViewBag.students = _dashManager.MyStudents(user.AccountId,10);
                ViewBag.units = _dashManager.MyUnits<Lecturer>(user.AccountId, 10).ToList();
                ViewBag.classes = _dashManager.MyClasses<Lecturer>(user.AccountId, 10).ToList();
                ViewBag.lecturers = _dashManager.MyColleagues(user.AccountId).Take(10).ToList();
            }
            else if(User.Role() == "Student" && user.AccountId > 0)
            {
                ViewBag.myprofile = _repos.Students
                                       .GetWith(user.AccountId, "Course");
                ViewBag.lecturers = _dashManager.MyLecturers(user.AccountId, 10).ToList();
                ViewBag.units = _dashManager.MyUnits<Student>(user.AccountId, 10).ToList();
                ViewBag.classes = _dashManager.MyClasses<Student>(user.AccountId, 10).ToList();
            }

            ViewBag.Notifications = _repos.Notifications.List.Count(x => x.AccountId == user.AccountId);

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
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
