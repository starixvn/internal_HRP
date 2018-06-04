namespace HRP.Core.Filters
{
    using System;
    using HRP.Core.Criteria;
    using HRP.Core.Services;
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
                context.Result = new RedirectToActionResult("logout", "auth", null);
                return;
            }
            else if (!certificateService.IsGeneratedCertificate())
            {
                context.Result = new RedirectToActionResult("generate", "SystemSecurity", null);
                return;
            }
            else if (!certificateService.IsExistedCertificate())
            {
                context.Result = new RedirectToActionResult("FileNotFound", "SystemSecurity", null);
                return;
            }
            else
            {
                string password = certificateService.GetPasswordAccessTheCertificate();

                if (string.IsNullOrEmpty(password) || !certificateService.IsCorrectPasswordAccessTheCertificate(password))
                {
                    context.Result = new RedirectToActionResult("password", "SystemSecurity", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}