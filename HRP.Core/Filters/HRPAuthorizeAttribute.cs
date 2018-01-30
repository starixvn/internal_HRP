namespace HRP.Core.Filters
{
    using System;
    using Criteria;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Routing;
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

            var urlHelper = new UrlHelper(context);

            if (currentSession.UserId <= 0 || string.IsNullOrWhiteSpace(currentSession.Username))
            {
                context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)
                    .ConfigureAwait(false);
                context.HttpContext.Response.Redirect(urlHelper.Action("Login", "Auth"));
            }
            else
            {
                ReCheckIn(context, currentSession);
            }
        }

        private static void ReCheckIn(ActionContext context, ISessionService currentSession)
        {
            var baseService = new BaseService();
            var criteria = new ReCheckInUserCriteria
            {
                OnUserId = currentSession.OnUserId
            };

            var isKickedOut = baseService.GetDataEntity<bool>(criteria);

            if (isKickedOut)
            {
                DoSignOut(context, currentSession, baseService);
            }
        }

        private static void DoSignOut(ActionContext context, ISessionService currentSession, IBaseService baseService)
        {
            var criteria = new LogOutUserCriteria
            {
                OnUserId = currentSession.OnUserId,
                IP = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                Platform = context.HttpContext.Request.Headers["User-Agent"].ToString()
            };

            baseService.ExecuteNonQuery(criteria);

            var urlHelper = new UrlHelper(context);

            context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.HttpContext.Response.Redirect(urlHelper.Action("Login", "Auth"));
        }
    }
}