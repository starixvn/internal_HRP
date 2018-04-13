namespace HRP.Website.Controllers
{
    using System.Threading.Tasks;
    using Core.Criteria;
    using Core.Services;
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
            var isKickedOut = baseService.GetDataEntityAsync<bool>(criteria).Result;

            if (isKickedOut)
            {
                DoSignOut(context).Wait();
            }
        }

        private async Task DoSignOut(ActionExecutingContext context)
        {
            var criteria = new LogOutUserCriteria
            {
                OnUserId = sessionService.OnUserId,
                IP = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                Platform = context.HttpContext.Request.Headers["User-Agent"].ToString()
            };

            await baseService.ExecuteNonQueryAsync(criteria);

            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.HttpContext.Response.Redirect("/auth/login");
        }
    }
}