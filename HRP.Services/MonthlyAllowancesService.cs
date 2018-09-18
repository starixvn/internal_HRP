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
    using HRP.Models.ServiceModels;
    using HRP.Repositories;

    public class MonthlyAllowancesService : IMonthlyAllowancesService
    {
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly IMonthlyPayrollRepository monthlyPayrollRepository;

        public MonthlyAllowancesService(
            IBaseService baseService,
            ICryptoService cryptoService,
            IMonthlyPayrollRepository monthlyPayrollRepository)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.monthlyPayrollRepository = monthlyPayrollRepository;
        }

        public async Task<MonthlyAllowancesModel> GetMonthlyAllowancesAsync(int payrollId, DateTime month)
        {
            var criteria = new GetMonthlyAllowanceCriteria { PayrollId = payrollId };
            var result = await monthlyPayrollRepository.GetMonthlyAllowancesAsync(criteria).ConfigureAwait(false);
            var model = new MonthlyAllowancesModel
            {
                Month = month,
                Allowance = result.Item1 ?? new MonthlyAllowanceModel(),
                Additions = result.Item2.OrderBy(x => x.Type).ToList(),
                Deductions = result.Item3.OrderBy(x => x.Type).ToList(),
                BasicAllowance = result.Item4 ?? new MonthlyPayrollBasicAllowanceModel()
            };

            DecryptMonthlyBasicAllowance(model.BasicAllowance);
            DecryptMonthlyAllowance(model.Allowance);
            DecryptMonthlyAllowanceAdditions(model.Additions, model.Allowance.CurrencyCode);
            DecryptMonthlyAllowanceDeductions(model.Deductions, model.Allowance.CurrencyCode);

            return model;
        }

        public async Task<CommandResult> EditMonthlyAllowancesAsync(MonthlyAllowancesModel model, int userId)
        {
            EncryptMonthlyAllowanceAddition(model.Additions);
            EncryptMonthlyAllowanceDeduction(model.Deductions);
            decimal.TryParse(model.Allowance.DirectBanking1Amount, out decimal directbanking1);
            decimal.TryParse(model.Allowance.DirectBanking2Amount, out decimal directbanking2);
            decimal.TryParse(model.Allowance.CashAmount, out decimal cash);
            var status = directbanking1 < 0 || directbanking2 < 0 || cash < 0 ? MonthlyPayrollStatus.Unsatisfied : MonthlyPayrollStatus.Satisfied;

            var criteria = new EditMonthlyAllowancesCriteria
            {
                PayrollId = model.Allowance.PayrollId,
                CurrencyId = model.Allowance.CurrencyId,
                TotalAmount = cryptoService.Encrypt(model.Allowance.TotalAmount),
                DirectBanking1CurrencyId = model.Allowance.DirectBanking1CurrencyId,
                DirectBanking1Amount = cryptoService.Encrypt(model.Allowance.DirectBanking1Amount),
                DirectBanking1AccountId = model.Allowance.DirectBanking1AccountId,
                DirectBanking2CurrencyId = model.Allowance.DirectBanking2CurrencyId,
                DirectBanking2Amount = cryptoService.Encrypt(model.Allowance.DirectBanking2Amount),
                DirectBanking2AccountId = model.Allowance.DirectBanking2AccountId,
                CashCurrencyId = model.Allowance.CashCurrencyId,
                CashAmount = cryptoService.Encrypt(model.Allowance.CashAmount),
                CashAccountId = model.Allowance.CashAccountId,
                AutoCalculateOn = model.Allowance.AutoCalculateOn,
                TotalNetAmount = cryptoService.Encrypt(model.Allowance.TotalNetAmount),
                DSTNetSalaryCurrencyId = model.Allowance.DSTNetSalaryCurrencyId,
                DSTNetSalaryAmount = cryptoService.Encrypt(model.Allowance.DSTNetSalaryAmount),
                AdditionAmount = cryptoService.Encrypt(model.Allowance.AdditionAmount),
                DeductionAmount = cryptoService.Encrypt(model.Allowance.DeductionAmount),
                AllowanceAddition = model.Additions.ToXML().ToString(),
                AllowanceDeduction = model.Deductions.ToXML().ToString(),
                UserId = userId,
                Status = status
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public MonthlyAllowanceModel InitializeMonthlyAllowance(
            MonthlyPayrollBasicAllowanceModel basicAllowance,
            IEnumerable<MonthlyAllowanceAdditionModel> allowanceAdditions,
            IEnumerable<MonthlyAllowanceDeductionModel> allowanceDeductions,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            MonthlyDSTNetSalary dstNetSalary)
        {
            var monthlyAllowance = new MonthlyAllowanceModel
            {
                PayrollId = basicAllowance.PayrollId,
                EmployeeId = basicAllowance.EmployeeId,
                CurrencyId = basicAllowance.CurrencyId,
                CurrencyCode = basicAllowance.CurrencyCode,
                DirectBanking1CurrencyId = basicAllowance.DirectBanking1CurrencyId,
                DirectBanking1CurrencyCode = basicAllowance.DirectBanking1CurrencyCode,
                DirectBanking1Amount = basicAllowance.DirectBanking1Amount,
                DirectBanking1AccountId = basicAllowance.DirectBanking1AccountId,
                DirectBanking2CurrencyId = basicAllowance.DirectBanking2CurrencyId,
                DirectBanking2CurrencyCode = basicAllowance.DirectBanking2CurrencyCode,
                DirectBanking2Amount = basicAllowance.DirectBanking2Amount,
                DirectBanking2AccountId = basicAllowance.DirectBanking2AccountId,
                CashCurrencyId = basicAllowance.CashCurrencyId,
                CashCurrencyCode = basicAllowance.CashCurrencyCode,
                CashAmount = basicAllowance.CashAmount,
                CashAccountId = basicAllowance.CashAccountId,
                AutoCalculateOn = basicAllowance.AutoCalculateOn,
                DSTNetSalaryAmount = dstNetSalary?.NetSalaryReceived,
                DSTNetSalaryCurrencyId = dstNetSalary?.CurrencyId
            };

            if (basicAllowance.CurrencyId.HasValue)
            {
                decimal.TryParse(basicAllowance.AfterAdjustmentAmount, out decimal totalAmount);

                if (basicAllowance.RoomingChargesCurrencyId.HasValue && !string.IsNullOrEmpty(basicAllowance.RoomingChargesAmount))
                {
                    var roomingCharge = CalculationService.ConvertAmount(currencyExchangeRates, basicAllowance.RoomingChargesAmount,
                        basicAllowance.RoomingChargesCurrencyId.Value, basicAllowance.CurrencyId.Value);
                    totalAmount = totalAmount - roomingCharge;
                }

                if (basicAllowance.FitnessFirstCurrencyId.HasValue && !string.IsNullOrEmpty(basicAllowance.FitnessFirstAmount))
                {
                    var fitnessFirst = CalculationService.ConvertAmount(currencyExchangeRates, basicAllowance.FitnessFirstAmount,
                        basicAllowance.FitnessFirstCurrencyId.Value, basicAllowance.CurrencyId.Value);
                    totalAmount = totalAmount - fitnessFirst;
                }

                var dutyMealAddition = allowanceAdditions.FirstOrDefault(x => x.Type == (int)AllowanceType.DutyMeal);
                decimal.TryParse(dutyMealAddition.NetAllowance, out decimal dutyMealAdditionAmount);
                totalAmount = totalAmount + dutyMealAdditionAmount;
                monthlyAllowance.AdditionAmount = dutyMealAdditionAmount.ToString(CultureInfo.InvariantCulture);

                var dutyMealDeduction = allowanceDeductions.FirstOrDefault(x => x.Type == (int)AllowanceType.DutyMeal);
                decimal.TryParse(dutyMealDeduction.NetAllowance, out decimal dutyMealDeductionAmount);
                totalAmount = totalAmount - dutyMealDeductionAmount;
                monthlyAllowance.DeductionAmount = dutyMealDeductionAmount.ToString(CultureInfo.InvariantCulture);

                monthlyAllowance.TotalAmount = CalculationService.RoundAmountWithCurrency(totalAmount, monthlyAllowance.CurrencyCode, 0).ToString(CultureInfo.InvariantCulture);
                CalculateAutoAllowance(currencyExchangeRates, monthlyAllowance);
                CalculateTotalNetAmount(currencyExchangeRates, monthlyAllowance);
            }

            return monthlyAllowance;
        }

        public void CalculateMonthlyBasicAllowance(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            MonthlyPayrollBasicAllowanceModel monthlyPayrollBasicAllowance,
            DateTime workingMonth,
            DateTime groupStartDate,
            DateTime? groupEndDate)
        {
            decimal totalAmount = 0;
            if (monthlyPayrollBasicAllowance.CurrencyId.HasValue)
            {
                var currencyId = monthlyPayrollBasicAllowance.CurrencyId.Value;

                totalAmount += CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.SubsistenceAmount,
                    monthlyPayrollBasicAllowance.SubsistenceCurrencyId, currencyId);

                totalAmount += CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.HomePassageAmount,
                    monthlyPayrollBasicAllowance.HomePassageCurrencyId, currencyId);

                totalAmount += CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.HousingAmount,
                    monthlyPayrollBasicAllowance.HousingCurrencyId, currencyId);

                totalAmount += CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.MobilePhoneAmount,
                    monthlyPayrollBasicAllowance.MobilePhoneCurrencyId, currencyId);

                totalAmount += CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.HPIAmount,
                    monthlyPayrollBasicAllowance.HPICurrencyId, currencyId);

                totalAmount += CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.DutyMealAmount,
                    monthlyPayrollBasicAllowance.DutyMealCurrencyId, currencyId);

                totalAmount += CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.SecondmentAmount,
                    monthlyPayrollBasicAllowance.SecondmentCurrencyId, currencyId);

                totalAmount += CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.OthersAmount,
                    monthlyPayrollBasicAllowance.OthersCurrencyId, currencyId);

                monthlyPayrollBasicAllowance.TotalAmount = CalculationService.RoundAmountWithCurrency(totalAmount, monthlyPayrollBasicAllowance.CurrencyCode, 0)
                    .ToString(CultureInfo.InvariantCulture);

                CalculateAfterAdjustment(totalAmount, monthlyPayrollBasicAllowance, workingMonth, groupStartDate, groupEndDate, currencyExchangeRates);
                CalculateAutoBasicAllowance(currencyExchangeRates, monthlyPayrollBasicAllowance);
            }
        }

        public IEnumerable<MonthlyAllowanceAdditionModel> InitializeAllowanceAdditions(
            int standardWorkingDays,
            decimal actualWorkingDays,
            MonthlyPayrollBasicAllowanceModel basicAllowance,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates)
        {
            var additions = Enum.GetValues(typeof(AllowanceType)).Cast<AllowanceType>()
                .Select(x => new MonthlyAllowanceAdditionModel
                {
                    PayrollId = basicAllowance.PayrollId,
                    EmployeeId = basicAllowance.EmployeeId,
                    Type = (int)x,
                    CurrencyId = GetCurrencyBaseOnAllowanceType(x, basicAllowance)
                }).ToList();

            if (standardWorkingDays < actualWorkingDays)
            {
                var dutyMeal = additions.FirstOrDefault(x => x.Type == (int)AllowanceType.DutyMeal);
                dutyMeal.NoOfDays = actualWorkingDays - standardWorkingDays;
                decimal.TryParse(basicAllowance.DutyMealAmount, out decimal dutyMealAmount);

                var unitAmount = dutyMealAmount / standardWorkingDays;
                var totalAmount = unitAmount * dutyMeal.NoOfDays;
                dutyMeal.UnitAmount = unitAmount.ToString(CultureInfo.InvariantCulture);
                dutyMeal.TotalAmount = totalAmount.ToString();

                if (dutyMeal.CurrencyId.HasValue && basicAllowance.CurrencyId.HasValue)
                {
                    var netAllowance = totalAmount.Value * GetExchangeRate(currencyExchangeRates, dutyMeal.CurrencyId.Value, basicAllowance.CurrencyId.Value);
                    dutyMeal.NetAllowance = netAllowance.ToString(CultureInfo.InvariantCulture);
                }
            }

            return additions;
        }

        public IEnumerable<MonthlyAllowanceDeductionModel> InitializeAllowanceDeductions(
            int standardWorkingDays,
            decimal actualWorkingDays,
            MonthlyPayrollBasicAllowanceModel basicAllowance,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates)
        {
            var deductions = Enum.GetValues(typeof(AllowanceType)).Cast<AllowanceType>()
                .Select(x => new MonthlyAllowanceDeductionModel
                {
                    PayrollId = basicAllowance.PayrollId,
                    EmployeeId = basicAllowance.EmployeeId,
                    Type = (int)x,
                    CurrencyId = GetCurrencyBaseOnAllowanceType(x, basicAllowance)
                }).ToList();

            if (standardWorkingDays > actualWorkingDays)
            {
                var dutyMeal = deductions.FirstOrDefault(x => x.Type == (int)AllowanceType.DutyMeal);
                dutyMeal.NoOfDays = standardWorkingDays - actualWorkingDays;
                decimal.TryParse(basicAllowance.DutyMealAmount, out decimal dutyMealAmount);

                var unitAmount = dutyMealAmount / standardWorkingDays;
                var totalAmount = unitAmount * dutyMeal.NoOfDays;
                dutyMeal.UnitAmount = unitAmount.ToString(CultureInfo.InvariantCulture);
                dutyMeal.TotalAmount = totalAmount.ToString();

                if (dutyMeal.CurrencyId.HasValue && basicAllowance.CurrencyId.HasValue)
                {
                    var netAllowance = totalAmount.Value * GetExchangeRate(currencyExchangeRates, dutyMeal.CurrencyId.Value, basicAllowance.CurrencyId.Value);
                    dutyMeal.NetAllowance = netAllowance.ToString(CultureInfo.InvariantCulture);
                }
            }

            return deductions;
        }

        private static void CalculateAfterAdjustment(
            decimal totalAmount,
            MonthlyPayrollBasicAllowanceModel monthlyPayrollBasicAllowance,
            DateTime workingMonth,
            DateTime groupStartDate,
            DateTime? groupEndDate,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRate)
        {
            int daysInMonth = DateTime.DaysInMonth(workingMonth.Year, workingMonth.Month);
            int startDay = CalculationService.GetDifferenceMonth(groupStartDate, workingMonth) == 0 ? groupStartDate.Day : 1;
            int endDay = (groupEndDate.HasValue && CalculationService.GetDifferenceMonth(groupEndDate.Value, workingMonth) == 0) ? groupEndDate.Value.Day : daysInMonth;
            var afterAdjustmentAmount = endDay == daysInMonth && startDay == 1 ? totalAmount : totalAmount / daysInMonth * (endDay - startDay + 1);
            monthlyPayrollBasicAllowance.AfterAdjustmentAmount = afterAdjustmentAmount.ToString(CultureInfo.InvariantCulture);

            monthlyPayrollBasicAllowance.SubsistenceAfterAdjustmentAmount =
                CalculateAfterAdjustmentAllowanceDetail(monthlyPayrollBasicAllowance, monthlyPayrollBasicAllowance.SubsistenceCurrencyId,
                monthlyPayrollBasicAllowance.SubsistenceAmount, daysInMonth, startDay, endDay, currencyExchangeRate);

            monthlyPayrollBasicAllowance.HousingAfterAdjustmentAmount =
                CalculateAfterAdjustmentAllowanceDetail(monthlyPayrollBasicAllowance, monthlyPayrollBasicAllowance.HousingCurrencyId,
                monthlyPayrollBasicAllowance.HousingAmount, daysInMonth, startDay, endDay, currencyExchangeRate);

            monthlyPayrollBasicAllowance.HomePassageAfterAdjustmentAmount =
                CalculateAfterAdjustmentAllowanceDetail(monthlyPayrollBasicAllowance, monthlyPayrollBasicAllowance.HomePassageCurrencyId,
                monthlyPayrollBasicAllowance.HomePassageAmount, daysInMonth, startDay, endDay, currencyExchangeRate);

            monthlyPayrollBasicAllowance.MobilePhoneAfterAdjustmentAmount =
                CalculateAfterAdjustmentAllowanceDetail(monthlyPayrollBasicAllowance, monthlyPayrollBasicAllowance.MobilePhoneCurrencyId,
                monthlyPayrollBasicAllowance.MobilePhoneAmount, daysInMonth, startDay, endDay, currencyExchangeRate);

            monthlyPayrollBasicAllowance.DutyMealAfterAdjustmentAmount =
                CalculateAfterAdjustmentAllowanceDetail(monthlyPayrollBasicAllowance, monthlyPayrollBasicAllowance.DutyMealCurrencyId,
                monthlyPayrollBasicAllowance.DutyMealAmount, daysInMonth, startDay, endDay, currencyExchangeRate);

            monthlyPayrollBasicAllowance.HPIAfterAdjustmentAmount =
                CalculateAfterAdjustmentAllowanceDetail(monthlyPayrollBasicAllowance, monthlyPayrollBasicAllowance.HPICurrencyId,
                monthlyPayrollBasicAllowance.HPIAmount, daysInMonth, startDay, endDay, currencyExchangeRate);

            monthlyPayrollBasicAllowance.OthersAfterAdjustmentAmount =
                CalculateAfterAdjustmentAllowanceDetail(monthlyPayrollBasicAllowance, monthlyPayrollBasicAllowance.OthersCurrencyId,
                monthlyPayrollBasicAllowance.OthersAmount, daysInMonth, startDay, endDay, currencyExchangeRate);

            monthlyPayrollBasicAllowance.SecondmentAfterAdjustmentAmount =
                CalculateAfterAdjustmentAllowanceDetail(monthlyPayrollBasicAllowance, monthlyPayrollBasicAllowance.SecondmentCurrencyId,
                monthlyPayrollBasicAllowance.SecondmentAmount, daysInMonth, startDay, endDay, currencyExchangeRate);

            var amount = afterAdjustmentAmount - totalAmount;
            if (amount != 0)
            {
                var reason = new StringBuilder();
                string dateCommencementReason = CalculationService.GetPartOfAfterAdjustmentReason(workingMonth, groupStartDate, groupEndDate);

                reason.AppendFormat("{0} {1} ({2}). ", amount > 0 ? "Added" : "Deducted",
                    CalculationService.RoundAmountWithCurrency(Math.Abs(amount), monthlyPayrollBasicAllowance.CurrencyCode, roundingThousand: false),
                    monthlyPayrollBasicAllowance.CurrencyCode);
                reason.Append(dateCommencementReason);

                monthlyPayrollBasicAllowance.Reason = reason.ToString();
            }
        }

        private static string CalculateAfterAdjustmentAllowanceDetail(MonthlyPayrollBasicAllowanceModel monthlyPayrollBasicAllowance, int? allowanceDetailCurrencyId,
            string allowanceDetailAmount, int daysInMonth, int startDay, int endDay, IEnumerable<CurrencyExchangeRateModel> currencyExchangeRate)
        {
            if (allowanceDetailCurrencyId.HasValue)
            {
                decimal.TryParse(allowanceDetailAmount, out decimal amount);
                var afterAdjustmentAmount = amount / daysInMonth * (endDay - startDay + 1);
                afterAdjustmentAmount = CalculationService.RoundAmountWithCurrency(
                    CalculationService.ConvertAmount(currencyExchangeRate, afterAdjustmentAmount, allowanceDetailCurrencyId, monthlyPayrollBasicAllowance.CurrencyId),
                    monthlyPayrollBasicAllowance.CurrencyCode, roundingThousand: false);

                return afterAdjustmentAmount.ToString(CultureInfo.InvariantCulture);
            }

            return string.Empty;
        }

        private static decimal GetExchangeRate(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            int fromCurrencyId,
            int toCurrencyId)
        {
            CurrencyExchangeRateModel currencyExchangeRate =
                currencyExchangeRates.FirstOrDefault(x => x.FromCurrencyId == fromCurrencyId && x.ToCurrencyId == toCurrencyId);

            return currencyExchangeRate?.ExchangeRate ?? 0;
        }

        private void DecryptMonthlyBasicAllowance(MonthlyPayrollBasicAllowanceModel basicAllowance)
        {
            basicAllowance.TotalAmount = cryptoService.Decrypt(basicAllowance.TotalAmount);
            basicAllowance.DirectBanking1Amount = cryptoService.Decrypt(basicAllowance.DirectBanking1Amount);
            basicAllowance.DirectBanking2Amount = cryptoService.Decrypt(basicAllowance.DirectBanking2Amount);
            basicAllowance.CashAmount = cryptoService.Decrypt(basicAllowance.CashAmount);
            basicAllowance.AfterAdjustmentAmount = cryptoService.Decrypt(basicAllowance.AfterAdjustmentAmount);
            basicAllowance.FitnessFirstAmount = cryptoService.Decrypt(basicAllowance.FitnessFirstAmount);
            basicAllowance.RoomingChargesAmount = cryptoService.Decrypt(basicAllowance.RoomingChargesAmount);

            basicAllowance.SubsistenceAmount = cryptoService.Decrypt(basicAllowance.SubsistenceAmount);
            basicAllowance.HomePassageAmount = cryptoService.Decrypt(basicAllowance.HomePassageAmount);
            basicAllowance.HousingAmount = cryptoService.Decrypt(basicAllowance.HousingAmount);
            basicAllowance.MobilePhoneAmount = cryptoService.Decrypt(basicAllowance.MobilePhoneAmount);
            basicAllowance.HPIAmount = cryptoService.Decrypt(basicAllowance.HPIAmount);
            basicAllowance.DutyMealAmount = cryptoService.Decrypt(basicAllowance.DutyMealAmount);
            basicAllowance.SecondmentAmount = cryptoService.Decrypt(basicAllowance.SecondmentAmount);
            basicAllowance.OthersAmount = cryptoService.Decrypt(basicAllowance.OthersAmount);

            if (basicAllowance.CurrencyId.HasValue && !string.IsNullOrEmpty(basicAllowance.AfterAdjustmentAmount))
            {
                basicAllowance.AfterAdjustmentAmountDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(basicAllowance.AfterAdjustmentAmount),
                    basicAllowance.CurrencyCode, roundingThousand: false).ToStringWithCurrency(basicAllowance.CurrencyCode);
            }
        }

        private void DecryptMonthlyAllowance(MonthlyAllowanceModel allowance)
        {
            allowance.TotalAmount = cryptoService.Decrypt(allowance.TotalAmount);
            allowance.DirectBanking1Amount = cryptoService.Decrypt(allowance.DirectBanking1Amount);
            allowance.DirectBanking2Amount = cryptoService.Decrypt(allowance.DirectBanking2Amount);
            allowance.CashAmount = cryptoService.Decrypt(allowance.CashAmount);
            allowance.TotalNetAmount = cryptoService.Decrypt(allowance.TotalNetAmount);
            allowance.DSTNetSalaryAmount = cryptoService.Decrypt(allowance.DSTNetSalaryAmount);
            allowance.AdditionAmount = cryptoService.Decrypt(allowance.AdditionAmount);
            allowance.DeductionAmount = cryptoService.Decrypt(allowance.DeductionAmount);

            if (allowance.CurrencyId.HasValue)
            {
                if (!string.IsNullOrEmpty(allowance.AdditionAmount))
                {
                    allowance.AdditionAmountDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(allowance.AdditionAmount), allowance.CurrencyCode, 0)
                        .ToString(CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(allowance.DeductionAmount))
                {
                    allowance.DeductionAmountDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(allowance.DeductionAmount), allowance.CurrencyCode, 0)
                        .ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        private void DecryptMonthlyAllowanceAdditions(IEnumerable<MonthlyAllowanceAdditionModel> additions, string allowanceCurrencyCode)
        {
            foreach (var addition in additions)
            {
                addition.UnitAmount = cryptoService.Decrypt(addition.UnitAmount);
                addition.NetAllowance = cryptoService.Decrypt(addition.NetAllowance);
                addition.TotalAmount = cryptoService.Decrypt(addition.TotalAmount);

                if (addition.CurrencyId.HasValue)
                {
                    if (!string.IsNullOrEmpty(addition.NetAllowance))
                    {
                        addition.NetAllowanceToDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(addition.NetAllowance), allowanceCurrencyCode, 0)
                            .ToString(CultureInfo.InvariantCulture);
                    }

                    if (!string.IsNullOrEmpty(addition.TotalAmount))
                    {
                        addition.TotalAmountToDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(addition.TotalAmount), addition.CurrencyCode, roundingThousand: false)
                            .ToStringWithCurrency(addition.CurrencyCode);
                    }
                }
            }
        }

        private void DecryptMonthlyAllowanceDeductions(IEnumerable<MonthlyAllowanceDeductionModel> deductions, string allowanceCurrencyCode)
        {
            foreach (var deduction in deductions)
            {
                deduction.UnitAmount = cryptoService.Decrypt(deduction.UnitAmount);
                deduction.NetAllowance = cryptoService.Decrypt(deduction.NetAllowance);
                deduction.TotalAmount = cryptoService.Decrypt(deduction.TotalAmount);

                if (deduction.CurrencyId.HasValue)
                {
                    if (!string.IsNullOrEmpty(deduction.NetAllowance))
                    {
                        deduction.NetAllowanceToDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(deduction.NetAllowance), allowanceCurrencyCode, 0)
                            .ToString(CultureInfo.InvariantCulture);
                    }

                    if (!string.IsNullOrEmpty(deduction.TotalAmount))
                    {
                        deduction.TotalAmountToDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(deduction.TotalAmount), deduction.CurrencyCode, roundingThousand: false)
                            .ToStringWithCurrency(deduction.CurrencyCode);
                    }
                }
            }
        }

        private void EncryptMonthlyAllowanceAddition(IEnumerable<MonthlyAllowanceAdditionModel> additions)
        {
            foreach (var addition in additions)
            {
                addition.UnitAmount = cryptoService.Encrypt(addition.UnitAmount);
                addition.TotalAmount = cryptoService.Encrypt(addition.TotalAmount);
                addition.NetAllowance = cryptoService.Encrypt(addition.NetAllowance);
            }
        }

        private void EncryptMonthlyAllowanceDeduction(IEnumerable<MonthlyAllowanceDeductionModel> deductions)
        {
            foreach (var deduction in deductions)
            {
                deduction.UnitAmount = cryptoService.Encrypt(deduction.UnitAmount);
                deduction.TotalAmount = cryptoService.Encrypt(deduction.TotalAmount);
                deduction.NetAllowance = cryptoService.Encrypt(deduction.NetAllowance);
            }
        }

        private static int? GetCurrencyBaseOnAllowanceType(AllowanceType type, MonthlyPayrollBasicAllowanceModel basicAllowance)
        {
            switch (type)
            {
                case AllowanceType.Subsistence: return basicAllowance.SubsistenceCurrencyId;
                case AllowanceType.HomePassage: return basicAllowance.HomePassageCurrencyId;
                case AllowanceType.Housing: return basicAllowance.HousingCurrencyId;
                case AllowanceType.MobilePhone: return basicAllowance.MobilePhoneCurrencyId;
                case AllowanceType.HPI: return basicAllowance.HPICurrencyId;
                case AllowanceType.DutyMeal: return basicAllowance.DutyMealCurrencyId;
                case AllowanceType.Secondment: return basicAllowance.SecondmentCurrencyId;
                case AllowanceType.Others: return basicAllowance.OthersCurrencyId;
                default: return null;
            }
        }

        private static void CalculateAutoBasicAllowance(IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates, MonthlyPayrollBasicAllowanceModel monthlyPayrollBasicAllowance)
        {
            decimal.TryParse(monthlyPayrollBasicAllowance.TotalAmount, out decimal totalAmount);

            var directBanking1 = CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.DirectBanking1Amount,
                monthlyPayrollBasicAllowance.DirectBanking1CurrencyId, monthlyPayrollBasicAllowance.CurrencyId);
            var directBanking2 = CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.DirectBanking2Amount,
                monthlyPayrollBasicAllowance.DirectBanking2CurrencyId, monthlyPayrollBasicAllowance.CurrencyId);
            var cash = CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.CashAmount, monthlyPayrollBasicAllowance.CashCurrencyId,
                monthlyPayrollBasicAllowance.CurrencyId);

            switch (monthlyPayrollBasicAllowance.AutoCalculateOn)
            {
                case PayrollAutoCalculateOn.DirectBanking1:
                    if (monthlyPayrollBasicAllowance.DirectBanking1CurrencyId.HasValue)
                    {
                        var convertAmount = CalculationService.ConvertAmount(currencyExchangeRates, totalAmount - directBanking2 - cash,
                            monthlyPayrollBasicAllowance.CurrencyId, monthlyPayrollBasicAllowance.DirectBanking1CurrencyId);
                        directBanking1 = CalculationService.RoundAmountWithCurrency(convertAmount, monthlyPayrollBasicAllowance.DirectBanking1CurrencyCode, 0);

                        monthlyPayrollBasicAllowance.DirectBanking1Amount = directBanking1.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                case PayrollAutoCalculateOn.DirectBanking2:
                    if (monthlyPayrollBasicAllowance.DirectBanking2CurrencyId.HasValue)
                    {
                        var convertAmount = CalculationService.ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - cash, monthlyPayrollBasicAllowance.CurrencyId,
                            monthlyPayrollBasicAllowance.DirectBanking2CurrencyId);
                        directBanking2 = CalculationService.RoundAmountWithCurrency(convertAmount, monthlyPayrollBasicAllowance.DirectBanking2CurrencyCode, 0);

                        monthlyPayrollBasicAllowance.DirectBanking2Amount = directBanking2.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                case PayrollAutoCalculateOn.Cash:
                    if (monthlyPayrollBasicAllowance.CashCurrencyId.HasValue)
                    {
                        var convertAmount = CalculationService.ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - directBanking2,
                            monthlyPayrollBasicAllowance.CurrencyId, monthlyPayrollBasicAllowance.CashCurrencyId);
                        cash = CalculationService.RoundAmountWithCurrency(convertAmount, monthlyPayrollBasicAllowance.CashCurrencyCode, 0);

                        monthlyPayrollBasicAllowance.CashAmount = cash.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                default:
                    return;
            }
        }

        private static void CalculateAutoAllowance(IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates, MonthlyAllowanceModel monthlyAllowance)
        {
            decimal.TryParse(monthlyAllowance.TotalAmount, out decimal totalAmount);

            var directBanking1 = CalculationService.ConvertAmount(currencyExchangeRates, monthlyAllowance.DirectBanking1Amount,
                monthlyAllowance.DirectBanking1CurrencyId, monthlyAllowance.CurrencyId);
            var directBanking2 = CalculationService.ConvertAmount(currencyExchangeRates, monthlyAllowance.DirectBanking2Amount,
                monthlyAllowance.DirectBanking2CurrencyId, monthlyAllowance.CurrencyId);
            var cash = CalculationService.ConvertAmount(currencyExchangeRates, monthlyAllowance.CashAmount, monthlyAllowance.CashCurrencyId, monthlyAllowance.CurrencyId);
            decimal autoAmount = 0;

            switch (monthlyAllowance.AutoCalculateOn)
            {
                case PayrollAutoCalculateOn.DirectBanking1:
                    if (monthlyAllowance.DirectBanking1CurrencyId.HasValue)
                    {
                        autoAmount = CalculationService.RoundAmountWithCurrency(
                            CalculationService.ConvertAmount(currencyExchangeRates, totalAmount - directBanking2 - cash, monthlyAllowance.CurrencyId, monthlyAllowance.DirectBanking1CurrencyId),
                            monthlyAllowance.DirectBanking1CurrencyCode, 0);
                        monthlyAllowance.DirectBanking1Amount = autoAmount.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                case PayrollAutoCalculateOn.DirectBanking2:
                    if (monthlyAllowance.DirectBanking2CurrencyId.HasValue)
                    {
                        autoAmount = CalculationService.RoundAmountWithCurrency(
                            CalculationService.ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - cash, monthlyAllowance.CurrencyId, monthlyAllowance.DirectBanking2CurrencyId),
                            monthlyAllowance.DirectBanking2CurrencyCode, 0);
                        monthlyAllowance.DirectBanking2Amount = autoAmount.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                case PayrollAutoCalculateOn.Cash:
                    if (monthlyAllowance.CashCurrencyId.HasValue)
                    {
                        autoAmount = CalculationService.RoundAmountWithCurrency(
                            CalculationService.ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - directBanking2, monthlyAllowance.CurrencyId, monthlyAllowance.CashCurrencyId),
                            monthlyAllowance.CashCurrencyCode, 0);
                        monthlyAllowance.CashAmount = autoAmount.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                default:
                    return;
            }

            monthlyAllowance.Status = autoAmount >= 0 ? MonthlyPayrollStatus.Default : MonthlyPayrollStatus.Unsatisfied;
        }

        private static void CalculateTotalNetAmount(IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates, MonthlyAllowanceModel monthlyAllowance)
        {
            decimal.TryParse(monthlyAllowance.TotalAmount, out decimal totalAmount);

            if (monthlyAllowance.DSTNetSalaryCurrencyId.HasValue && !string.IsNullOrEmpty(monthlyAllowance.DSTNetSalaryAmount))
            {
                var totalNetAmount = totalAmount -
                    CalculationService.ConvertAmount(currencyExchangeRates, monthlyAllowance.DSTNetSalaryAmount, monthlyAllowance.DSTNetSalaryCurrencyId.Value, monthlyAllowance.CurrencyId.Value);

                monthlyAllowance.TotalNetAmount = CalculationService.RoundAmountWithCurrency(totalNetAmount, monthlyAllowance.CurrencyCode, roundingThousand: false)
                    .ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                monthlyAllowance.TotalNetAmount = totalAmount.ToStringWithCurrency(monthlyAllowance.CurrencyCode);
            }
        }

        

        
    }
}