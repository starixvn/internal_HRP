namespace HRP.Core.Filters
{
    using System;
    using System.Net;
    using Enumerations;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Services;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HRPPermissionAttribute : Attribute, IAuthorizationFilter
    {
        public string FunctionKey { get; set; }

        public Policy Policy { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context?.HttpContext == null)
            {
                return;
            }

            var currentSession = new SessionService(context.HttpContext.Session);

            if (currentSession.UserId <= 0 || !currentSession.HasPermissionOnFunction(FunctionKey, Policy))
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            }
        }
    }
}