namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.MonthlyPayroll;
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;
    using HRP.Repositories;
    using HRP.Resources;

    public class MonthlySalaryService : IMonthlySalaryService
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly IMonthlyPayrollRepository monthlyPayrollRepository;

        public MonthlySalaryService(
            ISessionService sessionService,
            IBaseService baseService,
            ICryptoService cryptoService,
            IMonthlyPayrollRepository monthlyPayrollRepository)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.monthlyPayrollRepository = monthlyPayrollRepository;
        }

        #region GetMonthlySalaryAsync

        public async Task<(MonthlyBasicSalaryModel,
                MonthlySalaryModel,
                IEnumerable<MonthlySalaryAdditionModel>,
                IEnumerable<MonthlySalaryDeductionModel>)> GetMonthlySalaryAsync(int payrollId)
        {
            var criteria = new GetMonthlySalaryToEditCriteria
            {
                PayrollId = payrollId,
                UserId = sessionService.UserId
            };

            (MonthlyBasicSalaryModel,
                MonthlySalaryModel,
                IEnumerable<MonthlySalaryAdditionModel>,
                IEnumerable<MonthlySalaryDeductionModel>) monthlySalary = await monthlyPayrollRepository.GetMonthlySalaryAsync(criteria);

            if (monthlySalary.Item1 != null)
            {
                DecryptAndRoundingAmountMonthlySalary(monthlySalary);
            }

            return monthlySalary;
        }

        private void DecryptAndRoundingAmountMonthlySalary((MonthlyBasicSalaryModel,
                MonthlySalaryModel,
                IEnumerable<MonthlySalaryAdditionModel>,
                IEnumerable<MonthlySalaryDeductionModel>) monthlySalary)
        {
            monthlySalary.Item1.Salary = cryptoService.Decrypt(monthlySalary.Item1.Salary);
            monthlySalary.Item1.EbankingAmount = cryptoService.Decrypt(monthlySalary.Item1.EbankingAmount);
            monthlySalary.Item1.DirectBanking1Amount = cryptoService.Decrypt(monthlySalary.Item1.DirectBanking1Amount);
            monthlySalary.Item1.DirectBanking2Amount = cryptoService.Decrypt(monthlySalary.Item1.DirectBanking2Amount);
            monthlySalary.Item1.CashAmount = cryptoService.Decrypt(monthlySalary.Item1.CashAmount);
            monthlySalary.Item1.AfterAdjustmentAmount = cryptoService.Decrypt(monthlySalary.Item1.AfterAdjustmentAmount);
            monthlySalary.Item1.AfterAdjustmentReason = cryptoService.Decrypt(monthlySalary.Item1.AfterAdjustmentReason);

            monthlySalary.Item2.Salary = cryptoService.Decrypt(monthlySalary.Item2.Salary);
            monthlySalary.Item2.EbankingAmount = cryptoService.Decrypt(monthlySalary.Item2.EbankingAmount);
            monthlySalary.Item2.DirectBanking1Amount = cryptoService.Decrypt(monthlySalary.Item2.DirectBanking1Amount);
            monthlySalary.Item2.DirectBanking2Amount = cryptoService.Decrypt(monthlySalary.Item2.DirectBanking2Amount);
            monthlySalary.Item2.CashAmount = cryptoService.Decrypt(monthlySalary.Item2.CashAmount);
            monthlySalary.Item2.AdditionAmount = cryptoService.Decrypt(monthlySalary.Item2.AdditionAmount);
            monthlySalary.Item2.DeductionAmount = cryptoService.Decrypt(monthlySalary.Item2.DeductionAmount);

            decimal.TryParse(monthlySalary.Item2.AdditionAmount, out decimal amount);
            monthlySalary.Item2.AdditionAmountDisplay = !string.IsNullOrWhiteSpace(monthlySalary.Item2.AdditionAmount)
                ? CalculationService.RoundAmountWithCurrency(amount, monthlySalary.Item2.CurrencyCode).ToString(CultureInfo.InvariantCulture) : string.Empty;
            decimal.TryParse(monthlySalary.Item2.DeductionAmount, out amount);
            monthlySalary.Item2.DeductionAmountDisplay = !string.IsNullOrWhiteSpace(monthlySalary.Item2.DeductionAmount)
                ? CalculationService.RoundAmountWithCurrency(amount, monthlySalary.Item2.CurrencyCode).ToString(CultureInfo.InvariantCulture) : string.Empty;

            foreach (MonthlySalaryAdditionModel addition in monthlySalary.Item3)
            {
                addition.UnitAmount = cryptoService.Decrypt(addition.UnitAmount);
                addition.NetSalary = cryptoService.Decrypt(addition.NetSalary);

                decimal.TryParse(addition.NetSalary, out amount);
                addition.NetSalaryDisplay = !string.IsNullOrWhiteSpace(addition.NetSalary)
                    ? CalculationService.RoundAmountWithCurrency(amount, monthlySalary.Item2.CurrencyCode, 0).ToString(CultureInfo.InvariantCulture) : string.Empty;
            }

            foreach (MonthlySalaryDeductionModel deduction in monthlySalary.Item4)
            {
                deduction.UnitAmount = cryptoService.Decrypt(deduction.UnitAmount);
                deduction.NetSalary = cryptoService.Decrypt(deduction.NetSalary);

                decimal.TryParse(deduction.NetSalary, out amount);
                deduction.NetSalaryDisplay = !string.IsNullOrWhiteSpace(deduction.NetSalary)
                    ? CalculationService.RoundAmountWithCurrency(amount, monthlySalary.Item2.CurrencyCode, 0).ToString(CultureInfo.InvariantCulture) : string.Empty;
            }
        }

        #endregion GetMonthlySalaryAsync

        public async Task<CommandResult> EditMonthlySalaryAsync(
            int payrollId,
            int companyId,
            DateTime month,
            MonthlySalaryModel monthlySalary,
            IEnumerable<MonthlySalaryAdditionModel> additions,
            IEnumerable<MonthlySalaryDeductionModel> deductions)
        {
            EncryptMonthlySalaryAddition(additions);
            EncryptMonthlySalaryDeduction(deductions);
            var criteria = new EditMonthlySalaryCommand
            {
                PayrollId = payrollId,
                CompanyId = companyId,
                UserId = sessionService.UserId,
                Month = month,
                Salary = cryptoService.Encrypt(monthlySalary.Salary),
                EbankingCurrencyId = monthlySalary.EbankingCurrencyId,
                EbankingAmount = cryptoService.Encrypt(monthlySalary.EbankingAmount),
                EbankingAccountId = monthlySalary.EbankingAccountId,
                DirectBanking1CurrencyId = monthlySalary.DirectBanking1CurrencyId,
                DirectBanking1Amount = cryptoService.Encrypt(monthlySalary.DirectBanking1Amount),
                DirectBanking1AccountId = monthlySalary.DirectBanking1AccountId,
                DirectBanking2CurrencyId = monthlySalary.DirectBanking2CurrencyId,
                DirectBanking2Amount = cryptoService.Encrypt(monthlySalary.DirectBanking2Amount),
                DirectBanking2AccountId = monthlySalary.DirectBanking2AccountId,
                CashCurrencyId = monthlySalary.CashCurrencyId,
                CashAmount = cryptoService.Encrypt(monthlySalary.CashAmount),
                CashAccountId = monthlySalary.CashAccountId,
                AdditionAmount = cryptoService.Encrypt(monthlySalary.AdditionAmount),
                DeductionAmount = cryptoService.Encrypt(monthlySalary.DeductionAmount),
                AutoCalculateOn = monthlySalary.AutoCalculateOn.ToString(),
                AdditionXml = additions.ToXML().ToString(),
                DeductionXml = deductions.ToXML().ToString()
            };
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public void CalculateBackPayAndAfterAdjustment(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            DateTime workingMonth,
            MonthlyBasicSalaryModel basicSetting,
            MonthlySalaryAdditionModel backPay,
            DateTime groupStartDate,
            DateTime? groupEndDate)
        {
            decimal? afterAdjustmentAmount = GetAfterAdjustmentAmount(currencyExchangeRates, workingMonth, basicSetting, groupStartDate, groupEndDate);
            basicSetting.AfterAdjustmentAmount = afterAdjustmentAmount.HasValue ? afterAdjustmentAmount.Value.ToString(CultureInfo.InvariantCulture) : null;
            GetAfterAdjustmentReason(workingMonth, basicSetting, groupStartDate, groupEndDate, afterAdjustmentAmount ?? 0);

            decimal? backPayAmount = GetBackPayAmount(currencyExchangeRates, workingMonth, basicSetting);
            backPay.UnitAmount = backPay.NetSalary = backPayAmount.HasValue ? backPayAmount.Value.ToString(CultureInfo.InvariantCulture) : null;
            backPay.Description = backPayAmount.HasValue && basicSetting.EffectiveDate.HasValue
                ? string.Format(Message.BackPayDescriptionFormat, basicSetting.EffectiveDate.Value.ToString(Constants.DateFormat)) : null;
        }

        public void CalculateMonthlySalary(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            MonthlyBasicSalaryModel basicSalary,
            MonthlySalaryModel monthlySalary,
            IList<MonthlySalaryAdditionModel> additions,
            IList<MonthlySalaryDeductionModel> deductions)
        {
            decimal amount = 0;
            decimal.TryParse(basicSalary.Salary, out decimal salaryAmount);
            decimal.TryParse(basicSalary.AfterAdjustmentAmount, out decimal afterAdjustmentAmount);
            var additionAmount = GetAdditionAmount(additions);
            var deductionAmount = GetDeductionAmount(deductions);
            decimal ebanking = CalculationService.ConvertAmount(currencyExchangeRates, basicSalary.EbankingAmount, basicSalary.EbankingCurrencyId, basicSalary.CurrencyId);
            decimal directBanking1 = CalculationService.ConvertAmount(currencyExchangeRates, basicSalary.DirectBanking1Amount, basicSalary.DirectBanking1CurrencyId, basicSalary.CurrencyId);
            decimal directBanking2 = CalculationService.ConvertAmount(currencyExchangeRates, basicSalary.DirectBanking2Amount, basicSalary.DirectBanking2CurrencyId, basicSalary.CurrencyId);
            decimal cash = CalculationService.ConvertAmount(currencyExchangeRates, basicSalary.CashAmount, basicSalary.CashCurrencyId, basicSalary.CurrencyId);
            decimal salary = CalculationService.RoundAmountWithCurrency(afterAdjustmentAmount + additionAmount - deductionAmount, basicSalary.CurrencyCode, 0);

            switch (basicSalary.AutoCalculateOn)
            {
                case PayrollAutoCalculateOn.Ebanking:
                    var ebankingSubAmount = directBanking1 + directBanking2 + cash;
                    PayrollAutoCalculateOnEbanking(monthlySalary, basicSalary, ref amount, currencyExchangeRates, salaryAmount, salary, ebankingSubAmount);

                    break;

                case PayrollAutoCalculateOn.DirectBanking1:
                    var directBanking1SubAmount = ebanking + directBanking2 + cash;
                    PayrollAutoCalculateOnDirectBanking1(monthlySalary, basicSalary, ref amount, currencyExchangeRates, salaryAmount, salary, directBanking1SubAmount);

                    break;

                case PayrollAutoCalculateOn.DirectBanking2:
                    var directBanking2SubAmount = ebanking + directBanking1 + cash;
                    PayrollAutoCalculateOnDirectBanking2(monthlySalary, basicSalary, ref amount, currencyExchangeRates, salaryAmount, salary, directBanking2SubAmount);

                    break;

                default:
                    var subAmount = ebanking + directBanking1 + directBanking2;
                    PayrollAutoCalculateOnDefault(monthlySalary, basicSalary, ref amount, currencyExchangeRates, salaryAmount, salary, subAmount);

                    break;
            }

            monthlySalary.CurrencyId = basicSalary.CurrencyId;
            monthlySalary.Salary = salary != 0 ? salary.ToString(CultureInfo.InvariantCulture) : string.Empty;
            monthlySalary.EbankingCurrencyId = basicSalary.EbankingCurrencyId;
            monthlySalary.EbankingAccountId = basicSalary.EbankingAccountId;
            monthlySalary.DirectBanking1CurrencyId = basicSalary.DirectBanking1CurrencyId;
            monthlySalary.DirectBanking1AccountId = basicSalary.DirectBanking1AccountId;
            monthlySalary.DirectBanking2CurrencyId = basicSalary.DirectBanking2CurrencyId;
            monthlySalary.DirectBanking2AccountId = basicSalary.DirectBanking2AccountId;
            monthlySalary.CashCurrencyId = basicSalary.CashCurrencyId;
            monthlySalary.CashAccountId = basicSalary.CashAccountId;
            monthlySalary.AutoCalculateOn = basicSalary.AutoCalculateOn;
            monthlySalary.AdditionAmount = additionAmount > 0
                ? CalculationService.RoundAmountWithCurrency(additionAmount, basicSalary.CurrencyCode, 0).ToString(CultureInfo.InvariantCulture) : string.Empty;
            monthlySalary.DeductionAmount = deductionAmount > 0
                ? CalculationService.RoundAmountWithCurrency(deductionAmount, basicSalary.CurrencyCode, 0).ToString(CultureInfo.InvariantCulture) : string.Empty;
            monthlySalary.Status = amount < 0 ? MonthlyPayrollStatus.Unsatisfied : MonthlyPayrollStatus.Default;
        }

        private static decimal GetAdditionAmount(IList<MonthlySalaryAdditionModel> additions)
        {
            decimal additionAmount = 0;
            foreach (var addition in additions)
            {
                decimal.TryParse(addition.NetSalary, out decimal amount);
                additionAmount += amount;
            }

            return additionAmount;
        }

        private static decimal GetDeductionAmount(IList<MonthlySalaryDeductionModel> deductions)
        {
            decimal deductionAmount = 0;
            foreach (var deduction in deductions)
            {
                decimal.TryParse(deduction.NetSalary, out decimal amount);
                deductionAmount += amount;
            }

            return deductionAmount;
        }

        private static void PayrollAutoCalculateOnEbanking(
            MonthlySalaryModel monthlySalary,
            MonthlyBasicSalaryModel basicSalary,
            ref decimal amount,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            decimal salaryAmount,
            decimal salary,
            decimal subAmount)
        {
            monthlySalary.DirectBanking1Amount = basicSalary.DirectBanking1Amount;
            monthlySalary.DirectBanking2Amount = basicSalary.DirectBanking2Amount;
            monthlySalary.CashAmount = basicSalary.CashAmount;
            if (basicSalary.EbankingCurrencyId.HasValue)
            {
                amount = CalculationService.ConvertAmount(currencyExchangeRates, salaryAmount - subAmount, basicSalary.CurrencyId,
                    basicSalary.EbankingCurrencyId.Value);
                amount = CalculationService.RoundAmountWithCurrency(amount, basicSalary.EbankingCurrencyCode, 0);
                basicSalary.EbankingAmount = amount.ToString(CultureInfo.InvariantCulture);

                amount = CalculationService.ConvertAmount(currencyExchangeRates, salary - subAmount, basicSalary.CurrencyId, basicSalary.EbankingCurrencyId.Value);
                amount = CalculationService.RoundAmountWithCurrency(amount, basicSalary.EbankingCurrencyCode, 0);
                monthlySalary.EbankingAmount = amount.ToString(CultureInfo.InvariantCulture);
            }
        }

        private static void PayrollAutoCalculateOnDirectBanking1(
            MonthlySalaryModel monthlySalary,
            MonthlyBasicSalaryModel basicSalary,
            ref decimal amount,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            decimal salaryAmount,
            decimal salary,
            decimal subAmount)
        {
            monthlySalary.EbankingAmount = basicSalary.EbankingAmount;
            monthlySalary.DirectBanking2Amount = basicSalary.DirectBanking2Amount;
            monthlySalary.CashAmount = basicSalary.CashAmount;
            if (basicSalary.DirectBanking1CurrencyId.HasValue)
            {
                amount = CalculationService.ConvertAmount(currencyExchangeRates, salaryAmount - subAmount, basicSalary.CurrencyId,
                    basicSalary.DirectBanking1CurrencyId.Value);
                amount = CalculationService.RoundAmountWithCurrency(amount, basicSalary.DirectBanking1CurrencyCode, 0);
                basicSalary.DirectBanking1Amount = amount.ToString(CultureInfo.InvariantCulture);

                amount = CalculationService.ConvertAmount(currencyExchangeRates, salary - subAmount, basicSalary.CurrencyId, basicSalary.DirectBanking1CurrencyId.Value);
                amount = CalculationService.RoundAmountWithCurrency(amount, basicSalary.DirectBanking1CurrencyCode, 0);
                monthlySalary.DirectBanking1Amount = amount.ToString(CultureInfo.InvariantCulture);
            }
        }

        private static void PayrollAutoCalculateOnDirectBanking2(
            MonthlySalaryModel monthlySalary,
            MonthlyBasicSalaryModel basicSalary,
            ref decimal amount,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            decimal salaryAmount,
            decimal salary,
            decimal subAmount)
        {
            monthlySalary.EbankingAmount = basicSalary.EbankingAmount;
            monthlySalary.DirectBanking1Amount = basicSalary.DirectBanking1Amount;
            monthlySalary.CashAmount = basicSalary.CashAmount;
            if (basicSalary.DirectBanking2CurrencyId.HasValue)
            {
                amount = CalculationService.ConvertAmount(currencyExchangeRates, salaryAmount - subAmount, basicSalary.CurrencyId,
                    basicSalary.DirectBanking2CurrencyId.Value);
                amount = CalculationService.RoundAmountWithCurrency(amount, basicSalary.DirectBanking2CurrencyCode, 0);
                basicSalary.DirectBanking2Amount = amount.ToString(CultureInfo.InvariantCulture);

                amount = CalculationService.ConvertAmount(currencyExchangeRates, salary - subAmount, basicSalary.CurrencyId, basicSalary.DirectBanking2CurrencyId.Value);
                amount = CalculationService.RoundAmountWithCurrency(amount, basicSalary.DirectBanking2CurrencyCode, 0);
                monthlySalary.DirectBanking2Amount = amount.ToString(CultureInfo.InvariantCulture);
            }
        }

        private static void PayrollAutoCalculateOnDefault(
            MonthlySalaryModel monthlySalary,
            MonthlyBasicSalaryModel basicSalary,
            ref decimal amount,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            decimal salaryAmount,
            decimal salary,
            decimal subAmount)
        {
            monthlySalary.EbankingAmount = basicSalary.EbankingAmount;
            monthlySalary.DirectBanking1Amount = basicSalary.DirectBanking1Amount;
            monthlySalary.DirectBanking2Amount = basicSalary.DirectBanking2Amount;
            if (basicSalary.CashCurrencyId.HasValue)
            {
                amount = CalculationService.ConvertAmount(currencyExchangeRates, salaryAmount - subAmount, basicSalary.CurrencyId,
                    basicSalary.CashCurrencyId.Value);
                amount = CalculationService.RoundAmountWithCurrency(amount, basicSalary.CashCurrencyCode, 0);
                basicSalary.CashAmount = amount.ToString(CultureInfo.InvariantCulture);

                amount = CalculationService.ConvertAmount(currencyExchangeRates, salary - subAmount, basicSalary.CurrencyId, basicSalary.CashCurrencyId.Value);
                amount = CalculationService.RoundAmountWithCurrency(amount, basicSalary.CashCurrencyCode, 0);
                monthlySalary.CashAmount = amount.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void EncryptMonthlySalaryAddition(IEnumerable<MonthlySalaryAdditionModel> additions)
        {
            foreach (var addition in additions)
            {
                addition.UnitAmount = cryptoService.Encrypt(addition.UnitAmount);
                addition.NetSalary = cryptoService.Encrypt(addition.NetSalary);
            }
        }

        private void EncryptMonthlySalaryDeduction(IEnumerable<MonthlySalaryDeductionModel> deductions)
        {
            foreach (var deduction in deductions)
            {
                deduction.UnitAmount = cryptoService.Encrypt(deduction.UnitAmount);
                deduction.NetSalary = cryptoService.Encrypt(deduction.NetSalary);
            }
        }

        #region Back Pay and After Adjustment

        private static decimal? GetAfterAdjustmentAmount(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            DateTime workingMonth,
            MonthlyBasicSalaryModel basicSetting,
            DateTime groupStartDate,
            DateTime? groupEndDate)
        {
            decimal? amount = null;
            if (!string.IsNullOrWhiteSpace(basicSetting.Salary))
            {
                var currencyId = basicSetting.CurrencyId ?? 0;
                decimal.TryParse(basicSetting.OldSalary, out decimal oldSalary);
                decimal.TryParse(basicSetting.Salary, out decimal newSalary);
                oldSalary *= CalculationService.GetExchangeRate(currencyExchangeRates, basicSetting.OldCurrencyId, currencyId);
                int daysInMonth = DateTime.DaysInMonth(workingMonth.Year, workingMonth.Month);
                int startDay = CalculationService.GetDifferenceMonth(groupStartDate, workingMonth) == 0 ? groupStartDate.Day : 1;
                int endDay = (groupEndDate.HasValue && CalculationService.GetDifferenceMonth(groupEndDate.Value, workingMonth) == 0) ? groupEndDate.Value.Day : daysInMonth;

                amount = (basicSetting.EffectiveDate.HasValue
                    && CalculationService.GetDifferenceMonth(basicSetting.EffectiveDate.Value, workingMonth) == 0
                    && oldSalary != newSalary)
                        ? (oldSalary / daysInMonth * (basicSetting.EffectiveDate.Value.Day - startDay))
                            + (newSalary / daysInMonth * (endDay - basicSetting.EffectiveDate.Value.Day + 1))
                        : newSalary / daysInMonth * (endDay - startDay + 1);

                amount = CalculationService.RoundAmountWithCurrency(amount.Value, basicSetting.CurrencyCode, roundingThousand: false);
            }

            return amount;
        }

        private static decimal? GetBackPayAmount(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            DateTime workingMonth,
            MonthlyBasicSalaryModel basicSetting)
        {
            var currencyId = basicSetting.CurrencyId ?? 0;
            decimal.TryParse(basicSetting.OldSalary, out decimal oldSalary);
            decimal.TryParse(basicSetting.Salary, out decimal newSalary);
            oldSalary *= CalculationService.GetExchangeRate(currencyExchangeRates, basicSetting.OldCurrencyId, currencyId);
            decimal? backPayAmount = null;

            if (basicSetting.SalaryChangeReason == BasicSalaryChangeReason.AnnualIncrement
                && basicSetting.EffectiveDate.HasValue
                && oldSalary != newSalary)
            {
                int monthDiff = CalculationService.GetDifferenceMonth(basicSetting.EffectiveDate.Value, workingMonth);
                if (monthDiff > 0)
                {
                    var daysInMonth = DateTime.DaysInMonth(basicSetting.EffectiveDate.Value.Year, basicSetting.EffectiveDate.Value.Month);
                    backPayAmount = ((newSalary - oldSalary) / daysInMonth * (daysInMonth - basicSetting.EffectiveDate.Value.Day + 1))
                        + ((newSalary - oldSalary) * (monthDiff - 1));

                    backPayAmount = CalculationService.RoundAmountWithCurrency(backPayAmount.Value, basicSetting.CurrencyCode, roundingThousand: false);
                }
            }

            return backPayAmount;
        }

        private static void GetAfterAdjustmentReason(
            DateTime workingMonth,
            MonthlyBasicSalaryModel basicSetting,
            DateTime groupStartDate,
            DateTime? groupEndDate,
            decimal afterAdjustmentAmount)
        {
            decimal.TryParse(basicSetting.OldSalary, out decimal _);
            decimal.TryParse(basicSetting.Salary, out decimal salary);
            decimal differenceAmount = CalculationService.RoundAmountWithCurrency(afterAdjustmentAmount - salary, basicSetting.CurrencyCode, roundingThousand: false);
            var reason = new StringBuilder();
            if (differenceAmount != 0)
            {
                string dateCommencementReason = CalculationService.GetPartOfAfterAdjustmentReason(workingMonth, groupStartDate, groupEndDate);
                reason.AppendFormat("{0} {1} ({2}).",
                    differenceAmount > 0 ? "Added" : "Deducted",
                    CalculationService.FormatAmount(Math.Abs(differenceAmount), basicSetting.CurrencyCode),
                    basicSetting.CurrencyCode);
                reason.Append(dateCommencementReason);

                switch (basicSetting.SalaryChangeReason)
                {
                    case BasicSalaryChangeReason.AnnualIncrement:
                    case BasicSalaryChangeReason.Promotion:
                    case BasicSalaryChangeReason.SalaryAdjustment:
                    case BasicSalaryChangeReason.Demotion:
                    case BasicSalaryChangeReason.Penalty:
                        reason.AppendFormat(
                            " Basic Salary changed on {0} for {1}.",
                            basicSetting.EffectiveDate.HasValue ? basicSetting.EffectiveDate.Value.ToString(Constants.DateFormat) : string.Empty,
                            basicSetting.SalaryChangeReason.Value.GetDisplayName());
                        break;

                    default:
                        // nothing to do
                        break;
                }
            }

            basicSetting.AfterAdjustmentReason = reason.ToString();
        }

        #endregion Back Pay and After Adjustment
    }
}