using Common.ViewModels;
using DAL.Models.Fees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace web.Controllers
{
    [Authorize]
    [Route("fees")]
    [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any)]

    public class FeesController : Controller
    {
        private readonly IRepositoryFactory _repos;
        private readonly IFeesManager _fees;

        public FeesController(IFeesManager feesManager, IRepositoryFactory factory)
        {
            _fees = feesManager;
            _repos = factory;
        }

        [HttpGet]
        [Route("basestructure")]
        [Authorize(Roles = "Admin")]
        public IActionResult BaseStructure()
        {
            var _base = _repos.BaseFeeStructures
                              .ListWith("PreparedBy", "PreparedBy.Profile")
                              .FirstOrDefault();

            if(_base == null)
            {
                _base = new BaseFeeStructure()
                {
                    AdminId = this.GetAccountId()
                };

                _base = _repos.BaseFeeStructures.Create(_base);
                _repos.Commit();
            }
            ViewBag.Notifications = this.GetNotifications();
            return View(_base);
        }

        [HttpPost]
        [Route("basestructure")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult BaseStructure(BaseFeeStructure model)
        {
            if (!ModelState.IsValid)
            {
                return View(ModelState);
            }

            model.EditDate = DateTime.Now;
            model = _repos.BaseFeeStructures.Update(model);
            _repos.Commit();

            return RedirectToAction("basestructure");
        }

        [HttpGet]
        public IActionResult Index(IList<FeesViewModel> model)
        {
            ViewBag.Notifications = this.GetNotifications();
            return View(model);
        }

        [HttpPost]
        [Route("structure")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return Ok();
        }

        [HttpPut]
        [Route("structure")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit()
        {
            return Ok();
        }

        [HttpGet]
        [Route("structures")]
        public IActionResult Structures()
        {
            List<FeesViewModel> list = new List<FeesViewModel>();

            return Index(list);
        }

        [HttpGet]
        [Route("structures/{year}/{sem}")]
        public IActionResult Structures(int year,int sem)
        {
            List<FeesViewModel> list = new List<FeesViewModel>();

            return Index(list);
        }

        [HttpGet]
        [Route("structure/{courseId}/{name}")]
        public IActionResult ForCourse(int courseId,string name)
        {
            List<FeesViewModel> list = new List<FeesViewModel>();

            return Index(list);
        }
    }
}
