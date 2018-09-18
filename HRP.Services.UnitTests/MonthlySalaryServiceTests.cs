namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.MonthlyPayroll;
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;
    using HRP.Repositories;
    using NSubstitute;
    using Xunit;

    public class MonthlySalaryServiceTests
    {
        private ISessionService sessionService;
        private IBaseService baseService;
        private ICryptoService cryptoService;
        private IMonthlyPayrollRepository monthlyPayrollRepository;

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
            var monthlyPayrollService = GetMonthlySalaryService();
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
            var monthlyPayrollService = GetMonthlySalaryService();
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
            var monthlyPayrollService = GetMonthlySalaryService();
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
            var monthlyPayrollService = GetMonthlySalaryService();

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
            var monthlyPayrollService = GetMonthlySalaryService();

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
            var monthlyPayrollService = GetMonthlySalaryService();

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
            var monthlyPayrollService = GetMonthlySalaryService();

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
            var monthlyPayrollService = GetMonthlySalaryService();

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
            var monthlyPayrollService = GetMonthlySalaryService();

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
            var monthlyPayrollService = GetMonthlySalaryService();

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
            var monthlyPayrollService = GetMonthlySalaryService();

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
            var monthlyPayrollService = GetMonthlySalaryService();

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
            var monthlyPayrollService = GetMonthlySalaryService();

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

        private MonthlySalaryService GetMonthlySalaryService()
        {
            sessionService = FakeSessionService();
            baseService = Substitute.For<IBaseService>();
            cryptoService = Substitute.For<ICryptoService>();
            monthlyPayrollRepository = Substitute.For<IMonthlyPayrollRepository>();
            var service = new MonthlySalaryService(sessionService, baseService, cryptoService, monthlyPayrollRepository);

            return service;
        }

        #endregion Helper
    }
}