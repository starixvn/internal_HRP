namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Models;
    using HRP.Models.Payslip;
    using HRP.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NSubstitute;
    using Xunit;

    public class PayslipControllerTests
    {
        private ISessionService sessionService;
        private ICommonService commonService;
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
            var selectList = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
            var payslips = new List<PayslipModel>
            {
                new PayslipModel { PayrollId = 1, FullName = "thang", Nickname = "tim.trinh" }
            };
            var payslipController = GetPayslipController();
            commonService.GetCompanyByFunctionKeyAsync(Arg.Any<string>()).Returns(companies);
            commonService.GetSelectList(Arg.Any<IEnumerable<Company>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(selectList);
            payslipService.GetPayslipAsync(month, companyId, null).Returns(payslips);

            //Act
            var actual = await payslipController.Index(month, companyId, null);

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

        #region Details

        [Fact]
        public async Task Details_HasNotPermission_ReturnRedirectToAccessDenied()
        {
            //Arrange
            var payslipController = GetPayslipController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(false);

            //Act
            var actual = await payslipController.Details(1, 1);

            //Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public async Task Details_PayrollIsNotExisted_ReturnRedirectToPageNotFound()
        {
            // Arrange
            PayslipDetailModel payslip = null;
            var payslipController = GetPayslipController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            payslipService.GetPayslipDetailAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(payslip);

            // Act
            var actual = await payslipController.Details(1, 1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public async Task Details_PayrollIsExisted_ReturnAsExpectedViewResult()
        {
            // Arrange
            var payslip = new PayslipDetailModel();
            var payslipController = GetPayslipController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            payslipService.GetPayslipDetailAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(payslip);

            // Act
            var actual = await payslipController.Details(1, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<PayslipDetailModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(payslip, viewModel);
        }

        #endregion Details

        [Fact]
        public async Task SendPayslip_HasNotPermission_ReturnRedirectToAccessDenied()
        {
            //Arrange
            var payslipController = GetPayslipController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(false);

            //Act
            var actual = await payslipController.SendPayslip(new SendPayslipViewModel());

            //Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("AccessDenied", viewResult.ActionName);
        }

        [Fact]
        public async Task SendPayslip_HasPermission_ReturnsAsExpectedViewResult()
        {
            //Arrange
            var viewModel = new SendPayslipViewModel();
            var expected = new CommandResult
            {
                Code = 0
            };
            var payslipController = GetPayslipController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            payslipService.SendPayslipAsync(viewModel).Returns(expected);

            //Act
            var actual = await payslipController.SendPayslip(viewModel);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        #region Helpers

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

        private static ICommonService FakeCommonService()
        {
            return Substitute.For<ICommonService>();
        }

        private static IPayslipService FakePayslipService()
        {
            var service = Substitute.For<IPayslipService>();

            return service;
        }

        private PayslipController GetPayslipController()
        {
            sessionService = FakeSessionService();
            var baseService = FakeBaseService();
            commonService = FakeCommonService();
            payslipService = FakePayslipService();
            var controller = new PayslipController(sessionService, baseService, commonService, payslipService);

            return controller;
        }

        #endregion Helpers
    }
}