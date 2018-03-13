namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.PaymentProcess;
    using HRP.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class PaymentProcessControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private IHostingEnvironment hostingEnvironment;
        private IPaymentProcessService paymentProcessService;
        private IMonthlyPayrollService monthlyPayrollService;

        [Fact]
        public async Task Index_WhenCalled_ReturnExpectedViewResult()
        {
            // Arrange
            var controller = GetPaymentProcessController();
            baseService.GetDataAsync<Company>(Arg.Any<GetCompaniesForPaymentProcessCriteria>()).Returns(new List<Company>());

            // Act
            var actual = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsType<SelectWorkingMonthCompanyViewModel>(viewResult.Model);
            Assert.Equal(DateTime.Today, viewModel.SelectedTime);
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
        public async Task ValidationFilesResult_WhenCalled_ReturnPartialViewWithValidationFilesResultViewModel()
        {
            // Arrange
            var companyId = 9;
            DateTime month = DateTime.Now;
            var controller = GetPaymentProcessController();
            paymentProcessService.GetValidationFilesResultAsync(companyId, month).Returns(new ValidationFilesResultViewModel());

            // Act
            var actual = await controller.ValidationFilesResult(companyId, month);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var model = Assert.IsAssignableFrom<ValidationFilesResultViewModel>(viewResult.Model);
            Assert.Equal("_ValidationFilesResult", viewResult.ViewName);
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
            var model = new EmployeeMonthlyPayrollViewModel();
            var employeePayrolls = new List<MonthlyPayrollEmployeeInfoViewModel>
            {
                new MonthlyPayrollEmployeeInfoViewModel { }
            };
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            monthlyPayrollService.GetDataFilterForIndexPageAsync(companyId, Arg.Any<string>()).Returns(model);
            monthlyPayrollService.GetMonthlyPayrollAsync(model).Returns(employeePayrolls);

            // Act
            var actual = await controller.EmployeeMonthlyPayroll(companyId, month, employeeId, null, null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<EmployeeMonthlyPayrollViewModel>(viewResult.Model);
            Assert.Equal("MonthlyPayroll", viewResult.ViewName);
            Assert.Equal(model.CompanyId, viewModel.CompanyId);
            Assert.Equal(model.CompanyCode, viewModel.CompanyCode);
            Assert.Equal(month, viewModel.Month);
            Assert.Equal(employeeId, viewModel.EmployeeId);
            Assert.Null(viewModel.Employee);
            Assert.Null(viewModel.IDNo);
            Assert.Equal(employeePayrolls, viewModel.Employees);
            Assert.Equal(model.StartDate, viewModel.StartDate);
            Assert.Equal(model.DueDate, viewModel.DueDate);
            Assert.Equal(model.WorkingDays, viewModel.WorkingDays);
        }

        #endregion EmployeeMonthlyPayroll

        #region EditBasicSalary

        [Fact]
        public void EditBasicSalary_HasNotPermission_ReturnRedirectToAccessDenied()
        {
            // Arrange
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(false);

            // Act
            var actual = controller.EditBasicSalary(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<DateTime>());

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public void EditBasicSalary_HasPermission_ReturnAsExpectedViewResult()
        {
            // Arrange
            var companyId = 1;
            var payrollId = 1;
            var month = DateTime.Now;
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);

            // Act
            var actual = controller.EditBasicSalary(payrollId, companyId, month);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<EditMonthlySalaryViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
        }

        #endregion EditBasicSalary

        [Fact]
        public async Task GetEmployeeSuggestion_WhenCalled_ReturnExpectedEmployees()
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
        public async Task EditAllowances_NotHavePermission_RederiectToAccessDenied()
        {
            var companyId = 1;
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.EditAllowances, Policy.Read).Returns(false);

            var actual = await controller.EditAllowances(1, companyId);

            var result = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", result.ControllerName);
            Assert.Equal("AccessDenied", result.ActionName);
        }

        [Fact]
        public async Task EditAllowances_HavePermission_ReturnView()
        {
            var companyId = 1;
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.EditAllowances, Policy.Read).Returns(true);
            sessionService.HasPermissionOnCompany(companyId, FunctionKey.EditAllowances, Policy.Host).Returns(true);
            monthlyPayrollService.GetMonthlyAllowancesAsync(Arg.Any<int>()).Returns(new MonthlyAllowancesViewModel());

            var actual = await controller.EditAllowances(1, companyId);

            var result = Assert.IsType<ViewResult>(actual);
            Assert.IsType<MonthlyAllowancesViewModel>(result.Model);
        }

        #region Helpers

        private PaymentProcessController GetPaymentProcessController()
        {
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            hostingEnvironment = FakeHostingEnvironment();
            paymentProcessService = FakePaymentProcessService();
            monthlyPayrollService = FakeMonthlyPayrollService();

            return new PaymentProcessController(baseService, sessionService, hostingEnvironment, paymentProcessService, monthlyPayrollService);
        }

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();
            return service;
        }

        private IHostingEnvironment FakeHostingEnvironment()
        {
            return Substitute.For<IHostingEnvironment>();
        }

        private IPaymentProcessService FakePaymentProcessService()
        {
            var service = Substitute.For<IPaymentProcessService>();
            return service;
        }

        private IMonthlyPayrollService FakeMonthlyPayrollService()
        {
            var service = Substitute.For<IMonthlyPayrollService>();
            return service;
        }

        #endregion Helpers
    }
}