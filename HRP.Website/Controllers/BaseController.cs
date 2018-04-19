namespace HRP.Website.Controllers
{
    using Core.Services;
    using HRP.Core.Filters;
    using Microsoft.AspNetCore.Mvc;

    [ServiceFilter(typeof(HRPActionFilterAttribute))]
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