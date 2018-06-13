namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Criteria;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Core.Filters;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.MonthlyPayroll.EmployeeMonthlyPayroll;
    using HRP.Models.PaymentProcess;
    using HRP.Models.ServiceModels;
    using HRP.Resources;
    using HRP.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public class PaymentProcessController : BaseController
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ICommonService commonService;
        private readonly IPaymentProcessService paymentProcessService;
        private readonly IMonthlyPayrollService monthlyPayrollService;
        private readonly IRijndaelService rijndaelService;

        public PaymentProcessController(
            IBaseService baseService,
            ISessionService sessionService,
            IHostingEnvironment hostingEnvironment,
            ICommonService commonService,
            IPaymentProcessService paymentProcessService,
            IMonthlyPayrollService monthlyPayrollService,
            IRijndaelService rijndaelService) : base(baseService, sessionService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.commonService = commonService;
            this.paymentProcessService = paymentProcessService;
            this.monthlyPayrollService = monthlyPayrollService;
            this.rijndaelService = rijndaelService;
        }

        #region UploadAttendanceAndDSTSalary

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Read)]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.PaymentProcess);
            PaymentProcess paymentProcess = await paymentProcessService.GetCurrentDuration();
            var viewModel = new SelectWorkingMonthCompanyViewModel
            {
                Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
                SelectedTime = paymentProcess.Month,
                StartDate = paymentProcess.StartDate,
                DueDate = paymentProcess.DueDate
            };

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Read)]
        [HttpPost]
        public async Task<IActionResult> Index(SelectWorkingMonthCompanyViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var isHost = sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.PaymentProcess, Policy.Host);
                PaymentProcess paymentProcess = await paymentProcessService.GetPaymentProcessAsync(viewModel.CompanyId, viewModel.SelectedTime);

                if (isHost && IsInPaymentProcessDuration(paymentProcess))
                {
                    if (paymentProcess.IsHasCurrencySetting)
                    {
                        return IsRedirectToPageUploadFiles(paymentProcess)
                            ? RedirectToAction("UploadAttendanceAndSalary", new { companyId = viewModel.CompanyId, month = viewModel.SelectedTime })
                            : RedirectToAction("EmployeeMonthlyPayroll", new { companyId = viewModel.CompanyId, month = viewModel.SelectedTime });
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

            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.PaymentProcess);
            viewModel.Companies = commonService.GetSelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));

            return View("Index", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> DownloadAttendance()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"ExcelTemplates\Template - Attendance (Expat).xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            MemoryStream stream = await paymentProcessService.CopyFileToMemoryStreamAsync(path);

            return File(stream, MimeType.XLSX, Path.GetFileName(path));
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> DownloadDSTNetSalary()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"ExcelTemplates\Template - DST Salary (Expat).xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            MemoryStream stream = await paymentProcessService.CopyFileToMemoryStreamAsync(path);

            return File(stream, MimeType.XLSX, Path.GetFileName(path));
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> UploadAttendanceAndSalary(int companyId, DateTime month)
        {
            var isHost = sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host);
            PaymentProcess paymentProcess = await paymentProcessService.GetPaymentProcessAsync(companyId, month);

            if (isHost && IsInPaymentProcessDuration(paymentProcess))
            {
                var viewModel = await paymentProcessService.GetUploadingFilesResultAsync(companyId, month);

                return View(viewModel);
            }

            return RedirectToAction("Index");
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        [HttpPost]
        public async Task<IActionResult> UploadAttendanceAndSalary(UploadAttendanceAndSalaryViewModel viewModel)
        {
            if (viewModel.AttendanceFile == null && viewModel.DSTNetSalaryFile == null)
            {
                ModelState.AddModelError(nameof(viewModel.CommonError), Message.PleaseSelectFile);
            }

            Result<AttendanceTemplate> readAttendanceResult = await paymentProcessService.ReadAttendanceFileAsync(viewModel);
            Result<DSTNetSalaryTemplate> readDSTNetSalaryResult = await paymentProcessService.ReadDSTNetSalaryFileAsync(viewModel);
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
                CommandResult uploadFiles = await paymentProcessService.UploadAttendanceAndDSTNetSalaryAsync(
                    viewModel.CompanyId,
                    viewModel.SelectedTime,
                    attendanceResult.Value,
                    dstNetSalaryResult.Value);

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
            UploadAttendanceAndSalaryViewModel viewModel = await paymentProcessService.GetUploadingFilesResultAsync(companyId, month);

            return PartialView("_ValidationFilesResult", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> GetPaymentProcess(int companyId, DateTime month)
        {
            PaymentProcess processInfo = await paymentProcessService.GetPaymentProcessAsync(companyId, month);

            return Json(processInfo);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        [HttpPost]
        public async Task<IActionResult> ProcessActionNextToMonthlyPayroll(int companyId, DateTime month)
        {
            CommandResult submitResult = await paymentProcessService.SubmitUploadingFilesAsync(companyId, month);
            if (submitResult.IsSuccessful)
            {
                CommandResult generateResult = await paymentProcessService.GenerateMonthlyPayrollAsync(companyId, month, null);
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
            CommandResult deleteResult = await paymentProcessService.DeleteUploadedAttendance(companyId, month);
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
            CommandResult deleteSalaryResult = await paymentProcessService.DeleteUploadedDSTNetSalary(companyId, month);
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
            var isAttendanceFileNotSubmitted = (paymentProcess.AttendanceFileName != null && !paymentProcess.IsAttendanceFileSubmitted);
            var isDSTNetSalaryFileNotSubmitted = (paymentProcess.DSTNetSalaryFileName != null && !paymentProcess.IsDSTNetSalaryFileSubmitted);

            return (!paymentProcess.IsPayrollGenerated
                        || isAttendanceFileNotSubmitted
                        || isDSTNetSalaryFileNotSubmitted);
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

            var viewModel = new EmployeeMonthlyPayrollViewModel
            {
                Month = month,
                CompanyId = companyId,
                EmployeeId = employeeId,
                Employee = employee,
                IDNo = idNo,
                CostCenterIds = costCenterIds,
                WorkingDays = workingDays
            };
            viewModel.FilterInfo = await monthlyPayrollService.GetFilterInfoAsync(companyId, month);
            viewModel.Employees = await monthlyPayrollService.GetMonthlyPayrollAsync(viewModel);

            Tuple<string, string> tuple = rijndaelService.GenerateKey();
            ViewData["RijndaelKey"] = tuple.Item1;
            ViewData["RijndaelIV"] = tuple.Item2;

            foreach (EmployeeMonthlyPayrollModel employeePayroll in viewModel.Employees)
            {
                employeePayroll.BasicSalary.EbankingAmount = rijndaelService.Encrypt(employeePayroll.BasicSalary.EbankingAmount, tuple.Item1, tuple.Item2);
                employeePayroll.BasicSalary.DirectBanking1Amount = rijndaelService.Encrypt(employeePayroll.BasicSalary.DirectBanking1Amount, tuple.Item1, tuple.Item2);
                employeePayroll.BasicSalary.DirectBanking2Amount = rijndaelService.Encrypt(employeePayroll.BasicSalary.DirectBanking2Amount, tuple.Item1, tuple.Item2);
                employeePayroll.BasicSalary.CashAmount = rijndaelService.Encrypt(employeePayroll.BasicSalary.CashAmount, tuple.Item1, tuple.Item2);

                employeePayroll.Allowance.DirectBanking1Amount = rijndaelService.Encrypt(employeePayroll.Allowance.DirectBanking1Amount, tuple.Item1, tuple.Item2);
                employeePayroll.Allowance.DirectBanking2Amount = rijndaelService.Encrypt(employeePayroll.Allowance.DirectBanking2Amount, tuple.Item1, tuple.Item2);
                employeePayroll.Allowance.CashAmount = rijndaelService.Encrypt(employeePayroll.Allowance.CashAmount, tuple.Item1, tuple.Item2);
            }

            return View("MonthlyPayroll", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Read)]
        public async Task<IActionResult> EditBasicSalary(
            int payrollId,
            int companyId,
            DateTime month,
            CachingFilterViewModel filter)
        {
            var isRead = sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Read);
            if (!isRead)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            Tuple<MonthlyBasicSalaryModel,
                MonthlySalaryModel,
                IEnumerable<MonthlySalaryAdditionModel>,
                IEnumerable<MonthlySalaryDeductionModel>> monthlySalary = await monthlyPayrollService.GetMonthlySalaryAsync(payrollId);
            if (monthlySalary.Item1 == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            var keys = rijndaelService.GenerateKey();
            sessionService.RijndaelKey = keys.Item1;
            sessionService.RijndaelIV = keys.Item2;
            var viewModel = new EditMonthlySalaryViewModel
            {
                Month = month,
                MonthlyBasicSalary = ConvertMonthlyBasicSalaryToViewModel(monthlySalary.Item1, sessionService.RijndaelKey, sessionService.RijndaelIV),
                MonthlySalary = ConvertMonthlySalaryToViewModel(monthlySalary.Item2, sessionService.RijndaelKey, sessionService.RijndaelIV),
                Additions = ConvertMonthlySalaryAdditionToViewModel(monthlySalary.Item3, sessionService.RijndaelKey, sessionService.RijndaelIV),
                Deductions = ConvertMonthlySalaryDeductionToViewModel(monthlySalary.Item4, sessionService.RijndaelKey, sessionService.RijndaelIV),
                RijndaelKey = keys.Item1,
                RijndaelIV = keys.Item2,
                CachingFilter = filter
            };

            var criteria = new GetExchangeRateCriteria
            {
                Month = month.Month,
                Year = month.Year
            };

            var currencies = await baseService.GetDataAsync<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria());
            var cashAccounts = await baseService.GetDataAsync<ReferenceResourceModel>(
                new GetRefResourceByNamesCriteria { RefNames = ReferenceResource.PaymentTransactionCash, IsActive = true });

            viewModel.FilterInfo = await monthlyPayrollService.GetFilterInfoAsync(companyId, month, payrollId);
            viewModel.CashAccounts = commonService.GetSelectList(cashAccounts, nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value));
            viewModel.Currencies = commonService.GetSelectList(currencies, nameof(CurrencyCodeModel.CurrencyId), nameof(CurrencyCodeModel.CurrencyCode));
            viewModel.CurrencyExchangeRates = await baseService.GetDataAsync<CurrencyExchangeRateModel>(criteria);
            viewModel.VNDCurrencyId = currencies.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.VNDCurrencyCode, StringComparison.OrdinalIgnoreCase))?.CurrencyId ?? 0;

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> EditBasicSalary(EditMonthlySalaryViewModel viewModel)
        {
            var isHost = sessionService.HasPermissionOnCompany(viewModel.FilterInfo.CompanyId, FunctionKey.PaymentProcess, Policy.Host);

            if (!isHost)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            ModelState.Clear();
            DecryptData(viewModel);
            TryValidateModel(viewModel);

            if (ModelState.IsValid)
            {
                var monthlySalary = new Tuple<MonthlySalaryModel,
                    IEnumerable<MonthlySalaryAdditionModel>,
                    IEnumerable<MonthlySalaryDeductionModel>>(
                        ConvertMonthlySalaryToModel(viewModel.MonthlySalary),
                        ConvertMonthlySalaryAdditionToModel(viewModel.Additions),
                        ConvertMonthlySalaryDeductionToModel(viewModel.Deductions));
                CommandResult commandResult = await monthlyPayrollService.EditMonthlySalaryAsync(
                    viewModel.FilterInfo.PayrollId,
                    viewModel.FilterInfo.CompanyId,
                    viewModel.Month,
                    monthlySalary
                    );
                var redirectTo = viewModel.IsRedirectToEditAllowance
                    ? Url.Action("EditAllowances", new
                    {
                        payrollId = viewModel.FilterInfo.PayrollId,
                        companyId = viewModel.FilterInfo.CompanyId,
                        month = viewModel.Month,
                        employeeId = viewModel.CachingFilter.EmployeeId,
                        employee = viewModel.CachingFilter.Employee,
                        costCenterIds = viewModel.CachingFilter.CostCenterIds,
                        idNo = viewModel.CachingFilter.IDNo,
                        workingDays = viewModel.CachingFilter.WorkingDays
                    })
                    : Url.Action("EmployeeMonthlyPayroll", new
                    {
                        companyId = viewModel.FilterInfo.CompanyId,
                        month = viewModel.Month,
                        employeeId = viewModel.CachingFilter.EmployeeId,
                        employee = viewModel.CachingFilter.Employee,
                        costCenterIds = viewModel.CachingFilter.CostCenterIds,
                        idNo = viewModel.CachingFilter.IDNo,
                        workingDays = viewModel.CachingFilter.WorkingDays
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
        public async Task<IActionResult> GetHierarchyCompany(int companyId, string costCenterIds)
        {
            IEnumerable<MultiSelectViewModel> hierarchy = await monthlyPayrollService.GetHierarchyCompanyAsync(companyId, costCenterIds);

            return Json(hierarchy);
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
        public async Task<IActionResult> EditAllowances(int payrollId, int companyId, DateTime month, CachingFilterViewModel filter)
        {
            if (!sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Read))
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            var keys = rijndaelService.GenerateKey();
            sessionService.RijndaelIV = keys.Item1;
            sessionService.RijndaelKey = keys.Item2;

            var viewModel = await InitializeEditAllowanceViewModel(payrollId, companyId, month, filter);
            viewModel.RijndaelIV = keys.Item1;
            viewModel.RijndaelKey = keys.Item2;

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> EditAllowances(EditMonthlyAllowancesViewModel viewModel, EditMonthlyAllowanceAction submitAction)
        {
            if (!sessionService.HasPermissionOnCompany(viewModel.EmployeeInfo.CompanyId, FunctionKey.PaymentProcess, Policy.Host))
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            ModelState.Clear();
            DecryptData(viewModel);

            if (TryValidateModel(viewModel))
            {
                var result = await monthlyPayrollService.EditMonthlyAllowancesAsync(ConvertToServiceModel(viewModel), sessionService.UserId);
                var redirectTo = string.Empty;

                redirectTo = submitAction == EditMonthlyAllowanceAction.SaveAndEditBasicSalary ? Url.Action("EditBasicSalary", new
                {
                    companyId = viewModel.CompanyId,
                    month = viewModel.Month,
                    payrollId = viewModel.Allowance.PayrollId,
                    employeeId = viewModel.Filter.EmployeeId,
                    employee = viewModel.Filter.Employee,
                    costCenterIds = viewModel.Filter.CostCenterIds,
                    idNo = viewModel.Filter.IDNo,
                    workingDays = viewModel.Filter.WorkingDays
                }) : Url.Action("EmployeeMonthlyPayroll", new
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
                    isSuccessful = result.IsSuccessful,
                    message = result.Message,
                    redirectTo = result.IsSuccessful ? redirectTo : string.Empty
                });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> Preset(int companyId, int employeeId, DateTime month)
        {
            if (!sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host))
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            CommandResult result = await paymentProcessService.GenerateMonthlyPayrollAsync(companyId, month, employeeId);

            return Json(result);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> PresetAllMonthlyPayroll(int companyId, DateTime month)
        {
            if (!sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host))
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            CommandResult result = await paymentProcessService.GenerateMonthlyPayrollAsync(companyId, month, null);

            return Json(result);
        }

        private async Task<EditMonthlyAllowancesViewModel> InitializeEditAllowanceViewModel(int payrollId, int companyId, DateTime month, CachingFilterViewModel filter)
        {
            var serviceModel = await monthlyPayrollService.GetMonthlyAllowancesAsync(payrollId, month);

            var viewModel = new EditMonthlyAllowancesViewModel
            {
                BasicAllowance = InitializeBasicAllowanceModel(serviceModel.BasicAllowance),
                Allowance = InitializeAllowanceModel(serviceModel.Allowance),
                Additions = InitializeAdjustmentModel(serviceModel.Additions),
                Deductions = InitializeAdjustmentModel(serviceModel.Deductions)
            };
            viewModel.EmployeeInfo = await monthlyPayrollService.GetFilterInfoAsync(companyId, month, payrollId);
            viewModel.CompanyId = companyId;
            viewModel.Month = month;
            viewModel.Filter = filter;
            viewModel.DaysInMonth = DateTime.DaysInMonth(month.Year, month.Month);

            var currencies = await baseService.GetDataAsync<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria());
            viewModel.Currencies = commonService.GetSelectList(currencies, nameof(CurrencyCodeModel.CurrencyId), nameof(CurrencyCodeModel.CurrencyCode));

            viewModel.CurrencyExchangeRates = await baseService.GetDataAsync<CurrencyExchangeRateModel>(new GetExchangeRateCriteria
            {
                Month = month.Month,
                Year = month.Year
            });
            viewModel.CashAccounts = commonService.GetSelectList(await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNamesCriteria
            {
                RefNames = ReferenceResource.PaymentTransactionCash,
                IsActive = true
            }), nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value));

            viewModel.CanEdit = sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host)
                && DateTime.Today >= viewModel.EmployeeInfo.StartDate && DateTime.Today <= viewModel.EmployeeInfo.DueDate;
            viewModel.VNDCurrencyId = currencies.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.VNDCurrencyCode, StringComparison.OrdinalIgnoreCase))?.CurrencyId ?? 0;

            return viewModel;
        }

        private IList<MonthlyAllowanceDetailAdjustmentViewModel> InitializeAdjustmentModel(IList<MonthlyAllowanceAdditionModel> additions)
        {
            var rijndaelIV = sessionService.RijndaelIV;
            var rijndaelKey = sessionService.RijndaelKey;

            return additions.Select(x => new MonthlyAllowanceDetailAdjustmentViewModel
            {
                PayrollId = x.PayrollId,
                EmployeeId = x.EmployeeId,
                CurrencyId = x.CurrencyId,
                CurrencyCode = x.CurrencyCode,
                Type = x.Type,
                NoOfDays = x.NoOfDays,
                EncryptedValue = rijndaelService.Encrypt(x.UnitAmount, rijndaelKey, rijndaelIV),
                TotalAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(x.TotalAmount, rijndaelKey, rijndaelIV)
                },
                TotalAmountToDisplay = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(x.TotalAmountToDisplay, rijndaelKey, rijndaelIV)
                },
                NetAllowance = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(x.NetAllowance, rijndaelKey, rijndaelIV)
                },
                NetAllowanceToDisplay = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(x.NetAllowanceToDisplay, rijndaelKey, rijndaelIV)
                }
            }).ToList();
        }

        private IList<MonthlyAllowanceDetailAdjustmentViewModel> InitializeAdjustmentModel(IList<MonthlyAllowanceDeductionModel> deductions)
        {
            var rijndaelIV = sessionService.RijndaelIV;
            var rijndaelKey = sessionService.RijndaelKey;

            return deductions.Select(x => new MonthlyAllowanceDetailAdjustmentViewModel
            {
                PayrollId = x.PayrollId,
                EmployeeId = x.EmployeeId,
                CurrencyId = x.CurrencyId,
                CurrencyCode = x.CurrencyCode,
                Type = x.Type,
                NoOfDays = x.NoOfDays,
                EncryptedValue = rijndaelService.Encrypt(x.UnitAmount, rijndaelKey, rijndaelIV),
                NetAllowance = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(x.NetAllowance, rijndaelKey, rijndaelIV)
                },
                NetAllowanceToDisplay = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(x.NetAllowanceToDisplay, rijndaelKey, rijndaelIV)
                },
                TotalAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(x.TotalAmount, rijndaelKey, rijndaelIV)
                },
                TotalAmountToDisplay = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(x.TotalAmountToDisplay, rijndaelKey, rijndaelIV)
                }
            }).ToList();
        }

        private MonthlyAllowanceViewModel InitializeAllowanceModel(MonthlyAllowanceModel allowance)
        {
            var rijndaelIV = sessionService.RijndaelIV;
            var rijndaelKey = sessionService.RijndaelKey;

            return new MonthlyAllowanceViewModel
            {
                PayrollId = allowance.PayrollId,
                EmployeeId = allowance.EmployeeId,
                Status = allowance.Status,
                AutoCalculateOn = allowance.AutoCalculateOn,
                Total = new CurrencyAmountModel
                {
                    CurrencyId = allowance.CurrencyId,
                    CurrencyCode = allowance.CurrencyCode,
                    EncryptedValue = rijndaelService.Encrypt(allowance.TotalAmount, rijndaelKey, rijndaelIV)
                },
                DST = new CurrencyAmountModel
                {
                    CurrencyId = allowance.DSTNetSalaryCurrencyId,
                    CurrencyCode = allowance.DSTNetSalaryCurrencyCode,
                    EncryptedValue = rijndaelService.Encrypt(allowance.DSTNetSalaryAmount, rijndaelKey, rijndaelIV)
                },
                TotalNet = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(allowance.TotalNetAmount, rijndaelKey, rijndaelIV)
                },
                Addition = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(allowance.AdditionAmount, rijndaelKey, rijndaelIV)
                },
                AdditionDisplay = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(allowance.AdditionAmountDisplay, rijndaelKey, rijndaelIV)
                },
                Deduction = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(allowance.DeductionAmount, rijndaelKey, rijndaelIV)
                },
                DeductiontDisplay = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(allowance.DeductionAmountDisplay, rijndaelKey, rijndaelIV)
                },
                DirectBanking1 = new PaymentTransactionModel
                {
                    CurrencyId = allowance.DirectBanking1CurrencyId,
                    CurrencyCode = allowance.DirectBanking1CurrencyCode,
                    EncryptedValue = rijndaelService.Encrypt(allowance.DirectBanking1Amount, rijndaelKey, rijndaelIV),
                    AccountId = allowance.DirectBanking1AccountId,
                    AccountName = allowance.DirectBanking1AccountName
                },
                DirectBanking2 = new PaymentTransactionModel
                {
                    CurrencyId = allowance.DirectBanking2CurrencyId,
                    CurrencyCode = allowance.DirectBanking2CurrencyCode,
                    EncryptedValue = rijndaelService.Encrypt(allowance.DirectBanking2Amount, rijndaelKey, rijndaelIV),
                    AccountId = allowance.DirectBanking2AccountId,
                    AccountName = allowance.DirectBanking2AccountName
                },
                Cash = new PaymentTransactionModel
                {
                    CurrencyId = allowance.CashCurrencyId,
                    CurrencyCode = allowance.CashCurrencyCode,
                    EncryptedValue = rijndaelService.Encrypt(allowance.CashAmount, rijndaelKey, rijndaelIV),
                    AccountId = allowance.CashAccountId,
                }
            };
        }

        private MonthlyPayrollBasicAllowanceViewModel InitializeBasicAllowanceModel(MonthlyPayrollBasicAllowanceModel basicAllowance)
        {
            var rijndaelIV = sessionService.RijndaelIV;
            var rijndaelKey = sessionService.RijndaelKey;

            return new MonthlyPayrollBasicAllowanceViewModel
            {
                PayrollId = basicAllowance.PayrollId,
                EmployeeId = basicAllowance.EmployeeId,
                CurrencyId = basicAllowance.CurrencyId,
                CurrencyCode = basicAllowance.CurrencyCode,
                AutoCalculateOn = basicAllowance.AutoCalculateOn,
                Reason = basicAllowance.Reason,
                TotalAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(basicAllowance.TotalAmount, rijndaelKey, rijndaelIV)
                },
                AfterAdjustmentAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(basicAllowance.AfterAdjustmentAmount, rijndaelKey, rijndaelIV)
                },
                AfterAdjustmentAmountDisplay = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(basicAllowance.AfterAdjustmentAmountDisplay, rijndaelKey, rijndaelIV)
                },
                DirectBanking1CurrencyId = basicAllowance.DirectBanking1CurrencyId,
                DirectBanking1Amount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(basicAllowance.DirectBanking1Amount, rijndaelKey, rijndaelIV)
                },
                DirectBanking1AccountId = basicAllowance.DirectBanking1AccountId,
                DirectBanking1AccountName = basicAllowance.DirectBanking1AccountName,
                DirectBanking2CurrencyId = basicAllowance.DirectBanking2CurrencyId,
                DirectBanking2Amount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(basicAllowance.DirectBanking2Amount, rijndaelKey, rijndaelIV)
                },
                DirectBanking2AccountId = basicAllowance.DirectBanking2AccountId,
                DirectBanking2AccountName = basicAllowance.DirectBanking2AccountName,
                CashCurrencyId = basicAllowance.CashCurrencyId,
                CashAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(basicAllowance.CashAmount, rijndaelKey, rijndaelIV)
                },
                CashAccountId = basicAllowance.CashAccountId,
                SubsistenceCurrencyId = basicAllowance.SubsistenceCurrencyId,
                SubsistenceAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(basicAllowance.SubsistenceAmount, rijndaelKey, rijndaelIV)
                },
                HousingCurrencyId = basicAllowance.HousingCurrencyId,
                HousingAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(basicAllowance.HousingAmount, rijndaelKey, rijndaelIV)
                },
                HomePassageCurrencyId = basicAllowance.HomePassageCurrencyId,
                HomePassageAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(basicAllowance.HomePassageAmount, rijndaelKey, rijndaelIV)
                },
                MobilePhoneCurrencyId = basicAllowance.MobilePhoneCurrencyId,
                MobilePhoneAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(basicAllowance.MobilePhoneAmount, rijndaelKey, rijndaelIV)
                },
                DutyMealCurrencyId = basicAllowance.DutyMealCurrencyId,
                DutyMealAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(basicAllowance.DutyMealAmount, rijndaelKey, rijndaelIV)
                },
                HPICurrencyId = basicAllowance.HPICurrencyId,
                HPIAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(basicAllowance.HPIAmount, rijndaelKey, rijndaelIV)
                },
                OthersCurrencyId = basicAllowance.OthersCurrencyId,
                OthersAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(basicAllowance.OthersAmount, rijndaelKey, rijndaelIV)
                },
                SecondmentCurrencyId = basicAllowance.SecondmentCurrencyId,
                SecondmentAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(basicAllowance.SecondmentAmount, rijndaelKey, rijndaelIV)
                },
                RoomingChargesCurrencyId = basicAllowance.RoomingChargesCurrencyId,
                RoomingChargesAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(basicAllowance.RoomingChargesAmount, rijndaelKey, rijndaelIV)
                },
                FitnessFirstCurrencyId = basicAllowance.FitnessFirstCurrencyId,
                FitnessFirstAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(basicAllowance.FitnessFirstAmount, rijndaelKey, rijndaelIV)
                }
            };
        }

        private MonthlyAllowancesModel ConvertToServiceModel(EditMonthlyAllowancesViewModel viewModel)
        {
            return new MonthlyAllowancesModel
            {
                EmployeeId = viewModel.EmployeeId,
                CompanyId = viewModel.CompanyId,
                Month = viewModel.Month,
                Allowance = new MonthlyAllowanceModel
                {
                    PayrollId = viewModel.Allowance.PayrollId,
                    EmployeeId = viewModel.Allowance.PayrollId,
                    CurrencyId = viewModel.Allowance.Total.CurrencyId,
                    TotalAmount = viewModel.Allowance.Total.PlainValue,
                    DirectBanking1CurrencyId = viewModel.Allowance.DirectBanking1.CurrencyId,
                    DirectBanking1Amount = viewModel.Allowance.DirectBanking1.PlainValue,
                    DirectBanking1AccountId = viewModel.Allowance.DirectBanking1.AccountId,
                    DirectBanking2CurrencyId = viewModel.Allowance.DirectBanking2.CurrencyId,
                    DirectBanking2Amount = viewModel.Allowance.DirectBanking2.PlainValue,
                    DirectBanking2AccountId = viewModel.Allowance.DirectBanking2.AccountId,
                    CashCurrencyId = viewModel.Allowance.Cash.CurrencyId,
                    CashAmount = viewModel.Allowance.Cash.PlainValue,
                    CashAccountId = viewModel.Allowance.Cash.AccountId,
                    AutoCalculateOn = viewModel.Allowance.AutoCalculateOn,
                    TotalNetAmount = viewModel.Allowance.TotalNet.PlainValue,
                    DSTNetSalaryCurrencyId = viewModel.Allowance.DST.CurrencyId,
                    DSTNetSalaryAmount = viewModel.Allowance.DST.PlainValue,
                    AdditionAmount = viewModel.Allowance.Addition.PlainValue,
                    DeductionAmount = viewModel.Allowance.Deduction.PlainValue,
                },
                Additions = viewModel.Additions.Select(x => new MonthlyAllowanceAdditionModel
                {
                    PayrollId = x.PayrollId,
                    Type = x.Type,
                    CurrencyId = x.CurrencyId,
                    UnitAmount = x.PlainValue,
                    NoOfDays = x.NoOfDays,
                    TotalAmount = x.TotalAmount.PlainValue,
                    NetAllowance = x.NetAllowance.PlainValue
                }).ToList(),
                Deductions = viewModel.Deductions.Select(x => new MonthlyAllowanceDeductionModel
                {
                    PayrollId = x.PayrollId,
                    Type = x.Type,
                    CurrencyId = x.CurrencyId,
                    UnitAmount = x.PlainValue,
                    NoOfDays = x.NoOfDays,
                    TotalAmount = x.TotalAmount.PlainValue,
                    NetAllowance = x.NetAllowance.PlainValue
                }).ToList()
            };
        }

        private void DecryptData(EditMonthlyAllowancesViewModel viewModel)
        {
            var rijndaelIV = sessionService.RijndaelIV;
            var rijndaelKey = sessionService.RijndaelKey;

            rijndaelService.DecryptObject(viewModel.Allowance, rijndaelKey, rijndaelIV);

            foreach (var addition in viewModel.Additions)
            {
                addition.PlainValue = rijndaelService.Decrypt(addition.EncryptedValue, rijndaelKey, rijndaelIV);
                rijndaelService.DecryptObject(addition, rijndaelKey, rijndaelIV);
            }

            foreach (var deduction in viewModel.Deductions)
            {
                deduction.PlainValue = rijndaelService.Decrypt(deduction.EncryptedValue, rijndaelKey, rijndaelIV);
                rijndaelService.DecryptObject(deduction, rijndaelKey, rijndaelIV);
            }
        }

        #endregion Monthly Payroll

        #region EditBasicSalary

        private MonthlyBasicSalaryViewModel ConvertMonthlyBasicSalaryToViewModel(
            MonthlyBasicSalaryModel monthlyBasicSalary,
            string rijndaelKey,
            string rijndaelIV)
        {
            return new MonthlyBasicSalaryViewModel
            {
                CurrencyId = monthlyBasicSalary.CurrencyId,
                CurrencyCode = monthlyBasicSalary.CurrencyCode,
                Salary = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(monthlyBasicSalary.Salary, rijndaelKey, rijndaelIV)
                },
                EbankingCurrencyId = monthlyBasicSalary.EbankingCurrencyId,
                EbankingCurrencyCode = monthlyBasicSalary.EbankingCurrencyCode,
                EbankingAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(monthlyBasicSalary.EbankingAmount, rijndaelKey, rijndaelIV)
                },
                DirectBanking1CurrencyId = monthlyBasicSalary.DirectBanking1CurrencyId,
                DirectBanking1CurrencyCode = monthlyBasicSalary.DirectBanking1CurrencyCode,
                DirectBanking1Amount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(monthlyBasicSalary.DirectBanking1Amount, rijndaelKey, rijndaelIV)
                },
                DirectBanking2CurrencyId = monthlyBasicSalary.DirectBanking2CurrencyId,
                DirectBanking2CurrencyCode = monthlyBasicSalary.DirectBanking2CurrencyCode,
                DirectBanking2Amount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(monthlyBasicSalary.DirectBanking2Amount, rijndaelKey, rijndaelIV)
                },
                CashCurrencyId = monthlyBasicSalary.CashCurrencyId,
                CashCurrencyCode = monthlyBasicSalary.CashCurrencyCode,
                CashAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(monthlyBasicSalary.CashAmount, rijndaelKey, rijndaelIV)
                },
                AfterAdjustmentAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(monthlyBasicSalary.AfterAdjustmentAmount, rijndaelKey, rijndaelIV)
                },
                AfterAdjustmentReason = monthlyBasicSalary.AfterAdjustmentReason,
                SalaryPerDay = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(monthlyBasicSalary.SalaryPerDay.ToString(CultureInfo.InvariantCulture), rijndaelKey, rijndaelIV)
                },
                WorkingDays = monthlyBasicSalary.WorkingDays
            };
        }

        private MonthlySalaryViewModel ConvertMonthlySalaryToViewModel(
            MonthlySalaryModel monthlySalary,
            string rijndaelKey,
            string rijndaelIV)
        {
            return new MonthlySalaryViewModel
            {
                Salary = new CurrencyAmountModel
                {
                    CurrencyId = monthlySalary.CurrencyId,
                    EncryptedValue = rijndaelService.Encrypt(monthlySalary.Salary, rijndaelKey, rijndaelIV)
                },
                Ebanking = new PaymentTransactionModel
                {
                    CurrencyId = monthlySalary.EbankingCurrencyId,
                    EncryptedValue = rijndaelService.Encrypt(monthlySalary.EbankingAmount, rijndaelKey, rijndaelIV),
                    AccountId = monthlySalary.EbankingAccountId,
                    AccountName = monthlySalary.EbankingAccountName
                },
                DirectBanking1 = new PaymentTransactionModel
                {
                    CurrencyId = monthlySalary.DirectBanking1CurrencyId,
                    EncryptedValue = rijndaelService.Encrypt(monthlySalary.DirectBanking1Amount, rijndaelKey, rijndaelIV),
                    AccountId = monthlySalary.DirectBanking1AccountId,
                    AccountName = monthlySalary.DirectBanking1AccountName
                },
                DirectBanking2 = new PaymentTransactionModel
                {
                    CurrencyId = monthlySalary.DirectBanking2CurrencyId,
                    EncryptedValue = rijndaelService.Encrypt(monthlySalary.DirectBanking2Amount, rijndaelKey, rijndaelIV),
                    AccountId = monthlySalary.DirectBanking2AccountId,
                    AccountName = monthlySalary.DirectBanking2AccountName
                },
                Cash = new PaymentTransactionModel
                {
                    CurrencyId = monthlySalary.CashCurrencyId,
                    EncryptedValue = rijndaelService.Encrypt(monthlySalary.CashAmount, rijndaelKey, rijndaelIV),
                    AccountId = monthlySalary.CashAccountId
                },
                AutoCalculateOn = monthlySalary.AutoCalculateOn,
                AdditionAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(monthlySalary.AdditionAmount, rijndaelKey, rijndaelIV)
                },
                AdditionAmountDisplay = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(monthlySalary.AdditionAmountDisplay, rijndaelKey, rijndaelIV)
                },
                DeductionAmount = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(monthlySalary.DeductionAmount, rijndaelKey, rijndaelIV)
                },
                DeductionAmountDisplay = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(monthlySalary.DeductionAmountDisplay, rijndaelKey, rijndaelIV)
                }
            };
        }

        private IList<MonthlySalaryAdditionViewModel> ConvertMonthlySalaryAdditionToViewModel(
            IEnumerable<MonthlySalaryAdditionModel> additions,
            string rijndaelKey,
            string rijndaelIV)
        {
            return additions.Select(x => new MonthlySalaryAdditionViewModel
            {
                Type = x.Type,
                EncryptedValue = rijndaelService.Encrypt(x.UnitAmount, rijndaelKey, rijndaelIV),
                NoOfDays = x.NoOfDays,
                NetSalary = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(x.NetSalary, rijndaelKey, rijndaelIV)
                },
                NetSalaryDisplay = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(x.NetSalaryDisplay, rijndaelKey, rijndaelIV)
                },
                Description = x.Description
            }).ToList();
        }

        private IList<MonthlySalaryDeductionViewModel> ConvertMonthlySalaryDeductionToViewModel(
            IEnumerable<MonthlySalaryDeductionModel> deductions,
            string rijndaelKey,
            string rijndaelIV)
        {
            return deductions.Select(x => new MonthlySalaryDeductionViewModel
            {
                Type = x.Type,
                EncryptedValue = rijndaelService.Encrypt(x.UnitAmount, rijndaelKey, rijndaelIV),
                NoOfDays = x.NoOfDays,
                NetSalary = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(x.NetSalary, rijndaelKey, rijndaelIV)
                },
                NetSalaryDisplay = new SecurityData
                {
                    EncryptedValue = rijndaelService.Encrypt(x.NetSalaryDisplay, rijndaelKey, rijndaelIV)
                },
                Description = x.Description
            }).ToList();
        }

        private void DecryptData(EditMonthlySalaryViewModel viewModel)
        {
            var rijndaelIV = sessionService.RijndaelIV;
            var rijndaelKey = sessionService.RijndaelKey;

            rijndaelService.DecryptObject(viewModel.MonthlySalary, rijndaelKey, rijndaelIV);

            foreach (MonthlySalaryAdditionViewModel addition in viewModel.Additions)
            {
                addition.PlainValue = rijndaelService.Decrypt(addition.EncryptedValue, rijndaelKey, rijndaelIV);
                rijndaelService.DecryptObject(addition, rijndaelKey, rijndaelIV);
            }

            foreach (MonthlySalaryDeductionViewModel deduction in viewModel.Deductions)
            {
                deduction.PlainValue = rijndaelService.Decrypt(deduction.EncryptedValue, rijndaelKey, rijndaelIV);
                rijndaelService.DecryptObject(deduction, rijndaelKey, rijndaelIV);
            }
        }

        private MonthlySalaryModel ConvertMonthlySalaryToModel(MonthlySalaryViewModel monthlySalary)
        {
            return new MonthlySalaryModel
            {
                Salary = monthlySalary.Salary.PlainValue,
                EbankingCurrencyId = monthlySalary.Ebanking.CurrencyId,
                EbankingAmount = monthlySalary.Ebanking.PlainValue,
                EbankingAccountId = monthlySalary.Ebanking.AccountId,
                DirectBanking1CurrencyId = monthlySalary.DirectBanking1.CurrencyId,
                DirectBanking1Amount = monthlySalary.DirectBanking1.PlainValue,
                DirectBanking1AccountId = monthlySalary.DirectBanking1.AccountId,
                DirectBanking2CurrencyId = monthlySalary.DirectBanking2.CurrencyId,
                DirectBanking2Amount = monthlySalary.DirectBanking2.PlainValue,
                DirectBanking2AccountId = monthlySalary.DirectBanking2.AccountId,
                CashCurrencyId = monthlySalary.Cash.CurrencyId,
                CashAmount = monthlySalary.Cash.PlainValue,
                CashAccountId = monthlySalary.Cash.AccountId,
                AdditionAmount = monthlySalary.AdditionAmount.PlainValue,
                DeductionAmount = monthlySalary.DeductionAmount.PlainValue,
                AutoCalculateOn = monthlySalary.AutoCalculateOn
            };
        }

        private IList<MonthlySalaryAdditionModel> ConvertMonthlySalaryAdditionToModel(IEnumerable<MonthlySalaryAdditionViewModel> additions)
        {
            return additions.Select(x => new MonthlySalaryAdditionModel
            {
                Type = x.Type,
                UnitAmount = x.PlainValue,
                NoOfDays = x.NoOfDays,
                NetSalary = x.NetSalary.PlainValue,
                Description = x.Description
            }).ToList();
        }

        private IList<MonthlySalaryDeductionModel> ConvertMonthlySalaryDeductionToModel(IEnumerable<MonthlySalaryDeductionViewModel> deductions)
        {
            return deductions.Select(x => new MonthlySalaryDeductionModel
            {
                Type = x.Type,
                UnitAmount = x.PlainValue,
                NoOfDays = x.NoOfDays,
                NetSalary = x.NetSalary.PlainValue,
                Description = x.Description
            }).ToList();
        }

        #endregion EditBasicSalary
    }
}