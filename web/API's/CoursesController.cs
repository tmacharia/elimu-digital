using AutoMapper;
using Common.Models;
using Common.ViewModels;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.API_s
{
    [Route("api/courses")]
    public class CoursesController : Controller
    {
        private readonly IUploader _uploader;
        private readonly IMapper _mapper;
        private readonly IRepositoryFactory _repos;

        public CoursesController(IUploader uploader,IMapper mapper, IRepositoryFactory factory)
        {
            _uploader = uploader;
            _mapper = mapper;
            _repos = factory;
        }

        // Index
        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Course> courses = _repos.Courses
                                          .ListWith();
                                          //.ToList();

            var entities = _mapper.Map<List<CourseViewModel>>(courses);

            return Ok(entities);
        }
        
        // Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            Course course = _mapper.Map<Course>(model);
            course.Code = Guid.NewGuid();

            // upload backdrop image
            if (Request.Form.Files.Count > 0)
            {
                var uploadedFile = Request.Form.Files[0];

                if(uploadedFile.Length > 0)
                {
                    IFile file = new FormFile(Request.Form.Files[0]);
                    course.BackdropUrl = await _uploader.Upload(file);
                }else
                {
                    course.BackdropUrl = "https://devtimmystorage.blob.core.windows.net/images/education_button2.jpg";
                }
            }

            // get school
            var school = _repos.Schools.Get(1);

            if(school == null)
            {
                return NotFound("Adding course to a non-existant school. School with that Id not found.");
            }

            course.School = school;
            course = _repos.Courses.Create(course);
            _repos.Commit();

            return RedirectPermanent(Request.Headers["Referer"].ToString());
        }

        // Get
        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(int id)
        {
            if (id < 1)
            {
                ModelState.AddModelError("Invalid course Id", "Provide a valid 'Id' value greater than 0 to get course.");
                return BadRequest(ModelState);
            }

            Course course = _repos.Courses
                                  .GetWith(id, "Units");

            if (course != null)
            {
                return Ok(course);
            }
            else
            {
                return NotFound();
            }
        }

        // Get course units
        [HttpGet]
        [Route("{id}/units")]
        public IActionResult GetUnits(int id)
        {
            if (id < 1)
            {
                ModelState.AddModelError("Invalid course Id", "Provide a valid 'Id' value greater than 0 to get course.");
                return BadRequest(ModelState);
            }

            Course course = _repos.Courses
                                  .GetWith(id, "Units");

            if (course != null)
            {
                return Ok(course.Units);
            }
            else
            {
                return NotFound();
            }
        }

        // Get course students
        [HttpGet]
        [Route("{id}/students")]
        public IActionResult GetStudents(int id)
        {
            if (id < 1)
            {
                ModelState.AddModelError("Invalid course Id", "Provide a valid 'Id' value greater than 0 to get course.");
                return BadRequest(ModelState);
            }

            Course course = _repos.Courses
                                  .GetWith(id, "Students");

            if (course != null)
            {
                return Ok(course.Students);
            }
            else
            {
                return NotFound();
            }
        }

        // Edit
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, CourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Course course = _repos.Courses.Get(id);

            if (course == null)
            {
                return NotFound("Course with that id can't be edited since it does not exist.");
            }
            else
            {
                var reflectResult = course.UpdateReflector(model);

                if (reflectResult.TotalUpdates < 1)
                {
                    return NoContent();
                }
                else
                {
                    course = _repos.Courses.Update(reflectResult.Value);

                    return Ok(course);
                }
            }
        }


        // Search
        [HttpGet]
        [Route("search")]
        public IActionResult Search(string query)
        {
            string pattern = "(" + query + ")";

            IList<Course> courses = _repos.Courses
                                          .ListWith("School", "Units")
                                          .Where(SearchFuncs.Course(query))
                                          .ToList();

            return Ok(courses);
        }


        // Delete
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (id < 1)
            {
                return BadRequest("Invalid course Id. Provide a valid 'Id' value greater than 0 to delete course.");
            }

            Course course = _repos.Courses
                                  .GetWith(id, "School", "Units");

            if (course != null)
            {
                _repos.Courses.Remove(course);
                _repos.Commit();

                return Ok("Done!");
            }
            else
            {
                return NotFound("Course with that Id does not exist!");
            }
        }


        // Enroll
        [HttpPost]
        [Route("{courseId}/enroll/{studentId}")]
        [Authorize(Roles = "Student")]
        public IActionResult Enroll(int courseId, int studentId)
        {
            if (courseId < 1 || studentId < 1)
            {
                return BadRequest("Course Id or Student Id during student enrollment to a course cannot be null.");
            }

            var course = _repos.Courses.Get(courseId);

            if(course == null)
            {
                return NotFound("Course not found.");
            }

            var student = _repos.Students.Get(studentId);

            if(student == null)
            {
                return NotFound("No record of that student exists.");
            }

            student.Course = course;
            student = _repos.Students.Update(student);
            _repos.Commit();

            return Ok("Course enrollment successful!");
        }
    }
}
