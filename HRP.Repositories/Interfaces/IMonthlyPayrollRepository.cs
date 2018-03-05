namespace HRP.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Models;

    public interface IMonthlyPayrollRepository
    {
        Task<EmployeeMonthlyPayrollViewModel> GetDataFilterForIndexPageAsync(ICriteria criteria);
    }
}