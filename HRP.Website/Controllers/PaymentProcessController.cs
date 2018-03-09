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
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.PaymentProcess;
    using HRP.Resources;
    using HRP.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [HRPAuthorize]
    public class PaymentProcessController : BaseController
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IPaymentProcessService paymentProcessService;
        private readonly IMonthlyPayrollService monthlyPayrollService;

        public PaymentProcessController(
            IBaseService baseService,
            ISessionService sessionService,
            IHostingEnvironment hostingEnvironment,
            IPaymentProcessService paymentProcessService,
            IMonthlyPayrollService monthlyPayrollService)
            : base(baseService, sessionService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.paymentProcessService = paymentProcessService;
            this.monthlyPayrollService = monthlyPayrollService;
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Read)]
        public async Task<IActionResult> Index()
        {
            var companies = await baseService.GetDataAsync<Company>(new GetCompaniesForPaymentProcessCriteria { UserId = sessionService.UserId });
            var viewModel = new SelectWorkingMonthCompanyViewModel
            {
                SelectedTime = DateTime.Today,
                Companies = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code))
            };

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Read)]
        [HttpPost]
        public async Task<IActionResult> SelectWorkingMonthAndCompany(SelectWorkingMonthCompanyViewModel viewModel)
        {
            //Check company permission
            //TODO
            if (ModelState.IsValid)
            {
                var process = new PaymentProcess
                {
                    CompanyId = viewModel.CompanyId,
                    Month = viewModel.SelectedTime
                };
                bool isPayrollGenerated = await paymentProcessService.IsPayrollGeneratedAsync(process);
                var completedDate = DateTime.Today.AddMonths(3);
                if (viewModel.SelectedTime.Month == DateTime.Today.Month && viewModel.SelectedTime < completedDate && !isPayrollGenerated)
                {
                    return RedirectToAction("UploadAttendanceAndSalary", new { companyId = viewModel.CompanyId, month = viewModel.SelectedTime });
                }

                if (viewModel.SelectedTime.Month < DateTime.Today.Month || viewModel.SelectedTime > completedDate || isPayrollGenerated)
                {
                    return RedirectToAction("EmployeeMonthlyPayroll");
                }
            }

            viewModel.Companies = new SelectList(
                await baseService.GetDataAsync<Company>(new GetCompaniesForPaymentProcessCriteria { UserId = sessionService.UserId }),
                nameof(Company.CompanyId), nameof(Company.Code));
            return View("Index", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        [HttpPost]
        public async Task<IActionResult> DownloadAttendance()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"ExcelTemplates\\Template - Attendance (Expat).xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            return File(memory, MimeType.XLSX, Path.GetFileName(path));
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        [HttpPost]
        public async Task<IActionResult> DownloadDSTNetSalary()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"ExcelTemplates\\Template - DST Salary (Expat).xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            return File(memory, MimeType.XLSX, Path.GetFileName(path));
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> UploadAttendanceAndSalary(int companyId, DateTime month)
        {
            var isHost = sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host);

            if (isHost)
            {
                var viewModel = await paymentProcessService.GetDataForUploadAttendanceAndSalary(companyId, month);

                return View(viewModel);
            }

            return RedirectToAction("AccessDenied", "Error");
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        [HttpPost]
        public async Task<IActionResult> UploadAttendanceAndSalary(UploadAttendanceAndSalaryViewModel viewModel)
        {
            CommandResult validateAttendanceFile = paymentProcessService.CheckValidExcelFile(viewModel.AttendanceFile);
            if (!validateAttendanceFile.IsSuccessful)
            {
                ModelState.AddModelError(nameof(viewModel.AttendanceFile), validateAttendanceFile.Message);
            }

            CommandResult validateDSTSalaryFile = paymentProcessService.CheckValidExcelFile(viewModel.DSTNetSalaryFile);
            if (!validateDSTSalaryFile.IsSuccessful)
            {
                ModelState.AddModelError(nameof(viewModel.DSTNetSalaryFile), validateDSTSalaryFile.Message);
            }

            if (viewModel.AttendanceFile == null && viewModel.DSTNetSalaryFile == null)
            {
                ModelState.AddModelError(nameof(viewModel.CommonError), Message.PleaseSelectFile);
            }

            if (ModelState.IsValid)
            {
                var attendanceResult = await paymentProcessService.UploadAttendanceFileAsync(viewModel);
                var salaryResult = paymentProcessService.UploadDSTNetSalaryFileAsync(viewModel);

                if (attendanceResult.IsSuccessful && salaryResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = true });
                }

                if (!attendanceResult.IsSuccessful)
                {
                    ModelState.AddModelError(nameof(viewModel.AttendanceFile), attendanceResult.Message);
                }

                if (!salaryResult.IsSuccessful)
                {
                    ModelState.AddModelError(nameof(viewModel.DSTNetSalaryFile), salaryResult.Message);
                }
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public IActionResult AttendanceValidationResult(int companyId, DateTime month)
        {
            var errors = paymentProcessService.GetAttendanceUploadingResult(companyId, month);
            var viewModel = new ValidationFileResultViewModel
            {
                ValidationErrors = errors
            };

            return PartialView("_AttendanceValidationResult", viewModel);
        }

        #region Monthly Payroll

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Read)]
        public async Task<IActionResult> EmployeeMonthlyPayroll(
            int companyId,
            DateTime month,
            int? employeeId = null,
            string employee = null,
            string costCenterIds = null,
            string idNo = null)
        {
            var isRead = sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Read);

            if (!isRead)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            EmployeeMonthlyPayrollViewModel viewModel = await monthlyPayrollService.GetDataFilterForIndexPageAsync(companyId, costCenterIds);
            viewModel.Month = month;
            viewModel.EmployeeId = employeeId;
            viewModel.Employee = employee;
            viewModel.IDNo = idNo;
            viewModel.Employees = await monthlyPayrollService.GetMonthlyPayrollAsync(viewModel);

            return View("MonthlyPayroll", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Read)]
        public IActionResult EditBasicSalary(
            int payrollId,
            int companyId,
            DateTime month)
        {
            var isRead = sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Read);

            if (!isRead)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            var viewModel = new EditMonthlySalaryViewModel();

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Read)]
        public async Task<IActionResult> GetEmployeeSuggestionByName(int companyId, string name)
        {
            IEnumerable<EmployeeSuggestionViewModel> employees = await monthlyPayrollService.GetEmployeeSuggestionByNameAsync(companyId, name);

            return Json(employees);
        }

        #endregion Monthly Payroll
    }
}