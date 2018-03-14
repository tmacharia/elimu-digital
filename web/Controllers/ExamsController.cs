using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IRepositoryFactory _repos;
        private readonly IMapper _mapper;

        public ExamsController(INotificationManager notificationManager,IRepositoryFactory factory, IMapper mapper)
        {
            _notify = notificationManager;
            _repos = factory;
            _mapper = mapper;
        }
    }
}
