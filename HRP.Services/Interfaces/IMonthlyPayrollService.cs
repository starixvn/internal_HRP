namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models;
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

        BackPayViewModel CalculateBackPay(DateTime effectiveDate, DateTime workingMonth, decimal oldBasicSalary, decimal newBasicSalary);
    }
}