namespace HRP.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Models.MonthlyPayroll;

    public interface IMonthlyPayrollRepository
    {
        Task<EmployeeMonthlyPayrollViewModel> GetDataFilterForIndexPageAsync(ICriteria criteria);

        Task<UpdateMonthlySalaryViewModel> GetMonthlySalaryToUpdateAsync(ICriteria criteria);
    }
}