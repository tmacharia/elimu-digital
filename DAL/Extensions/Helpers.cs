using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Linq;

namespace DAL.Extensions
{
    public static class Helpers
    {
        private const string avatar = "https://devtimmystorage.blob.core.windows.net/images/avatar-1577909_960_720.png";

        public static string PhotoUrl(this ClaimsPrincipal principal)
        {
            var claim = ((ClaimsIdentity)principal.Identity).FindFirst("PhotoUrl");

            if(claim != null)
            {
                if (string.IsNullOrWhiteSpace(claim.Value))
                {
                    return avatar;
                }
                else
                {
                    return claim.Value;
                }
            }
            else
            {
                return avatar;
            }
        }
        public static string FullNames(this ClaimsPrincipal principal)
        {
            var claim = ((ClaimsIdentity)principal.Identity).FindFirst("FullNames");

            if (claim != null)
            {
                if (string.IsNullOrWhiteSpace(claim.Value))
                {
                    return principal.Identity.Name.Split('@').First();
                }
                else
                {
                    return claim.Value;
                }
            }
            else
            {
                return principal.Identity.Name.Split('@').First();
            }
        }
        public static string Role(this ClaimsPrincipal principal)
        {
            var claim = ((ClaimsIdentity)principal.Identity).FindFirst("Role");

            if (claim != null)
            {
                if (string.IsNullOrWhiteSpace(claim.Value))
                {
                    return "Unknown";
                }
                else
                {
                    return claim.Value;
                }
            }
            else
            {
                return "Unknown";
            }
        }

    }
}
