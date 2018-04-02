using Common.ViewModels;
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
    [Route("api/progress")]
    public class ProgressController : Controller
    {
        private readonly IProgressTracker _tracker;
        private readonly UserManager<AppUser> _userManager;

        public ProgressController(IProgressTracker tracker, UserManager<AppUser> userManager)
        {
            _tracker = tracker;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("initialize")]
        public IActionResult Initial(int id, decimal current, decimal overall)
        {
            if(id < 1)
            {
                return this.Error("Invalid id.");
            }

            _tracker.TrackInitial(id, current, overall);

            return Ok();
        }

        [HttpPost]
        public IActionResult Track(int id, decimal current)
        {
            if (id < 1)
            {
                return this.Error("Invalid id.");
            }

            var data = _tracker.TrackProgress(id, current);

            return Ok(data);
        }

        [HttpPost]
        [Route("download")]
        public IActionResult Download(int id)
        {
            if (id < 1)
            {
                return this.Error("Invalid id.");
            }

            _tracker.TrackDownload(id);

            return Ok();
        }

        [HttpGet]
        [Route("unit/{id}")]
        public IActionResult UnitProgress(int id)
        {
            if(id < 1)
            {
                return BadRequest("Invalid unit id.");
            }

            var progress = _tracker.TrackUnitProgress(id);

            if(progress == null)
            {
                return NotFound("Progress data not found.");
            }

            return Ok(progress);
        }

        [HttpGet]
        [Route("unit/{id}/my")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyUnitProgress(int id)
        {
            if (id < 1)
            {
                return BadRequest("Invalid unit id.");
            }

            AppUser user = await _userManager.GetUserAsync(User);

            var progress = _tracker.GetProgressByUnit(id, user.AccountId);

            if(progress == null)
            {
                return NotFound("Coursework progress data for unit not found.");
            }

            var root = new RootCourseWorkPrgs()
            {
                Data = progress
            };

            return Ok(root);
        }
    }
}
