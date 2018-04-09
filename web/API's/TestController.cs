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
    [Authorize]
    [Route("api/test")]
    public class TestController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailer;

        public TestController(UserManager<AppUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailer = emailSender;
        }

        [HttpGet]
        [Route("sendemail")]
        public async Task<IActionResult> SendEmail(string to,string subject,string msg)
        {
            if(string.IsNullOrWhiteSpace(to) ||
               string.IsNullOrWhiteSpace(subject) ||
               string.IsNullOrWhiteSpace(msg))
            {
                return BadRequest("One of request parameters are empty or null. Specify 'to' address, email 'subject' and a message.");
            }

            await _emailer.SendEmailAsync(subject, msg,to);

            return Ok("Email sent. Check your inbox.");
        }
    }
}
