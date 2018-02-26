using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Controllers
{
    [Authorize]
    public class LecturersController : Controller
    {
        private readonly IRepositoryFactory _repos;
        private readonly IMapper _mapper;

        public LecturersController(IRepositoryFactory factory, IMapper mapper)
        {
            _repos = factory;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var lecturers = _repos.Lecturers
                                  .ListWith("Profile", "Units", "Likes")
                                  .ToList();

            return View(lecturers);
        }

        [HttpGet]
        [Route("{id}/{names}")]
        public IActionResult Details(int id, string names)
        {
            return View();
        }
    }
}
