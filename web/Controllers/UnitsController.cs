using AutoMapper;
using DAL.Extensions;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Paginator;
using Paginator.Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace web.Controllers
{
    [Authorize]
    public class UnitsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IDataManager _dataManager;
        private readonly IRepositoryFactory _repos;
        private readonly IMapper _mapper;

        public UnitsController(UserManager<AppUser> userManager,IDataManager dataManager, IRepositoryFactory factory, IMapper mapper)
        {
            _userManager = userManager;
            _dataManager = dataManager;
            _repos = factory;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int itemsperpage = 10)
        {
            AppUser user = await _userManager.GetUserAsync(User);
            IEnumerable<Unit> units = new List<Unit>();

            if(User.Role() == "Administrator")
            {
                units = _repos.Units
                              .ListWith("Lecturer", "Lecturer.Profile", "UnitStudents", "Course", "Likes");
            }
            else if(User.Role() == "Lecturer")
            {
                units = _dataManager.MyUnits<Lecturer>(user.AccountId);
            }
            else if(User.Role() == "Student")
            {
                units = _dataManager.MyUnits<Student>(user.AccountId);
            }

            Result<Unit> result = new Result<Unit>();

            result = units.OrderByDescending(x => x.Timestamp)
                          .ToPaged(page, itemsperpage);

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

            ViewBag.error = error;

            return View("~/Views/Units/Details.cshtml", unit);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string q, int page = 1, int itemsperpage = 10)
        {
            ViewBag.Action = "Units";

            IEnumerable<Unit> units = new List<Unit>();
            AppUser user = await _userManager.GetUserAsync(User);

            if(User.Role() == "Student")
            {
                units = _dataManager.MyUnits<Student>(user.AccountId);
            }
            else if(User.Role() == "Lecturer")
            {
                units = _dataManager.MyUnits<Lecturer>(user.AccountId);
            }
            else
            {
                units = _repos.Units
                              .ListWith("Course", "Lecturer", "UnitStudents", "Likes");
            }

            ViewBag.Query = q;

            string pattern = "(" + q + ")";

            Result<Unit> rest = units.Where(x => x.Name != null && Regex.IsMatch(x.Name, pattern, RegexOptions.IgnoreCase))
                                      .ToPaged(page, itemsperpage);

            return View(rest);
        }
    }
}
