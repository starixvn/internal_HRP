namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models.MonthlyPayroll;

    public interface IMonthlyPayrollService
    {
        Task<EmployeeMonthlyPayrollViewModel> GetDataFilterForIndexPageAsync(int companyId, string costCenterIds);

        Task<IEnumerable<EmployeeMonthlyPayrollBasicInfo>> GetMonthlyPayrollAsync(EmployeeMonthlyPayrollViewModel viewModel);

        Task<CommandResult> ResetMonthlySalaryAsync();

        Task<CommandResult> UpdateMonthlySalaryAsync();
    }
}