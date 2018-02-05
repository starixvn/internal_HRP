namespace HRP.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Model;
    using Microsoft.AspNetCore.Http;
    using Models.EmployeeManagement;

    public interface IEmployeeManagementService
    {
        EmployeeManagementViewModel GetDataFilterForIndexPage(string departmentIds, string employmentStatusIds);

        Task<IEnumerable<EmployeeBasicInfo>> GetEmployeesAsync(EmployeeManagementViewModel viewModel);

        Task<CreateEmployeeViewModel> GetDataToCreateEmployeeAsync();

        Task<CommandResult> CreateEmployeeAsync(CreateEmploymentViewModel employee);

        Task<CommandResult> UpdatePersonalDetailsAsync(CreateEmployeeViewModel employee);

        CreateEmployeeViewModel GetPersonalDetails(int employeeId);

        CommandResult CheckValidPicture(IFormFile file);

        CreateEmploymentViewModel GetDataToCreateEmployment();

        UpdateEmploymentViewModel GetDataToUpdateEmployment(int employeeId);

        Task<CommandResult> UpdateEmploymentAsync(UpdateEmploymentViewModel viewModel);
    }
}