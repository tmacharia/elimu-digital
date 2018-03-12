using AutoMapper;
using DAL.Extensions;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IDataManager _dataManager;
        private readonly IRepositoryFactory _repos;
        private readonly IMapper _mapper;

        public StudentsController(UserManager<AppUser> userManager,IDataManager dataManager,IRepositoryFactory factory, IMapper mapper)
        {
            _userManager = userManager;
            _dataManager = dataManager;
            _repos = factory;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Student> students = new List<Student>();
            AppUser user = await _userManager.GetUserAsync(User);

            if(User.Role() == "Student")
            {
                students = _dataManager.MyClassMates(user.AccountId);
            }
            else if(User.Role() == "Lecturer")
            {
                students = _dataManager.MyStudents(user.AccountId, 50);
            }
            else
            {
                students = _repos.Students
                                 .ListWith("Profile", "Course", "StudentUnits");
            }

            var model = students.OrderByDescending(x => x.Timestamp)
                                .ToList();

            return View(model);
        }

        [HttpGet]
        [Route("students/{id}/{names}")]
        public IActionResult Details(int id, string names)
        {
            return View();
        }
    }
}
