namespace HRP.Core.Filters
{
    using System;
    using System.Threading.Tasks;
    using HRP.Core.Criteria;
    using HRP.Core.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HRPActionFilterAttribute : ActionFilterAttribute
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly ICertificateService certificateService;

        public HRPActionFilterAttribute(
            IBaseService baseService,
            ISessionService sessionService,
            ICertificateService certificateService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
            this.certificateService = certificateService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var criteria = new ReCheckInUserCriteria
            {
                OnUserId = sessionService.OnUserId
            };
            var isKickedOut = baseService.GetDataEntityAsync<bool>(criteria).Result;

            if (isKickedOut)
            {
                DoSignOut(context).Wait();
                return;
            }
            else if (!certificateService.IsGeneratedCertificate())
            {
                context.Result = new RedirectResult("/SystemSecurity/generate");
                return;
            }
            else if (!certificateService.IsExistedCertificate())
            {
                context.Result = new RedirectResult("/SystemSecurity/FileNotFound");
                return;
            }
            else
            {
                string password = certificateService.GetPasswordAccessTheCertificate();

                if (string.IsNullOrEmpty(password) || !certificateService.IsCorrectPasswordAccessTheCertificate(password))
                {
                    context.Result = new RedirectResult("/SystemSecurity/password");
                    return;
                }
            }

            base.OnActionExecuting(context);
        }

        private async Task DoSignOut(ActionExecutingContext context)
        {
            var criteria = new LogOutUserCriteria
            {
                OnUserId = sessionService.OnUserId,
                IP = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                Platform = context.HttpContext.Request.Headers["User-Agent"].ToString()
            };

            await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            context.Result = new RedirectResult("/auth/login");
        }
    }
}