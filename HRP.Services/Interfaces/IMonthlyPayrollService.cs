namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;

    public interface IMonthlyPayrollService
    {
        Task<EmployeeMonthlyPayrollViewModel> GetDataFilterForIndexPageAsync(DateTime month, int companyId, string costCenterIds);

        Task<IEnumerable<MonthlyPayrollEmployeeInfoViewModel>> GetMonthlyPayrollAsync(EmployeeMonthlyPayrollViewModel viewModel);

        Task<IEnumerable<EmployeeSuggestionViewModel>> GetEmployeeSuggestionByNameAsync(int companyId, string name);

        Task<IEnumerable<EmployeeSuggestionViewModel>> GetEmployeeSuggestionByIDNoAsync(int companyId, string idNo);

        Task<EditMonthlySalaryViewModel> GetMonthlySalaryAsync(int payrollId);

        Task<CommandResult> EditMonthlySalaryAsync(EditMonthlySalaryViewModel viewModel);

        Task<MonthlyAllowancesViewModel> GetMonthlyAllowancesAsync(int payrollId, DateTime month);

        Task<CommandResult> EditMonthlyAllowancesAsync(MonthlyAllowancesViewModel viewModel, int userId);

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

        void CalculateMonthlyBasicAllowance(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            MonthlyPayrollBasicAllowanceModel monthlyPayrollBasicAllowance,
            DateTime startDate,
            DateTime? endDate,
            int month, int year);

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