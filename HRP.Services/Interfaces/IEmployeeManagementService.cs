namespace HRP.Services
{
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models.EmployeeManagement;
    using Microsoft.AspNetCore.Http;

    public interface IEmployeeManagementService
    {
        Task<CreateEmployeeViewModel> InitDataForNewEmployeeAsync();

        Task<CommandResult> CreateEmployeeAsync(CreateEmployeeViewModel employee);

        Task<CommandResult> EditEmployeeAsync(CreateEmployeeViewModel employee);

        CreateEmployeeViewModel GetEmployeeById(int employeeId);

        CommandResult CheckValidPicture(IFormFile file);

        Task<UpdateEmploymentViewModel> GetEmploymentToUpdateAsync(int employeeId);

        Task<CommandResult> CreateEmploymentAsync(CreateEmploymentViewModel viewModel);

        Task<CommandResult> UpdateEmploymentAsync(UpdateEmploymentViewModel viewModel);
    }
}