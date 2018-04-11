namespace HRP.Website.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Filters;
    using Core.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Models.Report;
    using Services;

    [HRPAuthorize]
    public class ReportController : BaseController
    {
        private readonly IPaymentProcessService paymentProcessService;
        private readonly IReportService reportService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IExportExcelService exportExcelService;

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
            this.hostingEnvironment = hostingEnvironment;
            this.exportExcelService = exportExcelService;
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
        [ValidateAntiForgeryToken]
        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public async Task<IActionResult> BankIn(BasicReportViewModel viewModel)
        {
            var fileContent = await reportService.ExportBankInAsync(
                viewModel.CompanyId,
                viewModel.SelectedTime,
                hostingEnvironment.ContentRootPath + "\\ExcelTemplates\\4-KLBankAndCash.xlsx");

            return File(fileContent,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "4-KLBankAndCash.xlsx");
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

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public IActionResult PayDetails()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"ExcelTemplates\1. Pay Details.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            var templateStream = new FileStream(path, FileMode.Open);

            var stream = exportExcelService.GetPayDetails(templateStream);

            return File(stream, MimeType.XLSX, Path.GetFileName(path));
        }
    }
}