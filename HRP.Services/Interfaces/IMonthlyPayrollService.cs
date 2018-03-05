namespace HRP.Services
{
    using System.Threading.Tasks;
    using HRP.Models;

    public interface IMonthlyPayrollService
    {
        Task<EmployeeMonthlyPayrollViewModel> GetDataFilterForIndexPageAsync(int companyId, string costCenterIds);
    }
}