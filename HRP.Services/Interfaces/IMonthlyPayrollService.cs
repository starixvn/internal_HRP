namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.ServiceModels;

    public interface IMonthlyPayrollService
    {
        Task<IEnumerable<MultiSelectViewModel>> GetHierarchyCompanyAsync(int companyId, string costCenterIds);

        Task<MonthlyPayrollFilterInfoModel> GetFilterInfoAsync(int companyId, DateTime month, int? payrollId = null);

        Task<IEnumerable<MonthlyPayrollEmployeeInfoViewModel>> GetMonthlyPayrollAsync(EmployeeMonthlyPayrollViewModel viewModel);

        Task<IEnumerable<EmployeeSuggestionViewModel>> GetEmployeeSuggestionByNameAsync(int companyId, string name);

        Task<IEnumerable<EmployeeSuggestionViewModel>> GetEmployeeSuggestionByIDNoAsync(int companyId, string idNo);

        Task<EditMonthlySalaryViewModel> GetMonthlySalaryAsync(int payrollId);

        Task<CommandResult> EditMonthlySalaryAsync(EditMonthlySalaryViewModel viewModel);

        Task<MonthlyAllowances> GetMonthlyAllowancesAsync(int payrollId, DateTime month);

        Task<CommandResult> EditMonthlyAllowancesAsync(MonthlyAllowances viewModel, int userId);

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
            MonthlyPayrollBasicAllowance monthlyPayrollBasicAllowance,
            DateTime workingMonth,
            DateTime groupStartDate,
            DateTime? groupEndDate);

        IEnumerable<MonthlyAllowanceAddition> InitializeAllowanceAdditions(
            int standardWorkingDays,
            decimal actualWorkingDays,
            MonthlyPayrollBasicAllowance basicAllowance,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates);

        IEnumerable<MonthlyAllowanceDeduction> InitializeAllowanceDeductions(
            int standardWorkingDays,
            decimal actualWorkingDays,
            MonthlyPayrollBasicAllowance basicAllowance,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates);

        MonthlyAllowance InitializeMonthlyAllowance(
            MonthlyPayrollBasicAllowance basicAllowance,
            IEnumerable<MonthlyAllowanceAddition> allowanceAdditions,
            IEnumerable<MonthlyAllowanceDeduction> allowanceDeductions,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            MonthlyDSTNetSalary dstNetSalary);
    }
}