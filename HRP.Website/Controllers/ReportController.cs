namespace HRP.Website.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Filters;
    using Core.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Models.Report;
    using Services.Interfaces;
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
            var viewModel = new BankInReportViewModel
            {
                Companies = await paymentProcessService.GetCompanySelectListAsync(),
                SelectedTime = DateTime.Now
            };

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public async Task<IActionResult> GetBankInReport(BankInReportViewModel viewModel)
        {
            var bankInData = await reportService.GetBankInAsync(viewModel.CompanyId, viewModel.SelectedTime);

            FileInfo fileInfo = new FileInfo(hostingEnvironment.ContentRootPath + "\\ExcelTemplates\\BankIn.xlsx");
            FileContentResult result = null;

            if (bankInData.Any())
            {
                var fileContent = exportExcelService.ExportBankInReports(
                    viewModel.SelectedTime,
                    fileInfo,
                    bankInData);

                result = File(fileContent, MimeType.XLSX, $"BankIn {viewModel.SelectedTime:MMMM yyyy}.xlsx");
            }

            return result;
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

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public async Task<IActionResult> GenerateReport()
        {
            var viewModel = new GenerateReportViewModel
            {
                SelectedTime = DateTime.Today,
                Companies = await reportService.GetCompanySelectListAsync()
            };

            return View("GenerateReport", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        [HttpPost]
        public async Task<IActionResult> GenerateReport(GenerateReportViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                return Content("ahihi");
            }

            viewModel.Companies = await reportService.GetCompanySelectListAsync();
            return View("GenerateReport", viewModel);
        }
    }
}