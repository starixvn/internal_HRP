namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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