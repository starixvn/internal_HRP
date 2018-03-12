namespace HRP.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Models.MonthlyPayroll;

    public interface IMonthlyPayrollRepository
    {
        Task<EmployeeMonthlyPayrollViewModel> GetDataFilterForIndexPageAsync(ICriteria criteria);

        Task<IEnumerable<MonthlyPayrollEmployeeInfoViewModel>> GetMonthlyPayrollAsync(ICriteria criteria);

        Task<EditMonthlySalaryViewModel> GetMonthlySalaryToEditAsync(ICriteria criteria);

        Task<Tuple<MonthlyAllowanceModel, IEnumerable<MonthlyAllowanceAdditionModel>, IEnumerable<MonthlyAllowanceDeductionModel>>> GetMonthlyAllowancesAsync(ICriteria criteria);
    }
}