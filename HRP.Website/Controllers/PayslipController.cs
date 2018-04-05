namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Models;
    using HRP.Models.Payslip;
    using HRP.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [HRPAuthorize]
    public class PayslipController : BaseController
    {
        private readonly IViewRenderService viewRenderService;
        private readonly IPdfRenderService pdfRenderService;
        private readonly IPayslipService payslipService;

        public PayslipController(
            ISessionService sessionService,
            IBaseService baseService,
            IPayslipService payslipService,
            IViewRenderService viewRenderService,
            IPdfRenderService pdfRenderService) : base(baseService, sessionService)
        {
            this.payslipService = payslipService;
            this.viewRenderService = viewRenderService;
            this.pdfRenderService = pdfRenderService;
        }

        [HRPPermission(FunctionKey = FunctionKey.Payslip, Policy = Policy.Read)]
        public async Task<IActionResult> Index(DateTime? month = null, int? companyId = null, bool? isSent = null)
        {
            IEnumerable<Company> companies = await payslipService.GetCompanyAsync();
            Company company = companies.FirstOrDefault();
            var monthSelected = month ?? DateTime.Today;
            var companyIdSelected = companyId ?? (company != null ? company.CompanyId : 0);
            IEnumerable<PayslipModel> payslips = await payslipService.GetPayslipAsync(monthSelected, companyIdSelected, isSent);

            var viewModel = new PayslipViewModel
            {
                Month = monthSelected,
                CompanyId = companyIdSelected,
                IsSent = isSent,
                Payslips = payslips,
                Companies = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
                IsHost = sessionService.HasPermissionOnCompany(companyIdSelected, FunctionKey.Payslip, Policy.Host)
            };

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.Payslip, Policy = Policy.Read)]
        public async Task<IActionResult> Details(int id, int companyId)
        {
            var isRead = sessionService.HasPermissionOnCompany(companyId, FunctionKey.Payslip, Policy.Read);

            if (!isRead)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            PayslipDetailModel payslipDetails = await payslipService.GetPayslipDetailAsync(id, companyId);

            if (payslipDetails == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return View(payslipDetails);
        }

        [HRPPermission(FunctionKey = FunctionKey.Payslip, Policy = Policy.Host)]
        [HttpPost]
        public async Task<IActionResult> SendPayslip(SendPayslipViewModel viewModel)
        {
            var isHost = sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.Payslip, Policy.Host);

            if (!isHost)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            CommandResult commandResult = await payslipService.SendPayslipAsync(viewModel);

            return Json(commandResult);
        }

        [HRPPermission(FunctionKey = FunctionKey.Payslip, Policy = Policy.Host)]
        public async Task<IActionResult> Pdf()
        {
            string cssFilePath = @"assets\\styles\\pages\\payslip\\payslip-detail.css";
            PayslipDetailModel payslipDetails = await payslipService.GetPayslipDetailAsync(1056, 1);
            var htmlString = await viewRenderService.RenderViewToStringAsync("Payslip/Details", payslipDetails);

            byte[] streamContent = pdfRenderService.RenderContent(cssFilePath, htmlString);

            return File(streamContent, @"application/pdf", "Payslip.pdf");
        }
    }
}