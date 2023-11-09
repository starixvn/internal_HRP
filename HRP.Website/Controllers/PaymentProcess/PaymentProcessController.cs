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
    using HRP.Models;
    using HRP.Models.PaymentProcess;
    using HRP.Resources;
    using HRP.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public partial class PaymentProcessController : BaseController
    {
        private const string EmployeeMonthlyPayrollString = "EmployeeMonthlyPayroll";
        private const string AccessDenied = "AccessDenied";
        private const string Error = "Error";
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ICommonService commonService;
        private readonly IPaymentProcessService paymentProcessService;
        private readonly IMonthlyPayrollService monthlyPayrollService;
        private readonly IMonthlySalaryService monthlySalaryService;
        private readonly IMonthlyAllowancesService monthlyAllowancesService;
        private readonly IRijndaelService rijndaelService;

        public PaymentProcessController(
            IBaseService baseService,
            ISessionService sessionService,
            IHostingEnvironment hostingEnvironment,
            ICommonService commonService,
            IPaymentProcessService paymentProcessService,
            IMonthlyPayrollService monthlyPayrollService,
            IMonthlySalaryService monthlySalaryService,
            IMonthlyAllowancesService monthlyAllowancesService,
            IRijndaelService rijndaelService) : base(baseService, sessionService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.commonService = commonService;
            this.paymentProcessService = paymentProcessService;
            this.monthlyPayrollService = monthlyPayrollService;
            this.monthlySalaryService = monthlySalaryService;
            this.monthlyAllowancesService = monthlyAllowancesService;
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

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Read)]
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
                            ? RedirectToAction("UploadFiles", new { companyId = viewModel.CompanyId, month = viewModel.SelectedTime })
                            : RedirectToAction(EmployeeMonthlyPayrollString, new { companyId = viewModel.CompanyId, month = viewModel.SelectedTime });
                    }

                    ModelState.AddModelError(nameof(viewModel.CommonError), Message.NotHasCurrencySetting);
                }
                else
                {
                    if (paymentProcess.IsPayrollGenerated)
                    {
                        return RedirectToAction(EmployeeMonthlyPayrollString, new { companyId = viewModel.CompanyId, month = viewModel.SelectedTime });
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
            const string sFileName = @"ExcelTemplates\Template - Attendance (Expat).xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            MemoryStream stream = await paymentProcessService.CopyFileToMemoryStreamAsync(path);

            return File(stream, MimeType.XLSX, Path.GetFileName(path));
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> DownloadDSTNetSalary()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            const string sFileName = @"ExcelTemplates\Template - DST Salary (Expat).xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            MemoryStream stream = await paymentProcessService.CopyFileToMemoryStreamAsync(path);

            return File(stream, MimeType.XLSX, Path.GetFileName(path));
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> DownloadAdditionDeduction()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            const string sFileName = @"ExcelTemplates\Template - Addition - Deduction.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            MemoryStream stream = await paymentProcessService.CopyFileToMemoryStreamAsync(path);

            return File(stream, MimeType.XLSX, Path.GetFileName(path));
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> UploadFiles(int companyId, DateTime month)
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

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> UploadFiles(UploadFilesViewModel viewModel)
        {
            if (viewModel.AttendanceFile == null && viewModel.DSTNetSalaryFile == null && viewModel.AdditionDeductionFile == null)
            {
                ModelState.AddModelError(nameof(viewModel.CommonError), Message.PleaseSelectFile);
            }

            Result<AttendanceTemplate> readAttendanceResult = await paymentProcessService.ReadAttendanceFileAsync(viewModel);
            Result<DSTNetSalaryTemplate> readDSTNetSalaryResult = await paymentProcessService.ReadDSTNetSalaryFileAsync(viewModel);
            Result<AdditionDeductionTemplate> readAdditionDeductionResult = await paymentProcessService.ReadAdditionDeductionFileAsync(viewModel);
            if (readAttendanceResult is Failure<AttendanceTemplate> failureAttendance)
            {
                ModelState.AddModelError(nameof(viewModel.AttendanceFile), string.Concat(ViewLang.AttendanceFile, ": ", failureAttendance.Message));
            }

            if (readDSTNetSalaryResult is Failure<DSTNetSalaryTemplate> failureDSTNetSalary)
            {
                ModelState.AddModelError(nameof(viewModel.DSTNetSalaryFile), string.Concat(ViewLang.DSTNetSalaryFile, ": ", failureDSTNetSalary.Message));
            }

            if (readAdditionDeductionResult is Failure<AdditionDeductionTemplate> failureAdditionDeduction)
            {
                ModelState.AddModelError(nameof(viewModel.AdditionDeductionFile), string.Concat(ViewLang.UploadAdditionAndDeductionFile, ": ", failureAdditionDeduction.Message));
            }

            if (ModelState.IsValid)
            {
                var attendanceResult = (Success<AttendanceTemplate>)readAttendanceResult;
                var dstNetSalaryResult = (Success<DSTNetSalaryTemplate>)readDSTNetSalaryResult;
                var additionDeductionResult = (Success<AdditionDeductionTemplate>)readAdditionDeductionResult;
                CommandResult uploadFiles = await paymentProcessService.UploadFilesAsync(
                    viewModel.CompanyId,
                    viewModel.SelectedTime,
                    attendanceResult.Value,
                    dstNetSalaryResult.Value,
                    additionDeductionResult.Value);

                if (uploadFiles.IsSuccessful)
                {
                    return Json(new { isSuccessful = true });
                }

                ModelState.AddModelError(nameof(viewModel.CommonError), uploadFiles.Message);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors[0].ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> ValidationFilesResult(int companyId, DateTime month)
        {
            UploadFilesViewModel viewModel = await paymentProcessService.GetUploadingFilesResultAsync(companyId, month);

            return PartialView("_ValidationFilesResult", viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> GetPaymentProcess(int companyId, DateTime month)
        {
            PaymentProcess processInfo = await paymentProcessService.GetPaymentProcessAsync(companyId, month);

            return Json(processInfo);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> ProcessActionNextToMonthlyPayroll(int companyId, DateTime month)
        {
            CommandResult submitResult = await paymentProcessService.SubmitUploadingFilesAsync(companyId, month);
            if (submitResult.IsSuccessful)
            {
                CommandResult generateResult = await paymentProcessService.GenerateMonthlyPayrollAsync(companyId, month);
                if (generateResult.IsSuccessful)
                {
                    return Json(new { isSuccessful = true });
                }

                ModelState.AddModelError(nameof(UploadFilesViewModel.CommonError), generateResult.Message);
            }
            else
            {
                ModelState.AddModelError(nameof(UploadFilesViewModel.CommonError), submitResult.Message);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors[0].ErrorMessage);
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

            ModelState.AddModelError(nameof(UploadFilesViewModel.CommonError), deleteResult.Message);

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors[0].ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> DeleteUploadedDSTNetSalary(int companyId, DateTime month)
        {
            CommandResult deleteSalaryResult = await paymentProcessService.DeleteUploadedDSTNetSalary(companyId, month);
            if (deleteSalaryResult.IsSuccessful)
            {
                return Json(new { isSuccessful = true });
            }

            ModelState.AddModelError(nameof(UploadFilesViewModel.CommonError), deleteSalaryResult.Message);

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors[0].ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.PaymentProcess, Policy = Policy.Host)]
        public async Task<IActionResult> DeleteUploadedAdditionDeduction(int companyId, DateTime month)
        {
            CommandResult deleteSalaryResult = await paymentProcessService.DeleteUploadedAdditionDeductionAsync(companyId, month);
            if (deleteSalaryResult.IsSuccessful)
            {
                return Json(new { isSuccessful = true });
            }

            ModelState.AddModelError(nameof(UploadFilesViewModel.CommonError), deleteSalaryResult.Message);

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors[0].ErrorMessage);
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
            var today = DateTimeOffset.Now.Date;
            var previousMonth = today.AddMonths(-1);
            return (paymentProcess.StartDate <= previousMonth && previousMonth <= paymentProcess.DueDate)
                || (paymentProcess.StartDate <= today && today <= paymentProcess.DueDate);
        }

        #endregion UploadAttendanceAndDSTSalary
    }
}