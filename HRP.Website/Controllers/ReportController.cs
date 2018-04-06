namespace HRP.Website.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Filters;
    using Core.Services;
    using HRP.Models.Report;
    using HRP.Services;
    using Microsoft.AspNetCore.Mvc;

    [HRPAuthorize]
    public class ReportController : BaseController
    {
        private readonly IPaymentProcessService paymentProcessService;

        public ReportController(
            IBaseService baseService,
            ISessionService sessionService,
            IPaymentProcessService paymentProcessService) : base(baseService, sessionService)
        {
            this.paymentProcessService = paymentProcessService;
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public IActionResult BankIn()
        {
            return View();
        }

        public async Task<IActionResult> BasicSalaryDirectBankingAndCash()
        {
            var viewModel = new BasicReportViewModel
            {
                Companies = await paymentProcessService.GetCompanySelectListAsync(),
                Month = DateTime.Now
            };
            return View(viewModel);
        }
    }
}