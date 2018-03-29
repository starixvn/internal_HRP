namespace HRP.Website.Controllers
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
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.RefResource;
    using HRP.Models;
    using HRP.Models.Currency;
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

        #region UploadAttendanceAndDSTSalary

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Read)]
        public async Task<IActionResult> Index()
        {
            var viewModel = await paymentProcessService.GetDataForSelectWorkingDayMonthAsync();

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Read)]
        [HttpPost]
        public async Task<IActionResult> Index(SelectWorkingMonthCompanyViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var isHost = sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.PaymentProcess, Policy.Host);
                var paymentProcess = await paymentProcessService.GetPaymentProcessAsync(viewModel.CompanyId, viewModel.SelectedTime);

                if (isHost && IsInPaymentProcessDuration(paymentProcess))
                {
                    if (paymentProcess.IsHasCurrencySetting)
                    {
#pragma warning disable S134 // Control flow statements "if", "switch", "for", "foreach", "while", "do"  and "try" should not be nested too deeply
                        if (IsRedirectToPageUploadFiles(paymentProcess))
#pragma warning restore S134 // Control flow statements "if", "switch", "for", "foreach", "while", "do"  and "try" should not be nested too deeply
                        {
                            return RedirectToAction("UploadAttendanceAndSalary", new { companyId = viewModel.CompanyId, month = viewModel.SelectedTime });
                        }

                        return RedirectToAction("EmployeeMonthlyPayroll", new { companyId = viewModel.CompanyId, month = viewModel.SelectedTime });
                    }

                    ModelState.AddModelError(nameof(viewModel.CommonError), Message.NotHasCurrencySetting);
                }
                else
                {
                    if (paymentProcess.IsPayrollGenerated)
                    {
                        return RedirectToAction("EmployeeMonthlyPayroll", new { companyId = viewModel.CompanyId, month = viewModel.SelectedTime });
                    }

                    ModelState.AddModelError(nameof(viewModel.CommonError), Message.PayrollNotGenerated);
                }
            }

            var data = await paymentProcessService.GetDataForSelectWorkingDayMonthAsync();
            viewModel.Companies = data.Companies;
            viewModel.SelectedTime = data.SelectedTime;
            viewModel.StartDate = data.StartDate;
            viewModel.DueDate = data.DueDate;
            return View("Index", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> DownloadAttendance()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"ExcelTemplates\Template - Attendance (Expat).xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            var stream = await paymentProcessService.CopyFileToMemoryStreamAsync(path);

            return File(stream, MimeType.XLSX, Path.GetFileName(path));
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> DownloadDSTNetSalary()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"ExcelTemplates\Template - DST Salary (Expat).xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            var stream = await paymentProcessService.CopyFileToMemoryStreamAsync(path);

            return File(stream, MimeType.XLSX, Path.GetFileName(path));
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> UploadAttendanceAndSalary(int companyId, DateTime month)
        {
            var isHost = sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host);
            var paymentProcess = await paymentProcessService.GetPaymentProcessAsync(companyId, month);

            if (isHost && IsInPaymentProcessDuration(paymentProcess))
            {
                var viewModel = await paymentProcessService.GetUploadingFilesResultAsync(companyId, month);

                return View(viewModel);
            }

            return RedirectToAction("AccessDenied", "Error");
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        [HttpPost]
        public async Task<IActionResult> UploadAttendanceAndSalary(UploadAttendanceAndSalaryViewModel viewModel)
        {
            if (viewModel.AttendanceFile == null && viewModel.DSTNetSalaryFile == null)
            {
                ModelState.AddModelError(nameof(viewModel.CommonError), Message.PleaseSelectFile);
            }

            var readAttendanceResult = await paymentProcessService.ReadAttendanceFileAsync(viewModel);
            var readDSTNetSalaryResult = await paymentProcessService.ReadDSTNetSalaryFileAsync(viewModel);
            if (readAttendanceResult is Failure<AttendanceTemplate> failureAttendance)
            {
                ModelState.AddModelError(nameof(viewModel.AttendanceFile), string.Concat(ViewLang.AttendanceFile, ": ", failureAttendance.Message));
            }

            if (readDSTNetSalaryResult is Failure<DSTNetSalaryTemplate> failureDSTNetSalary)
            {
                ModelState.AddModelError(nameof(viewModel.DSTNetSalaryFile), string.Concat(ViewLang.DSTNetSalaryFile, ": ", failureDSTNetSalary.Message));
            }

            if (ModelState.IsValid)
            {
                var attendanceResult = (Success<AttendanceTemplate>)readAttendanceResult;
                var dstNetSalaryResult = (Success<DSTNetSalaryTemplate>)readDSTNetSalaryResult;
                var uploadFiles = await paymentProcessService.UploadAttendanceAndDSTNetSalaryAsync(viewModel.CompanyId, viewModel.SelectedTime,
                                                                                                attendanceResult.Value, dstNetSalaryResult.Value);

                if (uploadFiles.IsSuccessful)
                {
                    return Json(new { isSuccessful = true });
                }

                ModelState.AddModelError(nameof(viewModel.CommonError), uploadFiles.Message);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> ValidationFilesResult(int companyId, DateTime month)
        {
            var viewModel = await paymentProcessService.GetUploadingFilesResultAsync(companyId, month);

            return PartialView("_ValidationFilesResult", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> GetPaymentProcess(int companyId, DateTime month)
        {
            var processInfo = await paymentProcessService.GetPaymentProcessAsync(companyId, month);

            return Json(processInfo);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        [HttpPost]
        public async Task<IActionResult> ProcessActionNextToMonthlyPayroll(int companyId, DateTime month)
        {
            var submitResult = await paymentProcessService.SubmitUploadingFilesAsync(companyId, month);
            if (submitResult.IsSuccessful)
            {
                var generateResult = await paymentProcessService.GenerateMonthlyPayrollAsync(companyId, month, null);
                if (generateResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = true });
                }

                ModelState.AddModelError(nameof(UploadAttendanceAndSalaryViewModel.CommonError), generateResult.Message);
            }
            else
            {
                ModelState.AddModelError(nameof(UploadAttendanceAndSalaryViewModel.CommonError), submitResult.Message);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        [HttpPost]
        public async Task<IActionResult> DeleteUploadedAttendance(int companyId, DateTime month)
        {
            var deleteResult = await paymentProcessService.DeleteUploadedAttendance(companyId, month);
            if (deleteResult.IsSuccessful)
            {
                return Json(new { isSuccessful = true });
            }

            ModelState.AddModelError(nameof(UploadAttendanceAndSalaryViewModel.CommonError), deleteResult.Message);

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        [HttpPost]
        public async Task<IActionResult> DeleteUploadedDSTNetSalary(int companyId, DateTime month)
        {
            var deleteSalaryResult = await paymentProcessService.DeleteUploadedDSTNetSalary(companyId, month);
            if (deleteSalaryResult.IsSuccessful)
            {
                return Json(new { isSuccessful = true });
            }

            ModelState.AddModelError(nameof(UploadAttendanceAndSalaryViewModel.CommonError), deleteSalaryResult.Message);

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        private static bool IsRedirectToPageUploadFiles(PaymentProcess paymentProcess)
        {
            var isAttendanceFileNotSubmited = (paymentProcess.AttendanceFileName != null && !paymentProcess.IsAttendanceFileSubmited);
            var isDSTNetSalaryFileNotSubmited = (paymentProcess.DSTNetSalaryFileName != null && !paymentProcess.IsDSTNetSalaryFileSubmited);

            return (!paymentProcess.IsPayrollGenerated
                        || isAttendanceFileNotSubmited
                        || isDSTNetSalaryFileNotSubmited);
        }

        private static bool IsInPaymentProcessDuration(PaymentProcess paymentProcess)
        {
            var today = DateTime.Today;
            return paymentProcess.StartDate <= today && today <= paymentProcess.DueDate;
        }

        #endregion UploadAttendanceAndDSTSalary

        #region Monthly Payroll

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Read)]
        public async Task<IActionResult> EmployeeMonthlyPayroll(
            int companyId,
            DateTime month,
            int? employeeId = null,
            string employee = null,
            string costCenterIds = null,
            string idNo = null,
            int? workingDays = null)
        {
            var isRead = sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Read);
            if (!isRead)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            EmployeeMonthlyPayrollViewModel viewModel = await monthlyPayrollService.GetDataFilterForIndexPageAsync(month, companyId, costCenterIds);
            viewModel.Month = month;
            viewModel.EmployeeId = employeeId;
            viewModel.Employee = employee;
            viewModel.IDNo = idNo;
            viewModel.WorkingDays = workingDays;
            viewModel.Employees = await monthlyPayrollService.GetMonthlyPayrollAsync(viewModel);

            return View("MonthlyPayroll", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Read)]
#pragma warning disable S107 // Methods should not have too many parameters
        public async Task<IActionResult> EditBasicSalary(
            int payrollId,
            int companyId,
            DateTime month,
            int? employeeId = null,
            string employee = null,
            string costCenterIds = null,
            string idNo = null,
            int? workingDays = null)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            var isRead = sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Read);
            if (!isRead)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            EditMonthlySalaryViewModel viewModel = await monthlyPayrollService.GetMonthlySalaryToEditAsync(payrollId);
            if (viewModel == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            var criteria = new GetExchangeRateCriteria
            {
                Month = month.Month,
                Year = month.Year
            };

            var currencies = await baseService.GetDataAsync<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria());
            var cashAccounts = baseService.GetData<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = ReferenceResource.PaymentMethodCash, IsActive = true });
            viewModel.Month = month;
            viewModel.CashAccounts = new SelectList(cashAccounts, nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value));
            viewModel.Currencies = new SelectList(currencies, nameof(CurrencyCodeModel.CurrencyId), nameof(CurrencyCodeModel.CurrencyCode));
            viewModel.CurrencyExchangeRates = await baseService.GetDataAsync<CurrencyExchangeRateModel>(criteria);
            viewModel.Filter.EmployeeId = employeeId;
            viewModel.Filter.Employee = employee;
            viewModel.Filter.IDNo = idNo;
            viewModel.Filter.CostCenterIds = costCenterIds;
            viewModel.Filter.WorkingDays = workingDays;

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        [HttpPost]
        public async Task<IActionResult> EditBasicSalary(EditMonthlySalaryViewModel viewModel)
        {
            var isHost = sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.PaymentProcess, Policy.Host);

            if (!isHost)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            if (ModelState.IsValid)
            {
                CommandResult commandResult = await monthlyPayrollService.EditMonthlySalaryAsync(viewModel);
                var redirectTo = viewModel.IsRedirectToEditAllowance
                    ? Url.Action("EditAllowances", new
                    {
                        payrollId = viewModel.PayrollId,
                        companyId = viewModel.CompanyId,
                        month = viewModel.Month,
                        employeeId = viewModel.Filter.EmployeeId,
                        employee = viewModel.Filter.Employee,
                        costCenterIds = viewModel.Filter.CostCenterIds,
                        idNo = viewModel.Filter.IDNo,
                        workingDays = viewModel.Filter.WorkingDays
                    })
                    : Url.Action("EmployeeMonthlyPayroll", new
                    {
                        companyId = viewModel.CompanyId,
                        month = viewModel.Month,
                        employeeId = viewModel.Filter.EmployeeId,
                        employee = viewModel.Filter.Employee,
                        costCenterIds = viewModel.Filter.CostCenterIds,
                        idNo = viewModel.Filter.IDNo,
                        workingDays = viewModel.Filter.WorkingDays
                    });

                return Json(new
                {
                    isSuccessful = commandResult.IsSuccessful,
                    message = commandResult.Message,
                    redirectTo = commandResult.IsSuccessful ? redirectTo : string.Empty
                });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Read)]
        public async Task<IActionResult> GetEmployeeSuggestionByName(int companyId, string name)
        {
            IEnumerable<EmployeeSuggestionViewModel> employees = await monthlyPayrollService.GetEmployeeSuggestionByNameAsync(companyId, name);

            return Json(employees);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Read)]
        public async Task<IActionResult> GetEmployeeSuggestionByIDNo(int companyId, string idNo)
        {
            IEnumerable<EmployeeSuggestionViewModel> employees = await monthlyPayrollService.GetEmployeeSuggestionByIDNoAsync(companyId, idNo);

            return Json(employees);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Read)]
        public async Task<IActionResult> EditAllowances(int payrollId, int companyId, DateTime month,
            int? searchWorkingDays = null,
            int? searchEmployeeId = null,
            string searchEmployee = null,
            string searchCostCenterIds = null,
            string searchIdNo = null)
        {
            if (!sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Read))
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            var viewModel = await monthlyPayrollService.GetMonthlyAllowancesAsync(payrollId, month);
            viewModel.SearchWorkingDays = searchWorkingDays;
            viewModel.SearchEmployeeId = searchEmployeeId;
            viewModel.SearchEmployee = searchEmployee;
            viewModel.SearcCostCenterIds = searchCostCenterIds;
            viewModel.SearchIdNo = searchIdNo;

            var currencies = baseService.GetData<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria());
            viewModel.Allowance.CurrencyCode = currencies.FirstOrDefault(x => x.CurrencyId == viewModel.Allowance.CurrencyId)?.CurrencyCode;
            viewModel.Currencies = new SelectList(currencies, nameof(CurrencyCodeModel.CurrencyId), nameof(CurrencyCodeModel.CurrencyCode));

            viewModel.CurrencyExchangeRates = baseService.GetData<CurrencyExchangeRateModel>(new GetExchangeRateCriteria { Month = month.Month, Year = month.Year });
            viewModel.CashAccounts = new SelectList(baseService.GetData<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = ReferenceResource.PaymentMethodCash, IsActive = true }),
                nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value));

            viewModel.CanEdit = sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host)
                && DateTime.Today >= viewModel.EmployeeInfo.StartDate && DateTime.Today <= viewModel.EmployeeInfo.DueDate;

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> EditAllowances(MonthlyAllowancesViewModel viewModel, EditMonthlyAllowanceAction submitAction)
        {
            if (!sessionService.HasPermissionOnCompany(viewModel.EmployeeInfo.CompanyId, FunctionKey.PaymentProcess, Policy.Host))
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            if (ModelState.IsValid)
            {
                var result = await monthlyPayrollService.EditMonthlyAllowancesAsync(viewModel, sessionService.UserId);

                if (!result.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
                else
                {
                    switch (submitAction)
                    {
                        case EditMonthlyAllowanceAction.SaveAndEditBasicSalary:
                            return RedirectToAction("EditBasicSalary", new
                            {
                                companyId = viewModel.EmployeeInfo.CompanyId,
                                month = viewModel.Month,
                                payrollId = viewModel.Allowance.PayrollId,
                                employeeId = viewModel.SearchEmployeeId,
                                employee = viewModel.SearchEmployee,
                                costCenterIds = viewModel.SearcCostCenterIds,
                                idNo = viewModel.SearchIdNo,
                                workingDays = viewModel.SearchWorkingDays
                            });

                        case EditMonthlyAllowanceAction.Save:
                        default:
                            return RedirectToAction("EmployeeMonthlyPayroll",
                                new
                                {
                                    companyId = viewModel.EmployeeInfo.CompanyId,
                                    month = viewModel.Month,
                                    employeeId = viewModel.SearchEmployeeId,
                                    employee = viewModel.SearchEmployee,
                                    costCenterIds = viewModel.SearcCostCenterIds,
                                    idNo = viewModel.SearchIdNo,
                                    workingDays = viewModel.SearchWorkingDays
                                });
                    }
                }
            }

            var currencies = baseService.GetData<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria());
            viewModel.Currencies = new SelectList(currencies, nameof(CurrencyCodeModel.CurrencyId), nameof(CurrencyCodeModel.CurrencyCode));
            viewModel.Allowance.CurrencyCode = currencies.FirstOrDefault(x => x.CurrencyId == viewModel.Allowance.CurrencyId)?.CurrencyCode;

            viewModel.CurrencyExchangeRates = baseService.GetData<CurrencyExchangeRateModel>(new GetExchangeRateCriteria { Month = viewModel.Month.Month, Year = viewModel.Month.Year });
            viewModel.CashAccounts = new SelectList(baseService.GetData<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = ReferenceResource.PaymentMethodCash, IsActive = true }),
                nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value));

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> Preset(int companyId, int employeeId, DateTime month)
        {
            if (!sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host))
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            var result = await paymentProcessService.GenerateMonthlyPayrollAsync(companyId, month, employeeId);

            return Json(result);
        }

        #endregion Monthly Payroll
    }
}