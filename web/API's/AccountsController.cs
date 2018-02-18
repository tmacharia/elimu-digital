using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.API_s
{
    [Route("api/accounts")]
    public class AccountsController : Controller
    {
        public AccountsController()
        {

        }
        // authorize
        [HttpPost]
        [Route("authorize")]
        [AllowAnonymous]
        public IActionResult Authorize()
        {
            return Ok();
        }
        // sign-up
        // login
    }
}
