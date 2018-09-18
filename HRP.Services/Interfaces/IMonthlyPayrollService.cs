namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Models;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.MonthlyPayroll.EmployeeMonthlyPayroll;

    public interface IMonthlyPayrollService
    {
        Task<IEnumerable<MultiSelectViewModel>> GetHierarchyCompanyAsync(int companyId, string costCenterIds);

        Task<MonthlyPayrollFilterInfoModel> GetFilterInfoAsync(int companyId, DateTime month, int? payrollId = null);

        Task<IEnumerable<EmployeeMonthlyPayrollModel>> GetMonthlyPayrollAsync(EmployeeMonthlyPayrollViewModel viewModel);

        Task<IEnumerable<EmployeeSuggestionViewModel>> GetEmployeeSuggestionByNameAsync(int companyId, string name);

        Task<IEnumerable<EmployeeSuggestionViewModel>> GetEmployeeSuggestionByIDNoAsync(int companyId, string idNo);
    }
}