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
    using Criteria.Employee;
    using Resources;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using Models.Report;
    using Newtonsoft.Json;
    using Services;
    using Services.Interfaces;
    using Newtonsoft.Json.Serialization;
    using HRP.Criteria;

    public class ReportController : BaseController
    {
        private readonly IReportService reportService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IExportExcelService exportExcelService;
        private readonly ICommonService commonService;

        private const string PaymentTransactionOptionRootParentId = "0";

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

        [HRPPermission(FunctionKey = FunctionKey.EbankingReport, Policy = Policy.Read)]
        public async Task<IActionResult> GetEbankingPaymentTransactions()
        {
            var refResources = await commonService.GetReferenceResourcesByNamesAsync(new[]
            {
                ReferenceResource.PaymentTransactionEbanking
            });

            var paymentOptions = new List<MultiSelectViewModel>();
            paymentOptions.AddRange(refResources.Select(x => new MultiSelectViewModel
            {
                Value = $"{(int)BankTypes.Ebanking}-{x.ResourceId}",
                Text = x.Value
            }));

            return Json(paymentOptions);
        }

        [HRPPermission(FunctionKey = FunctionKey.DirectBankingReport, Policy = Policy.Read)]
        public async Task<IActionResult> GetDirectBankingPaymentTransactions()
        {
            var refResources = await commonService.GetReferenceResourcesByNamesAsync(new[]
            {
                ReferenceResource.PaymentTransactionDirectBanking,
                ReferenceResource.PaymentTransactionCash
            });

            var paymentOptions = new List<MultiSelectViewModel>();

            var filteredReferenceResources = commonService.FilterReferenceResources(
                refResources,
                ReferenceResource.PaymentTransactionDirectBanking);

            paymentOptions.Add(new MultiSelectViewModel
            {
                Value = ((int)BankTypes.DirectBanking).ToString(),
                Text = BankTypes.DirectBanking.GetDisplayName(),
                ParentId = PaymentTransactionOptionRootParentId,
                Items = filteredReferenceResources.Select(x => new MultiSelectViewModel
                {
                    Value = $"{(int)BankTypes.DirectBanking}-{x.ResourceId}",
                    Text = x.Value,
                    ParentId = ((int)BankTypes.DirectBanking).ToString()
                }).ToList()
            });

            filteredReferenceResources = commonService.FilterReferenceResources(
                refResources,
                ReferenceResource.PaymentTransactionCash);

            paymentOptions.Add(new MultiSelectViewModel
            {
                Value = ((int)BankTypes.Cash).ToString(),
                Text = BankTypes.Cash.GetDisplayName(),
                ParentId = PaymentTransactionOptionRootParentId,
                Items = filteredReferenceResources.Select(x => new MultiSelectViewModel
                {
                    Value = $"{(int)BankTypes.Cash}-{x.ResourceId}",
                    Text = x.Value,
                    ParentId = ((int)BankTypes.Cash).ToString()
                }).ToList()
            });

            return Json(paymentOptions);
        }

        [HRPPermission(FunctionKey = FunctionKey.DirectBankingReport, Policy = Policy.Read)]
        public async Task<IActionResult> DirectBanking()
        {
            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.DirectBankingReport);

            var viewModel = new BankInReportViewModel
            {
                Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
                SelectedTime = DateTime.Now,
                BankType = BankTypes.DirectBanking
            };

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DirectBankingReport, Policy = Policy.Read)]
        public async Task<IActionResult> GetDirectBankingReport(BankInReportViewModel viewModel)
        {
            var bankInData = await reportService.GetDirectBankingBankInAsync(
                viewModel.CompanyId,
                viewModel.SelectedTime,
                viewModel.TypeOfPayment,
                viewModel.SelectedPaymentTransactions);

            var fileInfo = new FileInfo($"{hostingEnvironment.WebRootPath}\\ExcelTemplates\\DirectBanking.xlsx");

            IActionResult result = RedirectToAction("DirectBanking");

            if (bankInData != null)
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

        [HRPPermission(FunctionKey = FunctionKey.EbankingReport, Policy = Policy.Read)]
        public async Task<IActionResult> Ebanking()
        {
            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.EbankingReport);

            var viewModel = new BankInReportViewModel
            {
                Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
                SelectedTime = DateTime.Now
            };

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.EbankingReport, Policy = Policy.Read)]
        public async Task<IActionResult> GetEbankingReport(BankInReportViewModel viewModel)
        {
            var bankInData = await reportService.GetEbankingBankInAsync(
                viewModel.CompanyId,
                viewModel.SelectedTime,
                viewModel.SelectedPaymentTransactions);

            var fileInfo = new FileInfo($"{hostingEnvironment.WebRootPath}\\ExcelTemplates\\Ebanking.xlsx");

            IActionResult result = RedirectToAction("Ebanking");

            if (bankInData != null)
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

        [HRPPermission(FunctionKey = FunctionKey.MonthlyPayrollReport, Policy = Policy.Read)]
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

        [HRPPermission(FunctionKey = FunctionKey.MonthlyPayrollReport, Policy = Policy.Read)]
        [HttpPost]
        public async Task<IActionResult> MonthlyPayroll(MonthlyPayrollReportViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var report = await reportService.GetDataForMonthlyPayrollReportAsync(viewModel.SelectedTime, viewModel.CompanyId);
                if (report.Employees.Any())
                {
                    var company = await baseService.GetDataEntityAsync<Company>(new GetCompanyByIdCriteria { CompanyId = viewModel.CompanyId }).ConfigureAwait(false);
                    report.Company = company;
                    string webRootFolder = hostingEnvironment.WebRootPath;
                    string fileName = @"ExcelTemplates\MonthlyPayrollReport.xlsx";
                    var templatePath = Path.Combine(webRootFolder, fileName);
                    var data = exportExcelService.ExportMonthlyPayrollReport(templatePath, report);

                    return File(data, MimeType.XLSX, $"MonthlyPayrollReport_{report.Month.ToString("MM-yyyy")}_{report.Company.Code}.xlsx");
                }

                ModelState.AddModelError(nameof(viewModel.CommonError), Message.PayrollNotGenerated);
            }

            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.Report);
            viewModel.Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));

            return View("MonthlyPayroll", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DynamicReport, Policy = Policy.Read)]
        public async Task<IActionResult> DynamicReport()
        {
            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.Report);
            var criteria = new GetEmployeeStatusByUserCriteria
            {
                UserId = sessionService.UserId
            };

            var employeeStatusIds = new List<string> { "1", "2" };
            var employeeStatuses = (await baseService.GetDataAsync<MultiSelectViewModel>(criteria)).ToList();
            employeeStatuses.ForEach(x =>
            {
                if (!employeeStatusIds.Any(i => i == x.Value))
                {
                    x.Checked = false;
                }
            });

            var viewModel = new GenerateReportViewModel
            {
                Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
                EmployeeStatuses = JsonConvert.SerializeObject(employeeStatuses, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }),
                EmployeeStatusIds = string.Join(",", employeeStatusIds),
                Groups = ColumnsConfirguration.DynamicReportGroups
            };


            return View("DynamicReport", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DynamicReport, Policy = Policy.Read)]
        [HttpPost]
        public async Task<IActionResult> DynamicReport(GenerateReportViewModel viewModel)
        {
            if (!sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.DynamicReport, Policy.Read))
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            if (ModelState.IsValid)
            {
                var employeeDatas = await reportService.GetEmployeeDataForDynamicReport(viewModel.CompanyId, viewModel.EmployeeStatusIds);

                var data = exportExcelService.ExportDynamicReport(employeeDatas, viewModel.SelectedIds.Split(',').Select(int.Parse));

                return File(data, MimeType.XLSX, $"DynamicReport_Company-{viewModel.CompanyId}.xlsx");
            }

            var companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.Report);
            viewModel.Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));

            return View("DynamicReport", viewModel);
        }
    }
}