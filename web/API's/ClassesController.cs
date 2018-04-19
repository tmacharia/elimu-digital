using AutoMapper;
using Common.ViewModels;
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
    [Route("api/classes")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]

    public class ClassesController : Controller
    {
        private readonly IRepositoryFactory _repos;
        private readonly IMapper _mapper;

        public ClassesController(IRepositoryFactory factory, IMapper mapper)
        {
            _repos = factory;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var classes = _repos.Classes
                                .List
                                .ToList();

            var result = _mapper.Map<List<ClassViewModel>>(classes);

            return Ok(result);
        }

        [HttpPost]
        [Route("{id}/allocateToUnit/{unitId}")]
        [Authorize(Roles = "Admin, Lecturer")]
        public IActionResult Allocate(int id, int unitId)
        {
            if(id < 1 || unitId < 1)
            {
                return BadRequest("Invalid class or unit Id.");
            }

            var _class = _repos.Classes.Get(id);

            if(_class == null)
            {
                return NotFound("Class record with that Id does not exist.");
            }

            var unit = _repos.Units.Get(unitId);

            if(unit == null)
            {
                return NotFound("Unit with that Id does not exist.");
            }

            unit.Class = _class;
            unit = _repos.Units.Update(unit);
            _repos.Commit();

            return Ok("Room allocation successful!");
        }
    }
}
