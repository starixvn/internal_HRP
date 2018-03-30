namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.Payslip;
    using HRP.Models;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.Payslip;
    using HRP.Repositories;
    using NSubstitute;
    using Xunit;

    public class PayslipServiceTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private ICryptoService cryptoService;
        private IPayslipRepository payslipRepository;

        [Fact]
        public async Task GetCompanyAsync_WhenCalled_ReturnsAsExpectedCompanies()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company { CompanyId = 1, Code = "DAR" }
            };
            var payslipService = GetPayslipService();
            baseService.GetDataAsync<Company>(Arg.Is<GetCompanyByFunctionKeyCriteria>(x => x.UserId == sessionService.UserId && x.FunctionKey == "Payslip")).Returns(companies);

            // Act
            var actual = await payslipService.GetCompanyAsync();

            // Assert
            Assert.Equal(actual, companies);
        }

        [Fact]
        public async Task GetPayslipAsync_WhenCalled_ReturnsAsExpectedPayslips()
        {
            // Arrange
            var payslips = new List<PayslipModel>
            {
                new PayslipModel { PayrollId = 1, FullName = "thang", Nickname = "tim.trinh" }
            };
            var payslipService = GetPayslipService();
            baseService.GetDataAsync<PayslipModel>(Arg.Any<GetPayslipCriteria>()).Returns(payslips);

            // Act
            var actual = await payslipService.GetPayslipAsync(Arg.Any<DateTime>(), Arg.Any<int>(), Arg.Any<bool?>());

            // Assert
            Assert.Equal(actual, payslips);
        }

        #region GetPayslipDetailAsync

        [Fact]
        public async Task GetPayslipDetailAsync_NotExistedPayroll_ReturnDefaultModel()
        {
            // Arrange
            PayslipDetailModel expected = null;
            var payslipService = GetPayslipService();
            payslipRepository.GetPayslipDetailAsync(Arg.Any<GetPayslipDetailsCriteria>()).Returns(expected);

            // Act
            var actual = await payslipService.GetPayslipDetailAsync(1, 1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetPayslipDetailAsync_ExistedPayroll_ReturnsAsExpectedModel()
        {
            // Arrange
            var plainText = "1234";
            var expectedAmount = "1,234.00";
            var payslip = new PayslipDetailModel
            {
                SalaryAdditions = new List<MonthlySalaryAdditionModel> { new MonthlySalaryAdditionModel() },
                SalaryDeductions = new List<MonthlySalaryDeductionModel> { new MonthlySalaryDeductionModel() },
                AllowanceAdditions = new List<MonthlyAllowanceAdditionModel> { new MonthlyAllowanceAdditionModel() },
                AllowanceDeductions = new List<MonthlyAllowanceDeductionModel> { new MonthlyAllowanceDeductionModel() }
            };
            var payslipService = GetPayslipService();
            payslipRepository.GetPayslipDetailAsync(Arg.Any<GetPayslipDetailsCriteria>()).Returns(payslip);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(plainText);

            // Act
            var actual = await payslipService.GetPayslipDetailAsync(1, 1);

            // Assert
            Assert.Equal(payslip.PayrollId, actual.PayrollId);
            Assert.Equal(payslip.Month, actual.Month);
            Assert.Equal(payslip.FullName, actual.FullName);
            Assert.Equal(payslip.IDNo, actual.IDNo);
            Assert.Equal(payslip.CompanyName, actual.CompanyName);
            Assert.Equal(payslip.SalaryCurrencyId, actual.SalaryCurrencyId);
            Assert.Equal(payslip.SalaryCurrencyCode, actual.SalaryCurrencyCode);
            Assert.Equal(expectedAmount, actual.SalaryAmount);
            Assert.Equal(payslip.AllowanceCurrencyId, actual.AllowanceCurrencyId);
            Assert.Equal(payslip.AllowanceCurrencyCode, actual.AllowanceCurrencyCode);
            Assert.Equal(expectedAmount, actual.AllowanceAmount);
            Assert.Collection(
                actual.SalaryAdditions,
                kvp => { Assert.Equal(plainText, kvp.NetSalary); Assert.Equal(expectedAmount, kvp.NetSalaryDisplay); });

            Assert.Collection(
                actual.SalaryDeductions,
                kvp => { Assert.Equal(plainText, kvp.NetSalary); Assert.Equal(expectedAmount, kvp.NetSalaryDisplay); });

            Assert.Collection(
                actual.AllowanceAdditions,
                kvp => { Assert.Equal(plainText, kvp.NetAllowance); Assert.Equal(expectedAmount, kvp.NetAllowanceToDisplay); });

            Assert.Collection(
                actual.AllowanceDeductions,
                kvp => { Assert.Equal(plainText, kvp.NetAllowance); Assert.Equal(expectedAmount, kvp.NetAllowanceToDisplay); });
        }

        #endregion GetPayslipDetailAsync

        #region Helpers

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 99;

            return service;
        }

        private static ICryptoService FakeCryptoService()
        {
            return Substitute.For<ICryptoService>();
        }

        private static IPayslipRepository FakePayslipRepository()
        {
            return Substitute.For<IPayslipRepository>();
        }

        private PayslipService GetPayslipService()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            cryptoService = FakeCryptoService();
            payslipRepository = FakePayslipRepository();
            var service = new PayslipService(baseService, sessionService, cryptoService, payslipRepository);

            return service;
        }

        #endregion Helpers
    }
}