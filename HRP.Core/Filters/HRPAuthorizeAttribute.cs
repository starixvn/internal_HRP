namespace HRP.Core.Filters
{
    using System;
    using System.Net;
    using Enumerations;
    using Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.AspNetCore.Authorization;

    public class HRPAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context?.HttpContext == null)
            {
                CloseConnection(context);
                return;
            }
            
            var currentSession = new SessionService(context.HttpContext.Session);
            
            if (currentSession.UserId <= 0 || !string.IsNullOrWhiteSpace(currentSession.Username))
            {
                CloseConnection(context);
            }
        }
        
        internal static void CloseConnection(AuthorizationFilterContext context)
        {
            var response = context.HttpContext.Response;
            var messageContent = "<center><b>Unauthorized</b></center>";
            response.StatusCode = (int) HttpStatusCode.Forbidden;
            response.WriteAsync(messageContent);
        }     
    }
}