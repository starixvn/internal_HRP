namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.MonthlyPayroll;
    using HRP.Models;
    using HRP.Models.Currency;
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
            Assert.Equal(plainText, actual.First().BasicSalary.EbankingAmount);
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
            Assert.Equal(123.4M, actual.MonthlyBasicSalary.SalaryPerDay);
            Assert.Equal(viewModel.MonthlyBasicSalary.EbankingCurrencyCode, actual.MonthlyBasicSalary.EbankingCurrencyCode);
            Assert.Equal(plainText, actual.MonthlyBasicSalary.EbankingAmount);
            Assert.Equal(viewModel.MonthlyBasicSalary.DirectBanking1CurrencyCode, actual.MonthlyBasicSalary.DirectBanking1CurrencyCode);
            Assert.Equal(plainText, actual.MonthlyBasicSalary.DirectBanking1Amount);
            Assert.Equal(viewModel.MonthlyBasicSalary.DirectBanking2CurrencyCode, actual.MonthlyBasicSalary.DirectBanking2CurrencyCode);
            Assert.Equal(plainText, actual.MonthlyBasicSalary.DirectBanking2Amount);
            Assert.Equal(viewModel.MonthlyBasicSalary.CashCurrencyCode, actual.MonthlyBasicSalary.CashCurrencyCode);
            Assert.Equal(plainText, actual.MonthlyBasicSalary.CashAmount);
            Assert.Equal(viewModel.MonthlyBasicSalary.AutoCalculateOn, actual.MonthlyBasicSalary.AutoCalculateOn);

            Assert.Equal(viewModel.MonthlySalary.CurrencyId, actual.MonthlySalary.CurrencyId);
            Assert.Equal(viewModel.MonthlySalary.Salary, actual.MonthlySalary.Salary);
            Assert.Equal(viewModel.MonthlySalary.EbankingCurrencyId, actual.MonthlySalary.EbankingCurrencyId);
            Assert.Equal(viewModel.MonthlySalary.EbankingAccountId, actual.MonthlySalary.EbankingAccountId);
            Assert.Equal(plainText, actual.MonthlySalary.EbankingAmount);
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

        [Fact]
        public async Task GetEmployeeSuggestionByIDNoAsync_WhenCalled_ReturnExpectedEmployees()
        {
            // Arrange
            var expected = new List<EmployeeSuggestionViewModel>
            {
                new EmployeeSuggestionViewModel { EmployeeId = 1, IDNo = "DAR111", FullName = "Tim", Nickname = "tim.trinh" }
            };
            var monthlyPayrollService = GetMonthlyPayrollService();
            baseService.GetDataAsync<EmployeeSuggestionViewModel>(Arg.Any<GetEmployeeSuggestionByIDNoCriteria>()).Returns(expected);

            // Act
            var actual = await monthlyPayrollService.GetEmployeeSuggestionByIDNoAsync(1, "DAR111");

            // Assert
            Assert.Equal(expected, actual);
        }

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
            Assert.Equal(cipherText, criteria.EbankingAmount);
            Assert.Equal(cipherText, criteria.DirectBanking1Amount);
            Assert.Equal(cipherText, criteria.DirectBanking2Amount);
            Assert.Equal(cipherText, criteria.CashAmount);
        }

        #endregion EditMonthlySalaryAsync

        [Fact]
        public async Task GetMonthlyAllowancesAsync_ValidPayrollId_ReturnViewModel()
        {
            var allowance = new MonthlyAllowanceModel();
            var basicAllowance = new MonthlyPayrollBasicAllowanceModel();
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

            var info = new MonthlyAllowanceEmployeeInfoModel();

            var monthlyPayrollService = GetMonthlyPayrollService();
            cryptoService.Decrypt(Arg.Any<string>()).Returns(string.Empty);
            monthlyPayrollRepository.GetMonthlyAllowancesAsync(Arg.Any<GetMonthlyAllowanceCriteria>())
                .Returns(new Tuple<MonthlyAllowanceModel, IEnumerable<MonthlyAllowanceAdditionModel>, IEnumerable<MonthlyAllowanceDeductionModel>, MonthlyPayrollBasicAllowanceModel, MonthlyAllowanceEmployeeInfoModel>
                (allowance, additions, deductions, basicAllowance, info));

            var actual = await monthlyPayrollService.GetMonthlyAllowancesAsync(1, DateTime.Today);
            Assert.NotNull(actual);
            Assert.NotNull(actual.Allowance);
            Assert.NotNull(actual.Additions);
            Assert.NotNull(actual.Deductions);
            Assert.NotNull(actual.BasicAllowance);
            Assert.NotNull(actual.EmployeeInfo);
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

        [Fact]
        public void CalculateMonthlyBasicAllowance_StartDateInMonth_ReturnExpectedResult()
        {
            var monthlyPayrollService = GetMonthlyPayrollService();
            var payrollId = 1;

            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 }
            };

            var basicAllowance = new MonthlyPayrollBasicAllowanceModel
            {
                PayrollId = payrollId,
                AutoCalculateOn = PayrollAutoCalculateOn.Cash,
                CurrencyId = 1,
                CashCurrencyId = 1,
                SubsistenceAmount = "2000",
                SubsistenceCurrencyId = 1
            };
            cryptoService.Decrypt(basicAllowance.SubsistenceAmount).Returns(basicAllowance.SubsistenceAmount);
            cryptoService.Encrypt(basicAllowance.SubsistenceAmount).Returns(basicAllowance.SubsistenceAmount);
            cryptoService.Decrypt(null).Returns(string.Empty);
            cryptoService.Encrypt(null).Returns(string.Empty);
            baseService.GetData<CurrencyCodeModel>(Arg.Any<GetActiveCurrencyCodesCriteria>())
                .Returns(new List<CurrencyCodeModel> { new CurrencyCodeModel { CurrencyId = 1, CurrencyCode = "USD" } });

            monthlyPayrollService.CalculateMonthlyBasicAllowance(currencyExchangeRates, basicAllowance,
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, 15), null, DateTime.Today.Month, DateTime.Today.Year);

            Assert.NotEqual(basicAllowance.TotalAmount, basicAllowance.AfterAdjustmentAmount);
            Assert.NotNull(basicAllowance.Reason);
        }

        [Fact]
        public void CalculateMonthlyBasicAllowance_EndDateInMonth_ReturnExpectedResult()
        {
            var monthlyPayrollService = GetMonthlyPayrollService();
            var payrollId = 1;

            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 }
            };

            var basicAllowance = new MonthlyPayrollBasicAllowanceModel
            {
                PayrollId = payrollId,
                AutoCalculateOn = PayrollAutoCalculateOn.Cash,
                CurrencyId = 1,
                CashCurrencyId = 1,
                SubsistenceAmount = "2000",
                SubsistenceCurrencyId = 1
            };
            cryptoService.Decrypt(basicAllowance.SubsistenceAmount).Returns(basicAllowance.SubsistenceAmount);
            cryptoService.Encrypt(basicAllowance.SubsistenceAmount).Returns(basicAllowance.SubsistenceAmount);
            cryptoService.Decrypt(null).Returns(string.Empty);
            cryptoService.Encrypt(null).Returns(string.Empty);
            baseService.GetData<CurrencyCodeModel>(Arg.Any<GetActiveCurrencyCodesCriteria>())
                .Returns(new List<CurrencyCodeModel> { new CurrencyCodeModel { CurrencyId = 1, CurrencyCode = "USD" } });

            monthlyPayrollService.CalculateMonthlyBasicAllowance(currencyExchangeRates, basicAllowance, new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, 20), DateTime.Today.Month, DateTime.Today.Year);

            Assert.NotEqual(basicAllowance.TotalAmount, basicAllowance.AfterAdjustmentAmount);
            Assert.NotNull(basicAllowance.Reason);
        }

        [Fact]
        public void CalculateMonthlyBasicAllowance_StartDateAndEndDateInMonth_ReturnExpectedResult()
        {
            var monthlyPayrollService = GetMonthlyPayrollService();
            var payrollId = 1;

            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 }
            };

            var basicAllowance = new MonthlyPayrollBasicAllowanceModel
            {
                PayrollId = payrollId,
                AutoCalculateOn = PayrollAutoCalculateOn.Cash,
                CurrencyId = 1,
                CashCurrencyId = 1,
                SubsistenceAmount = "2000",
                SubsistenceCurrencyId = 1
            };

            baseService.GetData<CurrencyCodeModel>(Arg.Any<GetActiveCurrencyCodesCriteria>())
                .Returns(new List<CurrencyCodeModel> { new CurrencyCodeModel { CurrencyId = 1, CurrencyCode = "USD" } });

            monthlyPayrollService.CalculateMonthlyBasicAllowance(currencyExchangeRates, basicAllowance,
                DateTime.Today.AddMonths(-3), new DateTime(DateTime.Today.Year, DateTime.Today.Month, 20), DateTime.Today.Month, DateTime.Today.Year);

            Assert.NotEqual(basicAllowance.TotalAmount, basicAllowance.AfterAdjustmentAmount);
            Assert.NotNull(basicAllowance.Reason);
        }

        [Fact]
        public void InitializeMonthlyAllowance_ValidDate_ReturnModel()
        {
            var monthlyPayrollService = GetMonthlyPayrollService();
            var payrollId = 1;

            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 }
            };

            var allowanceAdditions = new List<MonthlyAllowanceAdditionModel>() { new MonthlyAllowanceAdditionModel { Type = (int)AllowanceType.DutyMeal } };
            var allowanceDeductions = new List<MonthlyAllowanceDeductionModel>() { new MonthlyAllowanceDeductionModel { Type = (int)AllowanceType.DutyMeal } };
            var dstNetSalary = new MonthlyDSTNetSalary { CurrencyId = 1, NetSalaryReceived = "333" };

            var basicAllowance = new MonthlyPayrollBasicAllowanceModel
            {
                PayrollId = payrollId,
                AutoCalculateOn = PayrollAutoCalculateOn.Cash,
                CurrencyId = 1,
                CashCurrencyId = 1,
                SubsistenceAmount = "2000",
                SubsistenceCurrencyId = 1,
                TotalAmount = "2000",
                AfterAdjustmentAmount = "2000",
                RoomingChargesCurrencyId = 1,
                RoomingChargesAmount = "100",
                FitnessFirstCurrencyId = 1,
                FitnessFirstAmount = "100"
            };

            var expectedTotalAmount = "1800";

            var result = monthlyPayrollService.InitializeMonthlyAllowance(basicAllowance, allowanceAdditions, allowanceDeductions, currencyExchangeRates, dstNetSalary);

            Assert.Equal(expectedTotalAmount, result.TotalAmount);
        }

        [Fact]
        public void InitializeAllowanceAdditions_ValidData_ReturnExpectedModel()
        {
            var monthlyPayrollService = GetMonthlyPayrollService();
            var payrollId = 1;
            var standardWorkingDays = 22;
            var actualWorkingDays = 24;
            decimal unitAmount = 10;
            decimal dutyMealAmount = 10 * standardWorkingDays;
            decimal totalAmount = unitAmount * (actualWorkingDays - standardWorkingDays);

            var basicAlloance = new MonthlyPayrollBasicAllowanceModel
            {
                PayrollId = payrollId,
                EmployeeId = 1,
                AutoCalculateOn = PayrollAutoCalculateOn.Cash,
                CurrencyId = 1,
                CashCurrencyId = 1,
                DutyMealAmount = dutyMealAmount.ToString(),
                DutyMealCurrencyId = 1
            };

            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 }
            };

            var result = monthlyPayrollService.InitializeAllowanceAdditions(standardWorkingDays, actualWorkingDays, basicAlloance, currencyExchangeRates);

            Assert.NotEmpty(result);
            var dutyMeal = result.FirstOrDefault(x => x.Type == (int)AllowanceType.DutyMeal);
            Assert.NotNull(dutyMeal);
            Assert.Equal(actualWorkingDays - standardWorkingDays, dutyMeal.NoOfDays);
            Assert.Equal(unitAmount.ToString(), dutyMeal.UnitAmount);
            Assert.Equal(totalAmount.ToString(), dutyMeal.TotalAmount);
        }

        [Fact]
        public void InitializeAllowanceDeductions_ValidData_ReturnExpectedModel()
        {
            var monthlyPayrollService = GetMonthlyPayrollService();
            var payrollId = 1;
            var standardWorkingDays = 22;
            var actualWorkingDays = 20;
            decimal unitAmount = 10;
            decimal dutyMealAmount = 10 * standardWorkingDays;
            decimal totalAmount = unitAmount * (standardWorkingDays - actualWorkingDays);

            var basicAllowance = new MonthlyPayrollBasicAllowanceModel
            {
                PayrollId = payrollId,
                EmployeeId = 1,
                AutoCalculateOn = PayrollAutoCalculateOn.Cash,
                CurrencyId = 1,
                CashCurrencyId = 1,
                DutyMealAmount = dutyMealAmount.ToString(),
                DutyMealCurrencyId = 1
            };

            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 }
            };

            var result = monthlyPayrollService.InitializeAllowanceDeductions(standardWorkingDays, actualWorkingDays, basicAllowance, currencyExchangeRates);

            Assert.NotEmpty(result);
            var dutyMeal = result.FirstOrDefault(x => x.Type == (int)AllowanceType.DutyMeal);
            Assert.NotNull(dutyMeal);
            Assert.Equal(standardWorkingDays - actualWorkingDays, dutyMeal.NoOfDays);
            Assert.Equal(unitAmount.ToString(), dutyMeal.UnitAmount);
            Assert.Equal(totalAmount.ToString(), dutyMeal.TotalAmount);
        }

        #region CalculateBackPayAndAfterAdjustment

        public static IEnumerable<object[]> AnnualIncrement_EffectiveDateIsNull_TestCase()
        {
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), null, "10000.00", string.Empty };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), null, "10000", string.Empty };
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), null, "5806.45", "Deducted 4193.55 (USD). Employee started working from Mar 14, 2018. Basic Salary changed on  for Annual Increment." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), null, "6000", "Deducted 4000 (VND). Employee started working from Mar 14, 2018. Basic Salary changed on  for Annual Increment." };
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "9677.42", "Deducted 322.58 (USD). Employee official leaves from Mar 30, 2018. Basic Salary changed on  for Annual Increment." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "10000", string.Empty };
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 30), "5483.87", "Deducted 4516.13 (USD). Employee worked from Mar 14, 2018 to Mar 30, 2018. Basic Salary changed on  for Annual Increment." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 30), "5000", "Deducted 5000 (VND). Employee worked from Mar 14, 2018 to Mar 30, 2018. Basic Salary changed on  for Annual Increment." };
        }

        [Theory, MemberData(nameof(AnnualIncrement_EffectiveDateIsNull_TestCase))]
        public void CalculateBackPay_AnnualIncrement_EffectiveDateIsNull_ReturnAsExpectedBackPayAndAfterAdjustment(
            int currencyId,
            string currencyCode,
            DateTime workingMonth,
            DateTime groupStartDate,
            DateTime? groupEndDate,
            string afterAdjustmentAmount,
            string afterAdjustmentReason)
        {
            // Arrange
            var currencyExchangeRates = new List<CurrencyExchangeRateModel>();
            var basicSetting = new MonthlyBasicSalaryModel
            {
                Salary = "10000",
                CurrencyId = currencyId,
                CurrencyCode = currencyCode,
                SalaryChangeReason = BasicSalaryChangeReason.AnnualIncrement
            };
            var backPay = new MonthlySalaryAdditionModel();
            var monthlyPayrollService = GetMonthlyPayrollService();

            // Act
            monthlyPayrollService.CalculateBackPayAndAfterAdjustment(
                currencyExchangeRates, workingMonth, basicSetting, backPay, groupStartDate, groupEndDate);

            // Assert
            Assert.Null(backPay.UnitAmount);
            Assert.Null(backPay.NetSalary);
            Assert.Null(backPay.Description);
            Assert.Equal(afterAdjustmentAmount, basicSetting.AfterAdjustmentAmount);
            Assert.Equal(afterAdjustmentReason, basicSetting.AfterAdjustmentReason);
        }

        [Theory]
        [InlineData(1, "2000.00", "14000.00")]
        [InlineData(11, "2000", "14000")]
        public void CalculateBackPay_AnnualIncrement_EffectiveMonthGreaterThanWorkingMonth_ReturnAsExpectedBackPayAndAfterAdjustment(
            int currencyId,
            string backPayAmount,
            string afterAdjustmentAmount)
        {
            // Arrange
            var workingMonth = new DateTime(2018, 3, 1);
            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = currencyId, ExchangeRate = 1 }
            };
            var basicSetting = new MonthlyBasicSalaryModel
            {
                OldSalary = "10000",
                OldCurrencyId = 1,
                Salary = "12000",
                CurrencyId = currencyId,
                SalaryChangeReason = BasicSalaryChangeReason.AnnualIncrement,
                EffectiveDate = new DateTime(2018, 2, 1)
            };
            var backPay = new MonthlySalaryAdditionModel();
            var monthlyPayrollService = GetMonthlyPayrollService();

            // Act
            monthlyPayrollService.CalculateBackPayAndAfterAdjustment(
                currencyExchangeRates, workingMonth, basicSetting, backPay, Arg.Any<DateTime>(), Arg.Any<DateTime>());

            // Assert
            Assert.Equal(backPayAmount, backPay.UnitAmount);
            Assert.Equal(backPayAmount, backPay.NetSalary);
            Assert.Equal("Annual Increment from Feb 01, 2018", backPay.Description);
            Assert.Equal(afterAdjustmentAmount, basicSetting.AfterAdjustmentAmount);
            Assert.Null(basicSetting.AfterAdjustmentReason);
        }

        public static IEnumerable<object[]> AnnualIncrement_EffectiveMonthEqualWorkingMonth_TestCase()
        {
            // Deducted
            yield return new object[] { "10000", "12000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), null, "11161.29", "Deducted 838.71 (USD). Employee started working from Mar 01, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "10000", "12000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), null, "11000", "Deducted 1000 (VND). Employee started working from Mar 01, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "10000", "12000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), new DateTime(2018, 3, 30), "10774.19", "Deducted 1225.81 (USD). Employee worked from Mar 01, 2018 to Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "10000", "12000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), new DateTime(2018, 3, 30), "11000", "Deducted 1000 (VND). Employee worked from Mar 01, 2018 to Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "10000", "12000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), null, "11161.29", "Deducted 838.71 (USD). Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "10000", "12000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), null, "11000", "Deducted 1000 (VND). Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "10000", "12000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "10774.19", "Deducted 1225.81 (USD). Employee official leaves from Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "10000", "12000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "11000", "Deducted 1000 (VND). Employee official leaves from Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            // Added
            yield return new object[] { "12000", "10000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), null, "10838.71", "Added 838.71 (USD). Employee started working from Mar 01, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "12000", "10000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), null, "11000", "Added 1000 (VND). Employee started working from Mar 01, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "12000", "10000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), new DateTime(2018, 3, 30), "10516.13", "Added 516.13 (USD). Employee worked from Mar 01, 2018 to Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "12000", "10000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), new DateTime(2018, 3, 30), "11000", "Added 1000 (VND). Employee worked from Mar 01, 2018 to Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "12000", "10000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), null, "10838.71", "Added 838.71 (USD). Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "12000", "10000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), null, "11000", "Added 1000 (VND). Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "12000", "10000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "10516.13", "Added 516.13 (USD). Employee official leaves from Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "12000", "10000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "11000", "Added 1000 (VND). Employee official leaves from Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
        }

        [Theory, MemberData(nameof(AnnualIncrement_EffectiveMonthEqualWorkingMonth_TestCase))]
        public void CalculateBackPay_AnnualIncrement_EffectiveMonthEqualWorkingMonth_ReturnAsExpectedBackPayAndAfterAdjustment(
            string oldSalary,
            string salary,
            int currencyId,
            string currencyCode,
            DateTime workingMonth,
            DateTime effectiveDate,
            DateTime groupStartDate,
            DateTime? groupEndDate,
            string afterAdjustmentAmount,
            string afterAdjustmentReason)
        {
            // Arrange
            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = currencyId, ExchangeRate = 1 }
            };
            var basicSetting = new MonthlyBasicSalaryModel
            {
                OldSalary = oldSalary,
                OldCurrencyId = 1,
                Salary = salary,
                CurrencyId = currencyId,
                CurrencyCode = currencyCode,
                SalaryChangeReason = BasicSalaryChangeReason.AnnualIncrement,
                EffectiveDate = effectiveDate
            };
            var backPay = new MonthlySalaryAdditionModel();
            var monthlyPayrollService = GetMonthlyPayrollService();

            // Act
            monthlyPayrollService.CalculateBackPayAndAfterAdjustment(
                currencyExchangeRates, workingMonth, basicSetting, backPay, groupStartDate, groupEndDate);

            // Assert
            Assert.Null(backPay.UnitAmount);
            Assert.Null(backPay.NetSalary);
            Assert.Null(backPay.Description);
            Assert.Equal(afterAdjustmentAmount, basicSetting.AfterAdjustmentAmount);
            Assert.Equal(afterAdjustmentReason, basicSetting.AfterAdjustmentReason);
        }

        public static IEnumerable<object[]> OthersBasicSalaryChangeReason_EffectiveDateIsNull_TestCase()
        {
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), null, "10000.00", string.Empty };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), null, "10000", string.Empty };
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), null, "5806.45", "Deducted 4193.55 (USD). Employee started working from Mar 14, 2018. Basic Salary changed on  for Promotion." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), null, "6000", "Deducted 4000 (VND). Employee started working from Mar 14, 2018. Basic Salary changed on  for Promotion." };
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "9677.42", "Deducted 322.58 (USD). Employee official leaves from Mar 30, 2018. Basic Salary changed on  for Promotion." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "10000", string.Empty };
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 30), "5483.87", "Deducted 4516.13 (USD). Employee worked from Mar 14, 2018 to Mar 30, 2018. Basic Salary changed on  for Promotion." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 30), "5000", "Deducted 5000 (VND). Employee worked from Mar 14, 2018 to Mar 30, 2018. Basic Salary changed on  for Promotion." };
        }

        [Theory, MemberData(nameof(OthersBasicSalaryChangeReason_EffectiveDateIsNull_TestCase))]
        public void CalculateBackPay_OthersBasicSalaryChangeReason_EffectiveDateIsNull_ReturnAsExpectedBackPayAndAfterAdjustment(
            int currencyId,
            string currencyCode,
            DateTime workingMonth,
            DateTime groupStartDate,
            DateTime? groupEndDate,
            string afterAdjustmentAmount,
            string afterAdjustmentReason)
        {
            // Arrange
            var currencyExchangeRates = new List<CurrencyExchangeRateModel>();
            var basicSetting = new MonthlyBasicSalaryModel
            {
                Salary = "10000",
                CurrencyId = currencyId,
                CurrencyCode = currencyCode,
                SalaryChangeReason = BasicSalaryChangeReason.Promotion
            };
            var backPay = new MonthlySalaryAdditionModel();
            var monthlyPayrollService = GetMonthlyPayrollService();

            // Act
            monthlyPayrollService.CalculateBackPayAndAfterAdjustment(
                currencyExchangeRates, workingMonth, basicSetting, backPay, groupStartDate, groupEndDate);

            // Assert
            Assert.Null(backPay.UnitAmount);
            Assert.Null(backPay.NetSalary);
            Assert.Null(backPay.Description);
            Assert.Equal(afterAdjustmentAmount, basicSetting.AfterAdjustmentAmount);
            Assert.Equal(afterAdjustmentReason, basicSetting.AfterAdjustmentReason);
        }

        public static IEnumerable<object[]> OthersBasicSalaryChangeReason_EffectiveDateIsNotNull_TestCase()
        {
            // Deducted
            yield return new object[] { "10000", "12000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), null, "11161.29", "Deducted 838.71 (USD). Employee started working from Mar 01, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "10000", "12000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), null, "11000", "Deducted 1000 (VND). Employee started working from Mar 01, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "10000", "12000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), new DateTime(2018, 3, 30), "10774.19", "Deducted 1225.81 (USD). Employee worked from Mar 01, 2018 to Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "10000", "12000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), new DateTime(2018, 3, 30), "11000", "Deducted 1000 (VND). Employee worked from Mar 01, 2018 to Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "10000", "12000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), null, "11161.29", "Deducted 838.71 (USD). Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "10000", "12000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), null, "11000", "Deducted 1000 (VND). Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "10000", "12000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "10774.19", "Deducted 1225.81 (USD). Employee official leaves from Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "10000", "12000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "11000", "Deducted 1000 (VND). Employee official leaves from Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            // Added
            yield return new object[] { "12000", "10000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), null, "10838.71", "Added 838.71 (USD). Employee started working from Mar 01, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "12000", "10000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), null, "11000", "Added 1000 (VND). Employee started working from Mar 01, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "12000", "10000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), new DateTime(2018, 3, 30), "10516.13", "Added 516.13 (USD). Employee worked from Mar 01, 2018 to Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "12000", "10000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), new DateTime(2018, 3, 30), "11000", "Added 1000 (VND). Employee worked from Mar 01, 2018 to Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "12000", "10000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), null, "10838.71", "Added 838.71 (USD). Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "12000", "10000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), null, "11000", "Added 1000 (VND). Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "12000", "10000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "10516.13", "Added 516.13 (USD). Employee official leaves from Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "12000", "10000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "11000", "Added 1000 (VND). Employee official leaves from Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
        }

        [Theory, MemberData(nameof(OthersBasicSalaryChangeReason_EffectiveDateIsNotNull_TestCase))]
        public void CalculateBackPay_OthersBasicSalaryChangeReason_EffectiveDateIsNotNull_ReturnAsExpectedBackPayAndAfterAdjustment(
            string oldSalary,
            string salary,
            int currencyId,
            string currencyCode,
            DateTime workingMonth,
            DateTime effectiveDate,
            DateTime groupStartDate,
            DateTime? groupEndDate,
            string afterAdjustmentAmount,
            string afterAdjustmentReason)
        {
            // Arrange
            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = currencyId, ExchangeRate = 1 }
            };
            var basicSetting = new MonthlyBasicSalaryModel
            {
                OldSalary = oldSalary,
                OldCurrencyId = 1,
                Salary = salary,
                CurrencyId = currencyId,
                CurrencyCode = currencyCode,
                SalaryChangeReason = BasicSalaryChangeReason.Promotion,
                EffectiveDate = effectiveDate
            };
            var backPay = new MonthlySalaryAdditionModel();
            var monthlyPayrollService = GetMonthlyPayrollService();

            // Act
            monthlyPayrollService.CalculateBackPayAndAfterAdjustment(
                currencyExchangeRates, workingMonth, basicSetting, backPay, groupStartDate, groupEndDate);

            // Assert
            Assert.Null(backPay.UnitAmount);
            Assert.Null(backPay.NetSalary);
            Assert.Null(backPay.Description);
            Assert.Equal(afterAdjustmentAmount, basicSetting.AfterAdjustmentAmount);
            Assert.Equal(afterAdjustmentReason, basicSetting.AfterAdjustmentReason);
        }

        public static IEnumerable<object[]> AnnualIncrement_GroupDateCommencement_TestCase()
        {
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), null, "10000.00", string.Empty };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), null, "10000", string.Empty };
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), null, "5806.45", "Deducted 4193.55 (USD). Employee started working from Mar 14, 2018." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), null, "6000", "Deducted 4000 (VND). Employee started working from Mar 14, 2018." };
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "9677.42", "Deducted 322.58 (USD). Employee official leaves from Mar 30, 2018." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "10000", string.Empty };
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 30), "5483.87", "Deducted 4516.13 (USD). Employee worked from Mar 14, 2018 to Mar 30, 2018." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 30), "5000", "Deducted 5000 (VND). Employee worked from Mar 14, 2018 to Mar 30, 2018." };
        }

        [Theory, MemberData(nameof(AnnualIncrement_GroupDateCommencement_TestCase))]
        public void CalculateBackPay_GroupDateCommencement_ReturnAsExpectedBackPayAndAfterAdjustment(
            int currencyId,
            string currencyCode,
            DateTime workingMonth,
            DateTime groupStartDate,
            DateTime? groupEndDate,
            string afterAdjustmentAmount,
            string afterAdjustmentReason)
        {
            // Arrange
            var currencyExchangeRates = new List<CurrencyExchangeRateModel>();
            var basicSetting = new MonthlyBasicSalaryModel
            {
                Salary = "10000",
                CurrencyId = currencyId,
                CurrencyCode = currencyCode
            };
            var backPay = new MonthlySalaryAdditionModel();
            var monthlyPayrollService = GetMonthlyPayrollService();

            // Act
            monthlyPayrollService.CalculateBackPayAndAfterAdjustment(
                currencyExchangeRates, workingMonth, basicSetting, backPay, groupStartDate, groupEndDate);

            // Assert
            Assert.Null(backPay.UnitAmount);
            Assert.Null(backPay.NetSalary);
            Assert.Null(backPay.Description);
            Assert.Equal(afterAdjustmentAmount, basicSetting.AfterAdjustmentAmount);
            Assert.Equal(afterAdjustmentReason, basicSetting.AfterAdjustmentReason);
        }

        #endregion CalculateBackPayAndAfterAdjustment

        #region CalculateMonthlySalary

        [Fact]
        public void CalculateMonthlySalary_AutoCalculateOnEbanking_ReturnAsExpected()
        {
            // Arrange
            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 }
            };
            var basicSalary = new MonthlyBasicSalaryModel
            {
                AutoCalculateOn = PayrollAutoCalculateOn.Ebanking,
                CurrencyId = 1,
                Salary = "10000",
                AfterAdjustmentAmount = "10000",
                EbankingCurrencyId = 1,
                DirectBanking1CurrencyId = 1,
                DirectBanking1Amount = "1000",
                CashCurrencyId = 1,
                CashAmount = "5000"
            };
            var backPay = new MonthlySalaryAdditionModel { NetSalary = "1000" };
            var monthlySalary = new MonthlySalaryModel();
            var monthlyPayrollService = GetMonthlyPayrollService();

            // Act
            monthlyPayrollService.CalculateMonthlySalary(currencyExchangeRates, basicSalary, backPay, monthlySalary);

            // Assert
            Assert.Equal(basicSalary.CurrencyId, monthlySalary.CurrencyId);
            Assert.Equal(basicSalary.AfterAdjustmentAmount, monthlySalary.Salary);
            Assert.Equal(basicSalary.EbankingCurrencyId, monthlySalary.EbankingCurrencyId);
            Assert.Equal("4000", basicSalary.EbankingAmount);
            Assert.Equal(basicSalary.EbankingAmount, monthlySalary.EbankingAmount);
            Assert.Equal(basicSalary.EbankingAccountId, monthlySalary.EbankingAccountId);
            Assert.Equal(basicSalary.DirectBanking1CurrencyId, monthlySalary.DirectBanking1CurrencyId);
            //Assert.Equal(basicSalary.DirectBanking1Amount, monthlySalary.DirectBanking1Amount);
            Assert.Equal(basicSalary.DirectBanking1BankTypeId, monthlySalary.DirectBanking1BankTypeId);
            Assert.Equal(basicSalary.DirectBanking1AccountId, monthlySalary.DirectBanking1AccountId);
            Assert.Equal(basicSalary.DirectBanking2CurrencyId, monthlySalary.DirectBanking2CurrencyId);
            Assert.Equal(basicSalary.DirectBanking2Amount, monthlySalary.DirectBanking2Amount);
            Assert.Equal(basicSalary.DirectBanking2BankTypeId, monthlySalary.DirectBanking2BankTypeId);
            Assert.Equal(basicSalary.DirectBanking2AccountId, monthlySalary.DirectBanking2AccountId);
            Assert.Equal(basicSalary.CashCurrencyId, monthlySalary.CashCurrencyId);
           // Assert.Equal(basicSalary.CashAmount, monthlySalary.CashAmount);
            Assert.Equal(basicSalary.CashAccountId, monthlySalary.CashAccountId);
            Assert.Equal(MonthlyPayrollStatus.Default, monthlySalary.Status);
            Assert.Equal(backPay.NetSalary, monthlySalary.AdditionAmount);
        }

        [Fact]
        public void CalculateMonthlySalary_AutoCalculateOnDirectBanking1_ReturnAsExpected()
        {
            // Arrange
            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 }
            };
            var basicSalary = new MonthlyBasicSalaryModel
            {
                AutoCalculateOn = PayrollAutoCalculateOn.DirectBanking1,
                CurrencyId = 1,
                Salary = "10000",
                AfterAdjustmentAmount = "10000",
                EbankingCurrencyId = 1,
                EbankingAmount = "1000",
                DirectBanking1CurrencyId = 1,
                CashCurrencyId = 1,
                CashAmount = "5000"
            };
            var backPay = new MonthlySalaryAdditionModel { NetSalary = "1000" };
            var monthlySalary = new MonthlySalaryModel();
            var monthlyPayrollService = GetMonthlyPayrollService();

            // Act
            monthlyPayrollService.CalculateMonthlySalary(currencyExchangeRates, basicSalary, backPay, monthlySalary);

            // Assert
            Assert.Equal(basicSalary.CurrencyId, monthlySalary.CurrencyId);
            Assert.Equal(basicSalary.AfterAdjustmentAmount, monthlySalary.Salary);
            Assert.Equal(basicSalary.EbankingCurrencyId, monthlySalary.EbankingCurrencyId);
            //Assert.Equal(basicSalary.EbankingAmount, monthlySalary.EbankingAmount);
            Assert.Equal(basicSalary.DirectBanking1CurrencyId, monthlySalary.DirectBanking1CurrencyId);
            Assert.Equal("4000", basicSalary.DirectBanking1Amount);
            Assert.Equal(basicSalary.DirectBanking1Amount, monthlySalary.DirectBanking1Amount);
            Assert.Equal(basicSalary.DirectBanking2CurrencyId, monthlySalary.DirectBanking2CurrencyId);
            Assert.Equal(basicSalary.DirectBanking2Amount, monthlySalary.DirectBanking2Amount);
            Assert.Equal(basicSalary.CashCurrencyId, monthlySalary.CashCurrencyId);
            //Assert.Equal(basicSalary.CashAmount, monthlySalary.CashAmount);
            Assert.Equal(MonthlyPayrollStatus.Default, monthlySalary.Status);
            Assert.Equal(backPay.NetSalary, monthlySalary.AdditionAmount);
        }

        [Fact]
        public void CalculateMonthlySalary_AutoCalculateOnDirectBanking2_ReturnAsExpected()
        {
            // Arrange
            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 }
            };
            var basicSalary = new MonthlyBasicSalaryModel
            {
                AutoCalculateOn = PayrollAutoCalculateOn.DirectBanking2,
                CurrencyId = 1,
                Salary = "10000",
                AfterAdjustmentAmount = "10000",
                EbankingCurrencyId = 1,
                EbankingAmount = "1000",
                DirectBanking2CurrencyId = 1,
                CashCurrencyId = 1,
                CashAmount = "5000"
            };
            var backPay = new MonthlySalaryAdditionModel { NetSalary = "1000" };
            var monthlySalary = new MonthlySalaryModel();
            var monthlyPayrollService = GetMonthlyPayrollService();

            // Act
            monthlyPayrollService.CalculateMonthlySalary(currencyExchangeRates, basicSalary, backPay, monthlySalary);

            // Assert
            Assert.Equal(basicSalary.CurrencyId, monthlySalary.CurrencyId);
            Assert.Equal(basicSalary.AfterAdjustmentAmount, monthlySalary.Salary);
            Assert.Equal(basicSalary.EbankingCurrencyId, monthlySalary.EbankingCurrencyId);
            //Assert.Equal(basicSalary.EbankingAmount, monthlySalary.EbankingAmount);
            Assert.Equal(basicSalary.DirectBanking1CurrencyId, monthlySalary.DirectBanking1CurrencyId);
            Assert.Equal(basicSalary.DirectBanking1Amount, monthlySalary.DirectBanking1Amount);
            Assert.Equal(basicSalary.DirectBanking2CurrencyId, monthlySalary.DirectBanking2CurrencyId);
            Assert.Equal("4000", basicSalary.DirectBanking2Amount);
            Assert.Equal(basicSalary.DirectBanking2Amount, monthlySalary.DirectBanking2Amount);
            Assert.Equal(basicSalary.CashCurrencyId, monthlySalary.CashCurrencyId);
            //Assert.Equal(basicSalary.CashAmount, monthlySalary.CashAmount);
            Assert.Equal(MonthlyPayrollStatus.Default, monthlySalary.Status);
            Assert.Equal(backPay.NetSalary, monthlySalary.AdditionAmount);
        }

        [Fact]
        public void CalculateMonthlySalary_AutoCalculateOnCash_ReturnAsExpected()
        {
            // Arrange
            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 }
            };
            var basicSalary = new MonthlyBasicSalaryModel
            {
                AutoCalculateOn = PayrollAutoCalculateOn.Cash,
                CurrencyId = 1,
                Salary = "10000",
                AfterAdjustmentAmount = "10000",
                EbankingCurrencyId = 1,
                EbankingAmount = "1000",
                DirectBanking1CurrencyId = 1,
                DirectBanking1Amount = "5000",
                CashCurrencyId = 1
            };
            var backPay = new MonthlySalaryAdditionModel { NetSalary = "1000" };
            var monthlySalary = new MonthlySalaryModel();
            var monthlyPayrollService = GetMonthlyPayrollService();

            // Act
            monthlyPayrollService.CalculateMonthlySalary(currencyExchangeRates, basicSalary, backPay, monthlySalary);

            // Assert
            Assert.Equal(basicSalary.CurrencyId, monthlySalary.CurrencyId);
            Assert.Equal(basicSalary.AfterAdjustmentAmount, monthlySalary.Salary);
            Assert.Equal(basicSalary.EbankingCurrencyId, monthlySalary.EbankingCurrencyId);
            //Assert.Equal(basicSalary.EbankingAmount, monthlySalary.EbankingAmount);
            Assert.Equal(basicSalary.DirectBanking1CurrencyId, monthlySalary.DirectBanking1CurrencyId);
            //Assert.Equal(basicSalary.DirectBanking1Amount, monthlySalary.DirectBanking1Amount);
            Assert.Equal(basicSalary.DirectBanking2CurrencyId, monthlySalary.DirectBanking2CurrencyId);
            Assert.Equal(basicSalary.DirectBanking2Amount, monthlySalary.DirectBanking2Amount);
            Assert.Equal(basicSalary.CashCurrencyId, monthlySalary.CashCurrencyId);
            Assert.Equal("4000", basicSalary.CashAmount);
            Assert.Equal(basicSalary.CashAmount, monthlySalary.CashAmount);
            Assert.Equal(MonthlyPayrollStatus.Default, monthlySalary.Status);
            Assert.Equal(backPay.NetSalary, monthlySalary.AdditionAmount);
        }

        #endregion CalculateMonthlySalary

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