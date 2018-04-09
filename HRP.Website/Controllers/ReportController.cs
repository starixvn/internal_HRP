namespace HRP.Website.Controllers
{
    using System;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Filters;
    using Core.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Models.Report;
    using Services;
    using System.Threading.Tasks;
    using Services.Interfaces;

    [HRPAuthorize]
    public class ReportController : BaseController
    {
        private readonly IPaymentProcessService paymentProcessService;
        private readonly IReportService reportService;
        private readonly IExportExcelService exportExcelService;
        private readonly IHostingEnvironment hostingEnvironment;

        public ReportController(
            IBaseService baseService,
            ISessionService sessionService,
            IPaymentProcessService paymentProcessService,
            IHostingEnvironment hostingEnvironment,
            IExportExcelService exportExcelService,
            IReportService reportService) : base(baseService, sessionService)
        {
            this.paymentProcessService = paymentProcessService;
            this.reportService = reportService;
            this.exportExcelService = exportExcelService;
            this.hostingEnvironment = hostingEnvironment;
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

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public async Task<IActionResult> BankIn(BasicReportViewModel viewModel)
        {
            exportExcelService.FileName = "4-KLBankAndCash";
            exportExcelService.SheetName = "KL Bank & Cash";

            exportExcelService.InitializeWithTemplate(hostingEnvironment.ContentRootPath +
                                                 "\\ExcelTemplates\\4-KLBankAndCash.xlsx");

            var bankInRecords = await reportService.GetBankInAsync(viewModel.CompanyId, viewModel.SelectedTime);

            exportExcelService.ExportWithTemplate(bankInRecords, HttpContext.Response);

            return new EmptyResult();
        }

        public async Task<IActionResult> BasicSalaryDirectBankingAndCash()
        {
            var viewModel = new BasicReportViewModel
            {
                Companies = await paymentProcessService.GetCompanySelectListAsync(),
                SelectedTime = DateTime.Now
            };
            return View(viewModel);
        }
    }
}