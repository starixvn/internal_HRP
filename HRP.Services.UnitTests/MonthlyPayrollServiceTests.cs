namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.MonthlyPayroll;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.MonthlyPayroll.EmployeeMonthlyPayroll;
    using HRP.Models.ServiceModels;
    using HRP.Repositories;
    using NSubstitute;
    using Xunit;

    public class MonthlyPayrollServiceTests
    {
        private ISessionService sessionService;
        private IBaseService baseService;
        private ICryptoService cryptoService;
        private IMonthlyPayrollRepository monthlyPayrollRepository;

        [Fact]
        public async Task GetFilterInfoAsync_WhenCalled_ReturnAsExpectedFilterInfo()
        {
            // Arrange
            var filterInfo = new MonthlyPayrollFilterInfoModel
            {
                PayrollId = 1,
                EmployeeId = 1,
                FullName = "a",
                Nickname = "b",
                IdNo = "c",
                CostCenter = "d",
                StartDate = new DateTime(2010, 1, 1),
                DueDate = new DateTime(2010, 1, 1),
                CompanyId = 1,
                CompanyCode = "DAR",
                WorkingDays = 22,
                IsHost = true,
                TotalEmployees = 100
            };
            var service = GetMonthlyPayrollService();
            baseService.GetDataEntityAsync<MonthlyPayrollFilterInfoModel>(Arg.Any<GetFilterInfoCriteria>()).Returns(filterInfo);

            // Act
            var actual = await service.GetFilterInfoAsync(1, DateTime.Today);

            // Assert
            Assert.Equal(filterInfo, actual);
        }

        [Fact]
        public async Task GetHierarchyCompanyAsync_WhenCalled_ReturnAsExpectedHierarchy()
        {
            // Arrange
            var divisions = new List<MultiSelectViewModel>
            {
                new MultiSelectViewModel { Value = "1", Text = "DAR", Checked = true }
            };
            var departments = new List<MultiSelectViewModel>
            {
                new MultiSelectViewModel { Value = "2", Text = "DAR", Checked = true, ParentId = "1" }
            };
            var costCenters = new List<MultiSelectViewModel>
            {
                new MultiSelectViewModel { Value = "3", Text = "DAR", Checked = true, ParentId = "2" }
            };
            var tuple = new Tuple<
                IEnumerable<MultiSelectViewModel>,
                IEnumerable<MultiSelectViewModel>,
                IEnumerable<MultiSelectViewModel>>(divisions, departments, costCenters);
            var service = GetMonthlyPayrollService();
            baseService.FetchMultipleAsync<MultiSelectViewModel, MultiSelectViewModel, MultiSelectViewModel>(Arg.Any<GetHierarchyCompanyCriteria>()).Returns(tuple);

            // Act
            var actual = await service.GetHierarchyCompanyAsync(1, "1,2");

            // Assert
            Assert.Single(actual);
            Assert.Equal(divisions.First().Value, actual.First().Value);
            Assert.Equal(divisions.First().Text, actual.First().Text);
            Assert.Equal(divisions.First().Checked, actual.First().Checked);

            Assert.Single(actual.First().Items);
            Assert.Equal(departments.First().Value, actual.First().Items.First().Value);
            Assert.Equal(departments.First().Text, actual.First().Items.First().Text);
            Assert.Equal(departments.First().Checked, actual.First().Items.First().Checked);

            Assert.Single(actual.First().Items.First().Items);
            Assert.Equal(costCenters.First().Value, actual.First().Items.First().Items.First().Value);
            Assert.Equal(costCenters.First().Text, actual.First().Items.First().Items.First().Text);
            Assert.Equal(costCenters.First().Checked, actual.First().Items.First().Items.First().Checked);
        }

        #region GetMonthlyPayrollAsync

        [Fact]
        public async Task GetMonthlyPayrollAsync_WhenCalled_ReturnAsExpectedViewModel()
        {
            // Arrange
            var plainText = "1234";
            var viewModel = new EmployeeMonthlyPayrollViewModel
            {
                CompanyId = 1
            };
            var tuple = new Tuple<IEnumerable<EmployeeMonthlyPayrollModel>,
                IEnumerable<EmployeeMonthlyPayrollBasicSalaryModel>,
                IEnumerable<EmployeeMonthlyPayrollBasicSalaryModel>>(
                    new List<EmployeeMonthlyPayrollModel> { new EmployeeMonthlyPayrollModel { PayrollId = 1 } },
                    new List<EmployeeMonthlyPayrollBasicSalaryModel> { new EmployeeMonthlyPayrollBasicSalaryModel { PayrollId = 1 } },
                    new List<EmployeeMonthlyPayrollBasicSalaryModel> { new EmployeeMonthlyPayrollBasicSalaryModel { PayrollId = 1 } });
            var monthlyPayrollService = GetMonthlyPayrollService();
            cryptoService.Decrypt(Arg.Any<string>()).Returns(plainText);
            baseService.FetchMultipleAsync<EmployeeMonthlyPayrollModel,
                    EmployeeMonthlyPayrollBasicSalaryModel,
                    EmployeeMonthlyPayrollBasicSalaryModel>(Arg.Any<GetMonthlyPayrollCriteria>()).Returns(tuple);

            // Act
            var actual = await monthlyPayrollService.GetMonthlyPayrollAsync(viewModel);

            // Assert
            Assert.Single(actual);
            Assert.Equal(tuple.Item1.First().PayrollId, actual.First().PayrollId);
            Assert.Equal(tuple.Item1.First().EmployeeId, actual.First().EmployeeId);
            Assert.Equal(tuple.Item1.First().IDNo, actual.First().IDNo);
            Assert.Equal(tuple.Item1.First().Nickname, actual.First().Nickname);
            Assert.Equal(tuple.Item1.First().FullName, actual.First().FullName);
            Assert.Equal(tuple.Item1.First().CostCenter, actual.First().CostCenter);
            Assert.Equal(tuple.Item1.First().WorkingDays, actual.First().WorkingDays);
            Assert.Equal(tuple.Item1.First().Policy, actual.First().Policy);

            Assert.Equal(tuple.Item1.First().BasicSalary.EbankingCurrencyCode, actual.First().BasicSalary.EbankingCurrencyCode);
            Assert.Equal("1,234.00", actual.First().BasicSalary.EbankingAmount);
            Assert.Equal(tuple.Item1.First().BasicSalary.DirectBanking1CurrencyCode, actual.First().BasicSalary.DirectBanking1CurrencyCode);
            Assert.Equal("1,234.00", actual.First().BasicSalary.DirectBanking1Amount);
            Assert.Equal(tuple.Item1.First().BasicSalary.DirectBanking2CurrencyCode, actual.First().BasicSalary.DirectBanking2CurrencyCode);
            Assert.Equal("1,234.00", actual.First().BasicSalary.DirectBanking2Amount);
            Assert.Equal(tuple.Item1.First().BasicSalary.CashCurrencyCode, actual.First().BasicSalary.CashCurrencyCode);
            Assert.Equal("1,234.00", actual.First().BasicSalary.CashAmount);

            Assert.Equal(tuple.Item1.First().BasicSalary.DirectBanking1CurrencyCode, actual.First().BasicSalary.DirectBanking1CurrencyCode);
            Assert.Equal("1,234.00", actual.First().BasicSalary.DirectBanking1Amount);
            Assert.Equal(tuple.Item1.First().BasicSalary.DirectBanking2CurrencyCode, actual.First().BasicSalary.DirectBanking2CurrencyCode);
            Assert.Equal("1,234.00", actual.First().BasicSalary.DirectBanking2Amount);
            Assert.Equal(tuple.Item1.First().BasicSalary.CashCurrencyCode, actual.First().BasicSalary.CashCurrencyCode);
            Assert.Equal("1,234.00", actual.First().BasicSalary.CashAmount);
        }

        #endregion GetMonthlyPayrollAsync

        #region GetMonthlySalaryAsync

        [Fact]
        public async Task GetMonthlySalaryAsync_MonthlyPayrollIsNotExisted_ReturnAsExpected()
        {
            // Arrange
            var monthlyPayroll = new Tuple<MonthlyBasicSalaryModel,
                MonthlySalaryModel,
                IEnumerable<MonthlySalaryAdditionModel>,
                IEnumerable<MonthlySalaryDeductionModel>>(
                    null,
                    new MonthlySalaryModel(),
                    new List<MonthlySalaryAdditionModel>(),
                    new List<MonthlySalaryDeductionModel>());
            var monthlyPayrollService = GetMonthlyPayrollService();
            monthlyPayrollRepository.GetMonthlySalaryAsync(Arg.Any<GetMonthlySalaryToEditCriteria>()).Returns(monthlyPayroll);

            // Act
            var actual = await monthlyPayrollService.GetMonthlySalaryAsync(1);

            // Assert
            Assert.Equal(monthlyPayroll, actual);
        }

        [Fact]
        public async Task GetMonthlySalaryAsync_MonthlyPayrollIsExisted_ReturnAsExpected()
        {
            // Arrange
            var plainText = "1234";
            var tuple = new Tuple<MonthlyBasicSalaryModel,
                MonthlySalaryModel,
                IEnumerable<MonthlySalaryAdditionModel>,
                IEnumerable<MonthlySalaryDeductionModel>>(new MonthlyBasicSalaryModel { WorkingDays = 10 },
                    new MonthlySalaryModel { CurrencyCode = "USD" },
                    new List<MonthlySalaryAdditionModel> { new MonthlySalaryAdditionModel() },
                    new List<MonthlySalaryDeductionModel> { new MonthlySalaryDeductionModel() });
            var monthlyPayrollService = GetMonthlyPayrollService();
            monthlyPayrollRepository.GetMonthlySalaryAsync(Arg.Any<GetMonthlySalaryToEditCriteria>()).Returns(tuple);
            cryptoService.Decrypt(Arg.Any<string>()).Returns(plainText);

            // Act
            var actual = await monthlyPayrollService.GetMonthlySalaryAsync(1);

            // Assert
            Assert.Equal(tuple.Item1.CurrencyCode, actual.Item1.CurrencyCode);
            Assert.Equal(tuple.Item1.Salary, actual.Item1.Salary);
            Assert.Equal(tuple.Item1.WorkingDays, actual.Item1.WorkingDays);
            Assert.Equal(123.4M, actual.Item1.SalaryPerDay);
            Assert.Equal(tuple.Item1.EbankingCurrencyCode, actual.Item1.EbankingCurrencyCode);
            Assert.Equal(plainText, actual.Item1.EbankingAmount);
            Assert.Equal(tuple.Item1.DirectBanking1CurrencyCode, actual.Item1.DirectBanking1CurrencyCode);
            Assert.Equal(plainText, actual.Item1.DirectBanking1Amount);
            Assert.Equal(tuple.Item1.DirectBanking2CurrencyCode, actual.Item1.DirectBanking2CurrencyCode);
            Assert.Equal(plainText, actual.Item1.DirectBanking2Amount);
            Assert.Equal(tuple.Item1.CashCurrencyCode, actual.Item1.CashCurrencyCode);
            Assert.Equal(plainText, actual.Item1.CashAmount);
            Assert.Equal(tuple.Item1.AutoCalculateOn, actual.Item1.AutoCalculateOn);
            Assert.Equal(plainText, actual.Item1.AfterAdjustmentReason);
            Assert.Equal(plainText, actual.Item1.AfterAdjustmentAmount);

            Assert.Equal(tuple.Item2.CurrencyId, actual.Item2.CurrencyId);
            Assert.Equal(tuple.Item2.Salary, actual.Item2.Salary);
            Assert.Equal(tuple.Item2.EbankingCurrencyId, actual.Item2.EbankingCurrencyId);
            Assert.Equal(tuple.Item2.EbankingAccountId, actual.Item2.EbankingAccountId);
            Assert.Equal(plainText, actual.Item2.EbankingAmount);
            Assert.Equal(tuple.Item2.DirectBanking1CurrencyId, actual.Item2.DirectBanking1CurrencyId);
            Assert.Equal(tuple.Item2.DirectBanking1AccountId, actual.Item2.DirectBanking1AccountId);
            Assert.Equal(plainText, actual.Item2.DirectBanking1Amount);
            Assert.Equal(tuple.Item2.DirectBanking2CurrencyId, actual.Item2.DirectBanking2CurrencyId);
            Assert.Equal(tuple.Item2.DirectBanking2AccountId, actual.Item2.DirectBanking2AccountId);
            Assert.Equal(plainText, actual.Item2.DirectBanking2Amount);
            Assert.Equal(tuple.Item2.CashCurrencyId, actual.Item2.CashCurrencyId);
            Assert.Equal(tuple.Item2.CashAccountId, actual.Item2.CashAccountId);
            Assert.Equal(plainText, actual.Item2.CashAmount);

            Assert.Collection(
                actual.Item3,
                kvp =>
                {
                    Assert.Equal(plainText, kvp.NetSalary);
                    Assert.Equal(plainText, kvp.NetSalaryDisplay);
                    Assert.Equal(plainText, kvp.UnitAmount);
                });

            Assert.Collection(
                actual.Item4,
                kvp =>
                {
                    Assert.Equal(plainText, kvp.NetSalary);
                    Assert.Equal(plainText, kvp.NetSalaryDisplay);
                    Assert.Equal(plainText, kvp.UnitAmount);
                });
        }

        #endregion GetMonthlySalaryAsync

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
        public async Task EditMonthlySalaryAsync_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var monthlySalary = new MonthlySalaryModel();
            var additions = new List<MonthlySalaryAdditionModel> { new MonthlySalaryAdditionModel() };
            var deductions = new List<MonthlySalaryDeductionModel> { new MonthlySalaryDeductionModel() };
            var commandResult = new CommandResult();
            var criteria = new EditMonthlySalaryCriteria();
            var cipherText = "cipherText";
            var payrollId = 1;
            var companyId = 1;
            var month = DateTime.Now;
            var monthlyPayrollService = GetMonthlyPayrollService();
            baseService.ExecuteNonQueryAsync(Arg.Do<EditMonthlySalaryCriteria>(x => criteria = x)).Returns(commandResult);
            cryptoService.Encrypt(Arg.Any<string>()).Returns(cipherText);

            // Act
            var actual = await monthlyPayrollService.EditMonthlySalaryAsync(payrollId, companyId, month, monthlySalary, additions, deductions);

            // Assert
            Assert.Equal(commandResult, actual);
            Assert.Equal(payrollId, criteria.PayrollId);
            Assert.Equal(companyId, criteria.CompanyId);
            Assert.Equal(month, criteria.Month);
            Assert.Equal(cipherText, criteria.Salary);

            Assert.Equal(monthlySalary.EbankingCurrencyId, criteria.EbankingCurrencyId);
            Assert.Equal(cipherText, criteria.EbankingAmount);
            Assert.Equal(monthlySalary.EbankingAccountId, criteria.EbankingAccountId);

            Assert.Equal(monthlySalary.DirectBanking1CurrencyId, criteria.DirectBanking1CurrencyId);
            Assert.Equal(cipherText, criteria.DirectBanking1Amount);
            Assert.Equal(monthlySalary.DirectBanking1AccountId, criteria.DirectBanking1AccountId);

            Assert.Equal(monthlySalary.DirectBanking2CurrencyId, criteria.DirectBanking2CurrencyId);
            Assert.Equal(cipherText, criteria.DirectBanking2Amount);
            Assert.Equal(monthlySalary.DirectBanking2AccountId, criteria.DirectBanking2AccountId);

            Assert.Equal(monthlySalary.CashCurrencyId, criteria.CashCurrencyId);
            Assert.Equal(cipherText, criteria.CashAmount);
            Assert.Equal(monthlySalary.CashAccountId, criteria.CashAccountId);

            Assert.Equal(monthlySalary.AutoCalculateOn.ToString(), criteria.AutoCalculateOn);
            Assert.Equal(cipherText, criteria.AdditionAmount);
            Assert.Equal(cipherText, criteria.DeductionAmount);

            Assert.Equal(additions.ToXML().ToString(), criteria.AdditionXml);
            Assert.Equal(deductions.ToXML().ToString(), criteria.DeductionXml);
        }

        #endregion EditMonthlySalaryAsync

        [Fact]
        public async Task GetMonthlyAllowancesAsync_ValidPayrollId_ReturnViewModel()
        {
            // Arrange
            var allowance = new MonthlyAllowanceModel { CurrencyId = 1 };
            var basicAllowance = new MonthlyPayrollBasicAllowanceModel { CurrencyId = 1 };
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
            cryptoService.Decrypt(Arg.Any<string>()).Returns("1");
            monthlyPayrollRepository.GetMonthlyAllowancesAsync(Arg.Any<GetMonthlyAllowanceCriteria>())
                .Returns(new Tuple<MonthlyAllowanceModel, IEnumerable<MonthlyAllowanceAdditionModel>, IEnumerable<MonthlyAllowanceDeductionModel>, MonthlyPayrollBasicAllowanceModel>
                (allowance, additions, deductions, basicAllowance));

            // Act
            var actual = await monthlyPayrollService.GetMonthlyAllowancesAsync(1, DateTime.Today);

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.Allowance);
            Assert.NotNull(actual.Additions);
            Assert.NotNull(actual.Deductions);
            Assert.NotNull(actual.BasicAllowance);
        }

        [Fact]
        public async Task EditMonthlyAllowancesAsync_ValidModel_ReturnCommandResult()
        {
            var monthlyPayrollService = GetMonthlyPayrollService();
            cryptoService.Encrypt(Arg.Any<string>()).Returns(string.Empty);
            baseService.ExecuteNonQueryAsync(Arg.Any<EditMonthlyAllowancesCriteria>()).Returns(new CommandResult { Code = 0 });
            var viewModel = new MonthlyAllowancesModel
            {
                EmployeeId = 22,
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

        [Theory]
        [InlineData(PayrollAutoCalculateOn.DirectBanking1)]
        [InlineData(PayrollAutoCalculateOn.DirectBanking2)]
        [InlineData(PayrollAutoCalculateOn.Cash)]
        public void CalculateMonthlyBasicAllowance_StartDateInMonth_ReturnExpectedResult(PayrollAutoCalculateOn payrollAutoCalculateOn)
        {
            var monthlyPayrollService = GetMonthlyPayrollService();
            var payrollId = 1;
            var workingMonth = DateTime.Today;

            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 }
            };

            var basicAllowance = new MonthlyPayrollBasicAllowanceModel
            {
                PayrollId = payrollId,
                AutoCalculateOn = payrollAutoCalculateOn,
                CurrencyId = 1,
                CashCurrencyId = 1,
                DirectBanking1CurrencyId = 1,
                DirectBanking2CurrencyId = 1,
                SubsistenceAmount = "2000",
                SubsistenceCurrencyId = 1
            };
            cryptoService.Decrypt(basicAllowance.SubsistenceAmount).Returns(basicAllowance.SubsistenceAmount);
            cryptoService.Encrypt(basicAllowance.SubsistenceAmount).Returns(basicAllowance.SubsistenceAmount);
            cryptoService.Decrypt(null).Returns(string.Empty);
            cryptoService.Encrypt(null).Returns(string.Empty);
            baseService.GetDataAsync<CurrencyCodeModel>(Arg.Any<GetActiveCurrencyCodesCriteria>())
                .Returns(new List<CurrencyCodeModel> { new CurrencyCodeModel { CurrencyId = 1, CurrencyCode = "USD" } });

            monthlyPayrollService.CalculateMonthlyBasicAllowance(
                currencyExchangeRates,
                basicAllowance,
                workingMonth,
                new DateTime(workingMonth.Year, workingMonth.Month, 15),
                null);

            Assert.NotEqual(basicAllowance.TotalAmount, basicAllowance.AfterAdjustmentAmount);
            Assert.NotNull(basicAllowance.Reason);
        }

        [Fact]
        public void CalculateMonthlyBasicAllowance_EndDateInMonth_ReturnExpectedResult()
        {
            var monthlyPayrollService = GetMonthlyPayrollService();
            var payrollId = 1;
            var workingMonth = DateTime.Today;

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
            baseService.GetDataAsync<CurrencyCodeModel>(Arg.Any<GetActiveCurrencyCodesCriteria>())
                .Returns(new List<CurrencyCodeModel> { new CurrencyCodeModel { CurrencyId = 1, CurrencyCode = "USD" } });

            monthlyPayrollService.CalculateMonthlyBasicAllowance(
                currencyExchangeRates,
                basicAllowance,
                workingMonth,
                new DateTime(workingMonth.Year, workingMonth.Month, 1),
                new DateTime(workingMonth.Year, workingMonth.Month, 20));

            Assert.NotEqual(basicAllowance.TotalAmount, basicAllowance.AfterAdjustmentAmount);
            Assert.NotNull(basicAllowance.Reason);
        }

        [Fact]
        public void CalculateMonthlyBasicAllowance_StartDateAndEndDateInMonth_ReturnExpectedResult()
        {
            var monthlyPayrollService = GetMonthlyPayrollService();
            var payrollId = 1;
            var workingMonth = DateTime.Today;

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

            baseService.GetDataAsync<CurrencyCodeModel>(Arg.Any<GetActiveCurrencyCodesCriteria>())
                .Returns(new List<CurrencyCodeModel> { new CurrencyCodeModel { CurrencyId = 1, CurrencyCode = "USD" } });

            monthlyPayrollService.CalculateMonthlyBasicAllowance(
                currencyExchangeRates,
                basicAllowance,
                workingMonth,
                workingMonth.AddMonths(-3),
                new DateTime(workingMonth.Year, workingMonth.Month, 20));

            Assert.NotEqual(basicAllowance.TotalAmount, basicAllowance.AfterAdjustmentAmount);
            Assert.NotNull(basicAllowance.Reason);
        }

        [Theory]
        [InlineData(PayrollAutoCalculateOn.DirectBanking1)]
        [InlineData(PayrollAutoCalculateOn.DirectBanking2)]
        [InlineData(PayrollAutoCalculateOn.Cash)]
        public void InitializeMonthlyAllowance_ValidDate_ReturnModel(PayrollAutoCalculateOn payrollAutoCalculateOn)
        {
            // Arrage
            var monthlyPayrollService = GetMonthlyPayrollService();
            var payrollId = 1;

            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 }
            };

            var allowanceAdditions = new List<MonthlyAllowanceAdditionModel> { new MonthlyAllowanceAdditionModel { Type = (int)AllowanceType.DutyMeal } };
            var allowanceDeductions = new List<MonthlyAllowanceDeductionModel> { new MonthlyAllowanceDeductionModel { Type = (int)AllowanceType.DutyMeal } };
            var dstNetSalary = new MonthlyDSTNetSalary { CurrencyId = 1, NetSalaryReceived = "333" };

            var basicAllowance = new MonthlyPayrollBasicAllowanceModel
            {
                PayrollId = payrollId,
                AutoCalculateOn = payrollAutoCalculateOn,
                CurrencyId = 1,
                CashCurrencyId = 1,
                DirectBanking1CurrencyId = 1,
                DirectBanking2CurrencyId = 1,
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

            // Act
            var result = monthlyPayrollService.InitializeMonthlyAllowance(basicAllowance, allowanceAdditions, allowanceDeductions, currencyExchangeRates, dstNetSalary);

            // Assert
            Assert.Equal(expectedTotalAmount, result.TotalAmount);
        }

        [Fact]
        public void InitializeAllowanceAdditions_ValidData_ReturnExpectedModel()
        {
            // Arrange
            var monthlyPayrollService = GetMonthlyPayrollService();
            var payrollId = 1;
            var standardWorkingDays = 22;
            var actualWorkingDays = 24;
            decimal unitAmount = 10;
            decimal dutyMealAmount = 10 * standardWorkingDays;
            decimal totalAmount = unitAmount * (actualWorkingDays - standardWorkingDays);

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

            // Act
            var result = monthlyPayrollService.InitializeAllowanceAdditions(standardWorkingDays, actualWorkingDays, basicAllowance, currencyExchangeRates);

            // Assert
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
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), null, "5806.45",
                "Deducted 4,193.55 (USD). Employee started working from Mar 14, 2018. Basic Salary changed on  for Annual Increment." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), null, "5806",
                "Deducted 4,194 (VND). Employee started working from Mar 14, 2018. Basic Salary changed on  for Annual Increment." };
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "9677.42",
                "Deducted 322.58 (USD). Employee official leaves from Mar 30, 2018. Basic Salary changed on  for Annual Increment." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "9677",
                "Deducted 323 (VND). Employee official leaves from Mar 30, 2018. Basic Salary changed on  for Annual Increment." };
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 30), "5483.87",
                "Deducted 4,516.13 (USD). Employee worked from Mar 14, 2018 to Mar 30, 2018. Basic Salary changed on  for Annual Increment." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 30), "5484",
                "Deducted 4,516 (VND). Employee worked from Mar 14, 2018 to Mar 30, 2018. Basic Salary changed on  for Annual Increment." };
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
        [InlineData(1, "eur", "2000.00", "12000.00")]
        [InlineData(11, "vnd", "2000", "12000")]
        public void CalculateBackPay_AnnualIncrement_EffectiveMonthGreaterThanWorkingMonth_ReturnAsExpectedBackPayAndAfterAdjustment(
            int currencyId,
            string currencyCode,
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
                CurrencyCode = currencyCode,
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
            Assert.Empty(basicSetting.AfterAdjustmentReason);
        }

        public static IEnumerable<object[]> AnnualIncrement_EffectiveMonthEqualWorkingMonth_TestCase()
        {
            // Deducted
            yield return new object[] { "10000", "12000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), null, "11161.29",
                "Deducted 838.71 (USD). Employee started working from Mar 01, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "10000", "12000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), null, "11161",
                "Deducted 839 (VND). Employee started working from Mar 01, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "10000", "12000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), new DateTime(2018, 3, 30), "10774.19",
                "Deducted 1,225.81 (USD). Employee worked from Mar 01, 2018 to Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "10000", "12000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), new DateTime(2018, 3, 30), "10774",
                "Deducted 1,226 (VND). Employee worked from Mar 01, 2018 to Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "10000", "12000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), null, "11161.29",
                "Deducted 838.71 (USD). Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "10000", "12000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), null, "11161",
                "Deducted 839 (VND). Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "10000", "12000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "10774.19",
                "Deducted 1,225.81 (USD). Employee official leaves from Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "10000", "12000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "10774",
                "Deducted 1,226 (VND). Employee official leaves from Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            //// Added
            yield return new object[] { "12000", "10000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), null, "10838.71",
                "Added 838.71 (USD). Employee started working from Mar 01, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "12000", "10000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), null, "10839",
                "Added 839 (VND). Employee started working from Mar 01, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "12000", "10000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), new DateTime(2018, 3, 30), "10516.13",
                "Added 516.13 (USD). Employee worked from Mar 01, 2018 to Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "12000", "10000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), new DateTime(2018, 3, 30), "10516",
                "Added 516 (VND). Employee worked from Mar 01, 2018 to Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "12000", "10000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), null, "10838.71",
                "Added 838.71 (USD). Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "12000", "10000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), null, "10839",
                "Added 839 (VND). Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "12000", "10000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "10516.13",
                "Added 516.13 (USD). Employee official leaves from Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
            yield return new object[] { "12000", "10000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "10516",
                "Added 516 (VND). Employee official leaves from Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Annual Increment." };
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
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), null, "5806.45",
                "Deducted 4,193.55 (USD). Employee started working from Mar 14, 2018. Basic Salary changed on  for Promotion." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), null, "5806",
                "Deducted 4,194 (VND). Employee started working from Mar 14, 2018. Basic Salary changed on  for Promotion." };
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "9677.42",
                "Deducted 322.58 (USD). Employee official leaves from Mar 30, 2018. Basic Salary changed on  for Promotion." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "9677",
                "Deducted 323 (VND). Employee official leaves from Mar 30, 2018. Basic Salary changed on  for Promotion." };
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 30), "5483.87",
                "Deducted 4,516.13 (USD). Employee worked from Mar 14, 2018 to Mar 30, 2018. Basic Salary changed on  for Promotion." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 30), "5484",
                "Deducted 4,516 (VND). Employee worked from Mar 14, 2018 to Mar 30, 2018. Basic Salary changed on  for Promotion." };
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
            yield return new object[] { "10000", "12000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), null, "11161.29",
                "Deducted 838.71 (USD). Employee started working from Mar 01, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "10000", "12000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), null, "11161",
                "Deducted 839 (VND). Employee started working from Mar 01, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "10000", "12000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), new DateTime(2018, 3, 30), "10774.19",
                "Deducted 1,225.81 (USD). Employee worked from Mar 01, 2018 to Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "10000", "12000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), new DateTime(2018, 3, 30), "10774",
                "Deducted 1,226 (VND). Employee worked from Mar 01, 2018 to Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "10000", "12000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), null, "11161.29",
                "Deducted 838.71 (USD). Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "10000", "12000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), null, "11161",
                "Deducted 839 (VND). Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "10000", "12000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "10774.19",
                "Deducted 1,225.81 (USD). Employee official leaves from Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "10000", "12000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "10774",
                "Deducted 1,226 (VND). Employee official leaves from Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            // Added
            yield return new object[] { "12000", "10000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), null, "10838.71",
                "Added 838.71 (USD). Employee started working from Mar 01, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "12000", "10000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), null, "10839",
                "Added 839 (VND). Employee started working from Mar 01, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "12000", "10000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), new DateTime(2018, 3, 30), "10516.13",
                "Added 516.13 (USD). Employee worked from Mar 01, 2018 to Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "12000", "10000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 1), new DateTime(2018, 3, 30), "10516",
                "Added 516 (VND). Employee worked from Mar 01, 2018 to Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "12000", "10000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), null, "10838.71",
                "Added 838.71 (USD). Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "12000", "10000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), null, "10839",
                "Added 839 (VND). Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "12000", "10000", 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "10516.13",
                "Added 516.13 (USD). Employee official leaves from Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
            yield return new object[] { "12000", "10000", 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "10516",
                "Added 516 (VND). Employee official leaves from Mar 30, 2018. Basic Salary changed on Mar 14, 2018 for Promotion." };
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
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), null, "5806.45",
                "Deducted 4,193.55 (USD). Employee started working from Mar 14, 2018." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), null, "5806",
                "Deducted 4,194 (VND). Employee started working from Mar 14, 2018." };
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "9677.42",
                "Deducted 322.58 (USD). Employee official leaves from Mar 30, 2018." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 2, 1), new DateTime(2018, 3, 30), "9677",
                "Deducted 323 (VND). Employee official leaves from Mar 30, 2018." };
            yield return new object[] { 1, "USD", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 30), "5483.87",
                "Deducted 4,516.13 (USD). Employee worked from Mar 14, 2018 to Mar 30, 2018." };
            yield return new object[] { 11, "VND", new DateTime(2018, 3, 1), new DateTime(2018, 3, 14), new DateTime(2018, 3, 30), "5484",
                "Deducted 4,516 (VND). Employee worked from Mar 14, 2018 to Mar 30, 2018." };
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
            Assert.Equal("10000", basicSalary.AfterAdjustmentAmount);
            Assert.Equal("11000", monthlySalary.Salary);
            Assert.Equal(basicSalary.EbankingCurrencyId, monthlySalary.EbankingCurrencyId);
            Assert.Equal("4000", basicSalary.EbankingAmount);
            Assert.Equal("5000", monthlySalary.EbankingAmount);
            Assert.Equal(basicSalary.EbankingAccountId, monthlySalary.EbankingAccountId);
            Assert.Equal(basicSalary.DirectBanking1CurrencyId, monthlySalary.DirectBanking1CurrencyId);
            Assert.Equal(basicSalary.DirectBanking1Amount, monthlySalary.DirectBanking1Amount);
            Assert.Equal(basicSalary.DirectBanking1AccountId, monthlySalary.DirectBanking1AccountId);
            Assert.Equal(basicSalary.DirectBanking2CurrencyId, monthlySalary.DirectBanking2CurrencyId);
            Assert.Equal(basicSalary.DirectBanking2Amount, monthlySalary.DirectBanking2Amount);
            Assert.Equal(basicSalary.DirectBanking2AccountId, monthlySalary.DirectBanking2AccountId);
            Assert.Equal(basicSalary.CashCurrencyId, monthlySalary.CashCurrencyId);
            Assert.Equal(basicSalary.CashAmount, monthlySalary.CashAmount);
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
            Assert.Equal("10000", basicSalary.AfterAdjustmentAmount);
            Assert.Equal("11000", monthlySalary.Salary);
            Assert.Equal(basicSalary.EbankingCurrencyId, monthlySalary.EbankingCurrencyId);
            Assert.Equal(basicSalary.EbankingAmount, monthlySalary.EbankingAmount);
            Assert.Equal(basicSalary.DirectBanking1CurrencyId, monthlySalary.DirectBanking1CurrencyId);
            Assert.Equal("4000", basicSalary.DirectBanking1Amount);
            Assert.Equal("5000", monthlySalary.DirectBanking1Amount);
            Assert.Equal(basicSalary.DirectBanking2CurrencyId, monthlySalary.DirectBanking2CurrencyId);
            Assert.Equal(basicSalary.DirectBanking2Amount, monthlySalary.DirectBanking2Amount);
            Assert.Equal(basicSalary.CashCurrencyId, monthlySalary.CashCurrencyId);
            Assert.Equal(basicSalary.CashAmount, monthlySalary.CashAmount);
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
            Assert.Equal("10000", basicSalary.AfterAdjustmentAmount);
            Assert.Equal("11000", monthlySalary.Salary);
            Assert.Equal(basicSalary.EbankingCurrencyId, monthlySalary.EbankingCurrencyId);
            Assert.Equal(basicSalary.EbankingAmount, monthlySalary.EbankingAmount);
            Assert.Equal(basicSalary.DirectBanking1CurrencyId, monthlySalary.DirectBanking1CurrencyId);
            Assert.Equal(basicSalary.DirectBanking1Amount, monthlySalary.DirectBanking1Amount);
            Assert.Equal(basicSalary.DirectBanking2CurrencyId, monthlySalary.DirectBanking2CurrencyId);
            Assert.Equal("4000", basicSalary.DirectBanking2Amount);
            Assert.Equal("5000", monthlySalary.DirectBanking2Amount);
            Assert.Equal(basicSalary.CashCurrencyId, monthlySalary.CashCurrencyId);
            Assert.Equal(basicSalary.CashAmount, monthlySalary.CashAmount);
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
            Assert.Equal("10000", basicSalary.AfterAdjustmentAmount);
            Assert.Equal("11000", monthlySalary.Salary);
            Assert.Equal(basicSalary.EbankingCurrencyId, monthlySalary.EbankingCurrencyId);
            Assert.Equal(basicSalary.EbankingAmount, monthlySalary.EbankingAmount);
            Assert.Equal(basicSalary.DirectBanking1CurrencyId, monthlySalary.DirectBanking1CurrencyId);
            Assert.Equal(basicSalary.DirectBanking1Amount, monthlySalary.DirectBanking1Amount);
            Assert.Equal(basicSalary.DirectBanking2CurrencyId, monthlySalary.DirectBanking2CurrencyId);
            Assert.Equal(basicSalary.DirectBanking2Amount, monthlySalary.DirectBanking2Amount);
            Assert.Equal(basicSalary.CashCurrencyId, monthlySalary.CashCurrencyId);
            Assert.Equal("4000", basicSalary.CashAmount);
            Assert.Equal("5000", monthlySalary.CashAmount);
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