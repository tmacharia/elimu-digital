using Microsoft.AspNetCore.Authorization;
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

        public ProgressController(IProgressTracker tracker)
        {
            _tracker = tracker;
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
    }
}
