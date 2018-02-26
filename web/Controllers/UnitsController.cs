using AutoMapper;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IRepositoryFactory _repos;
        private readonly IMapper _mapper;

        public UnitsController(IRepositoryFactory factory, IMapper mapper)
        {
            _repos = factory;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index(int page = 1, int itemsperpage = 10)
        {
            Result<Unit> rest = _repos.Units
                                      .ListWith("Lecturer", "UnitStudents", "Course", "Likes")
                                      .OrderBy(x => x.Name)
                                      .ToPaged(page, itemsperpage);

            return View(rest);
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
                              .GetWith(id, "Lecturer", "Exams", "Likes", "UnitStudents", "Classes", "Contents");

            if (unit == null)
            {
                error = "No record of units with that id exist.";
            }

            ViewBag.error = error;

            return View("~/Views/Units/Details.cshtml", unit);
        }

        [HttpGet]
        public IActionResult Search(string q, int page = 1, int itemsperpage = 10)
        {
            ViewBag.Action = "Units";

            ViewBag.Query = q;

            string pattern = "(" + q + ")";

            Result<Unit> rest = _repos.Units
                                      .ListWith("Lecturer", "UnitStudents", "Course", "Likes")
                                      .Where(x => x.Name != null && Regex.IsMatch(x.Name, pattern, RegexOptions.IgnoreCase))
                                      .ToPaged(page, itemsperpage);

            return View(rest);
        }
    }
}
