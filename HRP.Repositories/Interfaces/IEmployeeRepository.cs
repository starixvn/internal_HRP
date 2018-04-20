namespace HRP.Repositories
{
    using Fanex.Data.Repository;
    using HRP.Models.EmployeeManagement;

    public interface IEmployeeRepository
    {
        CreateEmploymentViewModel GetDataToCreateEmployment(ICriteria criteria);

        UpdateEmploymentViewModel GetDataToUpdateEmployment(ICriteria criteria);
    }
}