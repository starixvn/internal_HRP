namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Model;
    using HRP.Models;
    using Microsoft.AspNetCore.Http;
    using Models.EmployeeManagement;

    public interface IEmployeeService
    {
        Task<Employee> GetEmployeeBasicInfoByIdAsync(int employeeId);

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