using AutoMapper;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Controllers
{
    [Authorize]
    public class ExamsController : Controller
    {
        private readonly INotificationManager _notify;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepositoryFactory _repos;
        private readonly IExamManager _examManager;
        private readonly IMapper _mapper;

        public ExamsController(INotificationManager notificationManager,
                               UserManager<AppUser> userManager,
                               IRepositoryFactory factory, 
                               IExamManager examManager,
                               IMapper mapper)
        {
            _notify = notificationManager;
            _repos = factory;
            _examManager = examManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return View(_examManager.All.ToList());
        }

        [HttpGet]
        [Route("exams/set-for/{name}/{id}")]
        [Authorize(Roles = "Lecturer")]
        public IActionResult Create(string name,int id)
        {
            if(id < 1)
            {
                return BadRequest("Invalid unit id.");
            }

            Exam model = new Exam();

            var unit = _repos.Units.Get(id);

            if(unit == null)
            {
                return NotFound("Unit does not exist in records.");
            }

            model.Unit = unit;

            return View(model);
        }

        [HttpPost]
        [Route("api/exams/create")]
        [Authorize(Roles = "Lecturer")]
        public IActionResult Create(int id, Exam model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(id < 1)
            {
                return BadRequest("Invalid unit Id.");
            }

            var unit = _repos.Units.Get(id);
            if(unit == null)
            {
                return NotFound("Unit does not exist in records.");
            }

            model.Unit = unit;
            model = _repos.Exams.Create(model);
            _repos.Commit();

            return Ok(model.Id);
        }
    }
}
