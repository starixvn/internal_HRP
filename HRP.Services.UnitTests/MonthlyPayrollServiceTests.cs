namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.MonthlyPayroll;
    using HRP.Models;
    using HRP.Models.MonthlyPayroll;
    using HRP.Repositories;
    using NSubstitute;
    using Xunit;

    public class MonthlyPayrollServiceTests
    {
        private ISessionService sessionService;
        private IBaseService baseService;
        private ICryptoService cryptoService;
        private IMonthlyPayrollRepository monthlyPayrollRepository;

        #region GetDataFilterForIndexPageAsync

        [Theory]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        [InlineData(0, 0)]
        public async Task GetDataFilterForIndexPageAsync_ParamsAreInvalid_ReturnDefaultViewModel(int userId, int companyId)
        {
            // Arrange
            var monthlyPayrollService = GetMonthlyPayrollService();
            sessionService.UserId = userId;

            // Act
            var actual = await monthlyPayrollService.GetDataFilterForIndexPageAsync(companyId, Arg.Any<string>());

            // Assert
            Assert.IsType<EmployeeMonthlyPayrollViewModel>(actual);
            Assert.Equal(DateTime.MinValue, actual.Month);
            Assert.Equal(0, actual.CompanyId);
            Assert.Null(actual.CompanyCode);
            Assert.Null(actual.CostCenterIds);
            Assert.Null(actual.EmployeeId);
            Assert.Null(actual.WorkingDays);
            Assert.Empty(actual.HierarchyCompanyJson);
            Assert.Empty(actual.Employees);
            Assert.Empty(actual.HierarchyCompanies);
        }

        [Fact]
        public async Task GetDataFilterForIndexPageAsync_ParamsAreValid_ReturnAsExpectedViewModel()
        {
            // Arrange
            var viewModel = new EmployeeMonthlyPayrollViewModel
            {
                CostCenterIds = "1,2"
            };
            var monthlyPayrollService = GetMonthlyPayrollService();
            monthlyPayrollRepository.GetDataFilterForIndexPageAsync(Arg.Any<GetDataFilterForIndexPageCriteria>()).Returns(viewModel);

            // Act
            var actual = await monthlyPayrollService.GetDataFilterForIndexPageAsync(1, Arg.Any<string>());

            // Assert
            Assert.Equal(viewModel, actual);
        }

        #endregion GetDataFilterForIndexPageAsync

        #region GetMonthlyPayrollAsync

        [Fact]
        public async Task GetMonthlyPayrollAsync_CriteriaIsInvalid_ReturnEmptyList()
        {
            // Arrange
            var viewModel = new EmployeeMonthlyPayrollViewModel();
            var monthlyPayrollService = GetMonthlyPayrollService();

            // Act
            var actual = await monthlyPayrollService.GetMonthlyPayrollAsync(viewModel);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetMonthlyPayrollAsync_CriteriaIsValid_ReturnAsExpectedViewModel()
        {
            // Arrange
            var plainText = "1234";
            var viewModel = new EmployeeMonthlyPayrollViewModel
            {
                CompanyId = 1
            };
            var employeePayrolls = new List<MonthlyPayrollEmployeeInfoViewModel>
            {
                new MonthlyPayrollEmployeeInfoViewModel { }
            };
            var monthlyPayrollService = GetMonthlyPayrollService();
            cryptoService.Decrypt(Arg.Any<string>()).Returns(plainText);
            monthlyPayrollRepository.GetMonthlyPayrollAsync(Arg.Any<GetMonthlyPayrollCriteria>()).Returns(employeePayrolls);

            // Act
            var actual = await monthlyPayrollService.GetMonthlyPayrollAsync(viewModel);

            // Assert
            Assert.Single(actual);
            Assert.Equal(employeePayrolls.First().PayrollId, actual.First().PayrollId);
            Assert.Equal(employeePayrolls.First().EmployeeId, actual.First().EmployeeId);
            Assert.Equal(employeePayrolls.First().IDNo, actual.First().IDNo);
            Assert.Equal(employeePayrolls.First().Nickname, actual.First().Nickname);
            Assert.Equal(employeePayrolls.First().FullName, actual.First().FullName);
            Assert.Equal(employeePayrolls.First().CostCenter, actual.First().CostCenter);
            Assert.Equal(employeePayrolls.First().WorkingDays, actual.First().WorkingDays);
            Assert.Equal(employeePayrolls.First().EditSalaryPolicy, actual.First().EditSalaryPolicy);
            Assert.Equal(employeePayrolls.First().EditAllowancePolicy, actual.First().EditAllowancePolicy);

            Assert.Equal(employeePayrolls.First().BasicSalary.EbankingCurrencyCode, actual.First().BasicSalary.EbankingCurrencyCode);
            Assert.Equal(plainText, actual.First().BasicSalary.EBankingAmount);
            Assert.Equal(employeePayrolls.First().BasicSalary.DirectBanking1CurrencyCode, actual.First().BasicSalary.DirectBanking1CurrencyCode);
            Assert.Equal(plainText, actual.First().BasicSalary.DirectBanking1Amount);
            Assert.Equal(employeePayrolls.First().BasicSalary.DirectBanking2CurrencyCode, actual.First().BasicSalary.DirectBanking2CurrencyCode);
            Assert.Equal(plainText, actual.First().BasicSalary.DirectBanking2Amount);
            Assert.Equal(employeePayrolls.First().BasicSalary.CashCurrencyCode, actual.First().BasicSalary.CashCurrencyCode);
            Assert.Equal(plainText, actual.First().BasicSalary.CashAmount);

            Assert.Equal(employeePayrolls.First().BasicSalary.DirectBanking1CurrencyCode, actual.First().BasicSalary.DirectBanking1CurrencyCode);
            Assert.Equal(plainText, actual.First().BasicSalary.DirectBanking1Amount);
            Assert.Equal(employeePayrolls.First().BasicSalary.DirectBanking2CurrencyCode, actual.First().BasicSalary.DirectBanking2CurrencyCode);
            Assert.Equal(plainText, actual.First().BasicSalary.DirectBanking2Amount);
            Assert.Equal(employeePayrolls.First().BasicSalary.CashCurrencyCode, actual.First().BasicSalary.CashCurrencyCode);
            Assert.Equal(plainText, actual.First().BasicSalary.CashAmount);
        }

        #endregion GetMonthlyPayrollAsync

        [Fact]
        public async Task GetEmployeeSuggestionByNameAsync_WhenCalled_ReturnExpectedEmployees()
        {
            // Arrange
            var expected = new List<EmployeeSuggestionViewModel>
            {
                new EmployeeSuggestionViewModel { EmployeeId = 1, IDNo = "DAR111", FullName = "Tim", Nickname = "tim.trinh" }
            };
            var monthlyPayrollService = GetMonthlyPayrollService();
            baseService.GetDataAsync<EmployeeSuggestionViewModel>(Arg.Any<GetEmployeeSuggestionByNameCriteria>()).Returns(expected);

            // Act
            var actual = await monthlyPayrollService.GetEmployeeSuggestionByNameAsync(1, "tim");

            // Assert
            Assert.Equal(expected, actual);
        }

        #region ResetMonthlySalaryAsync

        [Fact]
        public async Task ResetMonthlySalaryAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var monthlyPayrollService = GetMonthlyPayrollService();
            baseService.ExecuteNonQueryAsync(Arg.Any<ResetMonthlySalaryCriteria>()).Returns(expected);

            // Act
            var actual = await monthlyPayrollService.ResetMonthlySalaryAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion ResetMonthlySalaryAsync

        #region EditMonthlySalaryAsync

        [Fact]
        public async Task EditMonthlySalaryAsync_WhenCalled_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var monthlyPayrollService = GetMonthlyPayrollService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditMonthlySalaryCriteria>()).Returns(expected);

            // Act
            var actual = await monthlyPayrollService.EditMonthlySalaryAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion EditMonthlySalaryAsync

        [Fact]
        public async Task GetMonthlyAllowancesAsync_ValidPayrollId_ReturnViewModel()
        {
            var monthlyPayrollService = GetMonthlyPayrollService();
            cryptoService.Decrypt(Arg.Any<string>()).Returns(string.Empty);
            monthlyPayrollRepository.GetMonthlyAllowancesAsync(Arg.Any<GetMonthlyAllowanceCriteria>())
                .Returns(new Tuple<MonthlyAllowanceModel, IEnumerable<MonthlyAllowanceAdditionModel>, IEnumerable<MonthlyAllowanceDeductionModel>>
                (new MonthlyAllowanceModel(), new List<MonthlyAllowanceAdditionModel>(), new List<MonthlyAllowanceDeductionModel>()));

            var actual = await monthlyPayrollService.GetMonthlyAllowancesAsync(1);
            Assert.NotNull(actual);
            Assert.NotNull(actual.Allowance);
            Assert.NotNull(actual.Additions);
            Assert.NotNull(actual.Deductions);
        }

        [Fact]
        public async Task UpdateMonthlyAllowancesAsync_ValidModel_ReturnCommandResult()
        {
            var monthlyPayrollService = GetMonthlyPayrollService();
            cryptoService.Encrypt(Arg.Any<string>()).Returns(string.Empty);
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateMonthlyAllowancesCriteria>()).Returns(new CommandResult { Code = 0 });
            var viewModel = new MonthlyAllowancesViewModel
            {
                Allowance = new MonthlyAllowanceModel(),
                Additions = new List<MonthlyAllowanceAdditionModel>
                {
                    new MonthlyAllowanceAdditionModel
                    {
                        PayrollId = 1,
                        NoOfDays = 20,
                        Type = 1,
                        NetAllowance = string.Empty,
                        TotalAmount = string .Empty,
                        UnitAmount = string.Empty
                    }
                },
                Deductions = new List<MonthlyAllowanceDeductionModel>
                {
                    new MonthlyAllowanceDeductionModel
                    {
                        PayrollId = 1,
                        NoOfDays = 20,
                        Type = 1,
                        NetAllowance = string.Empty,
                        TotalAmount = string .Empty,
                        UnitAmount = string.Empty
                    }
                }
            };

            var actual = await monthlyPayrollService.UpdateMonthlyAllowancesAsync(viewModel, 1);
            Assert.True(actual.IsSuccessful);
        }

        #region Helper

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private static IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        private static ICryptoService FakeCryptoService()
        {
            return Substitute.For<ICryptoService>();
        }

        private static IMonthlyPayrollRepository FakeMonthlyPayrollRepository()
        {
            return Substitute.For<IMonthlyPayrollRepository>();
        }

        private MonthlyPayrollService GetMonthlyPayrollService()
        {
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            cryptoService = FakeCryptoService();
            monthlyPayrollRepository = FakeMonthlyPayrollRepository();
            var service = new MonthlyPayrollService(sessionService, baseService, cryptoService, monthlyPayrollRepository);

            return service;
        }

        #endregion Helper
    }
}