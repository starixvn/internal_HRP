namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Model;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.EmployeeManagement;

    public interface IEmployeeService
    {
        Task<EmployeeBasicInfo> GetEmployeeBasicInfoByIdAsync(int employeeId);

        Task<IEnumerable<EmployeeBasicInfo>> GetEmployeesAsync(EmployeeManagementViewModel viewModel);

        Task<Tuple<SelectList, SelectList, SelectList, SelectList>> GetDataToCreateEmployeeAsync();

        Task<CommandResult> CreateEmployeeAsync(CreateEmploymentViewModel employee);

        Task<CommandResult> UpdatePersonalDetailsAsync(CreateEmployeeViewModel employee);

        Task<Employee> GetEmployeeByIdAsync(int employeeId);

        Task<EmergencyContact> GetEmergencyContactByEmployeeIdAsync(int employeeId);

        CommandResult CheckValidPicture(IFormFile file);

        CreateEmploymentViewModel GetDataToCreateEmployment();

        UpdateEmploymentViewModel GetDataToUpdateEmployment(int employeeId);

        Task<CommandResult> UpdateEmploymentAsync(UpdateEmploymentViewModel viewModel);
    }
}