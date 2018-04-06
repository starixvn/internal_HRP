namespace HRP.Website.Controllers
{
    using Core.Filters;
    using Core.Services;
    using Microsoft.AspNetCore.Mvc;

    [HRPAuthorize]
    public class ReportController : BaseController
    {
        public ReportController(
            IBaseService baseService,
            ISessionService sessionService) : base(baseService, sessionService)
        {
        }

        public IActionResult BankIn()
        {
            return View();
        }
    }
}