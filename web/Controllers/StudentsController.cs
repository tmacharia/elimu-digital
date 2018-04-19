using AutoMapper;
using DAL.Extensions;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace web.Controllers
{
    [Authorize]
    [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any)]
    public class StudentsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IDataManager _dataManager;
        private readonly IRepositoryFactory _repos;
        private readonly IMapper _mapper;
        private readonly Stopwatch _watch;

        public StudentsController(UserManager<AppUser> userManager,IDataManager dataManager,IRepositoryFactory factory, IMapper mapper)
        {
            _userManager = userManager;
            _dataManager = dataManager;
            _repos = factory;
            _mapper = mapper;
            _watch = new Stopwatch();
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Student> students = new List<Student>();

            if(User.Role() == "Student")
            {
                students = _dataManager.MyClassMates(this.GetAccountId());
            }
            else if(User.Role() == "Lecturer")
            {
                students = _dataManager.MyStudents(this.GetAccountId(), 50);
            }
            else
            {
                students = _repos.Students
                                 .ListWith("Profile", "Course", "StudentUnits");
            }

            var model = students.OrderByDescending(x => x.Timestamp)
                                .ToList();

            ViewBag.Notifications = this.GetNotifications();

            return View(model);
        }

        [HttpGet]
        [Route("students/{id}/{names}")]
        public IActionResult Details(int id, string names)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Search(string q)
        {
            IEnumerable<Student> students = new List<Student>();
            ViewBag.Query = q;
            _watch.Start();

            if (User.Role() == "Student")
            {
                students = _dataManager.MyClassMates(this.GetAccountId());
            }
            else if (User.Role() == "Lecturer")
            {
                students = _dataManager.MyStudents(this.GetAccountId(), 50);
            }
            else
            {
                students = _repos.Students
                                 .ListWith("Profile", "Course", "StudentUnits");
            }

            students = students.Where(Predicates.Student(q));
            var model = students.OrderByDescending(x => x.Timestamp)
                                .ToList();
            _watch.Stop();
            ViewBag.timespan = _watch.Elapsed;
            _watch.Reset();

            ViewBag.Notifications = this.GetNotifications();

            return View(model);
        }
    }
}
