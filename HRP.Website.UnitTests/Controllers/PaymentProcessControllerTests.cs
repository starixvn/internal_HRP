namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Extensions;
    using Core.Model;
    using HRP.Core.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Routing;
    using Models;
    using Models.PaymentProcess;
    using NSubstitute;
    using Resources;
    using Services;
    using Xunit;

    public partial class PaymentProcessControllerTests
    {
        private static readonly DateTime startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        private static readonly DateTime dueDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
        private static readonly DateTime selectedTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 14);
        private IBaseService baseService;
        private ISessionService sessionService;
        private IHostingEnvironment hostingEnvironment;
        private ICommonService commonService;
        private IPaymentProcessService paymentProcessService;
        private IMonthlyPayrollService monthlyPayrollService;
        private IMonthlySalaryService monthlySalaryService;
        private IMonthlyAllowancesService monthlyAllowancesService;
        private IRijndaelService rijndaelService;

        [Fact]
        public async Task Index_WhenCalled_ReturnExpectedViewResult()
        {
            // Arrange
            var selectList = MockCompanySelectList();
            var paymentProcess = new PaymentProcess
            {
                Month = selectedTime,
                StartDate = startDate,
                DueDate = dueDate
            };

            var controller = GetPaymentProcessController();
            paymentProcessService.GetCurrentDuration().Returns(paymentProcess);
            commonService.GetSelectList(Arg.Any<IEnumerable<Company>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(selectList);

            // Act
            var actual = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsType<SelectWorkingMonthCompanyViewModel>(viewResult.Model);
            Assert.Equal(selectList, viewModel.Companies);
            Assert.Equal(paymentProcess.Month, viewModel.SelectedTime);
            Assert.Equal(paymentProcess.StartDate, viewModel.StartDate);
            Assert.Equal(paymentProcess.DueDate, viewModel.DueDate);
        }

        [Fact]
        public async Task IndexPost_ModelStateIsInvalid_ReturnIndexViewResultWithViewModel()
        {
            // Arrange
            var selectList = MockCompanySelectList();
            var viewModel = MockSelectWorkingMonthCompanyViewModel();
            var controller = GetPaymentProcessController();
            controller.ModelState.AddModelError(nameof(viewModel.CompanyId), "Missing company id");
            commonService.GetSelectList(Arg.Any<IEnumerable<Company>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(selectList);

            // Act
            var actual = await controller.Index(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewDataModel = Assert.IsType<SelectWorkingMonthCompanyViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.Equal(selectList, viewDataModel.Companies);
            Assert.Equal(viewModel.SelectedTime, viewDataModel.SelectedTime);
            Assert.Equal(viewModel.StartDate, viewDataModel.StartDate);
            Assert.Equal(viewModel.DueDate, viewDataModel.DueDate);
        }

        [Fact]
        public async Task IndexPost_UserHasHostPermissionAndNotHasCurrencySetting_ReturnIndexViewResultWithModelStateError()
        {
            // Arrange
            var selectList = MockCompanySelectList();
            var viewModel = new SelectWorkingMonthCompanyViewModel { CompanyId = 23, SelectedTime = selectedTime };
            var paymentProcess = new PaymentProcess
            {
                StartDate = startDate,
                DueDate = dueDate,
                AttendanceFileName = "attendanceFile.xlsx",
                IsAttendanceFileSubmitted = true,
                DSTNetSalaryFileName = "salaryFile.xls",
                IsDSTNetSalaryFileSubmitted = true,
                IsPayrollGenerated = true,
                IsHasCurrencySetting = false
            };

            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.PaymentProcess, Policy.Host).Returns(true);
            paymentProcessService.GetPaymentProcessAsync(viewModel.CompanyId, viewModel.SelectedTime).Returns(paymentProcess);
            commonService.GetSelectList(Arg.Any<IEnumerable<Company>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(selectList);

            // Act
            var actual = await controller.Index(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewData = Assert.IsType<SelectWorkingMonthCompanyViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.Equal(Message.NotHasCurrencySetting, viewResult.ViewData.ModelState["CommonError"].Errors[0].ErrorMessage);
            Assert.Equal(selectList, viewData.Companies);
            Assert.Equal(viewModel.SelectedTime, viewData.SelectedTime);
            Assert.Equal(viewModel.StartDate, viewData.StartDate);
            Assert.Equal(viewModel.DueDate, viewData.DueDate);
        }

        [Fact]
        public async Task IndexPost_UserHasHostPermissionAndPayrollIsGenerated_ReturnRedirectToEmployeeMonthlyPayroll()
        {
            // Arrange
            var paymentProcess = new PaymentProcess
            {
                StartDate = startDate,
                DueDate = dueDate,
                AttendanceFileName = "attendanceFile.xlsx",
                IsAttendanceFileSubmitted = true,
                DSTNetSalaryFileName = "salaryFile.xls",
                IsDSTNetSalaryFileSubmitted = true,
                IsPayrollGenerated = true,
                IsHasCurrencySetting = true
            };

            var paramModel = new SelectWorkingMonthCompanyViewModel { CompanyId = 23, SelectedTime = selectedTime };
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(paramModel.CompanyId, FunctionKey.PaymentProcess, Policy.Host).Returns(true);
            paymentProcessService.GetPaymentProcessAsync(paramModel.CompanyId, paramModel.SelectedTime).Returns(paymentProcess);

            // Act
            var actual = await controller.Index(paramModel);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            var routeValues = viewResult.RouteValues;
            Assert.Equal("EmployeeMonthlyPayroll", viewResult.ActionName);
            Assert.Equal(paramModel.CompanyId, routeValues["companyId"]);
            Assert.Equal(paramModel.SelectedTime, routeValues["month"]);
        }

        [Theory]
        [InlineData(false, true, true)]
        [InlineData(true, false, true)]
        [InlineData(true, true, false)]
        public async Task IndexPost_UserHasHostPermissionAndPayrollIsNotGenerated_ReturnRedirectToUploadFiles
            (bool isPayrollGenerated, bool isAttendanceSubmitted, bool isDSTNetSalarySubmitted)
        {
            // Arrange
            var paymentProcess = new PaymentProcess
            {
                StartDate = startDate,
                DueDate = dueDate,
                AttendanceFileName = "attendanceFile.xlsx",
                IsAttendanceFileSubmitted = isAttendanceSubmitted,
                DSTNetSalaryFileName = "salaryFile.xls",
                IsDSTNetSalaryFileSubmitted = isDSTNetSalarySubmitted,
                IsPayrollGenerated = isPayrollGenerated,
                IsHasCurrencySetting = true
            };
            var paramModel = new SelectWorkingMonthCompanyViewModel { CompanyId = 23, SelectedTime = selectedTime };
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(paramModel.CompanyId, FunctionKey.PaymentProcess, Policy.Host).Returns(true);
            paymentProcessService.GetPaymentProcessAsync(paramModel.CompanyId, paramModel.SelectedTime).Returns(paymentProcess);

            // Act
            var actual = await controller.Index(paramModel);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            var routeValues = viewResult.RouteValues;
            Assert.Equal("UploadFiles", viewResult.ActionName);
            Assert.Equal(paramModel.CompanyId, routeValues["companyId"]);
            Assert.Equal(paramModel.SelectedTime, routeValues["month"]);
        }

        [Theory]
        [InlineData("2018-3-1", "2018-3-30", false)]
        [InlineData("2018-3-1", "2018-3-11", true)]
        public async Task IndexPost_UserHasReadPermissionAndPayrollIsGenerated_ReturnRedirectToEmployeeMonthlyPayroll
            (string startDate, string dueDate, bool isHostOnCompany)
        {
            // Arrange
            var paymentProcess = new PaymentProcess
            {
                StartDate = DateTime.Parse(startDate),
                DueDate = DateTime.Parse(dueDate),
                IsPayrollGenerated = true
            };
            var paramModel = new SelectWorkingMonthCompanyViewModel { CompanyId = 23, SelectedTime = DateTime.Parse("2018-3-15") };
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(paramModel.CompanyId, FunctionKey.PaymentProcess, Policy.Host).Returns(isHostOnCompany);
            paymentProcessService.GetPaymentProcessAsync(paramModel.CompanyId, paramModel.SelectedTime).Returns(paymentProcess);

            // Act
            var actual = await controller.Index(paramModel);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            var routeValues = viewResult.RouteValues;
            Assert.Equal("EmployeeMonthlyPayroll", viewResult.ActionName);
            Assert.Equal(paramModel.CompanyId, routeValues["companyId"]);
            Assert.Equal(paramModel.SelectedTime, routeValues["month"]);
        }

        [Theory]
        [InlineData("2018-3-1", "2018-3-30", false)]
        [InlineData("2018-3-1", "2018-3-11", true)]
        public async Task IndexPost_UserHasReadPermissionAndPayrollIsNotGenerated_ReturnIndexViewResultWithModelStateError
            (string startDate, string dueDate, bool isHostOnCompany)
        {
            // Arrange
            var selectList = MockCompanySelectList();
            var paymentProcess = new PaymentProcess
            {
                StartDate = DateTime.Parse(startDate),
                DueDate = DateTime.Parse(dueDate),
                IsPayrollGenerated = false
            };
            var paramModel = new SelectWorkingMonthCompanyViewModel { CompanyId = 23, SelectedTime = DateTime.Parse("2018-3-15") };
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(paramModel.CompanyId, FunctionKey.PaymentProcess, Policy.Host).Returns(isHostOnCompany);
            paymentProcessService.GetPaymentProcessAsync(paramModel.CompanyId, paramModel.SelectedTime).Returns(paymentProcess);
            commonService.GetSelectList(Arg.Any<IEnumerable<Company>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(selectList);

            // Act
            var actual = await controller.Index(paramModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.Equal(Message.PayrollNotGenerated, viewResult.ViewData.ModelState["CommonError"].Errors[0].ErrorMessage);
        }

        #region UploadAttendanceAndDSTSalary

        [Fact]
        public async Task DownloadAttendance_WhenCalled_ReturnExcelTemplateFile()
        {
            // Arrange
            var filePath = string.Empty;
            var controller = GetPaymentProcessController();
            hostingEnvironment.WebRootPath.Returns(string.Empty);
            paymentProcessService.CopyFileToMemoryStreamAsync(Arg.Do<string>(x => filePath = x)).Returns(new MemoryStream());

            // Act
            var actual = await controller.DownloadAttendance();

            // Assert
            var fileStream = Assert.IsType<FileStreamResult>(actual);
            Assert.Equal(@"ExcelTemplates/Template - Attendance (Expat).xlsx", filePath);
            Assert.Equal(MimeType.XLSX, fileStream.ContentType);
            Assert.Equal("Template - Attendance (Expat).xlsx", fileStream.FileDownloadName);
        }

        [Fact]
        public async Task DownloadDSTNetSalary_WhenCalled_ReturnExcelTemplateFile()
        {
            // Arrange
            var filePath = string.Empty;
            var controller = GetPaymentProcessController();
            hostingEnvironment.WebRootPath.Returns(string.Empty);
            paymentProcessService.CopyFileToMemoryStreamAsync(Arg.Do<string>(x => filePath = x)).Returns(new MemoryStream());

            // Act
            var actual = await controller.DownloadDSTNetSalary();

            // Assert
            var fileStream = Assert.IsType<FileStreamResult>(actual);
            Assert.Equal(@"ExcelTemplates/Template - DST Salary (Expat).xlsx", filePath);
            Assert.Equal(MimeType.XLSX, fileStream.ContentType);
            Assert.Equal("Template - DST Salary (Expat).xlsx", fileStream.FileDownloadName);
        }

        [Fact]
        public async Task UploadFiles_UserHasPermission_ReturnViewResultWithExpectedViewModel()
        {
            // Arrange
            var companyId = 13;
            var month = selectedTime;
            var paymentProcess = new PaymentProcess
            {
                StartDate = startDate,
                DueDate = dueDate,
            };
            var expected = new UploadFilesViewModel
            {
                CompanyId = companyId,
                SelectedTime = month
            };
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host).Returns(true);
            paymentProcessService.GetPaymentProcessAsync(companyId, month).Returns(paymentProcess);
            paymentProcessService.GetUploadingFilesResultAsync(companyId, month).Returns(expected);

            // Act
            var actual = await controller.UploadFiles(companyId, month);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewData = Assert.IsType<UploadFilesViewModel>(viewResult.ViewData.Model);
            Assert.Equal(expected, viewData);
        }

        [Theory]
        [InlineData("2018-3-1", "2018-3-30", false)]
        [InlineData("2018-3-1", "2018-3-11", true)]
        public async Task UploadFiles_UserNotHasPermission_ReturnViewResultWithExpectedViewModel
            (string startDate, string dueDate, bool isHostOnCompany)
        {
            // Arrange
            var companyId = 13;
            var month = DateTime.Parse("2018-3-15");
            var paymentProcess = new PaymentProcess
            {
                StartDate = DateTime.Parse(startDate),
                DueDate = DateTime.Parse(dueDate)
            };
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host).Returns(isHostOnCompany);
            paymentProcessService.GetPaymentProcessAsync(companyId, month).Returns(paymentProcess);

            // Act
            var actual = await controller.UploadFiles(companyId, month);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Null(viewResult.ControllerName);
            Assert.Equal("Index", viewResult.ActionName);
        }

        [Fact]
        public async Task ValidationFilesResult_WhenCalled_ReturnPartialViewWithValidationFilesResultViewModel()
        {
            // Arrange
            var expected = new UploadFilesViewModel
            {
                AttendanceResult = new FileResultViewModel { TotalRow = 3 },
                DSTNetSalaryResult = new FileResultViewModel { TotalRow = 4 }
            };
            var companyId = 9;
            DateTime month = DateTime.Now;
            var controller = GetPaymentProcessController();
            paymentProcessService.GetUploadingFilesResultAsync(companyId, month).Returns(expected);

            // Act
            var actual = await controller.ValidationFilesResult(companyId, month);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var model = Assert.IsType<UploadFilesViewModel>(viewResult.Model);
            Assert.Equal("_ValidationFilesResult", viewResult.ViewName);
            Assert.Equal(expected, model);
        }

        [Fact]
        public async Task GetPaymentProcess_WhenCalled_ReturnJsonResultWithExpectedData()
        {
            // Arrange
            var companyId = 19;
            DateTime month = DateTime.Today;
            var expected = new PaymentProcess
            {
                CompanyId = companyId,
                Month = month
            };
            var controller = GetPaymentProcessController();
            paymentProcessService.GetPaymentProcessAsync(companyId, month).Returns(expected);

            // Act
            var actual = await controller.GetPaymentProcess(companyId, month);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task ProcessActionNextToMonthlyPayroll_SubmitUploadingFilesNotSuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var companyId = 17;
            DateTime month = DateTime.Today;
            var submitResult = new CommandResult
            {
                Code = 1,
                Message = "Submit error"
            };
            var controller = GetPaymentProcessController();
            paymentProcessService.SubmitUploadingFilesAsync(companyId, month).Returns(submitResult);

            // Act
            var actual = await controller.ProcessActionNextToMonthlyPayroll(companyId, month);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal("Submit error", errors["CommonError"]);
        }

        [Fact]
        public async Task ProcessActionNextToMonthlyPayroll_GeneratePayrollNotSuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var companyId = 17;
            DateTime month = DateTime.Today;
            var submitResult = new CommandResult { Code = 0 };
            var generateResult = new CommandResult { Code = 1, Message = "Generate error" };
            var controller = GetPaymentProcessController();
            paymentProcessService.SubmitUploadingFilesAsync(companyId, month).Returns(submitResult);
            paymentProcessService.GenerateMonthlyPayrollAsync(companyId, month, null).Returns(generateResult);

            // Act
            var actual = await controller.ProcessActionNextToMonthlyPayroll(companyId, month);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal("Generate error", errors["CommonError"]);
        }

        [Fact]
        public async Task ProcessActionNextToMonthlyPayroll_GeneratePayrollSuccessful_ReturnJsonResultSuccessful()
        {
            // Arrange
            var companyId = 17;
            DateTime month = DateTime.Today;
            var submitResult = new CommandResult { Code = 0 };
            var generateResult = new CommandResult { Code = 0 };
            var controller = GetPaymentProcessController();
            paymentProcessService.SubmitUploadingFilesAsync(companyId, month).Returns(submitResult);
            paymentProcessService.GenerateMonthlyPayrollAsync(companyId, month, null).Returns(generateResult);

            // Act
            var actual = await controller.ProcessActionNextToMonthlyPayroll(companyId, month);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
        }

        [Fact]
        public async Task UploadFilesPost_AttendanceFileAndDSTNetSalaryFileAreNull_ReturnJsonResultWithErrors()
        {
            // Arrange
            var viewModel = new UploadFilesViewModel();
            var controller = GetPaymentProcessController();

            // Act
            var actual = await controller.UploadFiles(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(Message.PleaseSelectFile, errors["CommonError"]);
        }

        [Fact]
        public async Task UploadFilesPost_FailureWhenReadAttendanceFileAndDSTNetSalaryFile_ReturnJsonResultWithErrors()
        {
            // Arrange
            var viewModel = new UploadFilesViewModel { AttendanceFile = Substitute.For<IFormFile>() };
            var failureAttendance = MockFailureResultAttendanceTemplate();
            var failureDSTNetSalary = MockFailureResultDSTNetSalaryTemplate();
            var controller = GetPaymentProcessController();
            paymentProcessService.ReadAttendanceFileAsync(Arg.Is<UploadFilesViewModel>(x => x == viewModel)).Returns(failureAttendance);
            paymentProcessService.ReadDSTNetSalaryFileAsync(Arg.Is<UploadFilesViewModel>(x => x == viewModel)).Returns(failureDSTNetSalary);

            // Act
            var actual = await controller.UploadFiles(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Concat(ViewLang.AttendanceFile, ": ", failureAttendance.Message), errors["AttendanceFile"]);
            Assert.Equal(string.Concat(ViewLang.DSTNetSalaryFile, ": ", failureDSTNetSalary.Message), errors["DSTNetSalaryFile"]);
        }

        [Fact]
        public async Task UploadFilesPost_ModelStateIsValidAndUploadFilesSuccessful_ReturnJsonResultSuccessful()
        {
            // Arrange
            var commandResult = new CommandResult { Code = 0 };
            var viewModel = new UploadFilesViewModel
            {
                CompanyId = 3,
                SelectedTime = DateTime.Today,
                AttendanceFile = Substitute.For<IFormFile>()
            };
            var successAttendance = MockSuccessResultAttendanceTemplate();
            var successDSTNetSalary = MockSuccessResultDSTNetSalaryTemplate();
            var successAdditionDeduction = new Success<AdditionDeductionTemplate>(null);
            var controller = GetPaymentProcessController();
            paymentProcessService.ReadAttendanceFileAsync(Arg.Is<UploadFilesViewModel>(x => x == viewModel)).Returns(successAttendance);
            paymentProcessService.ReadDSTNetSalaryFileAsync(Arg.Is<UploadFilesViewModel>(x => x == viewModel)).Returns(successDSTNetSalary);
            paymentProcessService.ReadAdditionDeductionFileAsync(Arg.Is<UploadFilesViewModel>(x => x == viewModel)).Returns(successAdditionDeduction);
            paymentProcessService.UploadFilesAsync(
                Arg.Is<int>(x => x == viewModel.CompanyId),
                Arg.Is<DateTime>(x => x == viewModel.SelectedTime),
                Arg.Is<AttendanceTemplate>(x => x == successAttendance.Value),
                Arg.Is<DSTNetSalaryTemplate>(x => x == successDSTNetSalary.Value),
                Arg.Is<AdditionDeductionTemplate>(x => x == successAdditionDeduction.Value)).Returns(commandResult);

            // Act
            var actual = await controller.UploadFiles(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
        }

        [Fact]
        public async Task UploadFilesPost_ModelStateIsValidAndUploadFilesNotSuccessful_ReturnJsonResultSuccessful()
        {
            // Arrange
            var commandResult = new CommandResult();
            var viewModel = new UploadFilesViewModel
            {
                CompanyId = 3,
                SelectedTime = DateTime.Today,
                AttendanceFile = Substitute.For<IFormFile>()
            };
            var successAttendance = MockSuccessResultAttendanceTemplate();
            var successDSTNetSalary = MockSuccessResultDSTNetSalaryTemplate();
            var successAdditionDeduction = new Success<AdditionDeductionTemplate>(null);
            var controller = GetPaymentProcessController();
            paymentProcessService.ReadAttendanceFileAsync(Arg.Is<UploadFilesViewModel>(x => x == viewModel)).Returns(successAttendance);
            paymentProcessService.ReadDSTNetSalaryFileAsync(Arg.Is<UploadFilesViewModel>(x => x == viewModel)).Returns(successDSTNetSalary);
            paymentProcessService.ReadAdditionDeductionFileAsync(Arg.Is<UploadFilesViewModel>(x => x == viewModel)).Returns(successAdditionDeduction);
            paymentProcessService.UploadFilesAsync(
                Arg.Is<int>(x => x == viewModel.CompanyId),
                Arg.Is<DateTime>(x => x == viewModel.SelectedTime),
                Arg.Is<AttendanceTemplate>(x => x == successAttendance.Value),
                Arg.Is<DSTNetSalaryTemplate>(x => x == successDSTNetSalary.Value),
                Arg.Is<AdditionDeductionTemplate>(x => x == successAdditionDeduction.Value)).Returns(commandResult);

            // Act
            var actual = await controller.UploadFiles(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["CommonError"]);
        }

        [Fact]
        public async Task DeleteUploadedAttendance_DeleteDataNotSuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var companyId = 12;
            var month = DateTime.Today;
            var commandResult = new CommandResult();
            var controller = GetPaymentProcessController();
            paymentProcessService.DeleteUploadedAttendance(Arg.Is<int>(x => x == companyId), Arg.Is<DateTime>(x => x == month)).Returns(commandResult);

            // Act
            var actual = await controller.DeleteUploadedAttendance(companyId, month);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["CommonError"]);
        }

        [Fact]
        public async Task DeleteUploadedAttendance_DeleteDataSuccessful_ReturnJsonResultSuccessful()
        {
            // Arrange
            var companyId = 12;
            var month = DateTime.Today;
            var commandResult = new CommandResult { Code = 0 };
            var controller = GetPaymentProcessController();
            paymentProcessService.DeleteUploadedAttendance(Arg.Is<int>(x => x == companyId), Arg.Is<DateTime>(x => x == month)).Returns(commandResult);

            // Act
            var actual = await controller.DeleteUploadedAttendance(companyId, month);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
        }

        [Fact]
        public async Task DeleteUploadedDSTNetSalary_DeleteDataNotSuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var companyId = 17;
            var month = DateTime.Today;
            var commandResult = new CommandResult();
            var controller = GetPaymentProcessController();
            paymentProcessService.DeleteUploadedDSTNetSalary(Arg.Is<int>(x => x == companyId), Arg.Is<DateTime>(x => x == month)).Returns(commandResult);

            // Act
            var actual = await controller.DeleteUploadedDSTNetSalary(companyId, month);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["CommonError"]);
        }

        [Fact]
        public async Task DeleteUploadedDSTNetSalary_DeleteDataSuccessful_ReturnJsonResultSuccessful()
        {
            // Arrange
            var companyId = 17;
            var month = DateTime.Today;
            var commandResult = new CommandResult { Code = 0 };
            var controller = GetPaymentProcessController();
            paymentProcessService.DeleteUploadedDSTNetSalary(Arg.Is<int>(x => x == companyId), Arg.Is<DateTime>(x => x == month)).Returns(commandResult);

            // Act
            var actual = await controller.DeleteUploadedDSTNetSalary(companyId, month);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
        }

        #endregion UploadAttendanceAndDSTSalary

        #region Helpers

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private PaymentProcessController GetPaymentProcessController()
        {
            sessionService = FakeSessionService();
            baseService = Substitute.For<IBaseService>();
            hostingEnvironment = Substitute.For<IHostingEnvironment>();
            commonService = Substitute.For<ICommonService>();
            paymentProcessService = Substitute.For<IPaymentProcessService>();
            monthlyPayrollService = Substitute.For<IMonthlyPayrollService>();
            rijndaelService = Substitute.For<IRijndaelService>();
            monthlySalaryService = Substitute.For<IMonthlySalaryService>();
            monthlyAllowancesService = Substitute.For<IMonthlyAllowancesService>();

            return new PaymentProcessController(
                baseService,
                sessionService,
                hostingEnvironment,
                commonService,
                paymentProcessService,
                monthlyPayrollService,
                monthlySalaryService,
                monthlyAllowancesService,
                rijndaelService);
        }

        private Company MockCompany()
        {
            return new Company
            {
                CompanyId = 11,
                Code = "company",
                Name = "descC",
                IsActive = true
            };
        }

        private SelectList MockCompanySelectList()
        {
            var companies = new List<Company> { MockCompany() };

            return new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
        }

        private SelectWorkingMonthCompanyViewModel MockSelectWorkingMonthCompanyViewModel()
        {
            var companies = new List<Company> { MockCompany() };
            var selectList = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));

            return new SelectWorkingMonthCompanyViewModel
            {
                SelectedTime = DateTime.Now,
                StartDate = DateTime.Today.AddDays(-1),
                Companies = selectList,
                CommonError = string.Empty
            };
        }

        private static Failure<AttendanceTemplate> MockFailureResultAttendanceTemplate()
        {
            return new Failure<AttendanceTemplate>(Message.IncorrectTemplate);
        }

        private static Failure<DSTNetSalaryTemplate> MockFailureResultDSTNetSalaryTemplate()
        {
            return new Failure<DSTNetSalaryTemplate>(Message.IncorrectTemplate);
        }

        private static Success<AttendanceTemplate> MockSuccessResultAttendanceTemplate()
        {
            return new Success<AttendanceTemplate>(null);
        }

        private static Success<DSTNetSalaryTemplate> MockSuccessResultDSTNetSalaryTemplate()
        {
            return new Success<DSTNetSalaryTemplate>(null);
        }

        #endregion Helpers
    }
}