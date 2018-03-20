namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.EmployeeManagement;
    using HRP.Models.MonthlyPayroll;

    public interface IMonthlyPayrollService
    {
        Task<EmployeeMonthlyPayrollViewModel> GetDataFilterForIndexPageAsync(DateTime month, int companyId, string costCenterIds);

        Task<IEnumerable<MonthlyPayrollEmployeeInfoViewModel>> GetMonthlyPayrollAsync(EmployeeMonthlyPayrollViewModel viewModel);

        Task<IEnumerable<EmployeeSuggestionViewModel>> GetEmployeeSuggestionByNameAsync(int companyId, string name);

        Task<EditMonthlySalaryViewModel> GetMonthlySalaryToEditAsync(int payrollId);

        Task<CommandResult> ResetMonthlySalaryAsync(int payrollId, int companyId);

        Task<CommandResult> EditMonthlySalaryAsync(EditMonthlySalaryViewModel viewModel);

        Task<MonthlyAllowancesViewModel> GetMonthlyAllowancesAsync(int payrollId);

        Task<CommandResult> EditMonthlyAllowancesAsync(MonthlyAllowancesViewModel viewModel, int userId);

        BackPayAfterAdjustmentViewModel CalculateBackPayAndAfterAdjustment(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            DateTime workingMonth,
            MonthlyBasicSalaryModel basicSetting,
            DateTime groupStartDate,
            DateTime? groupEndDate);

        MonthlyPayrollBasicAllowanceModel InitializeMonthlyBasicAllowance(int payrollId, EmployeePayrollAllowanceModel employeePayrollAllowance, EmployeePayrollDeductionModel deduction);

        void CalculateMonthlyBasicAllowance(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            MonthlyPayrollBasicAllowanceModel monthlyPayrollBasicAllowance,
            DateTime startDate,
            DateTime? endDate);

        IEnumerable<MonthlyAllowanceAdditionModel> InitializeAllowanceAdditions(int payrollId, int standardWorkingDays, int actualWorkingDays,
            EmployeePayrollAllowanceModel employeePayrollAllowance, IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates);

        IEnumerable<MonthlyAllowanceDeductionModel> InitializeAllowanceDeductions(int payrollId, int standardWorkingDays, int actualWorkingDays,
            EmployeePayrollAllowanceModel employeePayrollAllowance, IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates);

        MonthlyAllowanceModel InitializeMonthlyAllowance(int payrollId, MonthlyPayrollBasicAllowanceModel basicAllowance, IEnumerable<MonthlyAllowanceAdditionModel> allowanceAdditions,
            IEnumerable<MonthlyAllowanceDeductionModel> allowanceDeductions, IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates);
    }
}