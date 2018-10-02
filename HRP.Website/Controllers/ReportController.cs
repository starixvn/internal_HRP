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
    using Criteria;
    using Criteria.Employee;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Report;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Resources;
    using Services;

    public class ReportController : BaseController
    {
        private readonly IReportService reportService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IExportExcelService exportExcelService;
        private readonly ICommonService commonService;
        private readonly IRijndaelService rijndaelService;

        private const string PaymentTransactionOptionRootParentId = "0";

        public ReportController(
            IBaseService baseService,
            ISessionService sessionService,
            IHostingEnvironment hostingEnvironment,
            IExportExcelService exportExcelService,
            IReportService reportService,
            IRijndaelService rijndaelService,
            ICommonService commonService) : base(baseService, sessionService)
        {
            this.reportService = reportService;
            this.hostingEnvironment = hostingEnvironment;
            this.exportExcelService = exportExcelService;
            this.rijndaelService = rijndaelService;
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
            var filePassword = await reportService.GetUserFilePasswordAsync(sessionService.UserId);
            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.DirectBankingReport);

            Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair> tuple = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[tuple.Item1.Key] = tuple.Item1.Value;
            TempData[tuple.Item2.Key] = tuple.Item2.Value;

            var viewModel = new BankInReportViewModel
            {
                Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
                SelectedTime = DateTime.Now,
                BankType = BankTypes.DirectBanking,
                RijndaelKey = tuple.Item1,
                RijndaelIV = tuple.Item2,
                EncryptedPassword = rijndaelService.Encrypt(filePassword, tuple.Item1.Value, tuple.Item2.Value)
            };

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DirectBankingReport, Policy = Policy.Read)]
        public async Task<IActionResult> GetDirectBankingReport(BankInReportViewModel viewModel)
        {
            var rijndaelKey = (string)TempData.Peek(viewModel.RijndaelKey.Key);
            var rijndaelIV = (string)TempData.Peek(viewModel.RijndaelIV.Key);
            viewModel.FilePassword = rijndaelService.Decrypt(viewModel.EncryptedPassword, rijndaelKey, rijndaelIV);

            var bankInData = await reportService.GetDirectBankingBankInAsync(
                viewModel.CompanyId,
                viewModel.SelectedTime,
                viewModel.TypeOfPayment,
                viewModel.SelectedPaymentTransactions);

            var fileInfo = new FileInfo($"{hostingEnvironment.WebRootPath}\\ExcelTemplates\\DirectBanking.xlsx");

            ModelState.Clear();

            if (bankInData != null && TryValidateModel(viewModel))
            {
                if (viewModel.ReusePassword)
                {
                    await reportService.EditUserFilePasswordAsync(sessionService.UserId, viewModel.FilePassword);
                }

                var fileContent = exportExcelService.ExportDirectBankingReports(
                    viewModel.SelectedTime,
                    fileInfo,
                    viewModel.TypeOfPayment,
                    bankInData,
                    viewModel.FilePassword);

                return File(fileContent, MimeType.XLSX, $"Direct_Banking-Cash_Report_{viewModel.SelectedTime:MMyyyy}.xlsx");
            }

            return RedirectToAction("DirectBanking");
        }

        [HRPPermission(FunctionKey = FunctionKey.EbankingReport, Policy = Policy.Read)]
        public async Task<IActionResult> Ebanking()
        {
            var filePassword = await reportService.GetUserFilePasswordAsync(sessionService.UserId);
            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.EbankingReport);

            Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair> tuple = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[tuple.Item1.Key] = tuple.Item1.Value;
            TempData[tuple.Item2.Key] = tuple.Item2.Value;

            var viewModel = new BankInReportViewModel
            {
                Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
                SelectedTime = DateTime.Now,
                RijndaelKey = tuple.Item1,
                RijndaelIV = tuple.Item2,
                EncryptedPassword = rijndaelService.Encrypt(filePassword, tuple.Item1.Value, tuple.Item2.Value)
            };

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.EbankingReport, Policy = Policy.Read)]
        public async Task<IActionResult> GetEbankingReport(BankInReportViewModel viewModel)
        {
            var rijndaelKey = (string)TempData.Peek(viewModel.RijndaelKey.Key);
            var rijndaelIV = (string)TempData.Peek(viewModel.RijndaelIV.Key);
            viewModel.FilePassword = rijndaelService.Decrypt(viewModel.EncryptedPassword, rijndaelKey, rijndaelIV);

            var bankInData = await reportService.GetEbankingBankInAsync(
                viewModel.CompanyId,
                viewModel.SelectedTime,
                viewModel.SelectedPaymentTransactions);

            var fileInfo = new FileInfo($"{hostingEnvironment.WebRootPath}\\ExcelTemplates\\Ebanking.xlsx");

            IActionResult result = RedirectToAction("Ebanking");

            ModelState.Clear();

            if (bankInData != null && TryValidateModel(viewModel))
            {
                if (viewModel.ReusePassword)
                {
                    await reportService.EditUserFilePasswordAsync(sessionService.UserId, viewModel.FilePassword);
                }

                var fileContent = exportExcelService.ExportEbankingReports(
                    viewModel.SelectedTime,
                    fileInfo,
                    viewModel.TypeOfPayment,
                    bankInData,
                    viewModel.FilePassword);

                result = File(fileContent, MimeType.XLSX, $"E-Banking_Report_{viewModel.SelectedTime:MMyyyy}.xlsx");
            }

            return result;
        }

        [HRPPermission(FunctionKey = FunctionKey.MonthlyPayrollReport, Policy = Policy.Read)]
        public async Task<IActionResult> MonthlyPayroll()
        {
            Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair> tuple = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[tuple.Item1.Key] = tuple.Item1.Value;
            TempData[tuple.Item2.Key] = tuple.Item2.Value;

            var filePassword = await reportService.GetUserFilePasswordAsync(sessionService.UserId);
            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.MonthlyPayrollReport);

            var viewModel = new MonthlyPayrollReportViewModel
            {
                SelectedTime = DateTime.Today,
                Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
                EncryptedPassword = rijndaelService.Encrypt(filePassword, tuple.Item1.Value, tuple.Item2.Value),
                RijndaelKey = tuple.Item1,
                RijndaelIV = tuple.Item2
            };

            return View("MonthlyPayroll", viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.MonthlyPayrollReport, Policy = Policy.Read)]
        public async Task<IActionResult> MonthlyPayroll(MonthlyPayrollReportViewModel viewModel)
        {
            if (!sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.MonthlyPayrollReport, Policy.Read))
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            var rijndaelKey = (string)TempData.Peek(viewModel.RijndaelKey.Key);
            var rijndaelIV = (string)TempData.Peek(viewModel.RijndaelIV.Key);

            viewModel.FilePassword = rijndaelService.Decrypt(viewModel.EncryptedPassword, rijndaelKey, rijndaelIV);
            ModelState.Clear();

            if (TryValidateModel(viewModel))
            {
                if (viewModel.ReusePassword)
                {
                    await reportService.EditUserFilePasswordAsync(sessionService.UserId, viewModel.FilePassword);
                }

                var report = await reportService.GetDataForMonthlyPayrollReportAsync(viewModel.SelectedTime, viewModel.CompanyId);
                if (report.Employees.Any())
                {
                    var company = await baseService.GetDataEntityAsync<Company>(new GetCompanyByIdCriteria { CompanyId = viewModel.CompanyId }).ConfigureAwait(false);
                    report.Company = company;
                    string webRootFolder = hostingEnvironment.WebRootPath;
                    string fileName = @"ExcelTemplates\MonthlyPayrollReport.xlsx";
                    var templatePath = Path.Combine(webRootFolder, fileName);
                    var data = exportExcelService.ExportMonthlyPayrollReport(templatePath, report, viewModel.FilePassword);

                    return File(data, MimeType.XLSX, $"Monthly_Payroll_Report_{report.Month.ToString("MM-yyyy")}_{report.Company.Code}.xlsx");
                }
                else
                {
                    ModelState.AddModelError(nameof(viewModel.CommonError), Message.PayrollNotGenerated);
                }
            }

            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.MonthlyPayrollReport);
            viewModel.Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));

            Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair> tuple = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[tuple.Item1.Key] = tuple.Item1.Value;
            TempData[tuple.Item2.Key] = tuple.Item2.Value;

            viewModel.RijndaelKey = tuple.Item1;
            viewModel.RijndaelIV = tuple.Item2;

            return View("MonthlyPayroll", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.DynamicReport, Policy = Policy.Read)]
        public async Task<IActionResult> DynamicReport()
        {
            Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair> tuple = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[tuple.Item1.Key] = tuple.Item1.Value;
            TempData[tuple.Item2.Key] = tuple.Item2.Value;

            var filePassword = await reportService.GetUserFilePasswordAsync(sessionService.UserId);

            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.DynamicReport);
            var criteria = new GetEmployeeStatusByUserCriteria
            {
                UserId = sessionService.UserId,
            };

            var employeeStatusIds = new List<string> { EmploymentStatus.Probation.ToString(), EmploymentStatus.Confirmed.ToString() };
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
                EncryptedPassword = rijndaelService.Encrypt(filePassword, tuple.Item1.Value, tuple.Item2.Value),
                Groups = ColumnsConfiguration.DynamicReportGroups,
                RijndaelKey = tuple.Item1,
                RijndaelIV = tuple.Item2
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

            var rijndaelKey = (string)TempData.Peek(viewModel.RijndaelKey.Key);
            var rijndaelIV = (string)TempData.Peek(viewModel.RijndaelIV.Key);

            viewModel.FilePassword = rijndaelService.Decrypt(viewModel.EncryptedPassword, rijndaelKey, rijndaelIV);
            ModelState.Clear();

            if (TryValidateModel(viewModel))
            {
                var employeeDatas = await reportService.GetEmployeeDataForDynamicReport(viewModel.CompanyId, viewModel.EmployeeStatusIds);
                var data = exportExcelService.ExportDynamicReport(employeeDatas, viewModel.SelectedIds?.Split(',').Select(int.Parse) ?? Enumerable.Empty<int>(), viewModel.FilePassword);

                if (viewModel.ReusePassword)
                {
                    await reportService.EditUserFilePasswordAsync(sessionService.UserId, viewModel.FilePassword);
                }

                return File(data, MimeType.XLSX, $"DynamicReport.xlsx");
            }

            var companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.Report);
            viewModel.Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
            viewModel.Groups = ColumnsConfiguration.DynamicReportGroups;

            Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair> tuple = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[tuple.Item1.Key] = tuple.Item1.Value;
            TempData[tuple.Item2.Key] = tuple.Item2.Value;

            viewModel.RijndaelKey = tuple.Item1;
            viewModel.RijndaelIV = tuple.Item2;

            return View("DynamicReport", viewModel);
        }
    }
}