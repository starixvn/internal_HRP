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
            var actual = await monthlyPayrollService.GetDataFilterForIndexPageAsync(Arg.Any<DateTime>(), companyId, Arg.Any<string>());

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
            var actual = await monthlyPayrollService.GetDataFilterForIndexPageAsync(Arg.Any<DateTime>(), 1, Arg.Any<string>());

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
            Assert.Equal(employeePayrolls.First().Policy, actual.First().Policy);

            Assert.Equal(employeePayrolls.First().BasicSalary.EbankingCurrencyCode, actual.First().BasicSalary.EbankingCurrencyCode);
            //Assert.Equal(plainText, actual.First().BasicSalary.EBankingAmount);
            Assert.Equal(employeePayrolls.First().BasicSalary.DirectBanking1CurrencyCode, actual.First().BasicSalary.DirectBanking1CurrencyCode);
            //Assert.Equal(plainText, actual.First().BasicSalary.DirectBanking1Amount);
            Assert.Equal(employeePayrolls.First().BasicSalary.DirectBanking2CurrencyCode, actual.First().BasicSalary.DirectBanking2CurrencyCode);
            //Assert.Equal(plainText, actual.First().BasicSalary.DirectBanking2Amount);
            Assert.Equal(employeePayrolls.First().BasicSalary.CashCurrencyCode, actual.First().BasicSalary.CashCurrencyCode);
            //Assert.Equal(plainText, actual.First().BasicSalary.CashAmount);

            Assert.Equal(employeePayrolls.First().BasicSalary.DirectBanking1CurrencyCode, actual.First().BasicSalary.DirectBanking1CurrencyCode);
            //Assert.Equal(plainText, actual.First().BasicSalary.DirectBanking1Amount);
            Assert.Equal(employeePayrolls.First().BasicSalary.DirectBanking2CurrencyCode, actual.First().BasicSalary.DirectBanking2CurrencyCode);
            //Assert.Equal(plainText, actual.First().BasicSalary.DirectBanking2Amount);
            Assert.Equal(employeePayrolls.First().BasicSalary.CashCurrencyCode, actual.First().BasicSalary.CashCurrencyCode);
            //Assert.Equal(plainText, actual.First().BasicSalary.CashAmount);
        }

        #endregion GetMonthlyPayrollAsync

        #region GetMonthlySalaryToEditAsync

        [Theory]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        [InlineData(0, 0)]
        public async Task GetMonthlySalaryToEditAsync_ParamsAreInvalid_ReturnNull(int payrollId, int userId)
        {
            // Arrange
            var monthlyPayrollService = GetMonthlyPayrollService();
            sessionService.UserId = userId;

            // Act
            var actual = await monthlyPayrollService.GetMonthlySalaryToEditAsync(payrollId);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task GetMonthlySalaryToEditAsync_MonthlyPayrollIsNotExisted_ReturnNull()
        {
            // Arrange
            var monthlyPayrollService = GetMonthlyPayrollService();
            monthlyPayrollRepository.GetMonthlySalaryToEditAsync(Arg.Any<GetMonthlySalaryToEditCriteria>())
                .Returns(x => Task.FromResult<EditMonthlySalaryViewModel>(null));

            // Act
            var actual = await monthlyPayrollService.GetMonthlySalaryToEditAsync(1);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task GetMonthlySalaryToEditAsync_MonthlyPayrollIsExisted_ReturnAsExpectedViewModel()
        {
            // Arrange
            var plainText = "1234";
            var viewModel = new EditMonthlySalaryViewModel
            {
                MonthlyBasicSalary = new MonthlyBasicSalaryModel { WorkingDays = 10 },
                Additions = new List<MonthlySalaryAdditionModel> { new MonthlySalaryAdditionModel() },
                Deductions = new List<MonthlySalaryDeductionModel> { new MonthlySalaryDeductionModel() }
            };
            var monthlyPayrollService = GetMonthlyPayrollService();
            monthlyPayrollRepository.GetMonthlySalaryToEditAsync(Arg.Any<GetMonthlySalaryToEditCriteria>()).Returns(viewModel);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(plainText);

            // Act
            var actual = await monthlyPayrollService.GetMonthlySalaryToEditAsync(1);

            // Assert
            Assert.Equal(viewModel.MonthlyBasicSalary.CurrencyCode, actual.MonthlyBasicSalary.CurrencyCode);
            Assert.Equal(viewModel.MonthlyBasicSalary.Salary, actual.MonthlyBasicSalary.Salary);
            Assert.Equal(viewModel.MonthlyBasicSalary.WorkingDays, actual.MonthlyBasicSalary.WorkingDays);
            Assert.Equal("123.4", actual.MonthlyBasicSalary.SalaryPerDay);
            Assert.Equal(viewModel.MonthlyBasicSalary.EBankingCurrencyCode, actual.MonthlyBasicSalary.EBankingCurrencyCode);
            Assert.Equal(plainText, actual.MonthlyBasicSalary.EBankingAmount);
            Assert.Equal(viewModel.MonthlyBasicSalary.DirectBanking1CurrencyCode, actual.MonthlyBasicSalary.DirectBanking1CurrencyCode);
            Assert.Equal(plainText, actual.MonthlyBasicSalary.DirectBanking1Amount);
            Assert.Equal(viewModel.MonthlyBasicSalary.DirectBanking2CurrencyCode, actual.MonthlyBasicSalary.DirectBanking2CurrencyCode);
            Assert.Equal(plainText, actual.MonthlyBasicSalary.DirectBanking2Amount);
            Assert.Equal(viewModel.MonthlyBasicSalary.CashCurrencyCode, actual.MonthlyBasicSalary.CashCurrencyCode);
            Assert.Equal(plainText, actual.MonthlyBasicSalary.CashAmount);
            Assert.Equal(viewModel.MonthlyBasicSalary.AutoCalculateOn, actual.MonthlyBasicSalary.AutoCalculateOn);

            Assert.Equal(viewModel.MonthlySalary.CurrencyId, actual.MonthlySalary.CurrencyId);
            Assert.Equal(viewModel.MonthlySalary.Salary, actual.MonthlySalary.Salary);
            Assert.Equal(viewModel.MonthlySalary.EBankingCurrencyId, actual.MonthlySalary.EBankingCurrencyId);
            Assert.Equal(viewModel.MonthlySalary.EBankingAccountId, actual.MonthlySalary.EBankingAccountId);
            Assert.Equal(plainText, actual.MonthlySalary.EBankingAmount);
            Assert.Equal(viewModel.MonthlySalary.DirectBanking1CurrencyId, actual.MonthlySalary.DirectBanking1CurrencyId);
            Assert.Equal(viewModel.MonthlySalary.DirectBanking1BankTypeId, actual.MonthlySalary.DirectBanking1BankTypeId);
            Assert.Equal(viewModel.MonthlySalary.DirectBanking1AccountId, actual.MonthlySalary.DirectBanking1AccountId);
            Assert.Equal(plainText, actual.MonthlySalary.DirectBanking1Amount);
            Assert.Equal(viewModel.MonthlySalary.DirectBanking2CurrencyId, actual.MonthlySalary.DirectBanking2CurrencyId);
            Assert.Equal(viewModel.MonthlySalary.DirectBanking2BankTypeId, actual.MonthlySalary.DirectBanking2BankTypeId);
            Assert.Equal(viewModel.MonthlySalary.DirectBanking2AccountId, actual.MonthlySalary.DirectBanking2AccountId);
            Assert.Equal(plainText, actual.MonthlySalary.DirectBanking2Amount);
            Assert.Equal(viewModel.MonthlySalary.CashCurrencyId, actual.MonthlySalary.CashCurrencyId);
            Assert.Equal(viewModel.MonthlySalary.CashAccountId, actual.MonthlySalary.CashAccountId);
            Assert.Equal(plainText, actual.MonthlySalary.CashAmount);

            Assert.Collection(
                actual.Additions,
                kvp => { Assert.Equal(plainText, kvp.NetSalary); Assert.Equal(plainText, kvp.UnitAmount); });

            Assert.Collection(
                actual.Deductions,
                kvp => { Assert.Equal(plainText, kvp.NetSalary); });
        }

        #endregion GetMonthlySalaryToEditAsync

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
            var actual = await monthlyPayrollService.ResetMonthlySalaryAsync(1, 1);

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
            var actual = await monthlyPayrollService.EditMonthlySalaryAsync(new EditMonthlySalaryViewModel());

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditMonthlySalaryAsync_WhenCalled_ExpectedEncryptData()
        {
            // Arrange
            var criteria = new EditMonthlySalaryCriteria();
            var cipherText = "cipherText";
            var monthlyPayrollService = GetMonthlyPayrollService();
            baseService.ExecuteNonQueryAsync(Arg.Do<EditMonthlySalaryCriteria>(x => criteria = x)).Returns(new CommandResult());
            cryptoService.Encrypt(Arg.Any<string>()).Returns(cipherText);

            // Act
            await monthlyPayrollService.EditMonthlySalaryAsync(new EditMonthlySalaryViewModel());

            // Assert
            Assert.Equal(cipherText, criteria.Salary);
            Assert.Equal(cipherText, criteria.EBankingAmount);
            Assert.Equal(cipherText, criteria.DirectBanking1Amount);
            Assert.Equal(cipherText, criteria.DirectBanking2Amount);
            Assert.Equal(cipherText, criteria.CashAmount);
        }

        #endregion EditMonthlySalaryAsync

        #region CalculateBackPay

        [Fact]
        public void CalculateBackPay_OldSalaryEqualNewSalary_ReturnDefaultBackPay()
        {
            // Arrange
            var monthlyPayrollService = GetMonthlyPayrollService();

            // Act
            var actual = monthlyPayrollService.CalculateBackPay(Arg.Any<DateTime>(), Arg.Any<DateTime>(), 1000, 1000);

            // Assert
            Assert.Null(actual.Amount);
            Assert.Null(actual.Description);
        }

        [Fact]
        public void CalculateBackPay_WorkingMonthEqualEffectiveMonth_ReturnDefaultBackPay()
        {
            // Arrange
            var workingMonth = DateTime.Now;
            var monthlyPayrollService = GetMonthlyPayrollService();

            // Act
            var actual = monthlyPayrollService.CalculateBackPay(workingMonth, workingMonth, 1000, 1100);

            // Assert
            Assert.Null(actual.Amount);
            Assert.Null(actual.Description);
        }

        //[Fact]
        //public void CalculateBackPay_EffectiveDateIsFirstDayOfMonth_ReturnAsExpectedBackPay()
        //{
        //    // Arrange
        //    var workingMonth = new DateTime(2018, 3, 1);
        //    var effectiveDate = workingMonth.AddMonths(-1);
        //    var monthlyPayrollService = GetMonthlyPayrollService();

        //    // Act
        //    var actual = monthlyPayrollService.CalculateBackPay(effectiveDate, workingMonth, 1000, 1200);

        //    // Assert
        //    Assert.Equal("400", actual.Amount);
        //    Assert.Equal("Annual Increment from Jan 01, 2018", actual.Description);
        //}

        #endregion CalculateBackPay

        [Fact]
        public async Task GetMonthlyAllowancesAsync_ValidPayrollId_ReturnViewModel()
        {
            var allowance = new MonthlyAllowanceModel();
            var additions = new List<MonthlyAllowanceAdditionModel>
            {
                new MonthlyAllowanceAdditionModel
                {
                    PayrollId = 1,
                    NoOfDays = 20,
                    Type = 1,
                    NetAllowance = string.Empty,
                    TotalAmount = string .Empty,
                    UnitAmount = string.Empty,
                    CurrencyId = 1
                }
            };
            var deductions = new List<MonthlyAllowanceDeductionModel>
            {
                new MonthlyAllowanceDeductionModel
                {
                    PayrollId = 1,
                    NoOfDays = 20,
                    Type = 1,
                    NetAllowance = string.Empty,
                    TotalAmount = string .Empty,
                    UnitAmount = string.Empty,
                    CurrencyId = 1
                }
            };

            var monthlyPayrollService = GetMonthlyPayrollService();
            cryptoService.Decrypt(Arg.Any<string>()).Returns(string.Empty);
            monthlyPayrollRepository.GetMonthlyAllowancesAsync(Arg.Any<GetMonthlyAllowanceCriteria>())
                .Returns(new Tuple<MonthlyAllowanceModel, IEnumerable<MonthlyAllowanceAdditionModel>, IEnumerable<MonthlyAllowanceDeductionModel>>(allowance, additions, deductions));

            var actual = await monthlyPayrollService.GetMonthlyAllowancesAsync(1);
            Assert.NotNull(actual);
            Assert.NotNull(actual.Allowance);
            Assert.NotNull(actual.Additions);
            Assert.NotNull(actual.Deductions);
        }

        [Fact]
        public async Task EditMonthlyAllowancesAsync_ValidModel_ReturnCommandResult()
        {
            var monthlyPayrollService = GetMonthlyPayrollService();
            cryptoService.Encrypt(Arg.Any<string>()).Returns(string.Empty);
            baseService.ExecuteNonQueryAsync(Arg.Any<EditMonthlyAllowancesCriteria>()).Returns(new CommandResult { Code = 0 });
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
                        UnitAmount = string.Empty,
                        CurrencyId = 1
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
                        UnitAmount = string.Empty,
                        CurrencyId = 1
                    }
                }
            };

            var actual = await monthlyPayrollService.EditMonthlyAllowancesAsync(viewModel, 1);
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