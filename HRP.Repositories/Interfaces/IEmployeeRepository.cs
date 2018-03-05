namespace HRP.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Models.EmployeeManagement;

    public interface IEmployeeRepository
    {
        CreateEmployeeViewModel GetPersonalDetails(ICriteria criteria);

        CreateEmploymentViewModel GetDataToCreateEmployment(ICriteria criteria);

        UpdateEmploymentViewModel GetDataToUpdateEmployment(ICriteria criteria);

        Task<EmployeeManagementViewModel> GetDataFilterForIndexPageAsync(ICriteria criteria);
    }
}