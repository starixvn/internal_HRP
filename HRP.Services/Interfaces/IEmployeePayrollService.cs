namespace HRP.Services
{
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using Models.Employee.Payroll;

    public interface IEmployeePayrollService
    {
        Task<EmployeePayrollViewModel> GetEmployeePayroll(int employeeId, int userId);
        Task<CommandResult> UpdatePayroll(EmployeePayrollViewModel model, int userId);
    }
}
