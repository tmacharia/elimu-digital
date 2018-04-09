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
    [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any)]
    public class ExamsController : Controller
    {
        private readonly INotificationManager _notify;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepositoryFactory _repos;
        private readonly IMapper _mapper;

        public ExamsController(INotificationManager notificationManager,
                               UserManager<AppUser> userManager,
                               IRepositoryFactory factory, 
                               IMapper mapper)
        {
            _notify = notificationManager;
            _userManager = userManager;
            _repos = factory;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            AppUser user = await _userManager.GetUserAsync(User);

            ViewBag.Notifications = _repos.Notifications.List.Count(x => x.AccountId == user.AccountId && x.Read == false);

            return View();
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
        public async Task<IActionResult> Create(int id, Exam model)
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

            await _notify.OnNewExam(model);

            return Ok(model.Id);
        }

        [HttpGet]
        [Route("exams/for-unit/{id}/{name}")]
        public IActionResult ExamsForUnit(int id,string name)
        {
            if(id < 1)
            {
                return BadRequest("Invalid unit Id.");
            }

            var unit = _repos.Units.Get(id);

            if(unit == null)
            {
                return NotFound("Unit with that id does not exist in records.");
            }

            return View(unit);
        }

        [HttpGet]
        [Route("exams/{id}/scores")]
        public IActionResult ExamScores(int id)
        {
            if(id < 1)
            {
                return BadRequest("Invalid exam id.");
            }

            var exam = _repos.Exams.Get(id);

            if (exam == null)
            {
                return NotFound("Exam not found.");
            }

            return View(exam);
        }

        [HttpGet]
        [Route("exams/{id}/myscore")]
        public IActionResult MyExamScore(int id)
        {
            if (id < 1)
            {
                return BadRequest("Invalid exam id.");
            }

            var exam = _repos.Exams.Get(id);

            if (exam == null)
            {
                return NotFound("Exam not found.");
            }

            return View(exam);
        }

        [HttpGet]
        [Route("exams/progress/{id}/{type}/{name}")]
        public IActionResult Progress(int id,string type,string name)
        {
            if (id < 1)
            {
                return BadRequest("Invalid exam id.");
            }

            var exam = _repos.Exams.Get(id);

            if (exam == null)
            {
                return NotFound("Exam not found.");
            }

            return View(exam);
        }

        [HttpGet]
        [Route("exams/{id}/session")]
        [Authorize(Roles = "Student")]
        public IActionResult Session(int id)
        {
            if(id < 1)
            {
                return BadRequest("Invalid exam id.");
            }

            var exam = _repos.Exams.GetWith(id,
                                    "Unit",
                                    "Unit.Course",
                                    "Unit.Lecturer.Profile",
                                    "Questions");

            if(exam == null)
            {
                return NotFound("Exam record with that id does not exist in records.");
            }

            return View(exam);
        }

        [HttpGet]
        [Route("exams/session-in-progress/{id}")]
        public IActionResult ExamSession(int id)
        {
            if(id < 1)
            {
                return BadRequest("Invalid exam session id.");
            }

            return View(id);
        }

        [HttpGet]
        [Route("exams/{id}/statistics")]
        public IActionResult Statistics(int id)
        {
            if(id < 1)
            {
                return BadRequest("Invalid exam id.");
            }

            var exam = _repos.Exams.Get(id);

            if(exam == null)
            {
                return NotFound("Exam record with that id does not exist.");
            }

            return View(exam);
        }
    }
}
