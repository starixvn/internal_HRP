namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using HRP.Models;
    using HRP.Models.Payslip;
    using HRP.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [HRPAuthorize]
    public class PayslipController : BaseController
    {
        private readonly IPayslipService payslipService;

        public PayslipController(
            ISessionService sessionService,
            IBaseService baseService,
            IPayslipService payslipService) : base(baseService, sessionService)
        {
            this.payslipService = payslipService;
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
        public IActionResult Details(int id)
        {
            return View();
        }

        [HRPPermission(FunctionKey = FunctionKey.Payslip, Policy = Policy.Host)]
        [HttpPost]
        public IActionResult SendPayslip()
        {
            return View();
        }
    }
}