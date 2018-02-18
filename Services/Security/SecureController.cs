using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Security
{
    public abstract class SecureController : ControllerBase
    {
        public IHttpContextAccessor _contextAccessor;

        protected SecureController()
        {
            Run();
        }
        public void Run()
        {
            
        }
    }
}
