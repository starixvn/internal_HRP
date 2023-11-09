namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.Report;
    using HRP.Resources;
    using HRP.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public class MonthlyPayrollReportController : BaseController
    {
        private readonly IReportService reportService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IExportMonthlyPayrollReportService excelService;
        private readonly ICommonService commonService;
        private readonly IRijndaelService rijndaelService;

        public MonthlyPayrollReportController(
            IBaseService baseService,
            ISessionService sessionService,
            IHostingEnvironment hostingEnvironment,
            IExportMonthlyPayrollReportService excelService,
            IReportService reportService,
            IRijndaelService rijndaelService,
            ICommonService commonService) : base(baseService, sessionService)
        {
            this.reportService = reportService;
            this.hostingEnvironment = hostingEnvironment;
            this.excelService = excelService;
            this.rijndaelService = rijndaelService;
            this.commonService = commonService;
        }

        [Route("Report/MonthlyPayroll")]
        [HRPPermission(FunctionKey = FunctionKey.MonthlyPayrollReport, Policy = Policy.Read)]
        public async Task<IActionResult> Index()
        {
            (var rijndaelKeyPair, var rijndaelIVPair) = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[rijndaelKeyPair.Key] = rijndaelKeyPair.Value;
            TempData[rijndaelIVPair.Key] = rijndaelIVPair.Value;

            var filePassword = await reportService.GetUserFilePasswordAsync(sessionService.UserId);
            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.MonthlyPayrollReport);
            IEnumerable<Currency> currencies = await commonService.GetCurrencyAsync();
            var currency = currencies.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.MYRCurrencyCode, StringComparison.OrdinalIgnoreCase));
            currency = currency ?? currencies.FirstOrDefault();

            var viewModel = new MonthlyPayrollReportViewModel
            {
                SelectedTime = DateTimeOffset.Now.Date,
                CurrencyId = currency?.CurrencyId,
                Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
                Currencies = commonService.GetSelectList(currencies, nameof(Currency.CurrencyId), nameof(Currency.CurrencyCode)),
                EncryptedPassword = rijndaelService.Encrypt(filePassword, rijndaelKeyPair.Value, rijndaelIVPair.Value),
                RijndaelKey = rijndaelKeyPair,
                RijndaelIV = rijndaelIVPair
            };

            return View("~/Views/Report/MonthlyPayroll.cshtml", viewModel);
        }

        [HttpPost]
        [Route("Report/MonthlyPayroll")]
        [HRPPermission(FunctionKey = FunctionKey.MonthlyPayrollReport, Policy = Policy.Read)]
        public async Task<IActionResult> Index(MonthlyPayrollReportViewModel viewModel)
        {
            var rijndaelKey = (string)TempData.Peek(viewModel.RijndaelKey.Key);
            var rijndaelIV = (string)TempData.Peek(viewModel.RijndaelIV.Key);
            viewModel.FilePassword = rijndaelService.Decrypt(viewModel.EncryptedPassword, rijndaelKey, rijndaelIV);
            IEnumerable<Currency> currencies = await commonService.GetCurrencyAsync();

            ModelState.Clear();
            if (TryValidateModel(viewModel))
            {
                int companyId = viewModel.CompanyId ?? 0;
                int currencyId = viewModel.CurrencyId ?? 0;
                if (!sessionService.HasPermissionOnCompany(companyId, FunctionKey.MonthlyPayrollReport, Policy.Read))
                {
                    return RedirectToAction("AccessDenied", "Error");
                }

                if (viewModel.ReusePassword)
                {
                    await reportService.EditUserFilePasswordAsync(sessionService.UserId, viewModel.FilePassword);
                }

                var report = await reportService.GetDataForMonthlyPayrollReportAsync(viewModel.SelectedTime, companyId, currencyId);
                if (report.Employees.Any())
                {
                    var company = await baseService.GetDataEntityAsync<Company>(new GetCompanyByIdCriteria { CompanyId = companyId });
                    report.Company = company;
                    report.SelectedCurrency = currencies.FirstOrDefault(x => x.CurrencyId == currencyId);
                    const string fileName = @"ExcelTemplates\MonthlyPayrollReport.xlsx";
                    var templatePath = Path.Combine(hostingEnvironment.WebRootPath, fileName);
                    var data = excelService.Export(templatePath, report, viewModel.FilePassword);

                    return File(data, MimeType.XLSX, $"Monthly_Payroll_Report_{report.Month:MM-yyyy}_{report.Company.Code}.xlsx");
                }
                else
                {
                    ModelState.AddModelError(nameof(viewModel.CommonError), Message.PayrollNotGenerated);
                }
            }

            var currency = currencies.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.MYRCurrencyCode, StringComparison.OrdinalIgnoreCase));
            currency = currency ?? currencies.FirstOrDefault();
            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.MonthlyPayrollReport);
            viewModel.Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
            viewModel.Currencies = commonService.GetSelectList(currencies, nameof(Currency.CurrencyId), nameof(Currency.CurrencyCode));
            viewModel.CurrencyId = currency?.CurrencyId;

            (var rijndaelKeyPair, var rijndaelIVPair) = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[rijndaelKeyPair.Key] = rijndaelKeyPair.Value;
            TempData[rijndaelIVPair.Key] = rijndaelIVPair.Value;

            viewModel.RijndaelKey = rijndaelKeyPair;
            viewModel.RijndaelIV = rijndaelIVPair;

            return View("~/Views/Report/MonthlyPayroll.cshtml", viewModel);
        }
    }
}