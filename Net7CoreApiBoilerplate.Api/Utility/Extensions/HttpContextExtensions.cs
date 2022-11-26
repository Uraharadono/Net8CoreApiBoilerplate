using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Net7CoreApiBoilerplate.Api.Utility.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetCurrentUserId(this HttpContext httpContext)
        {
            if (httpContext.User == null)
            {
                return string.Empty;
            }

            return httpContext.User.Claims.Single(claim => claim.Type == "uid").Value;
        }


        /* =========================================
         * Method below is meant to replace current way we fetch current user: 
         * ApplicationUser user = await _userManager
                .FindByIdAsync(User.FindFirst("uid")?.Value)
                .ConfigureAwait(false);
         * As it was doing database calls every time I needed current user.
         * So I added claim of GebruikerId to the token generation, and using this
         * method to fetch it.
         * ========================================= */
        public static long GetCurrentSomethingId(this HttpContext httpContext)
        {
            if (httpContext.User == null)
            {
                return 0;
            }

            return Convert.ToInt64(httpContext.User.Claims.Single(claim => claim.Type == "gid").Value);
        }
    }
}
