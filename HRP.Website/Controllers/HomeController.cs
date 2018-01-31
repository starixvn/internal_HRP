namespace HRP.Website.Controllers
{
    using Core.Filters;
    using HRP.Core.Services;
    using Microsoft.AspNetCore.Mvc;

    [HRPAuthorize]
    public class HomeController : BaseController
    {
        public HomeController(
            ISessionService sessionService,
            IBaseService baseService) : base(baseService, sessionService)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}