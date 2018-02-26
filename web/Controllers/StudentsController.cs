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
    public class StudentsController : Controller
    {
        private readonly IRepositoryFactory _repos;
        private readonly IMapper _mapper;

        public StudentsController(IRepositoryFactory factory, IMapper mapper)
        {
            _repos = factory;
            _mapper = mapper;
        }
    }
}
