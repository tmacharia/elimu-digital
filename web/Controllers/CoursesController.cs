using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using web.Extensions;

namespace web.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly IRepositoryFactory _repos;

        public CoursesController(IRepositoryFactory factory)
        {
            _repos = factory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Action = "Courses";

            var courses = _repos.Courses
                                .ListWith("Units","Students","Likes")
                                .OrderByDescending(x => x.Timestamp)
                                .ToList();

            return View(courses);
        }

        [HttpGet]
        [Route("courses/{id}/{name}")]
        public IActionResult Details(int id, string name)
        {
            ViewBag.Action = "Courses";
            string error = string.Empty;

            if (id < 1)
            {
                error = "Invalid course Id. Provide a valid 'Id' value greater than 0.";
            }

            Course course = _repos.Courses
                                  .GetWith(id, 
                                           "Units",
                                           "Units.Lecturer",
                                           "Units.Lecturer.Profile",
                                           "Students", 
                                           "Likes");

            if(course == null)
            {
                error = "No record of courses with that id exist.";
            }

            ViewBag.error = error;

            return View("~/Views/Courses/Details.cshtml", course);
        }

        [HttpGet]
        public IActionResult Search(string q)
        {
            ViewBag.Action = "Courses";
            ViewBag.Query = q;

            string pattern = "(" + q + ")";

            IList<Course> courses = _repos.Courses
                                          .ListWith("Units")
                                          .Where(SearchFuncs.Course(q))
                                          .ToList();

            return View(courses);
        }

        [HttpPost]
        public IActionResult Create(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("Specify course name.");
                }

                Course course = new Course()
                {
                    Name = name,
                };

                _repos.Courses.Create(course);
                _repos.Commit();

                return Ok("Course created successfully.");
            }
            catch (Exception ex)
            {
                return this.Error(ex.Message);
            }
        }
    }
}
