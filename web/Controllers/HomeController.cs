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
using System.Net;
using Newtonsoft.Json;

namespace web.Controllers
{
    [Authorize]
    [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any)]
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

        public IActionResult Index()
        {
            try
            {
                ViewBag.Action = "Dashboard";

                int account = this.GetAccountId();

                if (account < 1)
                    ViewBag.IsNew = true;
                else
                    ViewBag.IsNew = false;

                if (User.Role() == "Administrator")
                {
                    ViewBag.summary = _dashManager.GetSummary();
                    ViewBag.lecturers = _repos.Lecturers.ListWith("Profile").Take(10).ToList();
                    ViewBag.students = _repos.Students.ListWith("Profile").Take(10).ToList();
                }
                else if (User.Role() == "Lecturer" && account > 0)
                {
                    ViewBag.students = _dashManager.MyStudents(account, 10);
                    ViewBag.units = _dashManager.MyUnits<Lecturer>(account, 10).ToList();
                    ViewBag.classes = _dashManager.MyClasses<Lecturer>(account, 10).ToList();
                    ViewBag.lecturers = _dashManager.MyColleagues(account).Take(10).ToList();
                }
                else if (User.Role() == "Student" && account > 0)
                {
                    ViewBag.myprofile = _repos.Students
                                              .GetWith(account, "Course");
                    ViewBag.mycourses = _dashManager.MyCourses(account);
                    ViewBag.lecturers = _dashManager.MyLecturers(account, 10).ToList();
                    ViewBag.units = _dashManager.MyUnits<Student>(account, 10).ToList();
                    ViewBag.classes = _dashManager.MyClasses<Student>(account, 10).ToList();
                }

                ViewBag.Notifications = this.GetNotifications();
            }
            catch (Exception ex)
            {
                return this.Error(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(ex));
            }

            return View();
        }

        [Route("about-us")]
        [AllowAnonymous]
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
