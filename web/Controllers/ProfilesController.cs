using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly IUploader _uploader;
        private readonly IEmailSender _emailer;

        public ProfilesController(IUploader uploader, IEmailSender emailSender)
        {
            _uploader = uploader;
            _emailer = emailSender;
        }


    }
}
