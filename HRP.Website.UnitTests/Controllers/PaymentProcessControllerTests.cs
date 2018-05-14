namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Criteria;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.PaymentProcess;
    using HRP.Resources;
    using HRP.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Routing;
    using NSubstitute;
    using Xunit;

    public class PaymentProcessControllerTests
    {
        private static DateTime startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        private static DateTime dueDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
        private static DateTime selectedTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 14);
        private IBaseService baseService;
        private ISessionService sessionService;
        private IHostingEnvironment hostingEnvironment;
        private ICommonService commonService;
        private IPaymentProcessService paymentProcessService;
        private IMonthlyPayrollService monthlyPayrollService;

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
                IsAttendanceFileSubmited = true,
                DSTNetSalaryFileName = "salaryFile.xls",
                IsDSTNetSalaryFileSubmited = true,
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
            Assert.Equal(Message.NotHasCurrencySetting, viewResult.ViewData.ModelState["CommonError"].Errors.First().ErrorMessage);
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
                IsAttendanceFileSubmited = true,
                DSTNetSalaryFileName = "salaryFile.xls",
                IsDSTNetSalaryFileSubmited = true,
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
        public async Task IndexPost_UserHasHostPermissionAndPayrollIsNotGenerated_ReturnRedirectToUploadAttendanceAndSalary
            (bool isPayrollGenerated, bool isAttendanceSubmited, bool isDSTNetSalarySubmited)
        {
            // Arrange
            var paymentProcess = new PaymentProcess
            {
                StartDate = startDate,
                DueDate = dueDate,
                AttendanceFileName = "attendanceFile.xlsx",
                IsAttendanceFileSubmited = isAttendanceSubmited,
                DSTNetSalaryFileName = "salaryFile.xls",
                IsDSTNetSalaryFileSubmited = isDSTNetSalarySubmited,
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
            Assert.Equal("UploadAttendanceAndSalary", viewResult.ActionName);
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
            Assert.Equal(Message.PayrollNotGenerated, viewResult.ViewData.ModelState["CommonError"].Errors.First().ErrorMessage);
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
            Assert.Equal(@"ExcelTemplates\Template - Attendance (Expat).xlsx", filePath);
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
            Assert.Equal(@"ExcelTemplates\Template - DST Salary (Expat).xlsx", filePath);
            Assert.Equal(MimeType.XLSX, fileStream.ContentType);
            Assert.Equal("Template - DST Salary (Expat).xlsx", fileStream.FileDownloadName);
        }

        [Fact]
        public async Task UploadAttendanceAndSalary_UserHasPermission_ReturnViewResultWithExpectedViewModel()
        {
            // Arrange
            var companyId = 13;
            var month = selectedTime;
            var paymentProcess = new PaymentProcess
            {
                StartDate = startDate,
                DueDate = dueDate,
            };
            var expected = new UploadAttendanceAndSalaryViewModel
            {
                CompanyId = companyId,
                SelectedTime = month
            };
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host).Returns(true);
            paymentProcessService.GetPaymentProcessAsync(companyId, month).Returns(paymentProcess);
            paymentProcessService.GetUploadingFilesResultAsync(companyId, month).Returns(expected);

            // Act
            var actual = await controller.UploadAttendanceAndSalary(companyId, month);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewData = Assert.IsType<UploadAttendanceAndSalaryViewModel>(viewResult.ViewData.Model);
            Assert.Equal(expected, viewData);
        }

        [Theory]
        [InlineData("2018-3-1", "2018-3-30", false)]
        [InlineData("2018-3-1", "2018-3-11", true)]
        public async Task UploadAttendanceAndSalary_UserNotHasPermission_ReturnViewResultWithExpectedViewModel
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
            var actual = await controller.UploadAttendanceAndSalary(companyId, month);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Null(viewResult.ControllerName);
            Assert.Equal("Index", viewResult.ActionName);
        }

        [Fact]
        public async Task ValidationFilesResult_WhenCalled_ReturnPartialViewWithValidationFilesResultViewModel()
        {
            // Arrange
            var expected = new UploadAttendanceAndSalaryViewModel
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
            var model = Assert.IsType<UploadAttendanceAndSalaryViewModel>(viewResult.Model);
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
        public async Task UploadAttendanceAndSalaryPost_AttendanceFileAndDSTNetSalaryFileAreNull_ReturnJsonResultWithErrors()
        {
            // Arrange
            var viewModel = new UploadAttendanceAndSalaryViewModel();
            var controller = GetPaymentProcessController();

            // Act
            var actual = await controller.UploadAttendanceAndSalary(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(Message.PleaseSelectFile, errors["CommonError"]);
        }

        [Fact]
        public async Task UploadAttendanceAndSalaryPost_FailureWhenReadAttendanceFileAndDSTNetSalaryFile_ReturnJsonResultWithErrors()
        {
            // Arrange
            var viewModel = new UploadAttendanceAndSalaryViewModel { AttendanceFile = Substitute.For<IFormFile>() };
            var failureAttendance = MockFailureResultAttendanceTemplate();
            var failureDSTNetSalary = MockFailureResultDSTNetSalaryTemplate();
            var controller = GetPaymentProcessController();
            paymentProcessService.ReadAttendanceFileAsync(Arg.Is<UploadAttendanceAndSalaryViewModel>(x => x == viewModel)).Returns(failureAttendance);
            paymentProcessService.ReadDSTNetSalaryFileAsync(Arg.Is<UploadAttendanceAndSalaryViewModel>(x => x == viewModel)).Returns(failureDSTNetSalary);

            // Act
            var actual = await controller.UploadAttendanceAndSalary(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Concat(ViewLang.AttendanceFile, ": ", failureAttendance.Message), errors["AttendanceFile"]);
            Assert.Equal(string.Concat(ViewLang.DSTNetSalaryFile, ": ", failureDSTNetSalary.Message), errors["DSTNetSalaryFile"]);
        }

        [Fact]
        public async Task UploadAttendanceAndSalaryPost_ModelStateIsValidAndUploadFilesSuccessful_ReturnJsonResultSuccessful()
        {
            // Arrange
            var commandResult = new CommandResult { Code = 0 };
            var viewModel = new UploadAttendanceAndSalaryViewModel
            {
                CompanyId = 3,
                SelectedTime = DateTime.Today,
                AttendanceFile = Substitute.For<IFormFile>()
            };
            var successAttendance = MockSuccessResultAttendanceTemplate();
            var successDSTNetSalary = MockSuccessResultDSTNetSalaryTemplate();
            var controller = GetPaymentProcessController();
            paymentProcessService.ReadAttendanceFileAsync(Arg.Is<UploadAttendanceAndSalaryViewModel>(x => x == viewModel)).Returns(successAttendance);
            paymentProcessService.ReadDSTNetSalaryFileAsync(Arg.Is<UploadAttendanceAndSalaryViewModel>(x => x == viewModel)).Returns(successDSTNetSalary);
            paymentProcessService.UploadAttendanceAndDSTNetSalaryAsync(Arg.Is<int>(x => x == viewModel.CompanyId), Arg.Is<DateTime>(x => x == viewModel.SelectedTime),
                                   Arg.Is<AttendanceTemplate>(x => x == successAttendance.Value), Arg.Is<DSTNetSalaryTemplate>(x => x == successDSTNetSalary.Value)).Returns(commandResult);

            // Act
            var actual = await controller.UploadAttendanceAndSalary(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
        }

        [Fact]
        public async Task UploadAttendanceAndSalaryPost_ModelStateIsValidAndUploadFilesNotSuccessful_ReturnJsonResultSuccessful()
        {
            // Arrange
            var commandResult = new CommandResult();
            var viewModel = new UploadAttendanceAndSalaryViewModel
            {
                CompanyId = 3,
                SelectedTime = DateTime.Today,
                AttendanceFile = Substitute.For<IFormFile>()
            };
            var successAttendance = MockSuccessResultAttendanceTemplate();
            var successDSTNetSalary = MockSuccessResultDSTNetSalaryTemplate();
            var controller = GetPaymentProcessController();
            paymentProcessService.ReadAttendanceFileAsync(Arg.Is<UploadAttendanceAndSalaryViewModel>(x => x == viewModel)).Returns(successAttendance);
            paymentProcessService.ReadDSTNetSalaryFileAsync(Arg.Is<UploadAttendanceAndSalaryViewModel>(x => x == viewModel)).Returns(successDSTNetSalary);
            paymentProcessService.UploadAttendanceAndDSTNetSalaryAsync(Arg.Is<int>(x => x == viewModel.CompanyId), Arg.Is<DateTime>(x => x == viewModel.SelectedTime),
                                   Arg.Is<AttendanceTemplate>(x => x == successAttendance.Value), Arg.Is<DSTNetSalaryTemplate>(x => x == successDSTNetSalary.Value)).Returns(commandResult);

            // Act
            var actual = await controller.UploadAttendanceAndSalary(viewModel);

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

        #region EmployeeMonthlyPayroll

        [Fact]
        public async Task EmployeeMonthlyPayroll_HasNotPermission_ReturnRedirectToAccessDenied()
        {
            // Arrange
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(false);

            // Act
            var actual = await controller.EmployeeMonthlyPayroll(Arg.Any<int>(), Arg.Any<DateTime>());

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public async Task EmployeeMonthlyPayroll_HasPermission_ReturnAsExpectedViewResult()
        {
            // Arrange
            var companyId = 1;
            var employeeId = 31;
            var month = DateTime.Now;
            var filterInfo = new MonthlyPayrollFilterInfoModel();
            var employeePayrolls = new List<MonthlyPayrollEmployeeInfoViewModel>
            {
                new MonthlyPayrollEmployeeInfoViewModel { }
            };
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            monthlyPayrollService.GetFilterInfoAsync(companyId, month).Returns(filterInfo);
            monthlyPayrollService.GetMonthlyPayrollAsync(Arg.Any<EmployeeMonthlyPayrollViewModel>()).Returns(employeePayrolls);

            // Act
            var actual = await controller.EmployeeMonthlyPayroll(companyId, month, employeeId, null, null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<EmployeeMonthlyPayrollViewModel>(viewResult.Model);
            Assert.Equal("MonthlyPayroll", viewResult.ViewName);
            Assert.Equal(filterInfo, viewModelResult.FilterInfo);
            Assert.Equal(month, viewModelResult.Month);
            Assert.Equal(employeeId, viewModelResult.EmployeeId);
            Assert.Null(viewModelResult.Employee);
            Assert.Null(viewModelResult.IDNo);
            Assert.Equal(employeePayrolls, viewModelResult.Employees);
        }

        #endregion EmployeeMonthlyPayroll

        #region EditBasicSalary

        [Fact]
        public async Task EditBasicSalary_HasNotPermission_ReturnRedirectToAccessDenied()
        {
            // Arrange
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(false);

            // Act
            var actual = await controller.EditBasicSalary(1, 1, DateTime.Today, new CachingFilterViewModel());

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public async Task EditBasicSalary_MonthlyPayrollIsNotExisted_ReturnRedirectToPageNotFound()
        {
            // Arrange
            EditMonthlySalaryViewModel monthlySalary = null;
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            monthlyPayrollService.GetMonthlySalaryAsync(Arg.Any<int>()).Returns(monthlySalary);

            // Act
            var actual = await controller.EditBasicSalary(1, 1, DateTime.Today, new CachingFilterViewModel());

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public async Task EditBasicSalary_MonthlyPayrollIsExisted_ReturnAsExpectedViewResult()
        {
            // Arrange
            var companyId = 1;
            var payrollId = 1;
            var month = DateTime.Now;
            var viewModel = new EditMonthlySalaryViewModel();
            var currencyExchangeRates = new List<CurrencyExchangeRateModel>();
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            monthlyPayrollService.GetMonthlySalaryAsync(Arg.Any<int>()).Returns(viewModel);
            baseService.GetDataAsync<CurrencyExchangeRateModel>(Arg.Any<GetExchangeRateCriteria>()).Returns(currencyExchangeRates);

            // Act
            var actual = await controller.EditBasicSalary(payrollId, companyId, month, new CachingFilterViewModel());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<EditMonthlySalaryViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(month, viewModelResult.Month);
            Assert.Equal(viewModel.MonthlyBasicSalary, viewModelResult.MonthlyBasicSalary);
            Assert.Equal(viewModel.MonthlySalary, viewModelResult.MonthlySalary);
            Assert.Equal(viewModel.Additions, viewModelResult.Additions);
            Assert.Equal(viewModel.Deductions, viewModelResult.Deductions);
            Assert.Equal(currencyExchangeRates, viewModelResult.CurrencyExchangeRates);
        }

        [Fact]
        public async Task EditBasicSalaryPost_HasNotPermission_ReturnRedirectToAccessDenied()
        {
            // Arrange
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(false);

            // Act
            var actual = await controller.EditBasicSalary(new EditMonthlySalaryViewModel());

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public async Task EditBasicSalaryPost_ModelStateIsInvalid_ReturnAsExpectedErrors()
        {
            // Arrange
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            controller.ModelState.AddModelError("UnitAmount", "required");

            // Act
            var actual = await controller.EditBasicSalary(new EditMonthlySalaryViewModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value));
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Collection(
                errors,
                kvp =>
                {
                    Assert.Equal("UnitAmount", kvp.Key);
                    Assert.Equal("required", kvp.Value);
                }
            );
        }

        [Fact]
        public async Task EditBasicSalaryPost_ModelStateIsValid_ReturnAsExpected()
        {
            // Arrange
            var viewModel = new EditMonthlySalaryViewModel();
            var commandResult = new CommandResult();
            var controller = GetPaymentProcessController();
            controller.Url = Substitute.For<IUrlHelper>();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            monthlyPayrollService.EditMonthlySalaryAsync(viewModel).Returns(commandResult);

            // Act
            var actual = await controller.EditBasicSalary(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(commandResult.IsSuccessful, (bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Equal(commandResult.Message, jsonResult.Value.GetType().GetProperty("message").GetValue(jsonResult.Value));
        }

        #endregion EditBasicSalary

        [Fact]
        public async Task GetEmployeeSuggestionByName_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var expected = new List<EmployeeSuggestionViewModel>
            {
                new EmployeeSuggestionViewModel { EmployeeId = 1, IDNo = "DAR111", FullName = "Tim", Nickname = "tim.trinh" }
            };
            var controller = GetPaymentProcessController();
            monthlyPayrollService.GetEmployeeSuggestionByNameAsync(Arg.Any<int>(), Arg.Any<string>()).Returns(expected);

            // Act
            var actual = await controller.GetEmployeeSuggestionByName(1, "tim");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetEmployeeSuggestionByIDNo_WhenCalled_ReturnAsExpectedEmployees()
        {
            // Arrange
            var expected = new List<EmployeeSuggestionViewModel>
            {
                new EmployeeSuggestionViewModel { EmployeeId = 1, IDNo = "DAR111", FullName = "Tim", Nickname = "tim.trinh" }
            };
            var controller = GetPaymentProcessController();
            monthlyPayrollService.GetEmployeeSuggestionByIDNoAsync(Arg.Any<int>(), Arg.Any<string>()).Returns(expected);

            // Act
            var actual = await controller.GetEmployeeSuggestionByIDNo(1, "DAR111");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetHierarchyCompany_WhenCalled_ReturnAsExpectedHierarchy()
        {
            // Arrange
            var expected = new List<MultiSelectViewModel>();
            var controller = GetPaymentProcessController();
            monthlyPayrollService.GetHierarchyCompanyAsync(1, "1,2").Returns(expected);

            // Act
            var actual = await controller.GetHierarchyCompany(1, "1,2");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        #region EditAllowances

        [Fact]
        public async Task EditAllowances_NotHavePermission_RederiectToAccessDenied()
        {
            var companyId = 1;
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Read).Returns(false);

            var actual = await controller.EditAllowances(1, companyId, DateTime.Today, new CachingFilterViewModel());

            var result = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", result.ControllerName);
            Assert.Equal("AccessDenied", result.ActionName);
        }

        [Fact]
        public async Task EditAllowances_HavePermission_ReturnView()
        {
            var companyId = 1;
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Read).Returns(true);
            monthlyPayrollService.GetMonthlyAllowancesAsync(Arg.Any<int>(), Arg.Any<DateTime>())
                .Returns(new MonthlyAllowancesViewModel
                {
                    Allowance = new MonthlyAllowanceModel()
                });
            baseService.GetDataAsync<CurrencyCodeModel>(Arg.Any<GetActiveCurrencyCodesCriteria>()).Returns(new List<CurrencyCodeModel>());
            baseService.GetDataAsync<CurrencyExchangeRateModel>(Arg.Any<GetExchangeRateCriteria>()).Returns(new List<CurrencyExchangeRateModel>());
            baseService.GetDataAsync<ReferenceResourceModel>(Arg.Any<GetRefResourceByNamesCriteria>()).Returns(new List<ReferenceResourceModel>());

            var actual = await controller.EditAllowances(1, companyId, DateTime.Today, new CachingFilterViewModel());

            var result = Assert.IsType<ViewResult>(actual);
            Assert.IsType<MonthlyAllowancesViewModel>(result.Model);
        }

        [Fact]
        public async Task EditAllowancesPost_HasNotPermission_ReturnRedirectToAccessDenied()
        {
            // Arrange
            var viewModel = new MonthlyAllowancesViewModel();
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(false);

            // Act
            var actual = await controller.EditAllowances(viewModel, EditMonthlyAllowanceAction.Save);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public async Task EditAllowancesPost_ModelStateIsInvalid_ReturnAsExpectedErrors()
        {
            // Arrange
            var viewModel = new MonthlyAllowancesViewModel();
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            controller.ModelState.AddModelError("UnitAmount", "required");

            // Act
            var actual = await controller.EditAllowances(viewModel, EditMonthlyAllowanceAction.Save);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value));
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Collection(
                errors,
                kvp =>
                {
                    Assert.Equal("UnitAmount", kvp.Key);
                    Assert.Equal("required", kvp.Value);
                }
            );
        }

        [Fact]
        public async Task EditAllowancesPost_ModelStateIsValid_ReturnAsExpected()
        {
            // Arrange
            var viewModel = new MonthlyAllowancesViewModel();
            var commandResult = new CommandResult();
            var controller = GetPaymentProcessController();
            controller.Url = Substitute.For<IUrlHelper>();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            monthlyPayrollService.EditMonthlyAllowancesAsync(viewModel, sessionService.UserId).Returns(commandResult);

            // Act
            var actual = await controller.EditAllowances(viewModel, EditMonthlyAllowanceAction.Save);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(commandResult.IsSuccessful, (bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Equal(commandResult.Message, jsonResult.Value.GetType().GetProperty("message").GetValue(jsonResult.Value));
        }

        #endregion EditAllowances

        #region Preset

        [Fact]
        public async Task Preset_NotHavePermission_RederiectToAccessDenied()
        {
            // Arrange
            var companyId = 1;
            var employeeId = 1;
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host).Returns(false);

            // Act
            var actual = await controller.Preset(companyId, employeeId, DateTime.Today);

            // Assert
            var result = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", result.ControllerName);
            Assert.Equal("AccessDenied", result.ActionName);
        }

        [Fact]
        public async Task Preset_HavePermission_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var companyId = 1;
            var employeeId = 1;
            var commandResult = new CommandResult { Code = 0 };
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host).Returns(true);
            paymentProcessService.GenerateMonthlyPayrollAsync(companyId, DateTime.Today, employeeId).Returns(commandResult);

            // Act
            var actual = await controller.Preset(companyId, employeeId, DateTime.Today);

            // Assert
            var result = Assert.IsType<JsonResult>(actual);
            Assert.Equal(commandResult, result.Value);
        }

        #endregion Preset

        #region PresetAllMonthlyPayroll

        [Fact]
        public async Task PresetAllMonthlyPayroll_NotHavePermission_RederiectToAccessDenied()
        {
            // Arrange
            var companyId = 1;
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host).Returns(false);

            // Act
            var actual = await controller.PresetAllMonthlyPayroll(companyId, DateTime.Today);

            // Assert
            var result = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", result.ControllerName);
            Assert.Equal("AccessDenied", result.ActionName);
        }

        [Fact]
        public async Task PresetAllMonthlyPayroll_HavePermission_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var companyId = 1;
            var commandResult = new CommandResult { Code = 0 };
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.PaymentProcess, Policy.Host).Returns(true);
            paymentProcessService.GenerateMonthlyPayrollAsync(companyId, DateTime.Today, null).Returns(commandResult);

            // Act
            var actual = await controller.PresetAllMonthlyPayroll(companyId, DateTime.Today);

            // Assert
            var result = Assert.IsType<JsonResult>(actual);
            Assert.Equal(commandResult, result.Value);
        }

        #endregion PresetAllMonthlyPayroll

        #region Helpers

        private PaymentProcessController GetPaymentProcessController()
        {
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            hostingEnvironment = FakeHostingEnvironment();
            paymentProcessService = FakePaymentProcessService();
            monthlyPayrollService = FakeMonthlyPayrollService();
            commonService = FakeCommonService();

            return new PaymentProcessController(baseService, sessionService, hostingEnvironment, commonService, paymentProcessService, monthlyPayrollService);
        }

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();
            return service;
        }

        private static IHostingEnvironment FakeHostingEnvironment()
        {
            return Substitute.For<IHostingEnvironment>();
        }

        private static ICommonService FakeCommonService()
        {
            return Substitute.For<ICommonService>();
        }

        private static IPaymentProcessService FakePaymentProcessService()
        {
            var service = Substitute.For<IPaymentProcessService>();
            return service;
        }

        private static IMonthlyPayrollService FakeMonthlyPayrollService()
        {
            var service = Substitute.For<IMonthlyPayrollService>();
            return service;
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