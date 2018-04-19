using AutoMapper;
using Common.ViewModels;
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

namespace web.API_s
{
    [Route("api/exams")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]

    public class ExamsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryFactory _repos;
        private readonly UserManager<AppUser> _usermanager;
        private readonly IDataManager _dataManager;
        public ExamsController(IMapper mapper,IRepositoryFactory factory, UserManager<AppUser> userManager,IDataManager dataManager)
        {
            _mapper = mapper;
            _repos = factory;
            _usermanager = userManager;
            _dataManager = dataManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IList<ExamViewModel> exams = new List<ExamViewModel>();

            if(User.Role() == "Administrator")
            {
                exams = _repos.Exams
                              .ListWith("Unit")
                              .Select(TransformFuncs.ToViewModel())
                              .ToList();

                return Ok(exams);
            }

            AppUser user = await _usermanager.GetUserAsync(User);

            if(User.Role() == "Lecturer")
            {
                exams = _dataManager.MyExams<Lecturer>(user.AccountId);
            }
            else if(User.Role() == "Student")
            {
                exams = _dataManager.MyExams<Student>(user.AccountId);
            }

            return Ok(exams);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(int id)
        {
            if(id < 1)
            {
                return BadRequest("Invalid exam id.");
            }

            var model = _dataManager.GetExam(id);

            return Ok(model);
        }

        [HttpGet]
        [Route("unit/{id}")]
        public IActionResult GetByUnit(int id)
        {
            IList<ExamViewModel> exams = new List<ExamViewModel>();

            exams = _repos.Units.GetWith(id, "Exams", "Exams.Unit")
                               ?.Exams
                               .Select(TransformFuncs.ToViewModel())
                               .TakeWhile(x => x != null)
                               .ToList();

            return Ok(exams);
        }

        [HttpGet]
        [Route("sessions/{id}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetSession(int id)
        {
            if(id < 1)
            {
                return BadRequest("Invalid exam id.");
            }

            AppUser user = await _usermanager.GetUserAsync(User);

            var exam = _repos.Exams.GetWith(id, "Sessions");

            if(exam == null)
            {
                return NotFound("Exam record with that id does not exist.");
            }

            var session = exam.Sessions
                              .FirstOrDefault(x => x.StudentId == user.AccountId);

            if(session == null)
            {
                session = new ExamSession()
                {
                    ExamId = exam.Id,
                    StudentId = user.AccountId
                };

                session = _repos.ExamSessions.Create(session);
                exam.Sessions.Add(session);
                exam = _repos.Exams.Update(exam);
                _repos.Commit();
            }

            var examViewModel = _dataManager.GetExam(id);
            var examSession = new ExamSessionViewModel()
            {
                Id = session.Id,
                SessionId = session.SessionId,
                TotalQuestions = examViewModel.Questions.Count,
                TotalMarks = examViewModel.Questions.Sum(x => x.Marks),
                Exam = examViewModel
            };

            return Ok(examSession);
        }

        [HttpPost]
        [Route("{id}/submit")]
        public IActionResult Submit(int id,IList<Question> questions)
        {
            return Ok();
        }
    }
}
