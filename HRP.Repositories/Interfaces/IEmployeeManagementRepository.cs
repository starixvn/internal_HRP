namespace HRP.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using HRP.Models.EmployeeManagement;

    public interface IEmployeeManagementRepository
    {
        CreateEmployeeViewModel GetPersonalDetails(ICriteria criteria);

        CreateEmploymentViewModel GetDataToCreateEmployment(ICriteria criteria);

        UpdateEmploymentViewModel GetDataToUpdateEmployment(ICriteria criteria);

        EmployeeManagementViewModel GetDataFilterForIndexPage(ICriteria criteria);
    }
}