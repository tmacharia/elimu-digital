using AutoMapper;
using Common.ViewModels;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services;
using Services.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace web.API_s
{
    [Route("api/schools")]
    public class SchoolsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryFactory _repos;

        public SchoolsController(IMapper mapper, IRepositoryFactory factory)
        {
            _mapper = mapper;
            _repos = factory;
        }

        // Index
        [HttpGet]
        public IActionResult Index()
        {
            IList<School> schools = _repos.Schools
                                          .ListWith("Location","Courses")
                                          .ToList();

            var entities = _mapper.Map<List<SchoolViewModel>>(schools);

            return Ok(entities);
        }
        
        // Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(SchoolViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            School school = _mapper.Map<School>(model);

            school = _repos.Schools.Create(school);
            _repos.Commit();

            return Created($"/api/schools/{school.Id}", school);
        }
        
        // Get
        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(int id)
        {
            if(id < 1)
            {
                ModelState.AddModelError("Invalid school Id", "Provide a valid 'Id' value greater than 0 to get school.");
                return BadRequest(ModelState);
            }

            School school = _repos.Schools
                                  .GetWith(id, "Location", "Courses");

            if(school != null)
            {
                return Ok(school);
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
        public IActionResult Edit(int id, SchoolViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            School school = _repos.Schools.Get(id);

            if(school == null)
            {
                return NotFound("School with that id can't be edited since it does not exist.");
            }
            else
            {
                var reflectResult = school.UpdateReflector(model);

                if(reflectResult.TotalUpdates < 1)
                {
                    return NoContent();
                }
                else
                {
                    school = _repos.Schools.Update(reflectResult.Value);

                    return Ok(school);
                }
            }
        }
        
        // Search
        [HttpGet]
        [Route("search")]
        public IActionResult Search(string query)
        {
            string pattern = "(" + query + ")";

            IList<School> schools = _repos.Schools
                                          .ListWith("Location", "Courses")
                                          .Where(Predicates.School(query))
                                          .ToList();

            return Ok(schools);
        }
        
        // Courses
        [HttpGet]
        [Route("{schoolId}/courses")]
        public IActionResult GetCourses(int schoolId)
        {
            if(schoolId < 1)
            {
                return BadRequest("Invalid school id.");
            }

            var courses = _repos.Schools
                                .ListWith("Courses")
                                .FirstOrDefault(x => x.Id == schoolId)
                                .Courses
                                .ToList();

            IList<CourseViewModel> model = _mapper.Map<List<CourseViewModel>>(courses);

            return Ok(model);
        }

        // Delete
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (id < 1)
            {
                ModelState.AddModelError("Invalid school Id", "Provide a valid 'Id' value greater than 0 to delete school.");
                return BadRequest(ModelState);
            }

            School school = _repos.Schools
                                  .GetWith(id, "Location", "Courses");

            if (school != null)
            {
                _repos.Schools.Remove(school);

                return Ok("Done!");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
