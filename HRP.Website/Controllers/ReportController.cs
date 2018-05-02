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
    using HRP.Resources;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Report;
    using Services;
    using Services.Interfaces;

    public class ReportController : BaseController
    {
        private readonly IReportService reportService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IExportExcelService exportExcelService;
        private readonly ICommonService commonService;

        public ReportController(
            IBaseService baseService,
            ISessionService sessionService,
            IHostingEnvironment hostingEnvironment,
            IExportExcelService exportExcelService,
            IReportService reportService,
            ICommonService commonService) : base(baseService, sessionService)
        {
            this.reportService = reportService;
            this.hostingEnvironment = hostingEnvironment;
            this.exportExcelService = exportExcelService;
            this.commonService = commonService;
        }

        public IActionResult PaymentTransaction()
        {
            return View();
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public async Task<IActionResult> GetPaymentTransactions(BankTypes banktype)
        {
            var refResources = await commonService.GetReferenceResourcesByNamesAsync(new[]
            {
                ReferenceResource.PaymentTransactionDirectBanking,
                ReferenceResource.PaymentTransactionEbanking,
                ReferenceResource.PaymentTransactionCash
            });

            var paymentOptions = new List<MultiSelectViewModel>();

            if (banktype == BankTypes.Ebanking)
            {
                paymentOptions.AddRange(commonService.FilterReferenceResources(
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
                    Items = commonService.FilterReferenceResources(
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
                    Items = commonService.FilterReferenceResources(
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
            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.Report);

            var viewModel = new BankInReportViewModel
            {
                Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
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

            var fileInfo = new FileInfo($"{hostingEnvironment.WebRootPath}\\ExcelTemplates\\DirectBanking.xlsx");

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
            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.Report);

            var viewModel = new BankInReportViewModel
            {
                Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
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

            var fileInfo = new FileInfo($"{hostingEnvironment.WebRootPath}\\ExcelTemplates\\Ebanking.xlsx");

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
        public async Task<IActionResult> MonthlyPayroll()
        {
            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.Report);
            var viewModel = new MonthlyPayrollReportViewModel
            {
                SelectedTime = DateTime.Today,
                Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
            };

            return View("MonthlyPayroll", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        [HttpPost]
        public async Task<IActionResult> MonthlyPayroll(MonthlyPayrollReportViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var report = await reportService.GetDataForMonthlyPayrollReportAsync(viewModel.SelectedTime, viewModel.CompanyId);
                if (report.Employees.Any())
                {
                    string webRootFolder = hostingEnvironment.WebRootPath;
                    string fileName = @"ExcelTemplates\MonthlyPayrollReport.xlsx";
                    var templatePath = Path.Combine(webRootFolder, fileName);
                    var data = exportExcelService.ExportMonthlyPayrollReport(templatePath, report);

                    return File(data, MimeType.XLSX, $"MonthlyPayrollReport_{report.Month.ToString("MM-yyyy")}_{report.CompanyCode}.xlsx");
                }

                ModelState.AddModelError(nameof(viewModel.CommonError), Message.PayrollNotGenerated);
            }

            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.Report);
            viewModel.Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));

            return View("MonthlyPayroll", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        public async Task<IActionResult> DynamicReport()
        {
            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.Report);
            var viewModel = new GenerateReportViewModel
            {
                SelectedTime = DateTime.Today,
                Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
            };

            return View("DynamicReport", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.Report, Policy = Policy.Read)]
        [HttpPost]
        public async Task<IActionResult> DynamicReport(GenerateReportViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var employeeDatas = await baseService.GetDataAsync<DynamicReportModel>(new GetEmployeeDataForDynamicReportCriteria { CompanyId = viewModel.CompanyId, Month = viewModel.SelectedTime });

                var data = exportExcelService.ExportDynamicReport(employeeDatas, viewModel.SelectedIds);

                return File(data, MimeType.XLSX, $"Report_{viewModel.SelectedTime.ToString("MM-yyyy")}_Company-{viewModel.CompanyId}.xlsx");
            }

            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.Report);
            viewModel.Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));

            return View("DynamicReport", viewModel);
        }
    }
}