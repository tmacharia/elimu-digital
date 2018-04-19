using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace web.Controllers
{
    [Authorize]
    [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any)]
    public class BoardsController : Controller
    {
        private readonly IRepositoryFactory _repos;
        private readonly UserManager<AppUser> _userManager;
        private readonly IDataManager _dataManager;
        private readonly Stopwatch _watch;

        public BoardsController(IRepositoryFactory factory, UserManager<AppUser> userManager, IDataManager dataManager)
        {
            _repos = factory;
            _userManager = userManager;
            _dataManager = dataManager;
            _watch = new Stopwatch();
        }

        [HttpGet]
        [Route("discussionboards")]
        public IActionResult Index()
        {
            IList<DiscussionBoard> boards = this.GetMyBoards()
                                                .OrderByDescending(x => x.Posts.Count)
                                                .ToList();

            ViewBag.Title = "Discussion Boards";
            ViewBag.Notifications = this.GetNotifications();
            return View(boards);
        }
        [HttpGet]
        [Route("discussionboards/search")]
        public IActionResult Search(string q)
        {
            ViewBag.Query = q;

            _watch.Start();

            IList<DiscussionBoard> boards = this.GetMyBoards();
            boards = boards.Where(Predicates.Boards(q))
                           .OrderByDescending(x => x.Posts.Count)
                           .ToList();
            _watch.Stop();
            ViewBag.timespan = _watch.Elapsed;
            _watch.Reset();

            ViewBag.Notifications = this.GetNotifications();
            return View(boards);
        }
        [HttpGet]
        [Route("discussionboards/{id}/{name}")]
        public IActionResult Details(int id,string name)
        {
            if(id < 1)
            {
                return BadRequest("Invalid discussion board Id.");
            }

            var board = _repos.DiscussionBoards
                              .GetWith(id, "Unit");

            if(board == null)
            {
                return NotFound("Discussion board with that id does not exist in records.");
            }

            ViewBag.Notifications = this.GetNotifications();

            return View(board);
        }

        [HttpGet]
        [Route("api/discussionboards/{id}/participants")]
        public IActionResult GetParticipants(int id)
        {
            if(id < 1)
            {
                return BadRequest("Invalid discussion board id.");
            }

            var list = _dataManager.GetBoardParticipant(id);

            return Ok(list);
        }


        [HttpGet]
        [Route("api/discussionboards/{id}/posts")]
        public IActionResult GetPosts(int id)
        {
            if(id < 1)
            {
                return BadRequest("Invalid discussion board id.");
            }

            var board = _repos.DiscussionBoards
                             .GetWith(id,
                             "Posts",
                             "Posts.By",
                             "Posts.Likes",
                             "Posts.Likes.By",
                             "Posts.Comments",
                             "Posts.Comments.By");

            if(board == null)
            {
                return NotFound("Post with that id does not exist in records.");
            }
            else
            {
                return Ok(board.Posts.OrderByDescending(x => x.Timestamp));
            }
        }

        [HttpPost]
        [Route("api/discussionboards/{id}/post")]
        public IActionResult PostOnBoard(int id,string message)
        {
            if(id < 1)
            {
                return BadRequest("Invalid discussion board id");
            }

            var board = _repos.DiscussionBoards
                              .GetWith(id,
                              "Posts");

            if(board == null)
            {
                return NotFound("Discussion board with that id does not exist in records.");
            }

            if(board.Posts == null)
            {
                board.Posts = new List<Post>();
            }

            var post = new Post()
            {
                By = this.GetMyProfile(),
                Message = message
            };

            post = _repos.Posts.Create(post);
            _repos.Commit();
            board.Posts.Add(post);
            post = _repos.Posts.Update(post);
            _repos.Commit();

            return Ok(post);
        }

        [HttpGet]
        [Route("discussionboards/unit/{id}/{name}")]
        public IActionResult ByUnit(int id,string name)
        {
            if(id < 1)
            {
                return BadRequest("Invalid unit id.");
            }

            var boards = _repos.DiscussionBoards
                               .ListWith("Unit")
                               .Where(x => x.UnitId == id)
                               .ToList();

            ViewBag.Title = $"Discussion Boards ({name})";
            ViewBag.Notifications = this.GetNotifications();
            return View("~/Views/Boards/Index.cshtml", boards);
        }

        [HttpPost]
        [Route("api/discussionboards/unit")]
        public IActionResult Create(int unitId,string name)
        {
            var board = new DiscussionBoard()
            {
                Name = name,
                UnitId = unitId
            };

            board = _repos.DiscussionBoards.Create(board);
            _repos.Commit();

            return Ok("Board created successfully!");
        }
    }
}
