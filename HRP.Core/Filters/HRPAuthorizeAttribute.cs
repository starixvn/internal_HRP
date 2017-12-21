namespace HRP.Core.Filters
{
    using System;
    using HRP.Core.Criteria;
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
            else
            {
                ReCheckIn(context, currentSession);
            }
        }

        private void ReCheckIn(AuthorizationFilterContext context, SessionService currentSession)
        {
            var baseService = new BaseService();
            var criteria = new ReCheckInUserCriteria
            {
                OnUserId = currentSession.OnUserId
            };

            baseService.ExecuteNonQuery(criteria);

            if (criteria.IsKickedOut)
            {
                DoSignOut(context, currentSession, baseService);
            }
        }

        private void DoSignOut(
            AuthorizationFilterContext context,
            SessionService currentSession,
            BaseService baseService)
        {
            var criteria = new LogOutUserCriteria
            {
                OnUserId = currentSession.OnUserId,
                IP = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                Platform = context.HttpContext.Request.Headers["User-Agent"].ToString()
            };
            baseService.ExecuteNonQuery(criteria);

            context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.HttpContext.Response.Redirect("/auth/login");
        }
    }
}