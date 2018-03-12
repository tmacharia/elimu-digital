using DAL.Extensions;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using web.Extensions;

namespace web.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepositoryFactory _repos;

        public CoursesController(UserManager<AppUser> userManager,IRepositoryFactory factory)
        {
            _userManager = userManager;
            _repos = factory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Action = "Courses";

            IList<Course> courses = new List<Course>();
            AppUser user = await _userManager.GetUserAsync(User);

            if(User.Role() == "Administrator")
            {
                courses = _repos.Courses
                                .ListWith("Units", "Students", "Likes")
                                .ToArray();
            }
            else if(User.Role() == "Lecturer")
            {
                courses = _repos.Lecturers
                                .GetWith(user.AccountId,
                                        "Units",
                                        "Units.Course",
                                        "Units.Course.Units",
                                        "Units.Course.Students",
                                        "Units.Course.Likes")
                                .Units
                                .Select(x => x.Course)
                                .ToArray();
            }
            else if(User.Role() == "Student")
            {
                var course = _repos.Students
                                   .GetWith(user.AccountId, 
                                            "Course",
                                            "Course.Units",
                                            "Course.Students",
                                            "Course.Likes")
                                   .Course;

                if(course != null)
                {
                    courses.Add(course);
                }
            }

            var model = courses.Distinct()
                               .OrderByDescending(x => x.Timestamp)
                               .ToList();

            return View(model);
        }

        [HttpGet]
        [Route("courses/enrollment")]
        [Authorize(Roles = "Student, Administrator")]
        public IActionResult Enrollment()
        {
            IEnumerable<Course> courses = new List<Course>();

            courses = _repos.Courses
                            .ListWith("Units", "Students", "Likes");

            return View(courses);
        }

        [HttpGet]
        [Route("courses/enrollment/{id}/{name}")]
        [Authorize(Roles = "Student, Administrator")]
        public IActionResult EnrollmentDetails(int id,string name)
        {
            if(id < 1)
            {
                ViewBag.error = "Invalid course id chosen.";
                return View();
            }

            Course course = _repos.Courses
                                  .GetWith(id, "Units", "Likes");

            if(course == null)
            {
                ViewBag.error = "Details for course chosen were not found in records.";
                return View();
            }

            return View(course);
        }

        [HttpGet]
        [Route("courses/enrollment/{id}/accept")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Enroll(int id)
        {
            AppUser user = await _userManager.GetUserAsync(User);
            Course course = _repos.Courses
                                  .GetWith(id,"Units");

            Student student = _repos.Students
                                    .GetWith(user.AccountId, "Course");

            student.Course = course;
            student = _repos.Students.Update(student);

            foreach (var item in course.Units)
            {
                StudentUnit studentUnit = new StudentUnit()
                {
                    Student = student,
                    StudentId = student.Id,
                    Unit = item,
                    UnitId = item.Id
                };

                _repos.StudentUnits.Create(studentUnit);
            }

            _repos.Commit();

            return RedirectToActionPermanent("EnrollmentSuccess", course);
        }

        [HttpGet]
        [Route("courses/enrollmentsuccess")]
        public IActionResult EnrollmentSuccess(Course course)
        {
            return View(course);
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
                                           "Students.Profile", 
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
