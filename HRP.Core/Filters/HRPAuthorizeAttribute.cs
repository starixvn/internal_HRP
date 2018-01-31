namespace HRP.Core.Filters
{
    using System;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Services;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HRPAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context?.HttpContext == null)
            {
                return;
            }

            var currentSession = new SessionService(context.HttpContext.Session);

            if (currentSession.UserId <= 0 || string.IsNullOrWhiteSpace(currentSession.Username))
            {
                context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
                context.HttpContext.Response.Redirect("/auth/login");
            }
        }
    }
}