namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;

    public interface IMonthlySalaryService
    {
        Task<(MonthlyBasicSalaryModel,
                MonthlySalaryModel,
                IEnumerable<MonthlySalaryAdditionModel>,
                IEnumerable<MonthlySalaryDeductionModel>)> GetMonthlySalaryAsync(int payrollId);

        Task<CommandResult> EditMonthlySalaryAsync(
            int payrollId,
            int companyId,
            DateTime month,
            MonthlySalaryModel monthlySalary,
            IEnumerable<MonthlySalaryAdditionModel> additions,
            IEnumerable<MonthlySalaryDeductionModel> deductions);

        void CalculateBackPayAndAfterAdjustment(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            DateTime workingMonth,
            MonthlyBasicSalaryModel basicSetting,
            MonthlySalaryAdditionModel backPay,
            DateTime groupStartDate,
            DateTime? groupEndDate);

        void CalculateMonthlySalary(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            MonthlyBasicSalaryModel basicSalary,
            MonthlySalaryAdditionModel backPay,
            MonthlySalaryModel monthlySalary);
    }
}