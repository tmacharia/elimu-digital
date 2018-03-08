using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Linq;
using DAL.Models;
using DAL.Contexts;
using DAL.Attributes;

namespace DAL.Extensions
{
    public static class Helpers
    {
        private const string online = "https://devtimmystorage.blob.core.windows.net/images/avatar-1577909_960_720.png";
        private const string local = "~/images/avatar-1577909_960_720.png";

        private static string avatar
        {
            get
            {
                return local;
            }
        }

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
        public static int ProfileId(this ClaimsPrincipal principal)
        {
            var claim = ((ClaimsIdentity)principal.Identity).FindFirst("ProfileId");

            if (claim != null)
            {
                if (string.IsNullOrWhiteSpace(claim.Value))
                {
                    return 0;
                }
                else
                {
                    return int.Parse(claim.Value);
                }
            }
            else
            {
                return 0;
            }
        }

        public static Meridiem Meridiem(this DateTime dateTime)
        {
            string tt = dateTime.ToString("tt").ToUpper();

            if(tt == "AM")
            {
                return Attributes.Meridiem.AM;
            }
            else if(tt == "PM")
            {
                return Attributes.Meridiem.PM;
            }
            else
            {
                return Attributes.Meridiem.AM;
            }
        }
    }
}
