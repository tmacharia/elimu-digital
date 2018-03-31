using AutoMapper;
using Common.ViewModels;
using DAL.Extensions;
using DAL.Models;
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

            Exam exam = _repos.Exams.GetWith(id, "Unit",
                                     "Unit.Course",
                                     "Unit.Lecturer",
                                     "Unit.Lecturer.Profile",
                                     "Questions",
                                     "Questions.Answers",
                                     "Likes",
                                     "Comments");
                                     

            if(exam == null)
            {
                return NotFound("Exam with that id does not exist in records.");
            }

            var model = new ExamDetailsViewModel()
            {
                Code = exam.Code,
                Comments = exam.Comments,
                Date = exam.Date,
                End = exam.End,
                Id = exam.Id,
                Instructor = exam.Unit.Lecturer.Profile,
                Unit = new ExamUnit()
                {
                    Id = exam.Unit.Id,
                    Name = exam.Unit.Name
                },
                Course = new ExamCourse()
                {
                    Id = exam.Unit.Course.Id,
                    Code = exam.Unit.Course.Code,
                    Name = exam.Unit.Course.Name,
                    Type = exam.Unit.Course.Type
                },
                Name = exam.Name,
                Likes = exam.Likes,
                Start = exam.Start,
                Moment = exam.Moment,
                Questions = exam.Questions.Select(q => new ExamQuestion()
                {
                    Id = q.Id,
                    Marks = q.Marks,
                    Text = q.Text,
                    Answers = q.Answers.Select(a => new QuestionAnswer()
                    {
                        Id = a.Id,
                        Text = a.Text,
                        IsCorrect = a.IsCorrect
                    }).ToList()
                }).ToList()
            };

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
        [Route("{id}/session")]
        public IActionResult GetSession(int id)
        {
            return Ok();
        }

        [HttpPost]
        [Route("{id}/submit")]
        public IActionResult Submit(int id,IList<Question> questions)
        {
            return Ok();
        }
    }
}
