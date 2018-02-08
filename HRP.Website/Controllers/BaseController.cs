namespace HRP.Website.Controllers
{
    using HRP.Core.Criteria;
    using HRP.Core.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class BaseController : Controller
    {
        protected readonly IBaseService baseService;
        protected readonly ISessionService sessionService;

        public BaseController(IBaseService baseService, ISessionService sessionService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var criteria = new ReCheckInUserCriteria
            {
                OnUserId = sessionService.OnUserId
            };
            var isKickedOut = baseService.GetDataEntity<bool>(criteria);

            if (isKickedOut)
            {
                DoSignOut(context);
            }
        }

        private void DoSignOut(ActionExecutingContext context)
        {
            var criteria = new LogOutUserCriteria
            {
                OnUserId = sessionService.OnUserId,
                IP = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                Platform = context.HttpContext.Request.Headers["User-Agent"].ToString()
            };
            baseService.ExecuteNonQuery(criteria);

            context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.HttpContext.Response.Redirect("/auth/login");
        }
    }
}