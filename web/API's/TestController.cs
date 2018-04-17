using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interfaces;
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
        private readonly INotificationManager _notify;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailer;
        private readonly IRepositoryFactory _repos;

        public TestController(UserManager<AppUser> userManager,IRepositoryFactory factory, IEmailSender emailSender, INotificationManager notificationManager)
        {
            _userManager = userManager;
            _repos = factory;
            _emailer = emailSender;
            _notify = notificationManager;
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

        [HttpGet]
        [Route("mockcontent/{id}")]
        public async Task<IActionResult> MockContent(int id)
        {
            var content = _repos.Contents.GetWith(id, "Unit", "UploadedBy","UploadedBy.Profile");

            if(content == null)
            {
                return NotFound("Content with that id was not found in records");
            }

            try
            {
                await _notify.OnNewContent(content);

                return Ok("Notification emails sent out. Check your inboxes.");
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
    }
}
