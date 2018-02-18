﻿using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly IRepositoryFactory _repos;

        public CoursesController(IMapper mapper, IRepositoryFactory factory)
        {
            _mapper = mapper;
            _repos = factory;
        }

        // Index
        [HttpGet]
        public IActionResult Index()
        {
            IList<Course> courses = _repos.Courses
                                          .ListWith("Units")
                                          .ToList();

            var entities = _mapper.Map<List<CourseViewModel>>(courses);

            return Ok(entities);
        }
        
        // Create
        [HttpPost]
        [Route("{schoolId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(int schoolId, CourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Course course = _mapper.Map<Course>(model);

            // get school
            var school = _repos.Schools.Get(schoolId);

            if(school == null)
            {
                return NotFound("Adding course to a non-existant school. School with that Id not found.");
            }

            course.School = school;
            course = _repos.Courses.Create(course);
            _repos.Commit();

            return Created($"/api/courses/{course.Id}", course);
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
                ModelState.AddModelError("Invalid course Id", "Provide a valid 'Id' value greater than 0 to delete course.");
                return BadRequest(ModelState);
            }

            Course course = _repos.Courses
                                  .GetWith(id, "School", "Units");

            if (course != null)
            {
                _repos.Courses.Remove(course);

                return Ok("Done!");
            }
            else
            {
                return NotFound();
            }
        }


        // Enroll
        [HttpPost]
        [Route("{courseId}/enroll/{studentId}")]
        [Authorize(Roles = "Student")]
        public IActionResult Enroll(int courseId, int studentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
