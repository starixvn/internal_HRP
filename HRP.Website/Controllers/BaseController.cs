namespace HRP.Website.Controllers
{
    using Core.Services;
    using Microsoft.AspNetCore.Mvc;

    public class BaseController : Controller
    {
        protected readonly IBaseService baseService;
        protected readonly ISessionService sessionService;

        public BaseController(IBaseService baseService, ISessionService sessionService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
        }
    }
}