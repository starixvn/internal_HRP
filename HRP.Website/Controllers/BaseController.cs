namespace HRP.Website.Controllers
{
    using Core.Services;
    using HRP.Core.Filters;
    using Microsoft.AspNetCore.Mvc;

    [HRPAuthorize]
    [ServiceFilter(typeof(HRPActionFilterAttribute))]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
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