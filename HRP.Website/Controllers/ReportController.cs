namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Extensions;
    using Core.Filters;
    using Core.Services;
    using Criteria.Report;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc;
    using Models.Report;
    using Models;
    using Services.Interfaces;
    using Services;

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
        public async Task<IActionResult> GetPaymentTransactions(BankTypes banktype)
        {
            var refResources = await refResourceService.GetReferenceResourcesByNamesAsync(new[]
            {
                ReferenceResource.PaymentTransactionDirectBanking,
                ReferenceResource.PaymentTransactionEbanking,
                ReferenceResource.PaymentTransactionCash
            });

            var paymentOptions = new List<MultiSelectViewModel>();

            if (banktype == BankTypes.Ebanking)
            {
                paymentOptions.AddRange(refResourceService.FilterReferenceResources(
                    refResources,
                    ReferenceResource.PaymentTransactionEbanking).Select(x => new MultiSelectViewModel
                {
                    Value = $"{(int)BankTypes.Ebanking}-{x.ResourceId}",
                    Text = x.Value
                }));
            }

            if (banktype == BankTypes.DirectBanking)
            {
                paymentOptions.Add(new MultiSelectViewModel
                {
                    Value = ((int)BankTypes.DirectBanking).ToString(),
                    Text = BankTypes.DirectBanking.GetDisplayName(),
                    ParentId = "0",
                    Items = refResourceService.FilterReferenceResources(
                        refResources,
                        ReferenceResource.PaymentTransactionDirectBanking).Select(x => new MultiSelectViewModel
                    {
                        Value = $"{(int)BankTypes.DirectBanking}-{x.ResourceId}",
                        Text = x.Value,
                        ParentId = ((int)BankTypes.DirectBanking).ToString()
                    }).ToList()
                });

                paymentOptions.Add(new MultiSelectViewModel
                {
                    Value = "3",
                    Text = "Cash",
                    ParentId = "0",
                    Items = refResourceService.FilterReferenceResources(
                        refResources,
                        ReferenceResource.PaymentTransactionCash).Select(x => new MultiSelectViewModel
                    {
                        Value = $"{3}-{x.ResourceId}",
                        Text = x.Value,
                        ParentId = "3"
                    }).ToList()
                });
            }

            return Json(paymentOptions);
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public async Task<IActionResult> DirectBanking()
        {
            var companies = await reportService.GetCompaniesAsync();

            var viewModel = new BankInReportViewModel
            {
                Companies = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
                SelectedTime = DateTime.Now,
                BankType = BankTypes.DirectBanking
            };

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public async Task<IActionResult> GetDirectBankingReport(BankInReportViewModel viewModel)
        {
            var bankInData = await reportService.GetBankInAsync(
                viewModel.CompanyId,
                viewModel.SelectedTime,
                viewModel.TypeOfPayment,
                viewModel.SelectedPaymentTransactions);

            FileInfo fileInfo = new FileInfo($"{hostingEnvironment.WebRootPath}\\ExcelTemplates\\DirectBanking.xlsx");

            IActionResult result = RedirectToAction("DirectBanking");

            if (bankInData != null && bankInData.IsExportable)
            {
                var fileContent = exportExcelService.ExportDirectBankingReports(
                    viewModel.SelectedTime,
                    fileInfo,
                    viewModel.TypeOfPayment,
                    bankInData);

                result = File(fileContent, MimeType.XLSX, $"{viewModel.SelectedTime:yyyyMM}_DirectBankingDetails.xlsx");
            }

            return result;
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public async Task<IActionResult> Ebanking()
        {
            var companies = await reportService.GetCompaniesAsync();

            var viewModel = new BankInReportViewModel
            {
                Companies = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
                SelectedTime = DateTime.Now,
                BankType = BankTypes.Ebanking
            };

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public async Task<IActionResult> GetEbankingReport(BankInReportViewModel viewModel)
        {
            var bankInData = await reportService.GetBankInAsync(
                viewModel.CompanyId,
                viewModel.SelectedTime,
                viewModel.TypeOfPayment,
                viewModel.SelectedPaymentTransactions);

            FileInfo fileInfo = new FileInfo($"{hostingEnvironment.WebRootPath}\\ExcelTemplates\\Ebanking.xlsx");

            IActionResult result = RedirectToAction("Ebanking");

            if (bankInData != null && bankInData.IsExportable)
            {
                var fileContent = exportExcelService.ExportEbankingReports(
                    viewModel.SelectedTime,
                    fileInfo,
                    viewModel.TypeOfPayment,
                    bankInData);

                result = File(fileContent, MimeType.XLSX, $"{viewModel.SelectedTime:yyyyMM}_EbankingDetails.xlsx");
            }

            return result;
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public async Task<IActionResult> GetMonthlyPayrollReport(DateTime month, int companyId)
        {
            string webRootFolder = hostingEnvironment.WebRootPath;
            string fileName = @"ExcelTemplates\MonthlyPayrollReport.xlsx";
            var templatePath = Path.Combine(webRootFolder, fileName);
            var report = await reportService.GetDataForMonthlyPayrollReportAsync(month, companyId);
            var data = exportExcelService.ExportMonthlyPayrollReport(templatePath, report);

            return File(data, MimeType.XLSX, $"MonthlyPayrollReport_{month.ToString("MM-yyyy")}_{companyId}.xlsx");
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public async Task<IActionResult> GenerateReport()
        {
            var companies = await reportService.GetCompaniesAsync();
            var viewModel = new GenerateReportViewModel
            {
                SelectedTime = DateTime.Today,
                Companies = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
            };

            return View("GenerateReport", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        [HttpPost]
        public async Task<IActionResult> GenerateReport(GenerateReportViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var employeeDatas = await baseService.GetDataAsync<DynamicReportModel>(new GetEmployeeDataForDynamicReportCriteria { CompanyId = viewModel.CompanyId, Month = viewModel.SelectedTime });

                var data = exportExcelService.ExportDynamicReport(employeeDatas, viewModel.SelectedIds);

                return File(data, MimeType.XLSX, $"Report_{viewModel.SelectedTime.ToString("MM-yyyy")}_Company-{viewModel.CompanyId}.xlsx");
            }

            var companies = await reportService.GetCompaniesAsync();
            viewModel.Companies = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));

            return View("GenerateReport", viewModel);
        }
    }
}