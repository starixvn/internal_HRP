namespace HRP.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.EmployeeManagement;
    using Models.EmployeeManagement.Payroll;
    using NSubstitute;
    using Xunit;

    public class EmployeePayrollServiceTests
    {
        private IBaseService baseService;
        private ICryptoService cryptoService;
        private readonly IObjectDb objectDb;

        public EmployeePayrollServiceTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        #region GetEmployeePayroll

        [Fact]
        public async Task GetEmployeePayroll_NotExistedEmployeePayroll_ReturnNull()
        {
            // Arrange
            var multiResultReader = FakeMultipleResultReader();
            multiResultReader.ReadFirstOrDefault<EmployeePayrollViewModel>().Returns(x => null);
            objectDb.QueryMultiple(Arg.Any<ICriteria>()).Returns(multiResultReader);

            var service = GetEmployeePayrollService();

            // Act
            var actual = service.GetEmployeePayroll(1, 1);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task GetEmployeePayroll_ExistedEmployeePayroll_ReturnAsExpectedModel()
        {
            // Arrange
            var plainText = "1234";
            var employeeId = 1;
            var employeePayroll = new EmployeePayrollViewModel
            {
                EmployeeId = employeeId,
                FullName = "Full Name",
                Nickname = "Nick Name",
                Code = "DAR",
                CompanyId = 1
            };
            var basicSalary = new EmployeePayrollBasicSalaryModel
            {
                EmployeeId = employeeId,
                WorkingDays = 22,
                AutoCalculateOn = "Cash",
                InitFirstTime = true
            };
            var allowance = new EmployeePayrollAllowanceModel { EmployeeId = employeeId, AutoCalculateOn = "Cash" };
            var deduction = new EmployeePayrollDeduction { EmployeeId = employeeId };
            var currencyExchangeRates = new List<CurrencyExchangeRateModel>();
            var currencyCodes = new List<CurrencyCodeModel>();
            var refResources = new List<ReferenceResourceModel>();
            var multiResultReader = FakeMultipleResultReader();
            multiResultReader.ReadFirstOrDefault<EmployeePayrollViewModel>().Returns(employeePayroll);
            multiResultReader.ReadFirstOrDefault<EmployeePayrollBasicSalaryModel>().Returns(basicSalary);
            multiResultReader.ReadFirstOrDefault<EmployeePayrollAllowanceModel>().Returns(allowance);
            multiResultReader.ReadFirstOrDefault<EmployeePayrollDeduction>().Returns(deduction);
            objectDb.QueryMultiple(Arg.Any<ICriteria>()).Returns(multiResultReader);

            var service = GetEmployeePayrollService();
            baseService.GetData<CurrencyExchangeRateModel>(Arg.Any<ICriteria>()).Returns(currencyExchangeRates);
            baseService.GetData<CurrencyCodeModel>(Arg.Any<ICriteria>()).Returns(currencyCodes);
            baseService.GetData<ReferenceResourceModel>(Arg.Any<ICriteria>()).Returns(refResources);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(plainText);

            // Act
            var actual = await service.GetEmployeePayroll(employeeId, 1);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(employeePayroll.EmployeeId, actual.EmployeeId);
            Assert.Equal(employeePayroll.FullName, actual.FullName);
            Assert.Equal(employeePayroll.Nickname, actual.Nickname);
            Assert.Equal(employeePayroll.Code, actual.Code);
            Assert.Equal(employeePayroll.CompanyId, actual.CompanyId);
            Assert.Equal(employeePayroll.CanRead, actual.CanRead);
            Assert.Equal(employeePayroll.CanUpdate, actual.CanUpdate);

            Assert.Equal(plainText, actual.BasicSalary.Salary);
            Assert.Equal(plainText, actual.BasicSalary.OldSalary);
            Assert.Equal(plainText, actual.BasicSalary.EbankingAmount);
            Assert.Equal(plainText, actual.BasicSalary.DirectBanking1Amount);
            Assert.Equal(plainText, actual.BasicSalary.DirectBanking2Amount);
            Assert.Equal(plainText, actual.BasicSalary.CashAmount);

            Assert.Equal(plainText, actual.Allowance.DirectBanking1Amount);
            Assert.Equal(plainText, actual.Allowance.DirectBanking2Amount);
            Assert.Equal(plainText, actual.Allowance.CashAmount);
            Assert.Equal(plainText, actual.Allowance.SubsistenceAmount);
            Assert.Equal(plainText, actual.Allowance.HomePassageAmount);
            Assert.Equal(plainText, actual.Allowance.HousingAmount);
            Assert.Equal(plainText, actual.Allowance.MobilePhoneAmount);
            Assert.Equal(plainText, actual.Allowance.HPIAmount);
            Assert.Equal(plainText, actual.Allowance.DutyMealAmount);
            Assert.Equal(plainText, actual.Allowance.SecondmentAmount);
            Assert.Equal(plainText, actual.Allowance.OthersAmount);

            Assert.Equal(plainText, actual.Deduction.RoomingChargesAmount);
            Assert.Equal(plainText, actual.Deduction.FitnessFirstAmount);

            Assert.Equal(currencyExchangeRates, actual.CurrencyExchangeRates);
            Assert.Equal(currencyCodes, actual.Currencies);
            Assert.Equal(refResources, actual.ChangeReasons);
            Assert.Equal(refResources, actual.CashAccounts);
        }

        #endregion GetEmployeePayroll

        #region UpdatePayroll

        [Fact]
        public async Task UpdatePayroll_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var model = new EmployeePayrollViewModel();
            var expected = new CommandResult();
            var service = GetEmployeePayrollService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateEmployeePayrollCriteria>()).Returns(expected);
            cryptoService.Encrypt(Arg.Any<string>()).Returns(string.Empty);

            // Act
            var actual = await service.UpdatePayroll(model, 1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task UpdatePayroll_WhenCalled_ExpectedCallEncrypt()
        {
            // Arrange
            var model = new EmployeePayrollViewModel();
            var expected = new CommandResult();
            var service = GetEmployeePayrollService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateEmployeePayrollCriteria>()).Returns(expected);
            cryptoService.Encrypt(Arg.Any<string>()).Returns(string.Empty);

            // Act
            var actual = await service.UpdatePayroll(model, 1);

            // Assert
            cryptoService.Received(18).Encrypt(Arg.Any<string>());
        }

        #endregion UpdatePayroll

        #region Helpers

        private static IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        private static ICryptoService FakeCryptoService()
        {
            return Substitute.For<ICryptoService>();
        }

        private static IMultiResultReader FakeMultipleResultReader()
        {
            return Substitute.For<IMultiResultReader>();
        }

        private EmployeePayrollService GetEmployeePayrollService()
        {
            baseService = FakeBaseService();
            cryptoService = FakeCryptoService();
            var service = new EmployeePayrollService(baseService, cryptoService);

            return service;
        }

        #endregion Helpers
    }
}