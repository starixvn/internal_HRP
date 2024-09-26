namespace HRP.Website.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using HRP.Criteria.Report;
    using HRP.Models;
    using HRP.Models.Report;
    using HRP.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public class MiriBankAndCashReportController : BaseController
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly ICommonService commonService;
        private readonly IRijndaelService rijndaelService;
        private readonly IReportService reportService;
        private readonly IExportMiriBankAndCashReportService exportExcelService;

        private const string PaymentTransactionOptionRootParentId = "0";

        public MiriBankAndCashReportController(
            IBaseService baseService,
            ISessionService sessionService,
            IWebHostEnvironment hostingEnvironment,
            IRijndaelService rijndaelService,
            ICommonService commonService,
            IReportService reportService,
            IExportMiriBankAndCashReportService exportExcelService) : base(baseService, sessionService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.rijndaelService = rijndaelService;
            this.commonService = commonService;
            this.reportService = reportService;
            this.exportExcelService = exportExcelService;
        }

        [Route("Report/MiriBankAndCash")]
        [HRPPermission(FunctionKey = FunctionKey.MiriBankAndCashReport, Policy = Policy.Read)]
        public async Task<IActionResult> Index()
        {
            var filePassword = await reportService.GetUserFilePasswordAsync(sessionService.UserId);
            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.MiriBankAndCashReport);

            (var rijndaelKeyPair, var rijndaelIVPair) = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[rijndaelKeyPair.Key] = rijndaelKeyPair.Value;
            TempData[rijndaelIVPair.Key] = rijndaelIVPair.Value;

            var viewModel = new MiriBankAndCashReportViewModel
            {
                Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
                SelectedTime = DateTimeOffset.Now.Date,
                BankType = BankTypes.DirectBanking,
                RijndaelKey = rijndaelKeyPair,
                RijndaelIV = rijndaelIVPair,
                EncryptedPassword = rijndaelService.Encrypt(filePassword, rijndaelKeyPair.Value, rijndaelIVPair.Value),
                PaymentOptions = await GetPaymentTransactionAsync()
            };

            return View("~/Views/Report/MiriBankAndCash.cshtml", viewModel);
        }

        [Route("Report/MiriBankAndCash/ExportExcel")]
        [HRPPermission(FunctionKey = FunctionKey.MiriBankAndCashReport, Policy = Policy.Read)]
        public async Task<IActionResult> ExportExcel(MiriBankAndCashReportViewModel viewModel)
        {
            var rijndaelKey = (string)TempData.Peek(viewModel.RijndaelKey.Key);
            var rijndaelIV = (string)TempData.Peek(viewModel.RijndaelIV.Key);
            viewModel.FilePassword = rijndaelService.Decrypt(viewModel.EncryptedPassword, rijndaelKey, rijndaelIV);

            var criteria = new GetDataForDirectBankingReportCriteria
            {
                UserId = sessionService.UserId,
                CompanyId = viewModel.CompanyId,
                Month = viewModel.SelectedTime,
                PaymentTransactions = viewModel.PaymentTransactions,
                PaymentType = (byte)viewModel.TypeOfPayment
            };
            var bankInData = await reportService.GetDirectBankingBankInAsync(criteria);

            ModelState.Clear();

            if (bankInData != null && TryValidateModel(viewModel))
            {
                if (viewModel.ReusePassword)
                {
                    await reportService.EditUserFilePasswordAsync(sessionService.UserId, viewModel.FilePassword);
                }

                const string fileName = @"ExcelTemplates/MiriBank&Cash.xlsx";
                var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, fileName);
                var fileContent = exportExcelService.Export(
                    templatePath,
                    viewModel.SelectedTime,
                    viewModel.TypeOfPayment,
                    bankInData,
                    viewModel.FilePassword);

                return File(fileContent, MimeType.XLSX, $"Miri_Bank-Cash_Report_{viewModel.SelectedTime:MMyyyy}.xlsx");
            }

            return RedirectToAction("Index");
        }

        protected internal virtual async Task<IList<MultiSelectViewModel>> GetPaymentTransactionAsync()
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

            return paymentOptions;
        }
    }
}