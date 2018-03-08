namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models;
    using HRP.Models.MonthlyPayroll;

    public interface IMonthlyPayrollService
    {
        Task<EmployeeMonthlyPayrollViewModel> GetDataFilterForIndexPageAsync(int companyId, string costCenterIds);

        Task<IEnumerable<MonthlyPayrollEmployeeInfoViewModel>> GetMonthlyPayrollAsync(EmployeeMonthlyPayrollViewModel viewModel);

        Task<IEnumerable<EmployeeSuggestionViewModel>> GetEmployeeSuggestionByNameAsync(int companyId, string name);

        Task<CommandResult> ResetMonthlySalaryAsync();

        Task<CommandResult> UpdateMonthlySalaryAsync();
    }
}