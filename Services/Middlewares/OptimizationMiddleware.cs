using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using Services.Middlewares;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Services.Middlewares
{
    public class OptimizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;
        private IServiceProvider provider;

        public OptimizationMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            provider = context.RequestServices;

            if (context.User.Identity.IsAuthenticated)
            {
                var cacheOptions = new DistributedCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromSeconds(10)
                };
                // check account
                int account = 0;

                string s = string.Empty;

                byte[] accBytes = await _cache.GetAsync("AccountId");

                if (accBytes != null)
                {
                    s = new UnicodeEncoding().GetString(accBytes);
                }

                if (!string.IsNullOrWhiteSpace(s))
                {
                    account = int.Parse(s);
                }

                if (account < 1)
                {
                    // get user manager from DI container
                    UserManager<AppUser> _userManager = (UserManager<AppUser>)provider.GetService(typeof(UserManager<AppUser>));
                    AppUser user = await _userManager.GetUserAsync(context.User);
                    account = user.AccountId;

                    // store account id to session
                    await _cache.SetAsync("AccountId", new UnicodeEncoding().GetBytes(account.ToString()), cacheOptions);
                }

                // get repository factory from DI container
                IRepositoryFactory _repos = (IRepositoryFactory)provider.GetService(typeof(IRepositoryFactory));

                int count = _repos.Notifications.List.Count(x => x.AccountId == account && x.Read == false);

                await _cache.SetAsync("Notifications", new UnicodeEncoding().GetBytes(count.ToString()), cacheOptions);
            }

            // call the next middleware delegate in the pipeline
            await _next.Invoke(context);
        }
    }
}
