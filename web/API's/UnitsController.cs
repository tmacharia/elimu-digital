using AutoMapper;
using Common.ViewModels;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Paginator;
using Paginator.Models;
using Services.Interfaces;

namespace web.API_s
{
    [Route("api/units")]
    public class UnitsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryFactory _repos;
        private readonly INotificationManager _notify;

        public UnitsController(IMapper mapper, IRepositoryFactory factory, INotificationManager notificationManager)
        {
            _mapper = mapper;
            _repos = factory;
            _notify = notificationManager;
        }


        // Index
        [HttpGet]
        public IActionResult Index(int page = 1, int itemsperpage = 10)
        {
            Result<Unit> rest = _repos.Units
                                      .ListWith("Lecturer", "Students", "Course")
                                      .ToPaged(page, itemsperpage);

            return View(rest);
        }

        // Create
        [HttpPost]
        [Authorize(Roles = "Admin, Lecturer")]
        public IActionResult Create(int courseId, UnitViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Unit unit = _mapper.Map<Unit>(model);
            unit.Code = Guid.NewGuid();

            // get course
            var course = _repos.Courses.Get(courseId);

            if (course == null)
            {
                return NotFound("No record of that course exists.");
            }

            unit.Course = course;
            unit = _repos.Units.Create(unit);
            _repos.Commit();

            _notify.OnNewUnit(unit);

            return Ok("Created!");
        }

        // Get
        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(int id)
        {
            if (id < 1)
            {
                ModelState.AddModelError("Invalid unit Id", "Provide a valid 'Id' value greater than 0 to get unit.");
                return BadRequest(ModelState);
            }

            Unit unit = _repos.Units
                              .GetWith(id,
                              "Lecturer",
                              "Course");

            if (unit != null)
            {
                return Ok(unit);
            }
            else
            {
                return NotFound();
            }
        }


        // Get unit likes
        [HttpGet]
        [Route("{id}/likes")]
        public IActionResult GetLikes(int id)
        {
            if (id < 1)
            {
                ModelState.AddModelError("Invalid unit Id", "Provide a valid 'Id' value greater than 0 to get unit.");
                return BadRequest(ModelState);
            }

            Unit unit = _repos.Units
                              .GetWith(id, "Likes");

            if (unit != null)
            {
                return Ok(unit.Likes);
            }
            else
            {
                return NotFound();
            }
        }


        // Get unit students
        [HttpGet]
        [Route("{id}/students")]
        public IActionResult GetStudents(int id)
        {
            if (id < 1)
            {
                ModelState.AddModelError("Invalid unit Id", "Provide a valid 'Id' value greater than 0 to get unit.");
                return BadRequest(ModelState);
            }

            IList<Student> students = _repos.Units
                                             .GetWith(id, "UnitStudents")
                                             ?.UnitStudents
                                             .Select(x => x.Student)
                                             .ToList();

            if (students != null)
            {
                return Ok(students);
            }
            else
            {
                return NotFound();
            }
        }

        // Get unit contents
        [HttpGet]
        [Route("{id}/contents")]
        public IActionResult GetContents(int id)
        {
            if (id < 1)
            {
                ModelState.AddModelError("Invalid unit Id", "Provide a valid 'Id' value greater than 0 to get unit.");
                return BadRequest(ModelState);
            }

            Unit unit = _repos.Units
                              .GetWith(id, "Contents");

            if (unit != null)
            {
                return Ok(unit.Contents);
            }
            else
            {
                return NotFound();
            }
        }


        // Get unit exams
        [HttpGet]
        [Route("{id}/exams")]
        public IActionResult GetExams(int id)
        {
            if (id < 1)
            {
                ModelState.AddModelError("Invalid unit Id", "Provide a valid 'Id' value greater than 0 to get unit.");
                return BadRequest(ModelState);
            }

            Unit unit = _repos.Units
                              .GetWith(id, "Exams");

            if (unit != null)
            {
                return Ok(unit.Exams);
            }
            else
            {
                return NotFound();
            }
        }


        // Edit
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "Admin, Lecturer")]
        public IActionResult Edit(int id, UnitViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Unit unit = _repos.Units.Get(id);

            if (unit == null)
            {
                return NotFound("Unit with that id can't be edited since it does not exist.");
            }
            else
            {
                var reflectResult = unit.UpdateReflector(model);

                if (reflectResult.TotalUpdates < 1)
                {
                    return NoContent();
                }
                else
                {
                    unit = _repos.Units.Update(reflectResult.Value);

                    return Ok(unit);
                }
            }
        }


        // Search
        [HttpGet]
        [Route("search")]
        public IActionResult Search(string query)
        {
            string pattern = "(" + query + ")";

            IList<Unit> units = _repos.Units
                                       .ListWith("Course", "Lecturer")
                                       .Where(SearchFuncs.Unit(query))
                                       .ToList();

            return Ok(units);
        }


        // Delete
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (id < 1)
            {
                ModelState.AddModelError("Invalid unit Id", "Provide a valid 'Id' value greater than 0 to delete unit.");
                return BadRequest(ModelState);
            }

            Unit unit = _repos.Units
                              .GetWith(id, "Course", "Lecturer");

            if (unit != null)
            {
                _repos.Units.Remove(unit);

                return Ok("Done!");
            }
            else
            {
                return NotFound();
            }
        }


        // register for a unit
        [HttpPost]
        [Route("{unitId}/register/{studentId}")]
        [Authorize(Roles = "Student")]
        public IActionResult Enroll(int unitId, int studentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var unit = _repos.Units.Get(unitId);

            if (unit == null)
            {
                return NotFound("unit not found.");
            }

            var student = _repos.Students.Get(studentId);

            if (student == null)
            {
                return NotFound("No record of that student exists.");
            }

            StudentUnit studentUnit = new StudentUnit()
            {
                Student = student,
                StudentId = student.Id,
                Unit = unit,
                UnitId = unit.Id
            };

            student.StudentUnits.Add(studentUnit);
            student = _repos.Students.Update(student);
            _repos.Commit();

            return Ok("Unit registration successful!");
        }

        [HttpPost]
        [Route("{unitId}/assignLecturer/{lecId}")]
        public IActionResult AssignLec(int unitId, int lecId)
        {
            if (unitId < 1 || lecId < 1)
            {
                return BadRequest("Invalid unit or lecturer Id.");
            }

            var unit = _repos.Units.GetWith(unitId, "Lecturer");

            if (unit == null)
            {
                return NotFound("Unit does not exist.");
            }

            var lec = _repos.Lecturers.Get(lecId);

            if (lec == null)
            {
                return NotFound("Lecturer does not exist.");
            }

            unit.Lecturer = lec;

            unit = _repos.Units.Update(unit);
            _repos.Commit();

            return Ok("Assigned successfully!");
        }
    }
}
