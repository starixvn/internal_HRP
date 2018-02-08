namespace HRP.Website.Controllers
{
    using System;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.PaymentProcess;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class PaymentProcessController : BaseController
    {
        public PaymentProcessController(IBaseService baseService, ISessionService sessionService) : base(baseService, sessionService)
        {
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public IActionResult Index()
        {
            var viewModel = new SelectWorkingMonthCompanyViewModel
            {
                SelectedTime = DateTime.Today,
                Companies = new SelectList(baseService.GetData<Company>(new GetAllCompanyCriteria { IsActive = true }), nameof(Company.CompanyId), nameof(Company.Code))
            };

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        [HttpPost]
        public IActionResult SelectWorkingMonthAndCompany(SelectWorkingMonthCompanyViewModel viewModel)
        {
            var completedDate = DateTime.Today.AddMonths(3);
            if (ModelState.IsValid)
            {
                if (viewModel.SelectedTime.Month == DateTime.Today.Month && viewModel.SelectedTime < completedDate)
                {
                    return RedirectToAction("UploadAttendanceAndSalary");
                }

                if (viewModel.SelectedTime.Month < DateTime.Today.Month || viewModel.SelectedTime > completedDate)
                {
                    return RedirectToAction("EmployeeMonthlyPayroll");
                }
            }

            viewModel.Companies = new SelectList(baseService.GetData<Company>(new GetAllCompanyCriteria { IsActive = true }), nameof(Company.CompanyId), nameof(Company.Code));
            return View("Index", viewModel);
        }
    }
}