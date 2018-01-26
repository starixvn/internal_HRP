namespace HRP.Services.Interfaces
{
    using System.Threading.Tasks;
    using Core.Model;
    using Microsoft.AspNetCore.Http;
    using Models;
    using Models.EmployeeManagement;

    public interface IEmployeeManagementService
    {
        Task<CreateEmployeeViewModel> InitDataForNewEmployeeAsync();

        Task<CommandResult> CreateEmployeeAsync(CreateEmploymentViewModel employee);

        Task<CommandResult> UpdatePersonalDetailsAsync(CreateEmployeeViewModel employee);

        CreateEmployeeViewModel GetEmployeeById(int employeeId);

        CommandResult CheckValidPicture(IFormFile file);

        ListCompanyDivisionDepartment GetAllCompanyDivisonDepartment();

        CreateEmploymentViewModel GetDataToCreateEmployment();

        UpdateEmploymentViewModel GetDataToUpdateEmployment(int employeeId);

        Task<CommandResult> UpdateEmploymentAsync(UpdateEmploymentViewModel viewModel);
    }
}