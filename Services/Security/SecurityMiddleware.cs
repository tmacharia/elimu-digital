using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Security
{
    public class SecurityMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Check request headers
            IHeaderDictionary headers = context.Request.Headers;
            headers.TryGetValue("Authorization", out StringValues authToken);

            context.Response.Headers.Add("Ujinga", authToken[0]);

            // Authorization: Basic

            // Authorization: Bearer

            // call the next middleware delegate in the pipeline
            await _next.Invoke(context);
        }
    }
}
