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

            if(authToken.Count < 1)
            {
                context.Response.Headers.Add("Authorization", "Failed");
            }
            else
            {
                // Authorization: Basic
                if (authToken[0].StartsWith("Basic"))
                {
                    context.Response.Headers.Add("Authorization", "Pending...");
                }
                // Authorization: Bearer
                if (authToken[0].StartsWith("Bearer"))
                {
                    context.Response.Headers.Add("Authorization", "Verified");
                }
            }

            // call the next middleware delegate in the pipeline
            await _next.Invoke(context);
        }
    }
}
