namespace HRP.Services
{
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models;
    using HRP.Models.EmployeeManagement;
    using Microsoft.AspNetCore.Http;

    public interface IEmployeeManagementService
    {
        Task<CreateEmployeeViewModel> InitDataForNewEmployeeAsync();

        Task<CommandResult> CreateEmployeeAsync(CreateEmploymentViewModel employee);

        Task<CommandResult> UpdatePersonalDetailsAsync(CreateEmployeeViewModel employee);

        CreateEmployeeViewModel GetEmployeeById(int employeeId);

        CommandResult CheckValidPicture(IFormFile file);

        UpdateEmploymentViewModel GetEmploymentToUpdate(int employeeId);

        Task<CommandResult> UpdateEmploymentAsync(UpdateEmploymentViewModel viewModel);

        ListCompanyDivisionDepartment GetAllCompanyDivisonDepartment();
    }
}