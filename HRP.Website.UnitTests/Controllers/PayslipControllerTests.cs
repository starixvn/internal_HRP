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
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.Payslip;
    using HRP.Models.ServiceModels;
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
        private IRijndaelService rijndaelService;

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
            Assert.Equal(companies.Count, viewModel.Companies.Count());
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
            var tuple = new Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair>(new RijndaelKeyValuePair(), new RijndaelKeyValuePair());
            var cipherText = "1234abc";
            var payslip = new PayslipDetailModel
            {
                SalaryAdditions = new List<MonthlySalaryAdditionModel> { new MonthlySalaryAdditionModel() },
                SalaryDeductions = new List<MonthlySalaryDeductionModel> { new MonthlySalaryDeductionModel() },
                AllowanceAdditions = new List<MonthlyAllowanceAdditionModel> { new MonthlyAllowanceAdditionModel() },
                AllowanceDeductions = new List<MonthlyAllowanceDeductionModel> { new MonthlyAllowanceDeductionModel() }
            };
            var payslipController = GetPayslipController();
            sessionService.HasPermissionOnCompany(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            payslipService.GetPayslipDetailAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(payslip);
            rijndaelService.GenerateRijndaelKeyValuePair().Returns(tuple);
            rijndaelService.Encrypt(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(cipherText);

            // Act
            var actual = await payslipController.Details(1, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<PayslipDetailModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(payslip.PayrollId, viewModel.PayrollId);
            Assert.Equal(payslip.Month, viewModel.Month);
            Assert.Equal(payslip.FullName, viewModel.FullName);
            Assert.Equal(payslip.Nickname, viewModel.Nickname);
            Assert.Equal(payslip.IDNo, viewModel.IDNo);
            Assert.Equal(payslip.Email, viewModel.Email);
            Assert.Equal(payslip.CompanyName, viewModel.CompanyName);
            Assert.Equal(payslip.PDFPassword, viewModel.PDFPassword);
            Assert.Equal(payslip.SalaryCurrencyId, viewModel.SalaryCurrencyId);
            Assert.Equal(payslip.SalaryCurrencyCode, viewModel.SalaryCurrencyCode);
            Assert.Equal(cipherText, viewModel.SalaryAmount);
            Assert.Equal(payslip.AllowanceCurrencyId, viewModel.AllowanceCurrencyId);
            Assert.Equal(payslip.AllowanceCurrencyCode, viewModel.AllowanceCurrencyCode);
            Assert.Equal(cipherText, viewModel.AllowanceAmount);
            Assert.Equal(cipherText, viewModel.SalaryDeductionAmount);
            Assert.Equal(cipherText, viewModel.AllowanceDeductionAmount);
            Assert.Equal(cipherText, viewModel.GrossSalaryAmount);
            Assert.Equal(cipherText, viewModel.GrossAllowanceAmount);
            Assert.Equal(cipherText, viewModel.NetSalaryAmount);
            Assert.Equal(cipherText, viewModel.NetAllowanceAmount);

            Assert.Collection(
                viewModel.SalaryAdditions,
                kvp =>
                {
                    Assert.Equal(cipherText, kvp.NetSalaryDisplay);
                });

            Assert.Collection(
                viewModel.SalaryDeductions,
                kvp =>
                {
                    Assert.Equal(cipherText, kvp.NetSalaryDisplay);
                });

            Assert.Collection(
                viewModel.AllowanceAdditions,
                kvp =>
                {
                    Assert.Equal(cipherText, kvp.NetAllowanceToDisplay);
                });

            Assert.Collection(
                viewModel.AllowanceDeductions,
                kvp =>
                {
                    Assert.Equal(cipherText, kvp.NetAllowanceToDisplay);
                });
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

        private static IRijndaelService FakeRijndaelService()
        {
            var service = Substitute.For<IRijndaelService>();

            return service;
        }

        private PayslipController GetPayslipController()
        {
            sessionService = FakeSessionService();
            var baseService = FakeBaseService();
            commonService = FakeCommonService();
            payslipService = FakePayslipService();
            rijndaelService = FakeRijndaelService();
            var controller = new PayslipController(sessionService, baseService, commonService, payslipService, rijndaelService);

            return controller;
        }

        #endregion Helpers
    }
}