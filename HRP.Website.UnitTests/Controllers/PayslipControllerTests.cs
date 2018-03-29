namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Services;
    using HRP.Models;
    using HRP.Models.Payslip;
    using HRP.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class PayslipControllerTests
    {
        private IPayslipService payslipService;

        [Fact]
        public async Task Index_WhenCalled_ReturnsAsExpectedViewResult()
        {
            //Arrange
            var month = DateTime.Today;
            var companyId = 1;
            var companies = new List<Company>
            {
                new Company { CompanyId = 1, Code = "DAR" }
            };
            var payslips = new List<PayslipModel>
            {
                new PayslipModel { PayrollId = 1, FullName = "thang", Nickname = "tim.trinh" }
            };
            var homeController = GetPayslipController();
            payslipService.GetCompanyAsync().Returns(companies);
            payslipService.GetPayslipAsync(month, companyId, null).Returns(payslips);

            //Act
            var actual = await homeController.Index(month, companyId, null);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<PayslipViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(month, viewModel.Month);
            Assert.Equal(companyId, viewModel.CompanyId);
            Assert.Null(viewModel.IsSent);
            Assert.Equal(payslips, viewModel.Payslips);
            Assert.Equal(companies.Count(), viewModel.Companies.Count());
        }

        [Fact]
        public void Details_WhenCalled_ReturnsAsExpectedViewResult()
        {
            //Arrange
            var homeController = GetPayslipController();

            //Act
            var actual = homeController.Details(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        #region Helpers

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

        private IPayslipService FakePayslipService()
        {
            var service = Substitute.For<IPayslipService>();

            return service;
        }

        private PayslipController GetPayslipController()
        {
            var sessionService = FakeSessionService();
            var baseService = FakeBaseService();
            payslipService = FakePayslipService();
            var controller = new PayslipController(sessionService, baseService, payslipService);

            return controller;
        }

        #endregion Helpers
    }
}