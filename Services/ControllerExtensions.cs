using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

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
    }
}
