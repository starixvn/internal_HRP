namespace HRP.Website.Controllers
{
    using Core.Constants;
    using Core.Enumerations;
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

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public IActionResult BankIn()
        {
            return View();
        }
    }
}