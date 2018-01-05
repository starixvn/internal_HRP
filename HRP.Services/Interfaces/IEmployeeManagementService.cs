namespace HRP.Services
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Core.Model;
    using HRP.Models.EmployeeManagement;
    using Microsoft.AspNetCore.Http;

    public interface IEmployeeManagementService
    {
        Task<CommandResult> CreateEmployeeAsync(CreateEmployeeViewModel employee);

        CreateEmployeeViewModel GetEmployeeById(int employeeId);

        CommandResult CheckValidPicture(IFormFile file);
    }
}