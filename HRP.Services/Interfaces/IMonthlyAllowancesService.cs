namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.ServiceModels;

    public interface IMonthlyAllowancesService
    {
        Task<MonthlyAllowancesModel> GetMonthlyAllowancesAsync(int payrollId, DateTime month);

        Task<CommandResult> EditMonthlyAllowancesAsync(MonthlyAllowancesModel model, int userId);

        void CalculateMonthlyBasicAllowance(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            MonthlyPayrollBasicAllowanceModel monthlyPayrollBasicAllowance,
            DateTime workingMonth,
            DateTime groupStartDate,
            DateTime? groupEndDate);

        IEnumerable<MonthlyAllowanceAdditionModel> InitializeAllowanceAdditions(
            int standardWorkingDays,
            decimal actualWorkingDays,
            MonthlyPayrollBasicAllowanceModel basicAllowance,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates);

        IEnumerable<MonthlyAllowanceDeductionModel> InitializeAllowanceDeductions(
            int standardWorkingDays,
            decimal actualWorkingDays,
            MonthlyPayrollBasicAllowanceModel basicAllowance,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates);

        MonthlyAllowanceModel InitializeMonthlyAllowance(
            MonthlyPayrollBasicAllowanceModel basicAllowance,
            IEnumerable<MonthlyAllowanceAdditionModel> allowanceAdditions,
            IEnumerable<MonthlyAllowanceDeductionModel> allowanceDeductions,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            MonthlyDSTNetSalary dstNetSalary);
    }
}