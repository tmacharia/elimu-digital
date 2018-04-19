using AutoMapper;
using Common.ViewModels;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.API_s
{
    [Authorize]
    [Route("api/lecturers")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]

    public class LecturersController : Controller
    {
        private readonly IRepositoryFactory _repos;
        private readonly IMapper _mapper;

        public LecturersController(IRepositoryFactory factory, IMapper mapper)
        {
            _repos = factory;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var lecs = _repos.Lecturers
                             .ListWith("Profile")
                             .Where(x => x.Profile != null)
                             .ToList();

            return Ok(lecs);
        }

        [HttpPost]
        [Route("update")]
        public IActionResult UpdateBio(LecturerViewModel model)
        {
            var lecturer = _repos.Lecturers
                                 .GetWith(this.GetAccountId(), "Skills");

            lecturer.Bio = model.Bio;
            if(lecturer.Skills == null)
            {
                lecturer.Skills = new List<Skill>();
            }
            foreach (var item in model.Skills)
            {
                if (lecturer.Skills.Contains(x => x.Name == item.Name)) { continue; }
                else
                {
                    lecturer.Skills.Add(item);
                }
            }

            lecturer = _repos.Lecturers.Update(lecturer);
            _repos.Commit();

            return Ok(lecturer);
        }
    }
}
