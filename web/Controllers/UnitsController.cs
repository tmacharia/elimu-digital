using AutoMapper;
using DAL.Extensions;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Paginator;
using Paginator.Models;
using Services;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace web.Controllers
{
    [Authorize]
    [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any)]
    public class UnitsController : Controller
    {
        private readonly INotificationManager _notify;
        private readonly UserManager<AppUser> _userManager;
        private readonly IDataManager _dataManager;
        private readonly IRepositoryFactory _repos;
        private readonly IMapper _mapper;
        private readonly Stopwatch _watch;

        public UnitsController(INotificationManager notificationManager,UserManager<AppUser> userManager,IDataManager dataManager, IRepositoryFactory factory, IMapper mapper)
        {
            _notify = notificationManager;
            _userManager = userManager;
            _dataManager = dataManager;
            _repos = factory;
            _mapper = mapper;
            _watch = new Stopwatch();
        }

        [HttpGet]
        public IActionResult Index(int page = 1, int itemsperpage = 10)
        {
            int account = this.GetAccountId();
            IEnumerable<Unit> units = new List<Unit>();

            if(User.Role() == "Administrator")
            {
                units = _repos.Units
                              .ListWith("Lecturer", "Lecturer.Profile", "UnitStudents", "Course", "Likes");
            }
            else if(User.Role() == "Lecturer")
            {
                units = _dataManager.MyUnits<Lecturer>(account);
            }
            else if(User.Role() == "Student")
            {
                units = _dataManager.MyUnits<Student>(account);
            }

            Result<Unit> result = new Result<Unit>();

            result = units.OrderByDescending(x => x.Timestamp)
                          .ToPaged(page, itemsperpage);

            ViewBag.Notifications = this.GetNotifications();

            return View(result);
        }

        [HttpGet]
        [Route("units/{id}/{name}")]
        public IActionResult Details(int id, string name)
        {
            ViewBag.Action = "Units";
            string error = string.Empty;

            if (id < 1)
            {
                error = "Invalid unit Id. Provide a valid 'Id' value greater than 0.";
            }

            Unit unit = _repos.Units
                              .GetWith(id, 
                              "Lecturer", 
                              "Lecturer.Units", 
                              "Lecturer.Profile", 
                              "Exams", 
                              "Likes", 
                              "UnitStudents", 
                              "Course",
                              "Class", 
                              "Contents",
                              "Contents.Likes");

            if (unit == null)
            {
                error = "No record of units with that id exist.";
            }

            ViewBag.boards = this.GetMyBoards();
            ViewBag.Notifications = this.GetNotifications();
            ViewBag.error = error;

            return View("~/Views/Units/Details.cshtml", unit);
        }

        [HttpGet]
        public IActionResult Search(string q, int page = 1, int itemsperpage = 10)
        {
            ViewBag.Action = "Units";

            IEnumerable<Unit> units = new List<Unit>();
            int account = this.GetAccountId();

            _watch.Start();
            if(User.Role() == "Student")
            {
                units = _dataManager.MyUnits<Student>(account);
            }
            else if(User.Role() == "Lecturer")
            {
                units = _dataManager.MyUnits<Lecturer>(account);
            }
            else
            {
                units = _repos.Units
                              .ListWith("Course", "Lecturer", "UnitStudents", "Likes");
            }

            ViewBag.Query = q;
            ViewBag.Notifications = this.GetNotifications();

            string pattern = "(" + q + ")";

            Result<Unit> rest = units.Where(x => x.Name != null && Regex.IsMatch(x.Name, pattern, RegexOptions.IgnoreCase))
                                      .ToPaged(page, itemsperpage);
            _watch.Stop();
            ViewBag.timespan = _watch.Elapsed;
            _watch.Reset();

            return View(rest);
        }

        [HttpGet]
        [Route("units/{id}/{name}/exams")]
        public IActionResult UnitExams(int id,string name)
        {
            return Redirect($"/exams/for-unit/{id}/{name}");
        }
    }
}
