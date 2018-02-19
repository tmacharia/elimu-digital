using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using web.Extensions;

namespace web.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly IRepositoryFactory _repos;

        public CoursesController(IRepositoryFactory factory)
        {
            _repos = factory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var courses = _repos.Courses
                                .ListWith("Units","Students","Likes")
                                .ToList();

            return View(courses);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Create()
        {
            try
            {
                string name = Request.Form["name"];

                if (!string.IsNullOrWhiteSpace(name))
                {
                    this.AddNotification($"Posted! Data contents: {name}", "Info");
                }
                else
                {
                    this.AddNotification("Sorry, no data received.", "Error");
                }

                return RedirectToActionPermanent("Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
