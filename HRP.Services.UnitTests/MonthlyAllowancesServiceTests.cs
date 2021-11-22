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
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.PaymentProcess;
    using HRP.Models.ServiceModels;
    using HRP.Repositories;
    using NSubstitute;
    using Xunit;

    public class MonthlyAllowancesServiceTests
    {
        private IBaseService baseService;
        private ICryptoService cryptoService;
        private IMonthlyPayrollRepository monthlyPayrollRepository;

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

            var monthlyPayrollService = GetMonthlyAllowancesService();
            cryptoService.Decrypt(Arg.Any<string>()).Returns("1");
            monthlyPayrollRepository.GetMonthlyAllowancesAsync(Arg.Any<GetMonthlyAllowanceCriteria>())
                .Returns((allowance, additions, deductions, basicAllowance));

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
            var monthlyPayrollService = GetMonthlyAllowancesService();
            cryptoService.Encrypt(Arg.Any<string>()).Returns(string.Empty);
            baseService.ExecuteNonQueryAsync(Arg.Any<EditMonthlyAllowancesCommand>()).Returns(new CommandResult { Code = 0 });
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
            var monthlyPayrollService = GetMonthlyAllowancesService();
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
            var monthlyPayrollService = GetMonthlyAllowancesService();
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
            var monthlyPayrollService = GetMonthlyAllowancesService();
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
            var monthlyPayrollService = GetMonthlyAllowancesService();
            var payrollId = 1;

            var currencyExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 }
            };

            var allowanceAdditions = new List<MonthlyAllowanceAdditionModel> { new MonthlyAllowanceAdditionModel { Type = (int)MonthlyAllowanceAdditionType.DutyMeal } };
            var allowanceDeductions = new List<MonthlyAllowanceDeductionModel> { new MonthlyAllowanceDeductionModel { Type = (int)MonthlyAllowanceAdditionType.DutyMeal } };
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
            var monthlyPayrollService = GetMonthlyAllowancesService();
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
            var data = new GeneratePayrollModel
            {
                CurrencyExchangeRate = new List<CurrencyExchangeRateModel>
                {
                    new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 }
                }
            };

            // Act
            var result = monthlyPayrollService.InitializeAllowanceAdditions(standardWorkingDays, actualWorkingDays, basicAllowance, data);

            // Assert
            Assert.NotEmpty(result);
            var dutyMeal = result.FirstOrDefault(x => x.Type == (int)MonthlyAllowanceAdditionType.DutyMeal);
            Assert.NotNull(dutyMeal);
            Assert.Equal(actualWorkingDays - standardWorkingDays, dutyMeal.NoOfDays);
            Assert.Equal(unitAmount.ToString(), dutyMeal.UnitAmount);
            Assert.Equal(totalAmount.ToString(), dutyMeal.TotalAmount);
        }

        [Fact]
        public void InitializeAllowanceDeductions_ValidData_ReturnExpectedModel()
        {
            var monthlyPayrollService = GetMonthlyAllowancesService();
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
            var data = new GeneratePayrollModel
            {
                CurrencyExchangeRate = new List<CurrencyExchangeRateModel>
                {
                    new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 }
                }
            };

            var result = monthlyPayrollService.InitializeAllowanceDeductions(standardWorkingDays, actualWorkingDays, basicAllowance, data);

            Assert.NotEmpty(result);
            var dutyMeal = result.FirstOrDefault(x => x.Type == (int)MonthlyAllowanceAdditionType.DutyMeal);
            Assert.NotNull(dutyMeal);
            Assert.Equal(standardWorkingDays - actualWorkingDays, dutyMeal.NoOfDays);
            Assert.Equal(unitAmount.ToString(), dutyMeal.UnitAmount);
            Assert.Equal(totalAmount.ToString(), dutyMeal.TotalAmount);
        }

        #region Helper

        private MonthlyAllowancesService GetMonthlyAllowancesService()
        {
            baseService = Substitute.For<IBaseService>();
            cryptoService = Substitute.For<ICryptoService>();
            monthlyPayrollRepository = Substitute.For<IMonthlyPayrollRepository>();
            var service = new MonthlyAllowancesService(baseService, cryptoService, monthlyPayrollRepository);

            return service;
        }

        #endregion Helper
    }
}