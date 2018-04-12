using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly IRepositoryFactory _repos;
        private readonly UserManager<AppUser> _userManager;

        public NotificationsController(IRepositoryFactory factory, UserManager<AppUser> userManager)
        {
            _repos = factory;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Notifications = this.GetNotifications();
            return View(this.GetAccountId());
        }
        [HttpGet]
        [Route("api/notifications")]
        public IActionResult My()
        {
            var notifications = _repos.Notifications
                                      .List
                                      .Where(x => x.AccountId == this.GetAccountId())
                                      .ToList();

            return Ok(notifications);
        }

        [HttpGet]
        public IActionResult Get(int id)
        {
            if(id < 1)
            {
                return BadRequest("Invalid notification Id.");
            }

            var notification = _repos.Notifications.Get(id);

            if(notification == null)
            {
                return NotFound("Notification with that id does not exist in records.");
            }

            return Ok(notification);
        }

        [HttpPost]
        [Route("api/notifications/markasread")]
        public IActionResult MarkAsRead(IList<Notification> notifications)
        {
            if(notifications.Count < 1)
            {
                return Ok();
            }

            foreach (var item in notifications)
            {
                var ntf = _repos.Notifications.Get(item.Id);
                ntf.Read = true;
                ntf = _repos.Notifications.Update(ntf);
            }

            //_repos.Notifications.UpdateMany(notifications.ToArray());
            _repos.Commit();

            var items = _repos.Notifications.List
                              .Where(x => x.AccountId == this.GetAccountId())
                              .ToList();

            return Ok(items);
        }

        [HttpDelete]
        [Route("api/notifications/{id}")]
        public IActionResult Remove(int id)
        {
            if(id < 1)
            {
                return BadRequest("Invalid notification id.");
            }

            var notification = _repos.Notifications.Get(id);

            if(notification == null)
            {
                return NotFound("Notification with that Id does not exist.");
            }

            _repos.Notifications.Remove(id);
            _repos.Commit();

            return Ok("Notification deleted!");
        }
    }
}
