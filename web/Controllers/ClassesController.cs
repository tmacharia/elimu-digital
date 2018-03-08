using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paginator;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Controllers
{
    [Authorize]
    [Route("classes")]
    public class ClassesController : Controller
    {
        private readonly IRepositoryFactory _repos;
        public ClassesController(IRepositoryFactory factory)
        {
            _repos = factory;
        }

        [HttpGet]
        [Authorize(Roles = "Lecturer, Admin")]
        public IActionResult Index(int page = 1, int itemsperpage = 10)
        {
            var classes = _repos.Classes
                                .ListWith("Units", "Likes")
                                .OrderByDescending(x => x.Timestamp)
                                .ToPaged(page, itemsperpage);

            return View(classes);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Details(int id)
        {
            if (id < 1)
            {
                return BadRequest("Invalid class Id");
            }

            var _class = _repos.Classes
                               .GetWith(id,
                                        "Unit",
                                        "Unit.Lecturer",
                                        "Unit.Course",
                                        "Likes");

            if (_class == null)
            {
                return NotFound("Class record with that id does not exist.");
            }

            return View(_class);
        }


        [HttpGet]
        [Route("create")]
        [Authorize(Roles = "Lecturer, Admin")]
        public IActionResult Create()
        {
            Class @class = new Class();

            return View(@class);
        }


        [HttpPost]
        [Route("create")]
        [Authorize(Roles = "Lecturer, Admin")]
        public IActionResult Create(Class model)
        {
            if (!ModelState.IsValid)
            {
                string error = ModelState.Populate().First();

                return BadRequest(error);
            }

            model = _repos.Classes.Create(model);
            _repos.Commit();

            return Ok("Classroom added successfully!");
        }
    }
}
