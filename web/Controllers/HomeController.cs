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

namespace web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _appContext;
        private readonly IRepositoryFactory _repos;

        public HomeController(AppDbContext appDbContext, IRepositoryFactory factory)
        {
            _appContext = appDbContext;
            _repos = factory;
        }

        public IActionResult Index()
        {
            ViewBag.Action = "Dashboard";

            AppUser user = _appContext.Users.FirstOrDefault(x => x.Email == User.Identity.Name);

            if(user.AccountId < 1 || user.AccountType == AccountType.None)
            {
                ViewBag.IsNew = true;
            }
            else
            {
                ViewBag.IsNew = false;
            }

            if (User.Role() == "Administrator")
            {
                // create summary view model
                var summary = new SummaryViewModel
                {
                    Total_Classes = _repos.Classes.List.Count(),
                    Students_Total = _repos.Students.List.Count(),
                    Lec_Total = _repos.Lecturers.List.Count(),
                    Courses_Total = _repos.Courses.List.Count(),
                    Lec_NoProfile = _repos.Lecturers.ListWith("Profile")
                                                    .Count(x => x.Profile == null),
                    Students_Enrolled = _repos.Students.ListWith("Course")
                                                       .Count(x => x.Course != null),
                    Units_NoClass = _repos.Units.ListWith("Class")
                                                .Count(x => x.Class == null)
                };
                ViewBag.summary = summary;
            }

            ViewBag.lecturers = _repos.Lecturers.ListWith("Profile")
                                                .OrderByDescending(x => x.Timestamp)
                                                .Take(10)
                                                .ToList();

            ViewBag.students = _repos.Students.ListWith("Profile")
                                              .OrderByDescending(x => x.Timestamp)
                                              .Take(10)
                                              .ToList();

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

        public IActionResult Error()
        {
            return View();
        }
    }
}
