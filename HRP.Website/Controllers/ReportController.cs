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
    using Microsoft.AspNetCore.Hosting;

    [HRPAuthorize]
    public class ReportController : BaseController
    {
        private readonly IPaymentProcessService paymentProcessService;
        private readonly IReportService reportService;
        private readonly IHostingEnvironment hostingEnvironment;

        public ReportController(
            IBaseService baseService,
            ISessionService sessionService,
            IPaymentProcessService paymentProcessService,
            IHostingEnvironment hostingEnvironment,
            IReportService reportService) : base(baseService, sessionService)
        {
            this.paymentProcessService = paymentProcessService;
            this.reportService = reportService;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public async Task<IActionResult> BankIn()
        {
            var viewModel = new BasicReportViewModel
            {
                Companies = await paymentProcessService.GetCompanySelectListAsync(),
                Month = DateTime.Now
            };

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public IActionResult BankIn(BasicReportViewModel viewModel)
        {
            reportService.FileName = "4-KLBankAndCash";
            reportService.SheetName = "KL Bank & Cash";

            reportService.InitializeWithTemplate(hostingEnvironment.ContentRootPath +
                                                 "/ExcelTempates/4-KLBankAndCash.xlsx");

            reportService.ExportWithTemplate(null, HttpContext.Response);

            return new EmptyResult();
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