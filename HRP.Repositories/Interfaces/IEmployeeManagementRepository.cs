namespace HRP.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using HRP.Models.EmployeeManagement;

    public interface IEmployeeManagementRepository
    {
        UpdateEmploymentViewModel GetEmploymentToUpdate(ICriteria criteria);
    }
}