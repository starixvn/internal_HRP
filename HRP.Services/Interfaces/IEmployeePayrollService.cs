namespace HRP.Services
{
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models.ServiceModels;

    public interface IEmployeePayrollService
    {
        Task<EmployeePayrollModel> GetEmployeePayroll(int employeeId, int userId);
        Task<CommandResult> UpdatePayroll(EmployeePayrollModel model, int userId);
    }
}
