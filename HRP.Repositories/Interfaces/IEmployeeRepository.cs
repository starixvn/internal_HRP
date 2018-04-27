namespace HRP.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Models.Employee;

    public interface IEmployeeRepository
    {
        Task<Employment> GetEmploymentByEmployeeIdAsync(ICriteria criteria);
    }
}