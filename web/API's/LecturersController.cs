using AutoMapper;
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
    }
}
