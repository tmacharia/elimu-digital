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
using System.Threading.Tasks;

namespace web.Controllers
{
    [Authorize]
    [Route("classes")]
    public class ClassesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IDataManager _dataManager;
        private readonly IRepositoryFactory _repos;
        public ClassesController(UserManager<AppUser> userManager, IDataManager dataManager,IRepositoryFactory factory)
        {
            _userManager = userManager;
            _dataManager = dataManager;
            _repos = factory;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int itemsperpage = 10)
        {
            AppUser user = await _userManager.GetUserAsync(User);
            IEnumerable<Class> classes = new List<Class>();

            if(User.Role() == "Administrator")
            {
                classes = _repos.Classes
                                .ListWith("Units", "Likes");
            }
            else if(User.Role() == "Lecturer")
            {
                classes = _dataManager.MyClasses<Lecturer>(user.AccountId);
            }
            else if(User.Role() == "Student")
            {
                classes = _dataManager.MyClasses<Student>(user.AccountId);
            }

            Result<Class> model = classes.ToPaged(page, itemsperpage);

            return View(model);
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
