using AutoMapper;
using Common.Exceptions;
using Common.Models;
using Common.ViewModels;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interfaces;
using Services.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace web.API_s
{
    [Route("api/contents")]
    [Authorize]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]

    public class ContentsController : Controller
    {
        private readonly IRepositoryFactory _repos;
        private readonly UserManager<AppUser> _userManager;
        private readonly INotificationManager _notify;
        private readonly IUploader _uploader;
        private readonly IMapper _mapper;

        public ContentsController(IRepositoryFactory factory,
                                  UserManager<AppUser> userManager,
                                  INotificationManager notificationManager,
                                  IUploader uploader,
                                  IMapper mapper)
        {
            _repos = factory;
            _userManager = userManager;
            _notify = notificationManager;
            _uploader = uploader;
            _mapper = mapper;
        }

        // Add
        [HttpPost]
        [Route("{unitId}")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> Create(int unitId, ContentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(Request.Form.Files == null || Request.Form.Files.Count < 1)
            {
                return BadRequest("No file included in form data.");
            }

            var unit = _repos.Units.Get(unitId);

            if(unit == null)
            {
                return NotFound("No record of units exists with that Id.");
            }

            IFile file = new FormFile(Request.Form.Files[0]);

            string uploadResult = await _uploader.Upload(file);

            if (string.IsNullOrWhiteSpace(uploadResult))
                return this.Error("Uploading file failed. Please try again.");

            var content = _mapper.Map<DAL.Models.Content>(model);
            content.FileName = file.FileName;
            content.Type = file.Format;
            content.Url = uploadResult;
            content.Unit = unit;

            // get & attach lecturer (uploader)
            var user = await _userManager.GetUserAsync(User);
            var lecturer = _repos.Lecturers.Get(user.AccountId);

            if(lecturer == null)
            {
                return this.UnAuthorized("Current user account used in uploading content does not have have a valid Lecturer record.");
            }

            content.UploadedBy = lecturer;

            // save content finally
            content = _repos.Contents.Create(content);
            _repos.Commit();

            await _notify.OnNewContent(content);

            return Created($"api/contents/{content.Id}", content);
        }
        
        // Edit
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> Edit(int id, ContentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(id < 1)
            {
                return BadRequest("Invalid content Id.");
            }

            var content = _repos.Contents.Get(id);

            if (content == null)
            {
                return NotFound("No record of contents exists with that Id.");
            }

            var reflectResult = content.UpdateReflector(model);
            content = reflectResult.Value;

            if(reflectResult.TotalUpdates > 0 || Request.Form.Files.Count > 0)
            {
                // update
                if(Request.Form.Files.Count > 0)
                {
                    IFile file = new FormFile(Request.Form.Files[0]);

                    string uploadResult = await _uploader.Upload(file);

                    if (string.IsNullOrWhiteSpace(uploadResult))
                        return this.Error("Uploading file failed. Please try again.");

                    content.FileName = file.FileName;
                    content.Type = file.Format;
                    content.Url = uploadResult;
                }

                // update content
                content = _repos.Contents.Update(content);
                _repos.Commit();

                return Ok("Edited successfully!");
            }
            else
            {
                // no update
                return NoContent();
            }
        }

        // Get
        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(int id)
        {
            if(id < 1)
            {
                ModelState.AddModelError("Content Id", "Provide a valid content id.");
                return BadRequest(ModelState);
            }

            var content = _repos.Contents
                                .GetWith(id, "Unit", "UploadedBy");

            if(content != null)
            {
                return Ok(content);
            }
            else
            {
                return NotFound();
            }
        }
        
        // Get content likes
        [HttpGet]
        [Route("{id}/likes")]
        public IActionResult GetLikes(int id)
        {
            if(id < 1)
            {
                ModelState.AddModelError("Content Id", "Provide a valid content id.");
                return BadRequest(ModelState);
            }

            var content = _repos.Contents
                                .GetWith(id, "Likes");

            if(content != null)
            {
                return Ok(content.Likes);
            }
            else
            {
                return NotFound();
            }
        }
       
        // Get content comments
        [HttpGet]
        [Route("{id}/comments")]
        public IActionResult GetComments(int id)
        {
            if(id < 1)
            {
                ModelState.AddModelError("Content Id", "Provide a valid content id.");
                return BadRequest(ModelState);
            }

            var content = _repos.Contents
                                .GetWith(id, "Comments");

            if(content != null)
            {
                return Ok(content.Comments);
            }
            else
            {
                return NotFound();
            }
        }
        
        // Delete
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Admin, Lecturer")]
        public IActionResult Delete(int id)
        {
            if (id < 1)
            {
                ModelState.AddModelError("Content Id", "Provide a valid content id.");
                return BadRequest(ModelState);
            }

            var content = _repos.Contents
                                .GetWith(id, "Likes","Comments");

            if(content == null)
            {
                return this.Error(HttpStatusCode.PreconditionFailed, "Cannot delete this content since it does not exist.");
            }

            _repos.Contents.Remove(content);
            _repos.Commit();

            return Ok($"{content.Title} Deleted!");
        }


        // Like content
        [HttpPost]
        [Route("{id}/like")]
        [Authorize(Roles = "Student, Lecturer")]
        public async Task<IActionResult> LikeContent(int id, Reaction reaction)
        {
            if (id < 1)
            {
                ModelState.AddModelError("Content Id", "Provide a valid content id.");
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);
            var content = _repos.Contents
                                .GetWith(id, "Likes");

            if (content == null)
            {
                return this.Error(HttpStatusCode.PreconditionFailed, "Cannot like content that does not exist.");
            }
            var like = new Like()
            {
                Reaction = reaction,
            };
            
            like.By = GetProfile(user.AccountId, user.AccountType);

            //like = _repos.Likes.Create(like);
            content.Likes.Add(like);
            _repos.Contents.Update(content);
            _repos.Commit();

            return Ok("Liked!");
        }
        
        
        // Comment on content
        [HttpPost]
        [Route("{id}/comment")]
        public async Task<IActionResult> ContentComment(int id, CommentViewModel model)
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

            var user = await _userManager.GetUserAsync(User);
            var content = _repos.Contents
                                .GetWith(id, "Comments");

            if (content == null)
            {
                return this.Error(HttpStatusCode.PreconditionFailed, "Cannot comment on content that does not exist.");
            }
            Comment comment = _mapper.Map<Comment>(model);
            
            comment.By = GetProfile(user.AccountId, user.AccountType);

            try
            {
                comment = _repos.Comments.Create(comment);
                content.Comments.Add(comment);
                _repos.Contents.Update(content);
                _repos.Commit();
            }
            catch (Exception)
            {
                return this.Error(HttpStatusCode.InternalServerError, "An error occured.");
            }

            return Ok("Comment Posted!");
        }


        #region Private Section
        private DAL.Models.Profile GetProfile(int accountId, AccountType type)
        {
            DAL.Models.Profile profile = null;

            switch (type)
            {
                case AccountType.Administrator:
                    profile = _repos.Administrators.GetWith(accountId, "Profile")?.Profile;
                    break;
                case AccountType.Lecturer:
                    profile = _repos.Lecturers.GetWith(accountId, "Profile")?.Profile;
                    break;
                case AccountType.Student:
                    profile = _repos.Students.GetWith(accountId, "Profile")?.Profile;
                    break;
                case AccountType.None:
                    throw new ProfileUnsetException();
                default:
                    break;
            }

            return profile;
        }
        #endregion
    }
}
