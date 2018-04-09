namespace HRP.Website.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Filters;
    using Core.Services;
    using Microsoft.AspNetCore.Mvc;
    using Models.Report;
    using Services;

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
        public async Task<IActionResult> BankIn()
        {
            var viewModel = new BasicReportViewModel
            {
                Companies = await paymentProcessService.GetCompanySelectListAsync(),
                SelectedTime = DateTime.Now
            };

            return View(viewModel);
        }

        public async Task<IActionResult> BasicSalaryDirectBankingAndCash()
        {
            var viewModel = new BasicReportViewModel
            {
                SelectedTime = DateTime.Now,
                Companies = await paymentProcessService.GetCompanySelectListAsync()
            };
            return View(viewModel);
        }
    }
}