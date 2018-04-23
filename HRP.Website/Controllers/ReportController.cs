namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Filters;
    using Core.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using Models.Report;
    using Services;
    using Services.Interfaces;

    public class ReportController : BaseController
    {
        private readonly IReportService reportService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IExportExcelService exportExcelService;
        private readonly IRefResourceService refResourceService;

        public ReportController(
            IBaseService baseService,
            ISessionService sessionService,
            IHostingEnvironment hostingEnvironment,
            IExportExcelService exportExcelService,
            IReportService reportService,
            IRefResourceService refResourceService) : base(baseService, sessionService)
        {
            this.reportService = reportService;
            this.hostingEnvironment = hostingEnvironment;
            this.exportExcelService = exportExcelService;
            this.refResourceService = refResourceService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public async Task<IActionResult> GetPaymentTransactions()
        {
            var refResources = await refResourceService.GetReferenceResourcesByNamesAsync(new[]
            {
                ReferenceResource.PaymentTransactionDirectBanking,
                ReferenceResource.PaymentTransactionEbanking,
                ReferenceResource.PaymentTransactionCash
            });

            var paymentOptions = new List<MultiSelectViewModel>();

            paymentOptions.AddRange(refResourceService.FilterReferenceResources(
                refResources,
                ReferenceResource.PaymentTransactionDirectBanking).Select(x => new MultiSelectViewModel
                {
                Value = $"{(int)BankTypes.DirectBanking}-{x.ResourceId}",
                Text = x.Value
            }));
            paymentOptions.AddRange(refResourceService.FilterReferenceResources(
                refResources,
                ReferenceResource.PaymentTransactionEbanking).Select(x => new MultiSelectViewModel
                {
                Value = $"{(int)BankTypes.Ebanking}-{x.ResourceId}",
                Text = x.Value
            }));
            paymentOptions.AddRange(refResourceService.FilterReferenceResources(
                refResources,
                ReferenceResource.PaymentTransactionCash).Select(x => new MultiSelectViewModel
                {
                Value = $"{3}-{x.ResourceId}",
                Text = x.Value
            }));

            return Json(paymentOptions);
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public async Task<IActionResult> BankIn()
        {
            var companies = await reportService.GetCompaniesAsync();

            var viewModel = new BankInReportViewModel
            {
                Companies = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
                SelectedTime = DateTime.Now
            };

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public async Task<IActionResult> GetBankInReport(BankInReportViewModel viewModel)
        {
            var bankInData = await reportService.GetBankInAsync(viewModel.CompanyId, viewModel.SelectedTime);

            FileInfo fileInfo = new FileInfo($"{hostingEnvironment.WebRootPath}\\ExcelTemplates\\BankIn.xlsx");

            IActionResult result = RedirectToAction("BankIn");

            if (bankInData != null && bankInData.IsExportable)
            {
                var fileContent = exportExcelService.ExportBankInReports(
                    viewModel.SelectedTime,
                    fileInfo,
                    bankInData);

                result = File(fileContent, MimeType.XLSX, $"{viewModel.SelectedTime:yyyyMM}_BankingDetails.xlsx");
            }

            return result;
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
            var companies = await reportService.GetCompaniesAsync();
            var columns = reportService.GetAvailableColumnsList();
            var viewModel = new GenerateReportViewModel
            {
                SelectedTime = DateTime.Today,
                Companies = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
                ColumnGroups = new SelectList(columns, nameof(ColumnGroup.GroupId), nameof(ColumnGroup.GroupName))
            };

            return View("GenerateReport", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        [HttpPost]
        public async Task<IActionResult> GenerateReport(GenerateReportViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var records = await reportService.GetDataForDynamicReportAsync(viewModel);
                var stream = exportExcelService.ExportDynamicReport(records, viewModel);

                return File(stream, MimeType.XLSX, "Dynamic Report.xlsx");
            }

            var companies = await reportService.GetCompaniesAsync();
            viewModel.Companies = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));

            return View("GenerateReport", viewModel);
        }
    }
}