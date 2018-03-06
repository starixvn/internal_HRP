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
        public void EmployeeMonthlyPayroll_HasNotPermission_ReturnRedirectToAccessDenied()
        {
            // Arrange
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(false);

            // Act
            var actual = controller.EmployeeMonthlyPayroll(Arg.Any<int>(), Arg.Any<DateTime>());

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public void EmployeeMonthlyPayroll_HasPermission_ReturnAsExpectedViewResult()
        {
            // Arrange
            int companyId = 1;
            var controller = GetPaymentProcessController();
            sessionService.HasPermissionOnCompany(companyId, Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);

            // Act
            var actual = controller.EmployeeMonthlyPayroll(companyId, Arg.Any<DateTime>());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<EmployeeMonthlyPayrollViewModel>(viewResult.Model);
            Assert.Equal("MonthlyPayroll", viewResult.ViewName);
        }

        #endregion EmployeeMonthlyPayroll

        #region Helpers

        private PaymentProcessController GetPaymentProcessController()
        {
            FakeSessionService();
            FakeBaseService();
            FakeHostingEnvironment();
            FakePaymentProcessService();

            return new PaymentProcessController(baseService, sessionService, hostingEnvironment, paymentProcessService);
        }

        private void FakeSessionService()
        {
            sessionService = Substitute.For<ISessionService>();
        }

        private void FakeBaseService()
        {
            baseService = Substitute.For<IBaseService>();
        }

        private void FakeHostingEnvironment()
        {
            hostingEnvironment = Substitute.For<IHostingEnvironment>();
        }

        private void FakePaymentProcessService()
        {
            paymentProcessService = Substitute.For<IPaymentProcessService>();
        }

        #endregion Helpers
    }
}