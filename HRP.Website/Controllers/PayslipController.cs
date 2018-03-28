namespace HRP.Website.Controllers
{
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using Microsoft.AspNetCore.Mvc;

    [HRPAuthorize]
    public class PayslipController : BaseController
    {
        public PayslipController(
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