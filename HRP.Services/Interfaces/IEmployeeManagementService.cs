namespace HRP.Services
{
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models.EmployeeManagement;
    using Microsoft.AspNetCore.Http;

    public interface IEmployeeManagementService
    {
        Task<CommandResult> CreateEmployeeAsync(EmployeePersonalDetail employee);

        CommandResult CheckValidPicture(IFormFile file);
    }
}