namespace HRP.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using HRP.Models.EmployeeManagement;

    public interface IEmployeeManagementRepository
    {
        CreateEmploymentViewModel GetDataToCreateEmployment(ICriteria criteria);

        UpdateEmploymentViewModel GetDataToUpdateEmployment(ICriteria criteria);
    }
}