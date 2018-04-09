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
    [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any)]
    public class CoursesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepositoryFactory _repos;
        private readonly IDataManager _dataManager;

        public CoursesController(UserManager<AppUser> userManager,
                                 IRepositoryFactory factory,
                                 IDataManager dataManager)
        {
            _userManager = userManager;
            _repos = factory;
            _dataManager = dataManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Action = "Courses";

            List<Course> courses = new List<Course>();
            AppUser user = await _userManager.GetUserAsync(User);

            if(User.Role() == "Administrator")
            {
                courses = _repos.Courses
                                .ListWith("Units", "Students", "Likes")
                                .ToList();
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
                                .ToList();
            }
            else if(User.Role() == "Student")
            {
                var mycourses = _dataManager.MyCourses(user.AccountId);

                if(mycourses.Main != null)
                {
                    courses.Add(mycourses.Main);
                }

                if(mycourses.Others != null)
                {
                    if(mycourses.Others.Count > 0)
                    {
                        courses.AddRange(mycourses.Others);
                    }
                }
            }

            var model = courses.Distinct()
                               .OrderByDescending(x => x.Timestamp)
                               .ToList();


            ViewBag.Notifications = _repos.Notifications.List.Count(x => x.AccountId == user.AccountId && x.Read == false);

            return View(model);
        }

        [HttpGet]
        [Route("courses/enrollment")]
        [Authorize(Roles = "Student, Administrator")]
        public async Task<IActionResult> Enrollment()
        {
            List<Course> courses = new List<Course>();
            AppUser user = await _userManager.GetUserAsync(User);

            var mycourses = _dataManager.MyCourses(user.AccountId);

            courses = _repos.Courses
                            .ListWith("Units", "Students", "Likes")
                            .ToList();
                            
            if(mycourses.Main != null)
            {
                courses.Remove(mycourses.Main);
            }

            if(mycourses.Others != null)
            {
                foreach (var item in mycourses.Others)
                {
                    courses.Remove(item);
                }
            }

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

            if(student.Course == null)
            {
                student.Course = course;
                student = _repos.Students.Update(student);
            }
            else
            {
                StudentCourse studentCourse = new StudentCourse()
                {
                    CourseId = course.Id,
                    StudentId = user.AccountId,
                    Course = course,
                    Student = student
                };

                studentCourse = _repos.StudentCourses.Create(studentCourse);
            }

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

            var courseStudents = _repos.StudentCourses
                                       .ListWith("Student", "Student.Profile")
                                       .Where(x => x.CourseId == id)
                                       .Select(x => x.Student)
                                       .ToList();

            foreach (var item in courseStudents)
            {
                course.Students.Add(item);
            }

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
