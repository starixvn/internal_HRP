namespace HRP.Repositories.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Models.EmployeeManagement;

    public interface IEmployeeManagementRepository
    {
        Task<UpdateEmploymentViewModel> GetEmploymentToUpdateAsync(ICriteria criteria);
    }
}