using AutoMapper;
using Common.ViewModels;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.API_s
{
    [Authorize]
    [Route("api/posts")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]

    public class PostsController : Controller
    {
        private readonly IRepositoryFactory _repos;
        private readonly IMapper _mapper;

        public PostsController(IRepositoryFactory factory, IMapper mapper)
        {
            _repos = factory;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("{id}/like")]
        public IActionResult Like(int id)
        {
            if(id < 1)
            {
                return BadRequest("Invalid post id");
            }

            var post = _repos.Posts.GetWith(id,"Likes");

            if(post == null)
            {
                return NotFound("Post with that id does not exist in records.");
            }
            if (post.Likes == null)
            {
                post.Likes = new List<Like>();
            }
            var like = new Like()
            {
                Reaction = Reaction.Like,
                By = this.GetMyProfile()
            };

            like = _repos.Likes.Create(like);
            _repos.Commit();
            post.Likes.Add(like);
            post = _repos.Posts.Update(post);
            _repos.Commit();

            return Ok(post);
        }

        // Comment on post
        [HttpPost]
        [Route("{id}/comment")]
        public IActionResult Comment(int id, CommentViewModel model)
        {
            if (id < 1)
            {
                ModelState.AddModelError("Content Id", "Provide a valid content id.");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var post = _repos.Posts
                                .GetWith(id, "Comments");

            if (post == null)
            {
                return NotFound("Cannot comment on a post that does not exist.");
            }
            Comment comment = _mapper.Map<Comment>(model);

            comment.By = this.GetMyProfile();

            try
            {
                comment = _repos.Comments.Create(comment);
                _repos.Commit();
                post.Comments.Add(comment);
                _repos.Posts.Update(post);
                _repos.Commit();
            }
            catch (Exception)
            {
                return this.Error("An error occured.");
            }

            return Ok(comment);
        }

    }
}
