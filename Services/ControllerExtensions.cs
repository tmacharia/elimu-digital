using DAL.Extensions;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// Extension methods to add to the controller base for all controllers used
    /// project to extend functionalities.
    /// </summary>
    public static class ControllerExtensions
    {
        public static LepadActionResult Error(this ControllerBase controller, HttpStatusCode code)
        {
            return new LepadActionResult(code);
        }

        public static LepadActionResult Error(this ControllerBase controller, HttpStatusCode code, string message)
        {
            return new LepadActionResult(code)
            {
                Data = message
            };
        }

        public static LepadActionResult Error(this ControllerBase controller, HttpStatusCode code, object obj)
        {
            return new LepadActionResult(code)
            {
                Data = obj
            };
        }
        public static LepadActionResult Success(this ControllerBase controller, object obj)
        {
            return new LepadActionResult(HttpStatusCode.OK)
            {
                Data = obj
            };
        }
        public static LepadActionResult Error(this ControllerBase controller, string message)
        {
            return new LepadActionResult(HttpStatusCode.InternalServerError)
            {
                Data = message
            };
        }
        public static LepadActionResult Error(this ControllerBase controller, object obj)
        {
            return new LepadActionResult(HttpStatusCode.InternalServerError)
            {
                Data = obj
            };
        }
        public static LepadActionResult UnAuthorized(this ControllerBase controller, string message)
        {
            return new LepadActionResult(HttpStatusCode.Unauthorized)
            {
                Data = message
            };
        }
        public static Profile GetMyProfile(this ControllerBase controller)
        {
            IServiceProvider provider = controller.HttpContext.RequestServices;

            IDataManager _dataManager = (IDataManager)provider.GetService(typeof(IDataManager));
            IRepositoryFactory _repos = (IRepositoryFactory)provider.GetService(typeof(IRepositoryFactory));

            int account = controller.GetAccountId();

            switch (controller.User.Role())
            {
                case "Administrator":
                    return _repos.Administrators
                                 .GetWith(account,
                                 "Profile")
                                 .Profile;
                case "Lecturer":
                    return _repos.Lecturers
                                 .GetWith(account,
                                 "Profile")
                                 .Profile;
                case "Student":
                    return _repos.Students
                                 .GetWith(account,
                                 "Profile")
                                 .Profile;
                default:
                    break;
            }

            return null;
        }
        public static IList<DiscussionBoard> GetMyBoards(this ControllerBase controller)
        {
            IServiceProvider provider = controller.HttpContext.RequestServices;

            IDataManager _dataManager = (IDataManager)provider.GetService(typeof(IDataManager));
            IRepositoryFactory _repos = (IRepositoryFactory)provider.GetService(typeof(IRepositoryFactory));

            int account = controller.GetAccountId();
            
            IList<DiscussionBoard> boards = new List<DiscussionBoard>();

            switch (controller.User.Role())
            {
                case "Administrator":
                    boards = _repos.DiscussionBoards
                                   .ListWith("Unit","Posts")
                                   .ToList();
                    break;
                case "Lecturer":
                    boards = _dataManager.MyBoards<Lecturer>(account);
                    break;
                case "Student":
                    boards = _dataManager.MyBoards<Student>(account);
                    break;
                default:
                    break;
            }

            return boards;
        }
        public static int GetNotifications(this ControllerBase controller)
        {
            IDistributedCache _cache = (IDistributedCache)controller.HttpContext
                                                                    .RequestServices
                                                                    .GetService(typeof(IDistributedCache));
            string s = string.Empty;
            byte[] bytes = _cache.Get("Notifications");
            
            if(bytes != null)
            {
                s = new UnicodeEncoding().GetString(bytes);
            }

            if (string.IsNullOrWhiteSpace(s))
                return 0;
            else
            {
                int n = int.Parse(s);

                controller.Response.Headers.Add("Notifications", n.ToString());
                return n;
            }
        }
        public static int GetAccountId(this ControllerBase controller)
        {
            IDistributedCache _cache = (IDistributedCache)controller.HttpContext
                                                                    .RequestServices
                                                                    .GetService(typeof(IDistributedCache));
            string s = string.Empty;
            byte[] bytes = _cache.Get("AccountId");

            if(bytes != null)
            {
                s = new UnicodeEncoding().GetString(bytes);
            }

            if (string.IsNullOrWhiteSpace(s))
                return 0;
            else
            {
                int n = int.Parse(s);

                return n;
            }
        }
    }
}
